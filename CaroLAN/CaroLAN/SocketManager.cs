using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CaroLAN
{
    public class SocketManager
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

                // ✅ Cho phép tái sử dụng địa chỉ (quan trọng khi chạy nhiều client trên cùng máy)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
                // ✅ BẬT TCP KEEPALIVE để duy trì kết nối
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                
                // ✅ Cấu hình keepalive: bắt đầu sau 60s, gửi mỗi 30s
                byte[] keepAliveValues = new byte[12];
                BitConverter.GetBytes((uint)1).CopyTo(keepAliveValues, 0);  // enable
                BitConverter.GetBytes((uint)60000).CopyTo(keepAliveValues, 4);  // keepalivetime (ms)
                BitConverter.GetBytes((uint)30000).CopyTo(keepAliveValues, 8);  // keepaliveinterval (ms)
                socket.IOControl(IOControlCode.KeepAliveValues, keepAliveValues, null);
                
                
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000); // ❌ Bỏ
                
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

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
            try
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
            catch (SocketException)
            {
                isConnected = false;
                throw; // ✅ Throw lại để caller biết có lỗi
            }
            catch (ObjectDisposedException)
            {
                isConnected = false;
                throw;
            }
        }

        public string Receive()
        {
            try
            {
                if (socket == null || !socket.Connected)
                {
                    isConnected = false;
                    return string.Empty;
                }
                
                // ✅ Kiểm tra xem có dữ liệu sẵn sàng không trước khi receive
                if (socket.Available == 0)
                {
                    bool hasData = socket.Poll(100000, SelectMode.SelectRead); // 100ms
                    
                    if (!hasData)
                    {
                        return string.Empty;
                    }
                    
                    // ✅ Sau khi poll trả về true, kiểm tra lại Available
                    // Nếu Poll = true nhưng Available = 0 → Socket đã bị đóng
                    if (socket.Available == 0)
                    {
                        isConnected = false;
                        return string.Empty;
                    }
                }
                // Đọc toàn bộ dữ liệu hiện có trên socket (nhiều chunk có thể được gửi từ server)
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                byte[] buffer = new byte[2048];

                // Đảm bảo ít nhất một lần receive khi Poll/Available cho biết có dữ liệu
                do
                {
                    int recv = socket.Receive(buffer);
                    if (recv == 0)
                    {
                        // Server đã đóng kết nối
                        isConnected = false;
                        break;
                    }

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, recv));
                    // Tiếp tục vòng lặp nếu vẫn còn dữ liệu chờ (socket.Available > 0)
                } while (socket.Available > 0);

                string data = sb.ToString();
                return data;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.WouldBlock && 
                    ex.SocketErrorCode != SocketError.TimedOut)
                {
                    isConnected = false;
                }
                return string.Empty;
            }
            catch (ObjectDisposedException)
            {
                // Socket đã bị dispose
                isConnected = false;
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Các lỗi khác
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
                  
                    if (!socket.Connected)
                    {
                        isConnected = false;
                        return false;
                    }
                    
                    if (!isConnected)
                    {
                        return false;
                    }
                    
                    bool hasReadEvent = socket.Poll(1, SelectMode.SelectRead);
                    
                    if (hasReadEvent && socket.Available == 0)
                    {
                        isConnected = false;
                        return false;
                    }
                    
                    return true;
                }
                catch (SocketException ex)
                {
                    isConnected = false;
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    isConnected = false;
                    return false;
                }
                catch (Exception ex)
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

        // Lấy thông tin về địa chỉ và cổng của server đang kết nối.
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

        // ✅ Lấy địa chỉ local endpoint của chính client này
        public string GetLocalEndPoint()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    return socket.LocalEndPoint?.ToString() ?? "Unknown";
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
