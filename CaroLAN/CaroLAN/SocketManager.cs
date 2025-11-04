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



        public bool ConnectToServer(string ip)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverendpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                socket.Connect(serverendpoint);
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

        public bool IsConnected
        {
            get
            {
                try
                {
                    // Kiểm tra trạng thái kết nối thực tế
                    return socket != null && !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
                }
                catch
                {
                    return false;
                }
            }
        }

        
    }
}
