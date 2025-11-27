using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CaroLAN
{
    /// <summary>
    /// Thông tin về một server được phát hiện
    /// </summary>
    public class DiscoveredServer
    {
        public string ServerName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public DateTime LastSeen { get; set; }

        public DiscoveredServer(string serverName, string ipAddress, int port)
        {
            ServerName = serverName;
            IPAddress = ipAddress;
            Port = port;
            LastSeen = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{ServerName} ({IPAddress}:{Port})";
        }
    }

    /// <summary>
    /// Class để client tìm kiếm server trong mạng LAN thông qua broadcast
    /// </summary>
    public class ServerDiscoveryClient
    {
        private const int BROADCAST_PORT = 9998; // Port dùng cho broadcast
        private const int DISCOVERY_TIMEOUT = 5000; // Thời gian quét
        
        private UdpClient? udpClient;
        private Thread? listenThread;
        private bool isDiscovering = false;
        private Dictionary<string, DiscoveredServer> discoveredServers;
        private object lockObject = new object();
        
        public ServerDiscoveryClient()
        {
            discoveredServers = new Dictionary<string, DiscoveredServer>();
        }
        
        /// <summary>
        /// Bắt đầu quét tìm server trong mạng LAN
        /// </summary>
        /// <param name="onServerFound">Callback được gọi khi tìm thấy server mới</param>
        /// <param name="onDiscoveryComplete">Callback được gọi khi quét xong</param>
        public void StartDiscovery(Action<DiscoveredServer>? onServerFound = null, 
                                   Action<List<DiscoveredServer>>? onDiscoveryComplete = null)
        {
            if (isDiscovering)
            {
                return;
            }
            
            try
            {
                lock (lockObject)
                {
                    discoveredServers.Clear();
                }
                
                udpClient = new UdpClient(BROADCAST_PORT);
                udpClient.EnableBroadcast = true;
                isDiscovering = true;
                
                listenThread = new Thread(() => ListenForBroadcasts(onServerFound, onDiscoveryComplete));
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi bắt đầu discovery: {ex.Message}");
                isDiscovering = false;
            }
        }
        
        /// <summary>
        /// Dừng quét
        /// </summary>
        public void StopDiscovery()
        {
            isDiscovering = false;
            
            try
            {
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
                
                if (listenThread != null && listenThread.IsAlive)
                {
                    listenThread.Join(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi dừng discovery: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Lắng nghe broadcast từ server
        /// </summary>
        private void ListenForBroadcasts(Action<DiscoveredServer>? onServerFound, 
                                        Action<List<DiscoveredServer>>? onDiscoveryComplete)
        {
            DateTime startTime = DateTime.Now;
            
            while (isDiscovering)
            {
                try
                {
                    // Kiểm tra timeout
                    if ((DateTime.Now - startTime).TotalMilliseconds >= DISCOVERY_TIMEOUT)
                    {
                        break;
                    }
                    
                    // Kiểm tra có dữ liệu không (với timeout ngắn)
                    if (udpClient == null || !udpClient.Available.Equals(0) == false)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    
                    // Nhận broadcast
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(data);
                    
                    // Parse message: "GAMECARO_SERVER:<server_name>:<local_ip>:<game_port>"
                    if (message.StartsWith("GAMECARO_SERVER:"))
                    {
                        string[] parts = message.Split(':');
                        if (parts.Length >= 4)
                        {
                            string serverName = parts[1];
                            string ipAddress = parts[2];
                            int port = int.Parse(parts[3]);
                            
                            string key = $"{ipAddress}:{port}";
                            
                            lock (lockObject)
                            {
                                bool isNewServer = !discoveredServers.ContainsKey(key);
                                
                                if (isNewServer)
                                {
                                    DiscoveredServer server = new DiscoveredServer(serverName, ipAddress, port);
                                    discoveredServers[key] = server;
                                    
                                    // Gọi callback khi tìm thấy server mới
                                    onServerFound?.Invoke(server);
                                }
                                else
                                {
                                    // Cập nhật thời gian thấy lần cuối
                                    discoveredServers[key].LastSeen = DateTime.Now;
                                }
                            }
                        }
                    }
                }
                catch (SocketException)
                {
                    // Timeout hoặc lỗi socket, tiếp tục
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    if (isDiscovering)
                    {
                        Console.WriteLine($"Lỗi khi nhận broadcast: {ex.Message}");
                    }
                }
            }
            
            // Quét xong, gọi callback
            List<DiscoveredServer> serverList;
            lock (lockObject)
            {
                serverList = new List<DiscoveredServer>(discoveredServers.Values);
            }
            
            onDiscoveryComplete?.Invoke(serverList);
            
            // Dọn dẹp
            try
            {
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
            }
            catch { }
            
            isDiscovering = false;
        }
        
        /// <summary>
        /// Lấy danh sách server đã phát hiện
        /// </summary>
        public List<DiscoveredServer> GetDiscoveredServers()
        {
            lock (lockObject)
            {
                return new List<DiscoveredServer>(discoveredServers.Values);
            }
        }
    }
}
