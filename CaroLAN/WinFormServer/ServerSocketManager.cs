using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace WinFormServer
{
    internal class ServerSocketManager
    {
        public const int PORT = 9999;
        private Socket socket;
        private string IP = "127.0.0.1";
        private List<Socket> clients = new List<Socket>();
        private List<Thread> threads = new List<Thread>();
        private bool isRunning = false;
        private RoomManager roomManager;
        private ConcurrentDictionary<string, GameInvitation> invitations; // ✅ Quản lý lời mời
        private System.Threading.Timer invitationCleanupTimer; // ✅ Timer dọn dẹp lời mời hết hạn
        private System.Threading.Timer clientListUpdateTimer; // ✅ Timer cập nhật danh sách client
        private Action<string> globalLogAction; // ✅ Lưu log action
        private Action globalUpdateClientListAction; // ✅ Lưu update action

        public ServerSocketManager()
        {
            roomManager = new RoomManager();
            invitations = new ConcurrentDictionary<string, GameInvitation>();
            
            // Khởi tạo timer để dọn dẹp lời mời hết hạn mỗi 2 giây
            invitationCleanupTimer = new System.Threading.Timer(CleanupExpiredInvitations, null, 2000, 2000);
        }

        // ✅ Dọn dẹp lời mời hết hạn
        private void CleanupExpiredInvitations(object state)
        {
            var expiredInvitations = invitations.Values
                .Where(inv => !inv.IsValid())
                .ToList();

            foreach (var invitation in expiredInvitations)
            {
                if (invitations.TryRemove(invitation.InvitationId, out _))
                {
                    // Thông báo cho receiver rằng lời mời đã hết hạn
                    try
                    {
                        if (invitation.Receiver != null && invitation.Receiver.Connected)
                        {
                            string message = $"INVITATION_EXPIRED:{invitation.InvitationId}:{invitation.SenderEndPoint}";
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            invitation.Receiver.Send(data);
                        }
                    }
                    catch { }
                }
            }
        }

        // ✅ Cập nhật danh sách client định kỳ
        private void UpdateClientListPeriodically(object state)
        {
            if (!isRunning) return;

            try
            {
                // Cập nhật danh sách trên giao diện server
                globalUpdateClientListAction?.Invoke();

                // Gửi danh sách client đến tất cả client
                SendClientListToAll(globalLogAction);
            }
            catch (Exception ex)
            {
                globalLogAction?.Invoke($"Lỗi khi cập nhật danh sách client: {ex.Message}");
            }
        }

        public void CreateServer(Action<string> logAction, Action updateClientList)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(endpoint);
            socket.Listen(100);
            isRunning = true;

            // ✅ Lưu các action để dùng trong timer
            globalLogAction = logAction;
            globalUpdateClientListAction = updateClientList;

            logAction?.Invoke($"Server đang lắng nghe trên cổng {PORT}...");

            // ✅ Khởi tạo timer cập nhật danh sách client mỗi 1 giây
            //clientListUpdateTimer = new System.Threading.Timer(UpdateClientListPeriodically, null, 1000, 1000);

            Thread acceptThread = new Thread(() =>
            {
                while (isRunning)
                {
                    try
                    {
                        logAction?.Invoke("Đang chờ kết nối từ client...");
                        Socket client = socket.Accept();
                        lock (clients)
                        {
                            clients.Add(client);
                        }
                        SendClientListToAll(logAction);
                        updateClientList.Invoke();

                        Thread clientThread = new Thread(() => HandleClient(client,  logAction));
                        logAction?.Invoke($"Client {client.RemoteEndPoint} đã kết nối.");
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    catch (SocketException)
                    {
                        if (!isRunning) return; // Thoát nếu server đã dừng
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
                while (isRunning)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break; // Client ngắt kết nối
                      
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    logAction?.Invoke($"Nhận từ client {clientSocket.RemoteEndPoint}: {message}");

                    // ✅ Xử lý các lệnh - KHÔNG GỬI RESPONSE CHUNG NẾU ĐÃ XỬ LÝ
                    bool handled = false;
                    
                    if (message.StartsWith("JOIN_ROOM"))
                    {
                        HandleJoinRoom(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message.StartsWith("GAME_MOVE:"))
                    {
                        HandleGameMove(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message == "LEAVE_ROOM")
                    {
                        HandleLeaveRoom(clientSocket, logAction);
                        handled = true;
                    }
                    else if (message.StartsWith("SEND_INVITATION:"))
                    {
                        HandleSendInvitation(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message.StartsWith("ACCEPT_INVITATION:"))
                    {
                        HandleAcceptInvitation(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message.StartsWith("REJECT_INVITATION:"))
                    {
                        HandleRejectInvitation(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message == "DISCONNECT")
                    {
                        // Client yêu cầu ngắt kết nối
                        logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} yêu cầu ngắt kết nối.");
                        handled = true;
                        break;
                    }

                    // ✅ Chỉ gửi phản hồi chung cho các message không được xử lý đặc biệt
                    if (!handled && !string.IsNullOrEmpty(message))
                    {
                        string response = $"Server đã nhận: {message}";
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        try
                        {
                            clientSocket.Send(responseData);
                        }
                        catch
                        {
                            // Client có thể đã ngắt kết nối
                        }
                    }
                }
            }
            catch (SocketException)
            {
                if (!isRunning) return; // Thoát nếu server đã dừng
            }
            
            catch (Exception ex)
            {
                //logAction?.Invoke($"Lỗi khi xử lý client {clientSocket.RemoteEndPoint}: {ex.Message}");
                Console.WriteLine($"Lỗi khi xử lý client: {ex.Message}");
            }
            finally
            {
                roomManager.LeaveRoom(clientSocket);
                
                // Xóa các lời mời liên quan đến client này
                RemoveClientInvitations(clientSocket);
                
                SendClientListToAll(logAction);

                // Loại bỏ client khỏi danh sách và đóng kết nối
                lock (clients)
                {
                    clients.Remove(clientSocket);
                }
                clientSocket.Close();
                //logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} đã ngắt kết nối.");
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
                Broadcast("SERVER_STOPPED", clients, logAction);

                // Dừng các timer
                invitationCleanupTimer?.Dispose();
                clientListUpdateTimer?.Dispose();

                //dong tat ca ket noi client
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        client.Close();
                    }
                    clients.Clear();
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

                isRunning = false;

                //dong socket server
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
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
                            string endpoint = client.RemoteEndPoint.ToString();
                            
                            // ✅ Kiểm tra xem client có đang trong phòng không
                            var room = roomManager.GetPlayerRoom(client);
                            if (room != null)
                            {
                                endpoint += "|BUSY"; // Đánh dấu client đang bận
                            }
                            
                            connectedClients.Add(endpoint);
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
        private void SendClientListToAll(Action<string> logAction)
        {
            // Lấy danh sách các client hiện tại
            List<string> connectedClients = GetConnectedClients();
            string clientListMessage = "CLIENT_LIST:" + string.Join(",", connectedClients);

            // Gửi danh sách client đến tất cả các client
            Broadcast(clientListMessage, clients, logAction);
        }

        public void DisconnectClient(string remoteEndPoint, Action<string> logAction)
        {
            lock (clients)
            {
                // Tìm client dựa trên RemoteEndPoint
                var client = clients.FirstOrDefault(c => c.RemoteEndPoint?.ToString() == remoteEndPoint);
                if (client != null)
                {
                    try
                    {
                        //gui tin hieu ngat ket noi
                        byte[] disconnectMessage = Encoding.UTF8.GetBytes("SERVER_STOPPED");
                        client.Send(disconnectMessage);
                        //dong ket noi client
                        client.Close();
                        clients.Remove(client);
                        logAction?.Invoke($"Client {remoteEndPoint} đã bị ngắt kết nối.");
                    }
                    catch (Exception ex)
                    {
                        logAction?.Invoke($"Lỗi khi ngắt kết nối client {remoteEndPoint}: {ex.Message}");
                    }
                }
                else
                {
                    logAction?.Invoke($"Không tìm thấy client {remoteEndPoint} để ngắt kết nối.");
                }
            }
        }

        // ✅ Xử lý tham gia phòng
        private void HandleJoinRoom(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                string roomId = null;
                if (message.Contains(":"))
                {
                    roomId = message.Split(':')[1];
                }

                bool success = roomManager.JoinRoom(clientSocket, roomId);

                if (success)
                {
                    var room = roomManager.GetPlayerRoom(clientSocket);
                    string response = $"ROOM_JOINED:{room.RoomId}:{room.Players.Count}";
                    byte[] data = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(data);

                    logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} tham gia phòng {room.RoomId}");

                    // Nếu phòng đủ 2 người, bắt đầu game
                    if (room.IsFull())
                    {
                        room.IsGameStarted = true;
                        roomManager.BroadcastToRoom(room.RoomId, "GAME_START");
                        logAction?.Invoke($"Bắt đầu game trong phòng {room.RoomId}");
                    }
                }
                else
                {
                    byte[] errorData = Encoding.UTF8.GetBytes("ROOM_JOIN_FAILED");
                    clientSocket.Send(errorData);
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý tham gia phòng: {ex.Message}");
            }
        }

        // ✅ Xử lý nước đi trong game
        private void HandleGameMove(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null && room.IsGameStarted)
                {
                    // Chuyển tiếp nước đi cho đối thủ
                    roomManager.BroadcastToRoom(room.RoomId, message, clientSocket);
                    logAction?.Invoke($"Chuyển tiếp nước đi trong phòng {room.RoomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý nước đi: {ex.Message}");
            }
        }

        // ✅ Xử lý rời phòng
        private void HandleLeaveRoom(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null)
                {
                    string roomId = room.RoomId;
                    roomManager.LeaveRoom(clientSocket);

                    // Thông báo cho đối thủ
                    roomManager.BroadcastToRoom(roomId, "OPPONENT_LEFT");

                    logAction?.Invoke($"Client {clientSocket.RemoteEndPoint} rời phòng {roomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý rời phòng: {ex.Message}");
            }
        }

        // ✅ Xử lý gửi lời mời chơi game
        private void HandleSendInvitation(Socket senderSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: SEND_INVITATION:receiverEndPoint
                string[] parts = message.Split(':');
                if (parts.Length < 2)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Invalid format");
                    return;
                }

                string receiverEndPoint = parts[1]+":"+parts[2];
                
                // ✅ Loại bỏ suffix |BUSY nếu có
                receiverEndPoint = receiverEndPoint.Replace("|BUSY", "");

                // Tìm socket của receiver
                Socket receiverSocket = null;
                lock (clients)
                {
                    receiverSocket = clients.FirstOrDefault(c => c.RemoteEndPoint?.ToString() == receiverEndPoint);
                }

                if (receiverSocket == null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Client not found");
                    logAction?.Invoke($"Không tìm thấy client {receiverEndPoint}");
                    return;
                }

                // Kiểm tra xem người gửi đã trong phòng chưa
                var senderRoom = roomManager.GetPlayerRoom(senderSocket);
                if (senderRoom != null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:You are already in a room");
                    return;
                }

                // ✅ Kiểm tra xem người nhận đã trong phòng chưa
                var receiverRoom = roomManager.GetPlayerRoom(receiverSocket);
                if (receiverRoom != null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Receiver is busy");
                    logAction?.Invoke($"Client {receiverEndPoint} đang bận");
                    return;
                }

                // Tạo lời mời mới
                var invitation = new GameInvitation(senderSocket, receiverSocket);
                if (invitations.TryAdd(invitation.InvitationId, invitation))
                {
                    // Gửi lời mời đến receiver
                    string invitationMessage = $"INVITATION_RECEIVED:{invitation.InvitationId}:{invitation.SenderEndPoint}";
                    SendToClient(receiverSocket, invitationMessage);

                    // Thông báo cho sender rằng đã gửi thành công
                    SendToClient(senderSocket, $"INVITATION_SENT:{invitation.InvitationId}:{receiverEndPoint}");

                    logAction?.Invoke($"Client {senderSocket.RemoteEndPoint} gửi lời mời đến {receiverEndPoint} (ID: {invitation.InvitationId})");
                }
                else
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Could not create invitation");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý gửi lời mời: {ex.Message}");
                SendToClient(senderSocket, "INVITATION_SEND_FAILED:Server error");
            }
        }

        // ✅ Xử lý chấp nhận lời mời
        private void HandleAcceptInvitation(Socket receiverSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: ACCEPT_INVITATION:invitationId
                string[] parts = message.Split(':');
                if (parts.Length < 2)
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invalid format");
                    return;
                }

                string invitationId = parts[1];

                if (!invitations.TryGetValue(invitationId, out GameInvitation invitation))
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invitation not found");
                    return;
                }

                // Kiểm tra tính hợp lệ
                if (!invitation.IsValid())
                {
                    invitations.TryRemove(invitationId, out _);
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invitation expired");
                    return;
                }

                // Kiểm tra người nhận có đúng không
                if (invitation.Receiver != receiverSocket)
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invalid receiver");
                    return;
                }

                // Đánh dấu đã chấp nhận và xóa khỏi danh sách
                invitation.IsAccepted = true;
                invitations.TryRemove(invitationId, out _);

                // Tạo phòng mới và cho cả hai vào
                string roomId = roomManager.CreateRoom();
                roomManager.JoinRoom(invitation.Sender, roomId);
                roomManager.JoinRoom(invitation.Receiver, roomId);

                var room = roomManager.GetPlayerRoom(invitation.Sender);
                if (room != null && room.IsFull())
                {
                    room.IsGameStarted = true;

                    // Thông báo cho cả hai người chơi
                    SendToClient(invitation.Sender, $"INVITATION_ACCEPTED:{invitationId}:{roomId}");
                    SendToClient(invitation.Receiver, $"INVITATION_ACCEPTED:{invitationId}:{roomId}");

                    // Gửi tín hiệu bắt đầu game
                    roomManager.BroadcastToRoom(roomId, "GAME_START");

                    logAction?.Invoke($"Lời mời {invitationId} được chấp nhận. Tạo phòng {roomId} cho 2 người chơi.");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý chấp nhận lời mời: {ex.Message}");
                SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Server error");
            }
        }

        // ✅ Xử lý từ chối lời mời
        private void HandleRejectInvitation(Socket receiverSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: REJECT_INVITATION:invitationId
                string[] parts = message.Split(':');
                if (parts.Length < 2)
                {
                    return;
                }

                string invitationId = parts[1];

                if (invitations.TryRemove(invitationId, out GameInvitation invitation))
                {
                    // Thông báo cho sender rằng lời mời bị từ chối
                    SendToClient(invitation.Sender, $"INVITATION_REJECTED:{invitationId}:{invitation.ReceiverEndPoint}");

                    logAction?.Invoke($"Lời mời {invitationId} bị từ chối");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý từ chối lời mời: {ex.Message}");
            }
        }

        // ✅ Xóa các lời mời liên quan đến client
        private void RemoveClientInvitations(Socket clientSocket)
        {
            var relatedInvitations = invitations.Values
                .Where(inv => inv.Sender == clientSocket || inv.Receiver == clientSocket)
                .ToList();

            foreach (var invitation in relatedInvitations)
            {
                if (invitations.TryRemove(invitation.InvitationId, out _))
                {
                    // Thông báo cho người còn lại
                    try
                    {
                        Socket otherPlayer = invitation.Sender == clientSocket ? invitation.Receiver : invitation.Sender;
                        if (otherPlayer != null && otherPlayer.Connected)
                        {
                            SendToClient(otherPlayer, $"INVITATION_CANCELLED:{invitation.InvitationId}");
                        }
                    }
                    catch { }
                }
            }
        }

        // ✅ Helper method để gửi tin nhắn đến một client cụ thể
        private void SendToClient(Socket clientSocket, string message)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    clientSocket.Send(data);
                }
            }
            catch
            {
                // Ignore sending errors
            }
        }
    }
}