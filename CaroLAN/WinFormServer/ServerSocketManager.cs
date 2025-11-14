using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

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
        private Action<string> globalLogAction; // ✅ Lưu log action
        private Action globalUpdateClientListAction; // ✅ Lưu update action
        private UserManager userManager; // ✅ Quản lý user
        private ConcurrentDictionary<Socket, User> authenticatedUsers; // ✅ Lưu user đã đăng nhập

        public ServerSocketManager(UserManager userManager)
        {
            this.userManager = userManager;
            roomManager = new RoomManager();
            invitations = new ConcurrentDictionary<string, GameInvitation>();
            authenticatedUsers = new ConcurrentDictionary<Socket, User>();
            
            // Khởi tạo timer để dọn dẹp lời mời hết hạn mỗi 2 giây
            invitationCleanupTimer = new System.Threading.Timer(CleanupExpiredInvitations, null, 2000, 2000);
        }

        //xoa lời mời hết hạn
        private void CleanupExpiredInvitations(object state)
        {
            var expired = invitations.Values.Where(inv => !inv.IsValid()).ToList();

            foreach (var inv in expired)
            {
                if (invitations.TryRemove(inv.InvitationId, out _))
                {
                    // Báo cho người gửi & người nhận
                    SendToClient(inv.Sender, $"INVITATION_EXPIRED:{inv.InvitationId}");
                    SendToClient(inv.Receiver, $"INVITATION_EXPIRED:{inv.InvitationId}");
                }
            }
        }


        public void CreateServer(Action<string> logAction, Action updateClientList)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(endpoint);
            socket.Listen(100);
            isRunning = true;

            globalLogAction = logAction;
            globalUpdateClientListAction = updateClientList;

            logAction?.Invoke($"Server đang lắng nghe trên cổng {PORT}...");

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
                        
                        // Cập nhật danh sách khi có client mới kết nối
                        SendClientListToAll(logAction);
                        updateClientList?.Invoke();

                        Thread clientThread = new Thread(() => HandleClient(client, logAction));
                        logAction?.Invoke($"Client {client.RemoteEndPoint} đã kết nối.");
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    catch (SocketException)
                    {
                        if (!isRunning) return;
                    }
                    catch (Exception ex)
                    {
                        logAction?.Invoke($"Lỗi khi chấp nhận kết nối: {ex.Message}");
                    }
                }
            });

            acceptThread.IsBackground = true;
            acceptThread.Start();
            lock (threads) threads.Add(acceptThread);
        }

        private void HandleClient(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                while (isRunning)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    logAction?.Invoke($"📩 Nhận từ {clientSocket.RemoteEndPoint}: {message}");

                    // ✅ Xử lý các lệnh - KHÔNG GỬI RESPONSE CHUNG NẾU ĐÃ XỬ LÝ
                    bool handled = false;
                    
                    if (message.StartsWith("REGISTER:"))
                    {
                        HandleRegister(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (message.StartsWith("LOGIN:"))
                    {
                        HandleLogin(clientSocket, message, logAction);
                        handled = true;
                    }
                    else if (!IsAuthenticated(clientSocket))
                    {
                        SendToClient(clientSocket, "AUTH_REQUIRED:Vui lòng đăng nhập trước");
                        handled = true;
                    }
                    else if (message.StartsWith("JOIN_ROOM"))
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

                    // Chỉ gửi phản hồi chung cho các message không được xử lý đặc biệt
                    // ham nay giu lai tham khao, ko co tac dung nhieu
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
                if (!isRunning) return;
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"❌ Lỗi xử lý client {clientSocket.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                roomManager.LeaveRoom(clientSocket);
                
                // Xóa các lời mời liên quan đến client này
                RemoveClientInvitations(clientSocket);
                
                // ✅ Xóa user đã đăng nhập
                authenticatedUsers.TryRemove(clientSocket, out _);
                
                // Loại bỏ client khỏi danh sách và đóng kết nối
                lock (clients)
                {
                    clients.Remove(clientSocket);
                }
                clientSocket.Close();
                
                // ✅ Cập nhật danh sách khi có client ngắt kết nối
                SendClientListToAll(logAction);
                globalUpdateClientListAction?.Invoke();
            }
        }

        // ✅ Giữ nguyên hàm Broadcast
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
                            logAction?.Invoke($"Lỗi khi gửi đến {client.RemoteEndPoint}: {ex.Message}");
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

                // Dừng timer dọn dẹp lời mời
                invitationCleanupTimer?.Dispose();

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

        private void SendClientListToAll(Action<string> logAction)
        {
            List<string> connectedClients = GetConnectedClients();
            string clientListMessage = "CLIENT_LIST:" + string.Join(",", connectedClients);
            Broadcast(clientListMessage, clients, logAction);
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
                            // ✅ Lấy username nếu đã đăng nhập, nếu không thì dùng endpoint
                            string displayName = GetUsername(client);
                            
                            // ✅ Kiểm tra xem client có đang trong phòng không
                            var room = roomManager.GetPlayerRoom(client);
                            if (room != null)
                            {
                                displayName += "|BUSY"; // Đánh dấu client đang bận
                            }
                            
                            connectedClients.Add(displayName);
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

        public void DisconnectClient(string remoteEndPoint, Action<string> logAction)
        {
            lock (clients)
            {
                remoteEndPoint = remoteEndPoint.Replace("|BUSY", ""); // Loại bỏ suffix 
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

        // ✅ Cải tiến log & xử lý JOIN_ROOM
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
                    clientSocket.Send(Encoding.UTF8.GetBytes(response));

                    logAction?.Invoke($"✅ {clientSocket.RemoteEndPoint} tham gia phòng {room.RoomId} ({room.Players.Count}/2)");

                    // ✅ Cập nhật danh sách client khi có người vào phòng (trạng thái BUSY)
                    SendClientListToAll(logAction);
                    globalUpdateClientListAction?.Invoke();

                    // Khi đủ 2 người → bắt đầu game
                    if (room.IsFull() && !room.IsGameStarted)
                    {
                        room.IsGameStarted = true;
                        roomManager.BroadcastToRoom(room.RoomId, "GAME_START");
                        logAction?.Invoke($"🔥 Bắt đầu game trong phòng {room.RoomId}");
                    }
                }
                else
                {
                    clientSocket.Send(Encoding.UTF8.GetBytes("ROOM_JOIN_FAILED"));
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý JOIN_ROOM: {ex.Message}");
            }
        }

        // ✅ Truyền nước đi giữa 2 người chơi
        private void HandleGameMove(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null && room.IsGameStarted)
                {
                    roomManager.BroadcastToRoom(room.RoomId, message, clientSocket);
                    logAction?.Invoke($"➡️ Truyền nước đi trong phòng {room.RoomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi GAME_MOVE: {ex.Message}");
            }
        }

        // ✅ Khi người chơi thoát khỏi phòng
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

                    logAction?.Invoke($"👋 {clientSocket.RemoteEndPoint} rời phòng {roomId}");
                    
                    // ✅ Cập nhật danh sách client khi có người rời phòng (trở lại trạng thái rảnh)
                    SendClientListToAll(logAction);
                    globalUpdateClientListAction?.Invoke();
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi xử lý LEAVE_ROOM: {ex.Message}");
            }
        }

        //Xử lý gửi lời mời chơi game
        private void HandleSendInvitation(Socket senderSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: SEND_INVITATION:<username>
                string[] parts = message.Split(':');
                if (parts.Length < 2)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Invalid format");
                    return;
                }

                string receiverName = parts[1].Trim();

                // Tìm socket receiver theo username trước
                Socket receiverSocket = authenticatedUsers
                    .Where(x => x.Value.Username == receiverName)
                    .Select(x => x.Key)
                    .FirstOrDefault();

                if (receiverSocket == null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Client not found");
                    return;
                }

                // Không thể tự mời chính mình
                if (receiverSocket == senderSocket)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Cannot invite yourself");
                    return;
                }

                // Người gửi trong phòng? → không được gửi
                if (roomManager.GetPlayerRoom(senderSocket) != null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:You are already in a room");
                    return;
                }

                // Người nhận đang bận?
                if (roomManager.GetPlayerRoom(receiverSocket) != null)
                {
                    SendToClient(senderSocket, "INVITATION_SEND_FAILED:Receiver is busy");
                    return;
                }

                // Tạo lời mời mới
                GameInvitation invitation = new GameInvitation(senderSocket, receiverSocket);
                invitations.TryAdd(invitation.InvitationId, invitation);

                string senderName = GetUsername(senderSocket);

                // Gửi lời mời đến receiver
                SendToClient(receiverSocket,
                    $"INVITATION_RECEIVED:{invitation.InvitationId}:{senderName}");

                // Gửi thông báo cho sender
                SendToClient(senderSocket,
                    $"INVITATION_SENT:{invitation.InvitationId}:{receiverName}");

                logAction?.Invoke($"📨 {senderName} gửi lời mời đến {receiverName}");
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi HandleSendInvitation: {ex.Message}");
                SendToClient(senderSocket, "INVITATION_SEND_FAILED:Server error");
            }
        }


        //Xử lý chấp nhận lời mời
        private void HandleAcceptInvitation(Socket receiverSocket, string message, Action<string> logAction)
        {
            try
            {
                string[] parts = message.Split(':');
                if (parts.Length < 2)
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invalid format");
                    return;
                }

                string invitationId = parts[1];

                if (!invitations.TryRemove(invitationId, out GameInvitation invitation))
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invitation not found");
                    return;
                }

                // Sai người nhận?
                if (invitation.Receiver != receiverSocket)
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invalid receiver");
                    return;
                }

                // Hết hạn?
                if (!invitation.IsValid())
                {
                    SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Invitation expired");
                    return;
                }

                // Tạo phòng mới
                string roomId = roomManager.CreateRoom();
                roomManager.JoinRoom(invitation.Sender, roomId);
                roomManager.JoinRoom(invitation.Receiver, roomId);

                // Gửi thông báo ACCEPTED cho cả hai
                SendToClient(invitation.Sender, $"INVITATION_ACCEPTED:{invitationId}:{roomId}");
                SendToClient(invitation.Receiver, $"INVITATION_ACCEPTED:{invitationId}:{roomId}");

                // Bắt đầu game
                roomManager.BroadcastToRoom(roomId, "GAME_START");

                logAction?.Invoke($"✔ Lời mời {invitationId} được chấp nhận → tạo phòng {roomId}");

                // Cập nhật lại danh sách client (BUSY)
                SendClientListToAll(logAction);
                globalUpdateClientListAction?.Invoke();
            }
            catch (Exception ex)
            {
                SendToClient(receiverSocket, "INVITATION_ACCEPT_FAILED:Server error");
                logAction?.Invoke("Lỗi HandleAcceptInvitation: " + ex.Message);
            }
        }


        //Xử lý từ chối lời mời
        private void HandleRejectInvitation(Socket receiverSocket, string message, Action<string> logAction)
        {
            try
            {
                string[] parts = message.Split(':');
                if (parts.Length < 2) return;

                string invitationId = parts[1];

                if (invitations.TryRemove(invitationId, out GameInvitation invitation))
                {
                    SendToClient(invitation.Sender, $"INVITATION_REJECTED:{invitationId}");
                    logAction?.Invoke($"❌ Lời mời {invitationId} bị từ chối");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke("Lỗi HandleRejectInvitation: " + ex.Message);
            }
        }

        //Xóa các lời mời liên quan đến client
        private void RemoveClientInvitations(Socket clientSocket)
        {
            var related = invitations.Values
                .Where(inv => inv.Sender == clientSocket || inv.Receiver == clientSocket)
                .ToList();

            foreach (var inv in related)
            {
                if (invitations.TryRemove(inv.InvitationId, out _))
                {
                    Socket other = inv.Sender == clientSocket ? inv.Receiver : inv.Sender;

                    SendToClient(other, $"INVITATION_CANCELLED:{inv.InvitationId}");
                }
            }
            SendClientListToAll(globalLogAction);
            globalUpdateClientListAction?.Invoke();

        }


        //Helper method để gửi tin nhắn đến một client cụ thể
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

        // ✅ Xử lý đăng ký
        private void HandleRegister(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: REGISTER:username:password:email
                string[] parts = message.Split(':');
                if (parts.Length < 3)
                {
                    SendToClient(clientSocket, "REGISTER_FAILED:Định dạng không hợp lệ");
                    return;
                }

                string username = parts[1];
                string password = parts[2];
                string email = parts.Length > 3 ? parts[3] : "";

                // Validate
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    SendToClient(clientSocket, "REGISTER_FAILED:Username và password không được để trống");
                    return;
                }

                if (username.Length < 3 || username.Length > 50)
                {
                    SendToClient(clientSocket, "REGISTER_FAILED:Username phải từ 3-50 ký tự");
                    return;
                }

                if (password.Length < 6)
                {
                    SendToClient(clientSocket, "REGISTER_FAILED:Mật khẩu phải có ít nhất 6 ký tự");
                    return;
                }

                // Đăng ký
                bool success = userManager.Register(username, password, email);
                if (success)
                {
                    SendToClient(clientSocket, "REGISTER_SUCCESS:Đăng ký thành công");
                    logAction?.Invoke($"✅ User đăng ký: {username}");
                }
                else
                {
                    SendToClient(clientSocket, "REGISTER_FAILED:Username đã tồn tại");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi xử lý đăng ký: {ex.Message}");
                SendToClient(clientSocket, "REGISTER_FAILED:Lỗi server");
            }
        }

        // ✅ Xử lý đăng nhập
        private void HandleLogin(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                // Format: LOGIN:username:password
                string[] parts = message.Split(':');
                if (parts.Length < 3)
                {
                    SendToClient(clientSocket, "LOGIN_FAILED:Định dạng không hợp lệ");
                    return;
                }

                string username = parts[1];
                string password = parts[2];

                // Đăng nhập
                User? user = userManager.Login(username, password);
                if (user != null)
                {
                    // Lưu thông tin user đã đăng nhập
                    authenticatedUsers[clientSocket] = user;
                    
                    // Gửi thông tin user về client
                    string response = $"LOGIN_SUCCESS:{user.Id}:{user.Username}:{user.TotalGames}:{user.Wins}:{user.Losses}";
                    SendToClient(clientSocket, response);
                    
                    logAction?.Invoke($"✅ User đăng nhập: {user.Username} (ID: {user.Id})");
                    
                    // Cập nhật danh sách client
                    SendClientListToAll(logAction);
                    globalUpdateClientListAction?.Invoke();
                }
                else
                {
                    SendToClient(clientSocket, "LOGIN_FAILED:Username hoặc password không đúng");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi xử lý đăng nhập: {ex.Message}");
                SendToClient(clientSocket, "LOGIN_FAILED:Lỗi server");
            }
        }

        // ✅ Kiểm tra đã đăng nhập chưa
        private bool IsAuthenticated(Socket clientSocket)
        {
            return authenticatedUsers.ContainsKey(clientSocket);
        }

        // ✅ Lấy username từ socket
        private string GetUsername(Socket clientSocket)
        {
            if (authenticatedUsers.TryGetValue(clientSocket, out User? user))
            {
                return user.Username;
            }
            return clientSocket.RemoteEndPoint?.ToString() ?? "Unknown";
        }
    }
}
