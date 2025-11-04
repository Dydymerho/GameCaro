using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WinFormServer
{
    internal class ServerSocketManager
    {
        public const int PORT = 9999;
        private Socket socket;
        private string IP = "127.0.0.1";
        private List<Socket> clients = new List<Socket>();
        private List<Thread> threads = new List<Thread>();

        public void CreateServer(Action<string> logAction)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(endpoint);
            socket.Listen(100);

            logAction?.Invoke($"Server đang lắng nghe trên cổng {PORT}...");


            Thread acceptThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        logAction?.Invoke("Đang chờ kết nối từ client...");
                        Socket client = socket.Accept();
                        lock (clients)
                        {
                            clients.Add(client);
                        }

                        Thread clientThread = new Thread(() => HandleClient(client,  logAction));
                        logAction?.Invoke($"Client {client.RemoteEndPoint} đã kết nối.");
                        clientThread.IsBackground = true;
                        clientThread.Start();
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
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break; // Client ngắt kết nối

                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    logAction?.Invoke($"Nhận từ client {clientSocket.RemoteEndPoint}: {message}");


                    // Gửi phản hồi lại cho client (tùy chọn)
                    string response = $"Server đã nhận: {message}";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(responseData);
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý client {clientSocket.RemoteEndPoint}: {ex.Message}");
                Console.WriteLine($"Lỗi khi xử lý client: {ex.Message}");
            }
            finally
            {
                // Loại bỏ client khỏi danh sách và đóng kết nối
                lock (clients)
                {
                    clients.Remove(clientSocket);
                }
                clientSocket.Close();
                logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} đã ngắt kết nối.");
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

                //dong tat ca ket noi client
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        client.Close();
                    }
                    clients.Clear();
                }

                //dong socket server
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
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


    }
}