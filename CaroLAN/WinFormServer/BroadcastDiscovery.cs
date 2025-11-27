using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WinFormServer
{
    
    /// Class để server gửi broadcast thông báo địa chỉ IP trong mạng LAN
    /// Các client có thể lắng nghe để tự động tìm server
    /// </summary>
    public class BroadcastDiscovery
    {
        private const int BROADCAST_PORT = 9998; // Port dùng cho broadcast
        private const int BROADCAST_INTERVAL = 3000; // Gửi broadcast mỗi 3 giây
        
        private UdpClient? udpClient;
        private Thread? broadcastThread;
        private bool isRunning = false;
        private string serverName = "GameCaro Server";
        private int gamePort = 9999;
        
        public BroadcastDiscovery(string serverName, int gamePort)
        {
            this.serverName = serverName;
            this.gamePort = gamePort;
        }
        
        
        /// Bắt đầu gửi broadcast thông báo server
        public void Start()
        {
            if (isRunning)
            {
                return;
            }
            
            try
            {
                // Bind vào địa chỉ cụ thể thay vì Any để tránh conflict
                udpClient = new UdpClient();
                udpClient.EnableBroadcast = true;
                // Cho phép gửi đến nhiều interface
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                isRunning = true;
                
                broadcastThread = new Thread(BroadcastLoop);
                broadcastThread.IsBackground = true;
                broadcastThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi khởi động broadcast: {ex.Message}");
            }
        }
        
        
        /// Dừng gửi broadcast
        public void Stop()
        {
            isRunning = false;
            
            try
            {
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
                
                if (broadcastThread != null && broadcastThread.IsAlive)
                {
                    broadcastThread.Join(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi dừng broadcast: {ex.Message}");
            }
        }
        
        
        /// Vòng lặp gửi broadcast định kỳ
        private void BroadcastLoop()
        {
            while (isRunning)
            {
                try
                {
                    // Lấy địa chỉ IP local của server
                    string localIP = GetLocalIPAddress();
                    
                    // Tạo message broadcast: "GAMECARO_SERVER:<server_name>:<local_ip>:<game_port>"
                    string message = $"GAMECARO_SERVER:{serverName}:{localIP}:{gamePort}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    
                    // Gửi broadcast đến 255.255.255.255
                    // IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT);
                    // udpClient?.Send(data, data.Length, broadcastEndpoint);
                    
                    // Gửi đến subnet broadcast 
                    string subnetBroadcast = GetSubnetBroadcast(localIP);
                    if (!string.IsNullOrEmpty(subnetBroadcast))
                    {
                        IPEndPoint subnetEndpoint = new IPEndPoint(IPAddress.Parse(subnetBroadcast), BROADCAST_PORT);
                        udpClient?.Send(data, data.Length, subnetEndpoint);
                    }
                    
                    Thread.Sleep(BROADCAST_INTERVAL);
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        Console.WriteLine($"Lỗi khi gửi broadcast: {ex.Message}");
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        
        
        /// Lấy địa chỉ IP local đầu tiên của máy tính trong mạng LAN
        private string GetLocalIPAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                
                // Tìm địa chỉ IPv4 đầu tiên không phải localhost
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork && 
                        !IPAddress.IsLoopback(address))
                    {
                        return address.ToString();
                    }
                }
            }
            catch { }
            
            return "127.0.0.1";
        }
        
        
        /// Tính subnet broadcast address 
        /// VD: 192.168.1.100 → 192.168.1.255
        private string GetSubnetBroadcast(string ipAddress)
        {
            try
            {
                string[] parts = ipAddress.Split('.');
                if (parts.Length == 4)
                {
                    return $"{parts[0]}.{parts[1]}.{parts[2]}.255";
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
