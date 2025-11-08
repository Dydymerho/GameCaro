using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CaroLAN
{
    internal class SocketManager
    {
        public const int PORT = 9999;
        private Socket socket;
        private bool isConnected = false;



        public bool ConnectToServer(string ip)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    Disconnect();
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // time out 5s
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);

                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                socket.Connect(serverEndpoint);

                if (socket.Connected)
                {
                    isConnected = true;
                    return true;
                }

                return false;
            }
            catch (ArgumentNullException)
            {
                // IP null hoặc không hợp lệ
                Disconnect();
                return false;
            }
            catch (FormatException)
            {
                // Định dạng IP không đúng
                Disconnect();
                return false;
            }
            catch (SocketException)
            {
                // Lỗi kết nối mạng
                Disconnect();
                return false;
            }
            catch (Exception)
            {
                // Lỗi khác
                Disconnect();
                return false;
            }
        }

        public void Send(string message)
        {
            if (socket != null && socket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.Send(data);
            }
            else             
            {
                isConnected = false;
            }
        }

        public string Receive()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int recv = socket.Receive(buffer);
                return Encoding.UTF8.GetString(buffer, 0, recv);

                // Nếu nhận 0 byte, server đã đóng kết nối
                if (recv == 0)
                {
                    isConnected = false;
                    return string.Empty;
                }
            }
            catch
            {
                isConnected = false;

                return string.Empty;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (socket == null)
                {
                    isConnected = false;
                    return false;
                }

                try
                {
                    
                    bool isDisconnected = socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0;

                    bool socketConnected = socket.Connected;

                    isConnected = socketConnected && !isDisconnected;
                    return isConnected;
                }
                catch (SocketException)
                {
                    isConnected = false;
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    isConnected = false;
                    return false;
                }
                catch (Exception)
                {
                    isConnected = false;
                    return false;
                }
            }
        }

        public void SendMove(int x, int y)
        {
            Send($"GAME_MOVE:{x},{y}");
        }

        public bool IsSocketConnected()
        {
            return socket != null && socket.Connected && isConnected;
        }

        public string GetServerEndPoint()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    return socket.RemoteEndPoint?.ToString() ?? "Unknown";
                }
                return "Not connected";
            }
            catch
            {
                return "Error";
            }
        }

        public void Disconnect()
        {
            try
            {
                isConnected = false;

                if (socket != null)
                {
                    // Ngắt kết nối hai chiều (send và receive)
                    if (socket.Connected)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException)
                        {
                            // Socket có thể đã bị ngắt kết nối từ phía server
                        }
                    }

                    // Đóng socket
                    socket.Close();

                    // Giải phóng tài nguyên
                    socket.Dispose();
                    socket = null;
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket đã được dispose
                socket = null;
            }
            catch (Exception)
            {
                // Bỏ qua các lỗi khác khi ngắt kết nối
                socket = null;
            }
        }
    }
}
