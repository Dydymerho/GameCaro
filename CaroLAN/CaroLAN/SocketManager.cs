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
        private string IP = "127.0.0.1";
        private bool isServer;

        public bool IsServer => isServer;

        public void CreateServer()
        {
            isServer = true;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(endpoint);
            socket.Listen(1);

            Thread acceptThread = new Thread(() =>
            {
                Socket client = socket.Accept();
                socket = client;
            });
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        public bool ConnectToServer(string ip)
        {
            try
            {
                isServer = false;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                socket.Connect(endpoint);
                return true;
            }
            catch
            {
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
        }

        public string Receive()
        {
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

        public bool IsConnected => socket != null && socket.Connected;
    }
}
