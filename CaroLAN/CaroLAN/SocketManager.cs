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

                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                
                byte[] keepAliveValues = new byte[12];
                BitConverter.GetBytes((uint)1).CopyTo(keepAliveValues, 0);  
                BitConverter.GetBytes((uint)60000).CopyTo(keepAliveValues, 4);  
                BitConverter.GetBytes((uint)30000).CopyTo(keepAliveValues, 8);  
                socket.IOControl(IOControlCode.KeepAliveValues, keepAliveValues, null);
                
                
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                
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
                Disconnect();
                return false;
            }
            catch (FormatException)
            {
                Disconnect();
                return false;
            }
            catch (SocketException)
            {
                Disconnect();
                return false;
            }
            catch (Exception)
            {
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
                throw;
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
                
                if (socket.Available == 0)
                {
                    bool hasData = socket.Poll(100000, SelectMode.SelectRead);
                    
                    if (!hasData)
                    {
                        return string.Empty;
                    }
                    
                    if (socket.Available == 0)
                    {
                        isConnected = false;
                        return string.Empty;
                    }
                }
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                byte[] buffer = new byte[2048];

                do
                {
                    int recv = socket.Receive(buffer);
                    if (recv == 0)
                    {
                        isConnected = false;
                        break;
                    }

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, recv));
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
                isConnected = false;
                return string.Empty;
            }
            catch (Exception ex)
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
                    if (socket.Connected)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException)
                        {
                        }
                    }

                    socket.Close();

                    socket.Dispose();
                    socket = null;
                }
            }
            catch (ObjectDisposedException)
            {
                socket = null;
            }
            catch (Exception)
            {
                socket = null;
            }
        }
    }
}
