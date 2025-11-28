using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CaroLAN
{
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

    public class ServerDiscoveryClient
    {
        private const int BROADCAST_PORT = 9998;
        private const int DISCOVERY_TIMEOUT = 5000;
        
        private UdpClient? udpClient;
        private Thread? listenThread;
        private bool isDiscovering = false;
        private Dictionary<string, DiscoveredServer> discoveredServers;
        private object lockObject = new object();
        
        public ServerDiscoveryClient()
        {
            discoveredServers = new Dictionary<string, DiscoveredServer>();
        }
        
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
                
                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, BROADCAST_PORT));
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
        
        private void ListenForBroadcasts(Action<DiscoveredServer>? onServerFound, 
                                        Action<List<DiscoveredServer>>? onDiscoveryComplete)
        {
            DateTime startTime = DateTime.Now;
            
            while (isDiscovering)
            {
                try
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds >= DISCOVERY_TIMEOUT)
                    {
                        break;
                    }
                    
                    if (udpClient == null || udpClient.Available == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(data);
                    
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
                                    
                                    onServerFound?.Invoke(server);
                                }
                                else
                                {
                                    discoveredServers[key].LastSeen = DateTime.Now;
                                }
                            }
                        }
                    }
                }
                catch (SocketException)
                {
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
            
            List<DiscoveredServer> serverList;
            lock (lockObject)
            {
                serverList = new List<DiscoveredServer>(discoveredServers.Values);
            }
            
            onDiscoveryComplete?.Invoke(serverList);
            
            try
            {
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;
            }
            catch { }
            
            isDiscovering = false;
        }
        
        public List<DiscoveredServer> GetDiscoveredServers()
        {
            lock (lockObject)
            {
                return new List<DiscoveredServer>(discoveredServers.Values);
            }
        }
    }
}
