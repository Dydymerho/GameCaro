using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

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
        private RoomManager roomManager;

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
                        updateClientList?.Invoke();

                        Thread clientThread = new Thread(() => HandleClient(client, logAction));
                        logAction?.Invoke($"Client {client.RemoteEndPoint} đã kết nối.");
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    catch (SocketException)
                    {
                        if (!isRunning) return;
                    }
                    catch (Exception ex)
                    {
                        logAction?.Invoke($"Lỗi khi chấp nhận kết nối: {ex.Message}");
                    }
                }
            });

            acceptThread.IsBackground = true;
            acceptThread.Start();
            lock (threads) threads.Add(acceptThread);
        }

        private void HandleClient(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                while (isRunning)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    logAction?.Invoke($"📩 Nhận từ {clientSocket.RemoteEndPoint}: {message}");

                    // ✅ Xử lý các lệnh chính
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

                    // ✅ Tùy chọn: phản hồi echo để debug
                    // string response = $"Server đã nhận: {message}";
                    // clientSocket.Send(Encoding.UTF8.GetBytes(response));
                }
            }
            catch (SocketException)
            {
                if (!isRunning) return;
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"❌ Lỗi xử lý client {clientSocket.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                roomManager.LeaveRoom(clientSocket);
                SendClientListToAll(logAction);
                lock (clients)
                {
                    clients.Remove(clientSocket);
                }
                clientSocket.Close();
            }
        }

        // ✅ Giữ nguyên hàm Broadcast
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
                            logAction?.Invoke($"Lỗi khi gửi đến {client.RemoteEndPoint}: {ex.Message}");
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

        private void SendClientListToAll(Action<string> logAction)
        {
            List<string> connectedClients = GetConnectedClients();
            string clientListMessage = "CLIENT_LIST:" + string.Join(",", connectedClients);
            Broadcast(clientListMessage, clients, logAction);
        }

        public List<string> GetConnectedClients()
        {
            lock (clients)
            {
                return clients
                    .Where(c => c.Connected)
                    .Select(c => c.RemoteEndPoint.ToString())
                    .ToList();
            }
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

        // ✅ Cải tiến log & xử lý JOIN_ROOM
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
                    clientSocket.Send(Encoding.UTF8.GetBytes(response));

                    logAction?.Invoke($"✅ {clientSocket.RemoteEndPoint} tham gia phòng {room.RoomId} ({room.Players.Count}/2)");

                    // Khi đủ 2 người → bắt đầu game
                    if (room.IsFull() && !room.IsGameStarted)
                    {
                        room.IsGameStarted = true;
                        roomManager.BroadcastToRoom(room.RoomId, "GAME_START");
                        logAction?.Invoke($"🔥 Bắt đầu game trong phòng {room.RoomId}");
                    }
                }
                else
                {
                    clientSocket.Send(Encoding.UTF8.GetBytes("ROOM_JOIN_FAILED"));
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý JOIN_ROOM: {ex.Message}");
            }
        }

        // ✅ Truyền nước đi giữa 2 người chơi
        private void HandleGameMove(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null && room.IsGameStarted)
                {
                    roomManager.BroadcastToRoom(room.RoomId, message, clientSocket);
                    logAction?.Invoke($"➡️ Truyền nước đi trong phòng {room.RoomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi GAME_MOVE: {ex.Message}");
            }
        }

        // ✅ Khi người chơi thoát khỏi phòng
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

                    logAction?.Invoke($"👋 {clientSocket.RemoteEndPoint} rời phòng {roomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý LEAVE_ROOM: {ex.Message}");
            }
        }
    }
}
