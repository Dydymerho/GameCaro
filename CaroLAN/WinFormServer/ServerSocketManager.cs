using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace WinFormServer
{
    internal class ServerSocketManager
    {
        public const int PORT = 9999;
        private Socket socket;
        private string IP = "127.0.0.1";
        private List<Socket> clients = new List<Socket>();
        private List<Thread> threads = new List<Thread>();
        private bool isRunning = false;
        private RoomManager roomManager; // ✅ Thêm room manager

        public ServerSocketManager()
        {
            roomManager = new RoomManager();
        }

        public void CreateServer(Action<string> logAction, Action updateClientList)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(endpoint);
            socket.Listen(100);
            isRunning = true;

            logAction?.Invoke($"Server đang lắng nghe trên cổng {PORT}...");


            Thread acceptThread = new Thread(() =>
            {
                while (isRunning)
                {
                    try
                    {
                        logAction?.Invoke("Đang chờ kết nối từ client...");
                        Socket client = socket.Accept();
                        lock (clients)
                        {
                            clients.Add(client);
                        }
                        SendClientListToAll(logAction);
                        updateClientList.Invoke();

                        Thread clientThread = new Thread(() => HandleClient(client,  logAction));
                        logAction?.Invoke($"Client {client.RemoteEndPoint} đã kết nối.");
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    catch (SocketException)
                    {
                        if (!isRunning) return; // Thoát nếu server đã dừng
                    }
                    catch
                    {
                        logAction?.Invoke("Lỗi khi chấp nhận kết nối từ client.");
                    }
                }
                
            });
            acceptThread.IsBackground = true;
            acceptThread.Start();
            lock (threads) threads.Add(acceptThread);


        }

        //server nhan du lieu tu client va phan hoi lai
        private void HandleClient(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                while (isRunning)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break; // Client ngắt kết nối
                      
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    logAction?.Invoke($"Nhận từ client {clientSocket.RemoteEndPoint}: {message}");

                    // ✅ Xử lý các lệnh room
                    if (message.StartsWith("JOIN_ROOM"))
                    {
                        HandleJoinRoom(clientSocket, message, logAction);
                    }
                    else if (message.StartsWith("GAME_MOVE:"))
                    {
                        HandleGameMove(clientSocket, message, logAction);
                    }
                    else if (message == "LEAVE_ROOM")
                    {
                        HandleLeaveRoom(clientSocket, logAction);
                    }
                    //----------------------------------

                    // Gửi phản hồi lại cho client (tùy chọn)
                    string response = $"Server đã nhận: {message}";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(responseData);
                }
            }
            catch (SocketException)
            {
                if (!isRunning) return; // Thoát nếu server đã dừng
            }
            
            catch (Exception ex)
            {
                //logAction?.Invoke($"Lỗi khi xử lý client {clientSocket.RemoteEndPoint}: {ex.Message}");
                Console.WriteLine($"Lỗi khi xử lý client: {ex.Message}");
            }
            finally
            {
                roomManager.LeaveRoom(clientSocket);
                SendClientListToAll(logAction);

                // Loại bỏ client khỏi danh sách và đóng kết nối
                lock (clients)
                {
                    clients.Remove(clientSocket);
                }
                clientSocket.Close();
                //logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} đã ngắt kết nối.");
            }
        }

        public void Send(string message)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    socket.Send(data);
                }
                catch (Exception)
                {
                    Console.WriteLine("Lỗi khi gửi dữ liệu.");
                    throw;
                }
                
            }
        }

        public string Receive()
        {
            if (socket == null)
            {
                throw new InvalidOperationException("Socket is not initialized. Ensure CreateServer is called first.");
            }
            try
            {
                byte[] buffer = new byte[1024];
                int recv = socket.Receive(buffer);
                return Encoding.UTF8.GetString(buffer, 0, recv);
            }
            catch
            {
                return string.Empty;
            }
        }

        //ham send cho nhieu client
        public void Broadcast(string message, List<Socket> clients, Action<string> logAction)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client.Connected)
                    {
                        try
                        {
                            client.Send(data);
                        }
                        catch (Exception ex)
                        {
                            logAction?.Invoke($"Lỗi khi gửi đến client {client.RemoteEndPoint}: {ex.Message}");
                        }
                    }
                }
            }
        }

        internal void stopServer(Action<string> logAction)
        {
            try
            {
                logAction?.Invoke("Đang dừng server...");
                Broadcast("SERVER_STOPPED", clients, logAction);


                //dong tat ca ket noi client
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        client.Close();
                    }
                    clients.Clear();
                }

                //dong tat ca cac thread
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        if (thread.IsAlive)
                        {
                            thread.Interrupt();
                        }
                    }
                    threads.Clear();
                }


                isRunning = false;

                //dong socket server
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }

                

                logAction?.Invoke("Server đã dừng.");
            }
            catch
            {
                logAction?.Invoke("Lỗi khi dừng server.");
            }
            
        }

        public List<string> GetConnectedClients()
        {
            lock (clients)
            {
                List<string> connectedClients = new List<string>();

                foreach (var client in clients)
                {
                    try
                    {
                        // Perform a non-blocking check to ensure the client is still connected
                        if (client.Connected)
                        {
                            connectedClients.Add(client.RemoteEndPoint.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and skip this client
                        Console.WriteLine($"Error checking client connection: {ex.Message}");
                    }
                }

                return connectedClients;
            }
        }
        private void SendClientListToAll(Action<string> logAction)
        {
            // Lấy danh sách các client hiện tại
            List<string> connectedClients = GetConnectedClients();
            string clientListMessage = "CLIENT_LIST:" + string.Join(",", connectedClients);

            // Gửi danh sách client đến tất cả các client
            Broadcast(clientListMessage, clients, logAction);
        }

        public void DisconnectClient(string remoteEndPoint, Action<string> logAction)
        {
            lock (clients)
            {
                // Tìm client dựa trên RemoteEndPoint
                var client = clients.FirstOrDefault(c => c.RemoteEndPoint?.ToString() == remoteEndPoint);
                if (client != null)
                {
                    try
                    {
                        //gui tin hieu ngat ket noi
                        byte[] disconnectMessage = Encoding.UTF8.GetBytes("SERVER_STOPPED");
                        client.Send(disconnectMessage);
                        //dong ket noi client
                        client.Close();
                        clients.Remove(client);
                        logAction?.Invoke($"Client {remoteEndPoint} đã bị ngắt kết nối.");
                    }
                    catch (Exception ex)
                    {
                        logAction?.Invoke($"Lỗi khi ngắt kết nối client {remoteEndPoint}: {ex.Message}");
                    }
                }
                else
                {
                    logAction?.Invoke($"Không tìm thấy client {remoteEndPoint} để ngắt kết nối.");
                }
            }
        }

        // ✅ Xử lý tham gia phòng
        private void HandleJoinRoom(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                string roomId = null;
                if (message.Contains(":"))
                {
                    roomId = message.Split(':')[1];
                }

                bool success = roomManager.JoinRoom(clientSocket, roomId);

                if (success)
                {
                    var room = roomManager.GetPlayerRoom(clientSocket);
                    string response = $"ROOM_JOINED:{room.RoomId}:{room.Players.Count}";
                    byte[] data = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(data);

                    logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} tham gia phòng {room.RoomId}");

                    // Nếu phòng đủ 2 người, bắt đầu game
                    if (room.IsFull())
                    {
                        room.IsGameStarted = true;
                        roomManager.BroadcastToRoom(room.RoomId, "GAME_START");
                        logAction?.Invoke($"Bắt đầu game trong phòng {room.RoomId}");
                    }
                }
                else
                {
                    byte[] errorData = Encoding.UTF8.GetBytes("ROOM_JOIN_FAILED");
                    clientSocket.Send(errorData);
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý tham gia phòng: {ex.Message}");
            }
        }

        // ✅ Xử lý nước đi trong game
        private void HandleGameMove(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null && room.IsGameStarted)
                {
                    // Chuyển tiếp nước đi cho đối thủ
                    roomManager.BroadcastToRoom(room.RoomId, message, clientSocket);
                    logAction?.Invoke($"Chuyển tiếp nước đi trong phòng {room.RoomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý nước đi: {ex.Message}");
            }
        }

        // ✅ Xử lý rời phòng
        private void HandleLeaveRoom(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null)
                {
                    string roomId = room.RoomId;
                    roomManager.LeaveRoom(clientSocket);

                    // Thông báo cho đối thủ
                    roomManager.BroadcastToRoom(roomId, "OPPONENT_LEFT");

                    logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} rời phòng {roomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý rời phòng: {ex.Message}");
            }
        }
    }
}