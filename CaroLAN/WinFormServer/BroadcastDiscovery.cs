using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WinFormServer
{
    public class BroadcastDiscovery
    {
        private const int BROADCAST_PORT = 9998;
        private const int BROADCAST_INTERVAL = 3000;
        
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
        
        public void Start()
        {
            if (isRunning)
            {
                return;
            }
            
            try
            {
                udpClient = new UdpClient();
                udpClient.EnableBroadcast = true;
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
        
        private void BroadcastLoop()
        {
            while (isRunning)
            {
                try
                {
                    string localIP = GetLocalIPAddress();
                    
                    string message = $"GAMECARO_SERVER:{serverName}:{localIP}:{gamePort}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    
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
        
        private string GetLocalIPAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                
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
