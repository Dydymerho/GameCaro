using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroLAN
{
    public partial class sanhCho : Form
    {
        ChessBoardManager chessBoard;
        private readonly SocketManager socket;
        Thread listenThread;
        private CancellationTokenSource cancellationTokenSource;

        private string currentRoomId;
        private bool isInRoom = false;
        private bool amFirst = false;
        private string username = string.Empty;
        private string password = string.Empty; // ✅ Lưu password để tự động đăng nhập lại

        // ✅ Quản lý lời mời
        private Dictionary<string, string> receivedInvitations; // invitationId -> senderEndPoint
        private Dictionary<string, DateTime> invitationTimestamps; // invitationId -> thời gian nhận
        
        // ✅ Lưu địa chỉ endpoint của chính client này
        private string myEndPoint;

        public sanhCho() : this(string.Empty, string.Empty, null)
        {
        }

        public sanhCho(string username, SocketManager? existingSocket) : this(username, string.Empty, existingSocket)
        {
        }

        public sanhCho(string username, string password, SocketManager? existingSocket)
        {
            InitializeComponent();
            this.username = username;
            this.password = password; // ✅ Lưu password để tự động đăng nhập lại
            socket = existingSocket ?? new SocketManager();
            receivedInvitations = new Dictionary<string, string>();
            invitationTimestamps = new Dictionary<string, DateTime>();
            cancellationTokenSource = new CancellationTokenSource();

            FormClosing += sanhCho_FormClosing;

            btnConnect.Enabled = true;

            if (!string.IsNullOrEmpty(username))
            {
                Text = $"GameCaro - {username}";
            }

            if (socket.IsConnected)
            {
                lblStatus.Text = string.IsNullOrEmpty(username)
                    ? "Đã kết nối đến server"
                    : $"Đã kết nối - {username}";
                btnConnect.Text = "Ngắt kết nối";
                txtIP.Enabled = false;

                try
                {
                    string? remoteEndpoint = socket.GetServerEndPoint();
                    if (!string.IsNullOrEmpty(remoteEndpoint) && remoteEndpoint.Contains(':'))
                    {
                        txtIP.Text = remoteEndpoint.Split(':')[0];
                    }
                }
                catch
                {
                    // ignore parse errors
                }

                myEndPoint = socket.GetLocalEndPoint();
                lobbyListening();
            }
            else
            {
                lblStatus.Text = "Chưa kết nối";
                btnConnect.Text = "Kết nối";
                txtIP.Enabled = true;
                myEndPoint = string.Empty;
            }
        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            // NGẮT KẾT NỐI
            if (socket.IsConnected)
            {
                DisconnectFromServer();
            }
            // KẾT NỐI
            else
            {
                ConnectToServer();
            }
        }

        private void lobbyListening()
        {
            // Hủy token cũ nếu có
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            listenThread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // Kiểm tra kết nối
                        if (!socket.IsConnected)
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Kết nối đến server đã bị mất!";
                                lstClients.Items.Clear();
                                lstRequests.Items.Clear();
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtIP.Enabled = true;
                                isInRoom = false;
                                currentRoomId = null;
                                MessageBox.Show("Mất kết nối đến server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                            break;
                        }

                        // Nhận dữ liệu từ server
                        string data = socket.Receive();
                        if (string.IsNullOrEmpty(data))
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        // ✅ Bỏ qua message phản hồi chung từ server
                        if (data.StartsWith("Server đã nhận:"))
                        {
                            continue;
                        }

                        // ✅ Xử lý đăng nhập lại thành công
                        if (data.StartsWith("LOGIN_SUCCESS:"))
                        {
                            string[] parts = data.Split(':');
                            if (parts.Length >= 3)
                            {
                                string loggedInUsername = parts[2];
                                Invoke(new Action(() =>
                                {
                                    username = loggedInUsername; // Cập nhật username
                                    Text = $"GameCaro - {username}";
                                    lblStatus.Text = $"Đã đăng nhập lại: {username}";
                                }));
                            }
                            continue;
                        }

                        // ✅ Xử lý đăng nhập lại thất bại
                        if (data.StartsWith("LOGIN_FAILED:"))
                        {
                            string error = data.Substring("LOGIN_FAILED:".Length);
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = $"Đăng nhập lại thất bại: {error}";
                            }));
                            continue;
                        }

                        // ✅ Xử lý yêu cầu đăng nhập (nếu chưa đăng nhập)
                        if (data.StartsWith("AUTH_REQUIRED:"))
                        {
                            // Tự động đăng nhập lại nếu có thông tin
                            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                            {
                                try
                                {
                                    socket.Send($"LOGIN:{username}:{password}");
                                }
                                catch
                                {
                                    // Bỏ qua lỗi
                                }
                            }
                            continue;
                        }

                        // Xử lý server dừng
                        if (data == "SERVER_STOPPED")
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Server đã dừng!";
                                lstClients.Items.Clear();
                                lstRequests.Items.Clear();
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtIP.Enabled = true;
                                MessageBox.Show("Server đã dừng hoạt động!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                            break;
                        }

                        // ✅ Xử lý nhận lời mời
                        if (data.StartsWith("INVITATION_RECEIVED:"))
                        {
                            HandleInvitationReceived(data);
                        }

                        // ✅ Xử lý lời mời đã gửi thành công
                        if (data.StartsWith("INVITATION_SENT:"))
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Lời mời đã được gửi!";
                            }));
                        }

                        // ✅ Xử lý lời mời bị từ chối
                        if (data.StartsWith("INVITATION_REJECTED:"))
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Lời mời bị từ chối!";
                                MessageBox.Show("Đối thủ đã từ chối lời mời của bạn.", "Thông báo");
                            }));
                        }

                        // ✅ Xử lý lời mời được chấp nhận
                        if (data.StartsWith("INVITATION_ACCEPTED:"))
                        {
                            string[] parts = data.Split(':');
                            string invitationId = parts[1];
                            string roomId = parts[2];
                            string position = parts.Length > 3 ? parts[3] : ""; // ✅ Lấy vị trí FIRST/SECOND

                            Invoke(new Action(() =>
                            {
                                RemoveInvitationFromList(invitationId);
                                currentRoomId = roomId;
                                isInRoom = true;
                                
                                // ✅ XÁC ĐỊNH AI ĐI TRƯỚC DỰA VÀO VỊ TRÍ
                                // FIRST = người gửi lời mời = đi trước (X)
                                // SECOND = người nhận lời mời = đi sau (O)
                                amFirst = (position == "FIRST");
                                
                                string positionText = amFirst ? "Bạn đi trước (X)" : "Bạn đi sau (O)";
                                lblStatus.Text = $"Lời mời được chấp nhận. Vào phòng {roomId} - {positionText}";
                                StartGame();
                            }));
                        }


                        // ✅ Xử lý lời mời hết hạn
                        if (data.StartsWith("INVITATION_EXPIRED:"))
                        {
                            string id = data.Split(':')[1];

                            Invoke(new Action(() =>
                            {
                                RemoveInvitationFromList(id);
                                lblStatus.Text = "Một lời mời đã hết hạn.";
                            }));
                        }


                        // ✅ Xử lý lời mời bị hủy (người gửi ngắt kết nối)
                        if (data.StartsWith("INVITATION_CANCELLED:"))
                        {
                            string id = data.Split(':')[1];

                            Invoke(new Action(() =>
                            {
                                RemoveInvitationFromList(id);
                                lblStatus.Text = "Lời mời đã bị hủy bởi người gửi.";
                            }));
                        }


                        // ✅ Xử lý lỗi gửi lời mời
                        if (data.StartsWith("INVITATION_SEND_FAILED:"))
                        {
                            string[] parts = data.Split(':', 2); // Chỉ split thành 2 phần
                            string reason = parts.Length > 1 ? parts[1] : "Unknown error";
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Không thể gửi lời mời!";
                                MessageBox.Show($"Không thể gửi lời mời: {reason}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }

                        // Xử lý tham gia phòng thành công
                        if (data.StartsWith("ROOM_JOINED:"))
                        {
                            string[] parts = data.Split(':');
                            if (parts.Length >= 3)
                            {
                                currentRoomId = parts[1];
                                int playerCount = int.Parse(parts[2]);
                                amFirst = (playerCount == 1); // ✅ nếu mình là người đầu tiên, đi trước

                                Invoke(new Action(() =>
                                {
                                    isInRoom = true;
                                    lblStatus.Text = $"Đã tham gia phòng {currentRoomId} ({playerCount}/2 người chơi)";

                                    if (playerCount < 2)
                                    {
                                        lblStatus.Text += " - Đang chờ đối thủ...";
                                    }
                                }));
                            }
                        }

                        // Xử lý bắt đầu game
                        if (data == "GAME_START")
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = $"Trận đấu trong phòng {currentRoomId} đã bắt đầu!";
                                StartGame();
                            }));
                        }

                        // Xử lý đối thủ rời phòng
                        if (data == "OPPONENT_LEFT")
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Đối thủ đã rời phòng!";
                                MessageBox.Show("Đối thủ đã rời phòng. Bạn sẽ quay lại sảnh chờ.", "Thông báo");
                                isInRoom = false;
                                currentRoomId = null;
                            }));
                        }

                        // Xử lý nước đi từ đối thủ
                        if (data.StartsWith("GAME_MOVE:"))
                        {
                            string moveData = data.Substring("GAME_MOVE:".Length);
                            string[] moveParts = moveData.Split(',');
                            if (moveParts.Length == 2 && int.TryParse(moveParts[0], out int x) && int.TryParse(moveParts[1], out int y))
                            {
                                Invoke(new Action(() =>
                                {
                                    chessBoard?.OtherPlayerMove(new Point(x, y));
                                }));
                            }
                        }

                        // Xử lý danh sách client (chỉ khi không trong phòng)
                        if (data.StartsWith("CLIENT_LIST:") && !isInRoom)
                        {
                            string[] clients = data.Substring("CLIENT_LIST:".Length).Split(',');
                            Invoke(new Action(() =>
                            {
                                UpdateClientList(clients);
                            }));
                        }

                        // Xử lý lỗi tham gia phòng
                        if (data == "ROOM_JOIN_FAILED")
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Không thể tham gia phòng!";
                                MessageBox.Show("Không thể tham gia phòng. Vui lòng thử lại.", "Lỗi");
                            }));
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Socket đã bị đóng
                        if (!token.IsCancellationRequested)
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Kết nối bị gián đoạn!";
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtIP.Enabled = true;
                            }));
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Chỉ hiển thị lỗi nếu không phải do cancellation
                        if (!token.IsCancellationRequested)
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Lỗi kết nối!";
                                MessageBox.Show($"Lỗi khi nhận dữ liệu từ server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtIP.Enabled = true;
                            }));
                        }
                        break;
                    }
                }

                // Dọn dẹp sau khi thread kết thúc
                try
                {
                    Invoke(new Action(() =>
                    {
                        if (!socket.IsConnected)
                        {
                            btnConnect.Text = "Kết nối";
                            btnConnect.Enabled = true;
                            txtIP.Enabled = true;
                        }
                    }));
                }
                catch
                {
                    // Form có thể đã bị đóng
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        // ✅ Xử lý khi nhận lời mời
        private void HandleInvitationReceived(string data)
        {
            try
            {
                // Format: INVITATION_RECEIVED:<id>:<senderInfo>
                int firstColon = data.IndexOf(':');
                int secondColon = data.IndexOf(':', firstColon + 1);

                if (firstColon < 0 || secondColon < 0)
                    return;

                string invitationId = data.Substring(firstColon + 1, secondColon - firstColon - 1);
                string senderInfo = data.Substring(secondColon + 1);

                // Không nhận lời mời từ chính mình
                if (senderInfo == username || senderInfo == myEndPoint)
                    return;

                // Nếu đã trong phòng thì tự động từ chối lời mời
                if (isInRoom)
                {
                    socket.Send($"REJECT_INVITATION:{invitationId}");
                    return;
                }

                Invoke(new Action(() =>
                {
                    // Đã có rồi thì không thêm lại
                    if (receivedInvitations.ContainsKey(invitationId))
                        return;

                    receivedInvitations[invitationId] = senderInfo;
                    invitationTimestamps[invitationId] = DateTime.Now;

                    lstRequests.Items.Add($"{senderInfo} (ID: {invitationId})");
                    lblStatus.Text = $"Nhận lời mời từ {senderInfo}";
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in invitation: " + ex.Message);
            }
        }


        // ✅ Xóa lời mời khỏi danh sách
        private void RemoveInvitationFromList(string invitationId)
        {
            if (receivedInvitations.ContainsKey(invitationId))
            {
                receivedInvitations.Remove(invitationId);
                invitationTimestamps.Remove(invitationId);
            }

            for (int i = 0; i < lstRequests.Items.Count; i++)
            {
                string item = lstRequests.Items[i].ToString();

                if (item.EndsWith($"(ID: {invitationId})"))
                {
                    lstRequests.Items.RemoveAt(i);
                    break;
                }
            }
        }


        // ✅ Cập nhật danh sách client với trạng thái
        private void UpdateClientList(string[] clients)
        {
            lstClients.Items.Clear();

            if (clients.Length == 0 || string.IsNullOrEmpty(clients[0]))
                return;

            // ✅ Lấy endpoint hiện tại từ socket (cập nhật mỗi lần để đảm bảo đúng sau khi reconnect)
            string currentEndPoint = string.Empty;
            if (socket.IsConnected)
            {
                currentEndPoint = socket.GetLocalEndPoint();
                if (!string.IsNullOrEmpty(currentEndPoint) && currentEndPoint != "Not connected" && currentEndPoint != "Error")
                {
                    myEndPoint = currentEndPoint; // Cập nhật myEndPoint với giá trị hiện tại
                }
            }

            List<string> availableClients = new List<string>();
            List<string> busyClients = new List<string>();

            foreach (string client in clients)
            {
                if (string.IsNullOrWhiteSpace(client))
                    continue;

                string cleanClient = client.Replace("|BUSY", "").Trim();

                // ✅ So sánh để xác định đây có phải là chính mình không
                bool isMe = false;

                // 1. So sánh với username nếu có (ưu tiên cao nhất)
                if (!string.IsNullOrEmpty(username))
                {
                    isMe = cleanClient.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase);
                }
                
                // 2. Nếu chưa khớp với username, so sánh với endpoint hiện tại
                if (!isMe && !string.IsNullOrEmpty(currentEndPoint) && currentEndPoint != "Not connected" && currentEndPoint != "Error")
                {
                    isMe = cleanClient.Equals(currentEndPoint.Trim(), StringComparison.OrdinalIgnoreCase);
                }

                // 3. Nếu vẫn chưa khớp và có myEndPoint cũ, so sánh với nó (phòng trường hợp reconnect nhưng chưa cập nhật)
                if (!isMe && !string.IsNullOrEmpty(myEndPoint) && myEndPoint != "Not connected" && myEndPoint != "Error")
                {
                    isMe = cleanClient.Equals(myEndPoint.Trim(), StringComparison.OrdinalIgnoreCase);
                }

                // 4. So sánh theo IP nếu endpoint có format IP:Port (phòng trường hợp port thay đổi nhưng IP giống)
                if (!isMe && !string.IsNullOrEmpty(currentEndPoint) && currentEndPoint.Contains(':'))
                {
                    try
                    {
                        string myIP = currentEndPoint.Split(':')[0];
                        if (cleanClient.Contains(':'))
                        {
                            string clientIP = cleanClient.Split(':')[0];
                            // Chỉ so sánh IP nếu cả hai đều là localhost hoặc cùng IP
                            if (myIP == clientIP && (myIP == "127.0.0.1" || myIP == "localhost" || myIP.StartsWith("192.168.") || myIP.StartsWith("10.")))
                            {
                                // Nếu IP giống và đều là localhost/local network, có thể là cùng một client
                                // Nhưng để an toàn, chỉ bỏ qua nếu format endpoint hoàn toàn giống nhau
                                // (tránh bỏ qua nhầm người khác có cùng IP)
                            }
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi parse
                    }
                }

                if (isMe)
                    continue; // Bỏ qua chính mình

                if (client.Contains("|BUSY"))
                {
                    busyClients.Add($"[BUSY] {cleanClient}");
                }
                else
                {
                    availableClients.Add(cleanClient);
                }
            }

            // Thêm client available trước
            foreach (string client in availableClients)
                lstClients.Items.Add(client);

            // Thêm client busy xuống cuối
            foreach (string client in busyClients)
                lstClients.Items.Add(client);
        }


        // button bat dau choi
        private void button3_Click(object sender, EventArgs e)
        {
            if (!socket.IsConnected)
            {
                MessageBox.Show("Bạn chưa kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isInRoom)
            {
                MessageBox.Show("Bạn đã ở trong phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Gửi yêu cầu tham gia phòng
                socket.Send("JOIN_ROOM");
                lblStatus.Text = "Đang tìm phòng...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartGame()
        {
            cancellationTokenSource?.Cancel(); // Hủy token để vòng lặp lobbyListening kết thúc

            // ✅ Đợi thread kết thúc trước khi tiếp tục
            if (listenThread != null && listenThread.IsAlive)
            {
                if (!listenThread.Join(2000)) // Đợi tối đa 2 giây
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Thread lobbyListening không dừng kịp thời");
                }
            }

            try
            {
                // Mở form mới cho game
                Form1 gameForm = new Form1(currentRoomId, socket, amFirst);
                gameForm.FormClosed += (s, args) =>
                {
                    // Khi form game đóng, hiện lại form sảnh chờ
                    this.Show();

                    // Reset trạng thái
                    isInRoom = false;
                    currentRoomId = null;
                    lblStatus.Text = "Đã kết nối đến server";

                    // ✅ Khởi động lại lobbyListening khi quay về
                    lobbyListening();
                };

                gameForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu game: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // xu ly khi dong form
        private void sanhCho_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (socket.IsConnected)
                {
                    // Gửi tín hiệu rời phòng
                    if (isInRoom)
                    {
                        try
                        {
                            socket.Send("LEAVE_ROOM");
                        }
                        catch { }
                    }

                    // Gửi tín hiệu ngắt kết nối
                    try
                    {
                        socket.Send("DISCONNECT");
                    }
                    catch { }
                }

                // Hủy thread an toàn
                cancellationTokenSource?.Cancel();

                if (listenThread != null && listenThread.IsAlive)
                {
                    listenThread.Join(1000); // Đợi tối đa 1 giây
                }

                socket.Disconnect();
            }
            catch
            {
                // Ignore errors when closing
            }
        }

        private void sanhCho_Load(object sender, EventArgs e)
        {
            if (socket.IsConnected)
            {
                lblStatus.Text = string.IsNullOrEmpty(username)
                    ? "Đã kết nối đến server"
                    : $"Đã kết nối - {username}";
                btnConnect.Text = "Ngắt kết nối";
                txtIP.Enabled = false;
            }
            else
            {
                lblStatus.Text = "Chưa kết nối";
                btnConnect.Text = "Kết nối";
                txtIP.Enabled = true;
            }
        }

        // Kết nối đến server
        private void ConnectToServer()
        {
            string serverIP = txtIP.Text.Trim();

            // Kiểm tra IP có rỗng không
            if (string.IsNullOrEmpty(serverIP))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIP.Focus();
                return;
            }

            try
            {
                lblStatus.Text = "Đang kết nối...";
                btnConnect.Enabled = false; // Vô hiệu hóa nút trong lúc kết nối
                Application.DoEvents(); // Cập nhật UI

                if (socket.ConnectToServer(serverIP))
                {
                    // Kết nối thành công
                    lblStatus.Text = $"Đã kết nối đến server {socket.GetServerEndPoint()}";
                    btnConnect.Text = "Ngắt kết nối";
                    btnConnect.Enabled = true;
                    txtIP.Enabled = false; // Vô hiệu hóa textbox IP

                    // Lưu địa chỉ endpoint của chính mình
                    myEndPoint = socket.GetLocalEndPoint();

                    // ✅ Tự động đăng nhập lại nếu có username và password
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        try
                        {
                            socket.Send($"LOGIN:{username}:{password}");
                            lblStatus.Text = "Đang đăng nhập lại...";
                        }
                        catch
                        {
                            // Bỏ qua lỗi nếu không gửi được
                        }
                    }

                    // Bắt đầu lắng nghe
                    lobbyListening();
                }
                else
                {
                    // Kết nối thất bại
                    lblStatus.Text = "Không kết nối được server!";
                    btnConnect.Enabled = true;
                    MessageBox.Show(
                        "Không thể kết nối đến server.\n\n",
                        "Lỗi kết nối",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi kết nối!";
                btnConnect.Enabled = true;
                MessageBox.Show($"Lỗi khi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Ngắt kết nối khỏi server
        /// </summary>
        private void DisconnectFromServer()
        {
            try
            {
                btnConnect.Enabled = false;
                lblStatus.Text = "Đang ngắt kết nối...";
                Application.DoEvents();

                // Gửi tín hiệu rời phòng nếu đang trong phòng
                if (isInRoom)
                {
                    try
                    {
                        socket.Send("LEAVE_ROOM");
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi gửi LEAVE_ROOM
                    }
                    isInRoom = false;
                    currentRoomId = null;
                }

                // Gửi tín hiệu ngắt kết nối
                try
                {
                    socket.Send("DISCONNECT");
                    Thread.Sleep(100); // Đợi một chút để server nhận message
                }
                catch
                {
                    // Bỏ qua lỗi khi gửi DISCONNECT
                }

                // Hủy thread lắng nghe
                cancellationTokenSource?.Cancel();

                // Đợi thread kết thúc (tối đa 2 giây)
                if (listenThread != null && listenThread.IsAlive)
                {
                    if (!listenThread.Join(2000))
                    {
                        System.Diagnostics.Debug.WriteLine("Thread không dừng trong thời gian chờ");
                    }
                }

                // Ngắt kết nối socket hoàn toàn
                socket.Disconnect();

                // Cập nhật giao diện
                lblStatus.Text = "Đã ngắt kết nối khỏi server";
                lstClients.Items.Clear();
                btnConnect.Text = "Kết nối";
                btnConnect.Enabled = true;
                txtIP.Enabled = true; // Bật lại textbox IP
            }
            catch (Exception ex)
            {
                btnConnect.Enabled = true;
                MessageBox.Show($"Lỗi khi ngắt kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Xử lý nút mời chơi
        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (!socket.IsConnected)
            {
                MessageBox.Show("Bạn chưa kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isInRoom)
            {
                MessageBox.Show("Bạn đang trong phòng chơi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (lstClients.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một người chơi để mời!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string selectedClient = lstClients.SelectedItem.ToString();
                
                // ✅ Kiểm tra xem client có đang bận không
                if (selectedClient.StartsWith("[BUSY]"))
                {
                    MessageBox.Show("Người chơi này đang bận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ✅ Loại bỏ prefix [BUSY] nếu có (phòng trường hợp)
                string cleanClientName = selectedClient.Replace("[BUSY] ", "").Trim();
                
                // Gửi lời mời (có thể là username hoặc endpoint)
                socket.Send($"SEND_INVITATION:{cleanClientName}");
                lblStatus.Text = $"Đang gửi lời mời đến {cleanClientName}...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi lời mời: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Xử lý nút chấp nhận
        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một lời mời để chấp nhận!");
                return;
            }

            string selected = lstRequests.SelectedItem.ToString();

            int index = selected.IndexOf("(ID: ");
            if (index < 0) return;

            string invitationId = selected
                .Substring(index + 5)
                .TrimEnd(')');

            if (!receivedInvitations.ContainsKey(invitationId))
            {
                MessageBox.Show("Lời mời không còn hợp lệ!");
                RemoveInvitationFromList(invitationId);
                return;
            }

            // Gửi yêu cầu accept
            socket.Send($"ACCEPT_INVITATION:{invitationId}");
            lblStatus.Text = "Đang chấp nhận lời mời...";

            // KHÔNG XÓA Ở ĐÂY — chờ server xác nhận
        }


        private void lstRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Có thể thêm xử lý khi chọn lời mời nếu cần
        }
    }
}
