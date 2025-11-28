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
        private List<Socket> clients = new List<Socket>();
        private List<Thread> threads = new List<Thread>();
        private bool isRunning = false;
        private RoomManager roomManager;
        private ConcurrentDictionary<string, GameInvitation> invitations;
        private System.Threading.Timer invitationCleanupTimer;
        private Action<string> globalLogAction;
        private Action globalUpdateClientListAction;
        private UserManager userManager;
        private ConcurrentDictionary<Socket, User> authenticatedUsers;
        private ConcurrentDictionary<string, GameBoardState> roomBoards;
        
        // ✅ Thêm GameEngine để xử lý logic game
        private GameEngine gameEngine;

        public ServerSocketManager(UserManager userManager)
        {
            this.userManager = userManager;
            roomManager = new RoomManager();
            invitations = new ConcurrentDictionary<string, GameInvitation>();
            authenticatedUsers = new ConcurrentDictionary<Socket, User>();
            roomBoards = new ConcurrentDictionary<string, GameBoardState>();
            
            // ✅ Khởi tạo GameEngine (logAction sẽ được set sau)
            gameEngine = new GameEngine(userManager, null);
            
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
            
            // ✅ Cập nhật logAction cho GameEngine
            gameEngine = new GameEngine(userManager, logAction);

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
                        
                        // ✅ KHÔNG gửi CLIENT_LIST ngay khi kết nối, đợi client login xong
                        // SendClientListToAll(logAction);
                        // updateClientList?.Invoke();

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
                    else if (message == "GET_CLIENT_LIST") // ✅ Cho phép lấy danh sách TRƯỚC KHI kiểm tra đăng nhập
                    {
                        SendClientListToClient(clientSocket, logAction);
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
                    else if (message.StartsWith("CHAT:"))
                    {
                        try
                        {
                            var room = roomManager.GetPlayerRoom(clientSocket);
                            if (room != null && room.IsGameStarted)
                            {
                                string username = GetUsername(clientSocket);
                                string body = message.Substring("CHAT:".Length);
                                string outMsg = $"CHAT_FROM:{username}:{body}";
                                roomManager.BroadcastToRoom(room.RoomId, outMsg, clientSocket);
                                handled = true;
                            }
                            else
                            {
                                SendToClient(clientSocket, "CHAT_FAILED:Bạn chưa trong phòng");
                                handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            logAction?.Invoke($"Lỗi xử lý CHAT: {ex.Message}");
                        }
                    }
                    else if (message == "RESIGN") // ✅ XỬ LÝ KHI NGƯỜI CHƠI ĐẦU HÀNG
                    {
                        HandleResign(clientSocket, logAction);
                        handled = true;
                    }
                    else if (message == "LEAVE_ROOM")
                    {
                        HandleLeaveRoom(clientSocket, logAction);
                        handled = true;
                    }
                    else if (message == "GET_MY_HISTORY") // ✅ LẤY LỊCH SỬ CỦA USER
                    {
                        HandleGetMyHistory(clientSocket, logAction);
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
                // ✅ Xóa bàn cờ khi người chơi rời khỏi phòng
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room != null)
                {
                    roomBoards.TryRemove(room.RoomId, out _);
                }
                
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

        // Gửi danh sách client đến client 
        private void SendClientListToClient(Socket clientSocket, Action<string> logAction)
        {
            List<string> connectedClients = GetConnectedClients();
            string clientListMessage = "CLIENT_LIST:" + string.Join(",", connectedClients);
            SendToClient(clientSocket, clientListMessage);
            logAction?.Invoke($"📋 Gửi danh sách ({connectedClients.Count} client) đến {GetUsername(clientSocket)} ({clientSocket.RemoteEndPoint})");
            System.Diagnostics.Debug.WriteLine($"📋 CLIENT_LIST gửi: {clientListMessage}");
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

        public void DisconnectClient(string clientIdentifier, Action<string> logAction)
        {
            lock (clients)
            {
                // ✅ Loại bỏ suffix |BUSY nếu có
                clientIdentifier = clientIdentifier.Replace("|BUSY", "").Replace("[BUSY] ", "").Trim();
                
                Socket clientToDisconnect = null;
                
                // ✅ Tìm client theo username trước
                var userEntry = authenticatedUsers.FirstOrDefault(x => x.Value.Username == clientIdentifier);
                if (userEntry.Key != null)
                {
                    clientToDisconnect = userEntry.Key;
                }
                
                // ✅ Nếu không tìm thấy theo username, tìm theo RemoteEndPoint
                if (clientToDisconnect == null)
                {
                    clientToDisconnect = clients.FirstOrDefault(c => c.RemoteEndPoint?.ToString() == clientIdentifier);
                }
                
                if (clientToDisconnect != null)
                {
                    try
                    {
                        string displayName = GetUsername(clientToDisconnect);
                        
                        // Gửi tín hiệu ngắt kết nối
                        byte[] disconnectMessage = Encoding.UTF8.GetBytes("SERVER_STOPPED");
                        clientToDisconnect.Send(disconnectMessage);
                        
                        // Đóng kết nối client
                        clientToDisconnect.Close();
                        clients.Remove(clientToDisconnect);
                        
                        logAction?.Invoke($"Client {displayName} ({clientToDisconnect.RemoteEndPoint}) đã bị ngắt kết nối.");
                        
                        // ✅ Cập nhật danh sách client sau khi ngắt kết nối
                        SendClientListToAll(logAction);
                        globalUpdateClientListAction?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        logAction?.Invoke($"Lỗi khi ngắt kết nối client {clientIdentifier}: {ex.Message}");
                    }
                }
                else
                {
                    logAction?.Invoke($"Không tìm thấy client '{clientIdentifier}' để ngắt kết nối.");
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

                    // Khi đủ 2 người → bắt đầu game và khởi tạo bàn cờ
                    if (room.IsFull() && !room.IsGameStarted)
                    {
                        room.IsGameStarted = true;
                        // ✅ Khởi tạo bàn cờ cho phòng này
                        roomBoards.TryAdd(room.RoomId, new GameBoardState());
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

        // ✅ Refactor HandleGameMove để sử dụng GameEngine
        private void HandleGameMove(Socket clientSocket, string message, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(clientSocket);
                if (room == null || !room.IsGameStarted)
                {
                    SendToClient(clientSocket, "GAME_MOVE_FAILED:Bạn chưa trong phòng hoặc game chưa bắt đầu");
                    return;
                }

                // Parse tọa độ
                string[] parts = message.Substring("GAME_MOVE:".Length).Split(',');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
                {
                    SendToClient(clientSocket, "GAME_MOVE_FAILED:Định dạng không hợp lệ");
                    return;
                }

                // Lấy trạng thái bàn cờ
                if (!roomBoards.TryGetValue(room.RoomId, out GameBoardState? boardState))
                {
                    logAction?.Invoke($"⚠️ Không tìm thấy bàn cờ cho phòng {room.RoomId}");
                    return;
                }

                // ✅ XỬ LÝ NƯỚC ĐI QUA GAMEENGINE
                var result = gameEngine.ProcessMove(
                    boardState,
                    room,
                    clientSocket,
                    row,
                    col,
                    GetAuthenticatedUser
                );

                // Kiểm tra lỗi
                if (result.ErrorMessage != null)
                {
                    SendToClient(clientSocket, $"GAME_MOVE_FAILED:{result.ErrorMessage}");
                    return;
                }

                // Xử lý kết quả
                if (result.IsGameOver)
                {
                    HandleGameEnd(room, result, logAction);
                }
                else
                {
                    // Nước đi bình thường, chuyển cho đối thủ
                    roomManager.BroadcastToRoom(room.RoomId, $"GAME_MOVE:{row},{col}", clientSocket);
                    logAction?.Invoke($"➡️ Truyền nước đi [{row},{col}] trong phòng {room.RoomId}");
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi GAME_MOVE: {ex.Message}");
                SendToClient(clientSocket, $"GAME_MOVE_FAILED:{ex.Message}");
            }
        }

        // ✅ Hàm helper xử lý khi game kết thúc
        private void HandleGameEnd(GameRoom room, GameMoveResult result, Action<string> logAction)
        {
            if (result.EndReason == GameEndReason.FiveInRow && result.Winner != null && result.Loser != null)
            {
                // Gửi thông báo
                SendToClient(result.Winner, "YOU_WON");
                SendToClient(result.Loser, $"OPPONENT_WON:{result.LastMove.X},{result.LastMove.Y}");

                // Gửi lịch sử cập nhật
                SendHistoryToUser(result.Winner, logAction);
                SendHistoryToUser(result.Loser, logAction);
            }
            else if (result.EndReason == GameEndReason.Draw)
            {
                // Hòa
                roomManager.BroadcastToRoom(room.RoomId, "GAME_DRAW");
                logAction?.Invoke($"🤝 Trận đấu trong phòng {room.RoomId} hòa");
            }

            // Xóa bàn cờ
            roomBoards.TryRemove(room.RoomId, out _);

            SendClientListToAll(logAction);
            globalUpdateClientListAction?.Invoke();
        }

        // ✅ Xóa HandleGameWin vì đã không sử dụng
        
        // ✅ Refactor HandleResign để sử dụng GameEngine
        private void HandleResign(Socket resignerSocket, Action<string> logAction)
        {
            try
            {
                var room = roomManager.GetPlayerRoom(resignerSocket);
                if (room == null || !room.IsGameStarted) return;

                // ✅ XỬ LÝ ĐẦU HÀNG QUA GAMEENGINE
                var result = gameEngine.ProcessResign(room, resignerSocket, GetAuthenticatedUser);

                // Gửi thông báo
                SendToClient(result.Winner, "OPPONENT_RESIGNED");
                
                // Gửi lịch sử cập nhật
                SendHistoryToUser(result.Winner, logAction);
                SendHistoryToUser(result.Loser, logAction);

                // Xóa bàn cờ
                roomBoards.TryRemove(room.RoomId, out _);

                SendClientListToAll(logAction);
                globalUpdateClientListAction?.Invoke();
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi HandleResign: {ex.Message}");
            }
        }

        // Hàm hỗ trợ để lấy User từ Socket 
        private User? GetAuthenticatedUser(Socket clientSocket)
        {
            authenticatedUsers.TryGetValue(clientSocket, out User? user);
            return user;
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

                    // ✅ XỬ LÝ RỜI PHÒNG QUA GAMEENGINE
                    var result = gameEngine.ProcessDisconnect(room, clientSocket, GetAuthenticatedUser);

                    if (result != null)
                    {
                        // Game đã bắt đầu → có winner/loser
                        SendHistoryToUser(result.Winner, logAction);
                        SendHistoryToUser(result.Loser, logAction);
                    }

                    // Xóa bàn cờ
                    roomBoards.TryRemove(roomId, out _);
                    roomManager.LeaveRoom(clientSocket);

                    // Thông báo cho đối thủ
                    roomManager.BroadcastToRoom(roomId, "OPPONENT_LEFT");

                    logAction?.Invoke($"👋 {clientSocket.RemoteEndPoint} rời phòng {roomId}");

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

                // ✅ Khởi tạo bàn cờ cho phòng mới
                roomBoards.TryAdd(roomId, new GameBoardState());

                // ✅ GỬI THÔNG TIN VỊ TRÍ CHO CẢ HAI NGƯỜI CHƠI
                SendToClient(invitation.Sender, $"INVITATION_ACCEPTED:{invitationId}:{roomId}:FIRST");
                SendToClient(invitation.Receiver, $"INVITATION_ACCEPTED:{invitationId}:{roomId}:SECOND");

                // ✅ Đánh dấu game đã bắt đầu
                var room = roomManager.GetPlayerRoom(invitation.Sender);
                if (room != null)
                {
                    room.IsGameStarted = true;
                }

                // Bắt đầu game
                roomManager.BroadcastToRoom(roomId, "GAME_START");

                logAction?.Invoke($"✔ Lời mời {invitationId} được chấp nhận → tạo phòng {roomId}. Sender đi trước (X), Receiver đi sau (O)");

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
                    
                    // ✅ Đợi một chút để client kịp khởi động listening thread
                    Thread.Sleep(200);
                    
                    // ✅ Tự động gửi lịch sử đấu ngay sau khi đăng nhập
                    SendHistoryToUser(clientSocket, logAction);
                    
                    // ✅ Đợi thêm một chút trước khi gửi CLIENT_LIST
                    Thread.Sleep(100);
                    
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

        // ✅ Xử lý lấy lịch sử của user
        private void HandleGetMyHistory(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                User? user = GetAuthenticatedUser(clientSocket);
                if (user == null)
                {
                    SendToClient(clientSocket, "HISTORY_MY_ERROR:Chưa đăng nhập");
                    return;
                }

                var history = userManager.GetUserMatchHistory(user.Id, 100);
                string response = "HISTORY_MY:";
                
                foreach (var match in history)
                {
                    string matchStr = $"{match.Id}|{match.RoomId}|{match.Player1Username}|{match.Player2Username}|" +
                                    $"{match.WinnerUsername}|{match.StartedAt:yyyy-MM-dd HH:mm:ss}|" +
                                    $"{(match.EndedAt.HasValue ? match.EndedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}";
                    response += matchStr + ";";
                }
                
                SendToClient(clientSocket, response);
                logAction?.Invoke($"📜 Gửi lịch sử đấu của {user.Username}");
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi HandleGetMyHistory: {ex.Message}");
                SendToClient(clientSocket, "HISTORY_MY_ERROR:Lỗi khi lấy lịch sử");
            }
        }

        // ✅ Tự động gửi lịch sử cập nhật cho một user cụ thể
        private void SendHistoryToUser(Socket clientSocket, Action<string> logAction)
        {
            try
            {
                if (clientSocket == null || !clientSocket.Connected) return;

                User? user = GetAuthenticatedUser(clientSocket);
                if (user == null) return;

                var history = userManager.GetUserMatchHistory(user.Id, 100);
                string response = "HISTORY_MY:";
                
                foreach (var match in history)
                {
                    string matchStr = $"{match.Id}|{match.RoomId}|{match.Player1Username}|{match.Player2Username}|" +
                                    $"{match.WinnerUsername}|{match.StartedAt:yyyy-MM-dd HH:mm:ss}|" +
                                    $"{(match.EndedAt.HasValue ? match.EndedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}";
                    response += matchStr + ";";
                }
                
                SendToClient(clientSocket, response);
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi SendHistoryToUser: {ex.Message}");
            }
        }
    }
}
