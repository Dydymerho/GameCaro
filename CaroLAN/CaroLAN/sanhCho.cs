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

        // ✅ Quản lý lời mời
        private Dictionary<string, string> receivedInvitations; // invitationId -> senderEndPoint
        private Dictionary<string, DateTime> invitationTimestamps; // invitationId -> thời gian nhận
        
        // ✅ Lưu địa chỉ endpoint của chính client này
        private string myEndPoint;

        public sanhCho() : this(string.Empty, null)
        {
        }

        public sanhCho(string username, SocketManager? existingSocket)
        {
            InitializeComponent();
            this.username = username;
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
                            if (parts.Length >= 3)
                            {
                                currentRoomId = parts[2];
                                Invoke(new Action(() =>
                                {
                                    isInRoom = true;
                                    lblStatus.Text = $"Lời mời được chấp nhận! Phòng: {currentRoomId}";
                                }));
                            }
                        }

                        // ✅ Xử lý lời mời hết hạn
                        if (data.StartsWith("INVITATION_EXPIRED:"))
                        {
                            string[] parts = data.Split(':');
                            if (parts.Length >= 2)
                            {
                                string invitationId = parts[1];
                                Invoke(new Action(() =>
                                {
                                    RemoveInvitationFromList(invitationId);
                                }));
                            }
                        }

                        // ✅ Xử lý lời mời bị hủy (người gửi ngắt kết nối)
                        if (data.StartsWith("INVITATION_CANCELLED:"))
                        {
                            string[] parts = data.Split(':', 2);
                            if (parts.Length >= 2)
                            {
                                string invitationId = parts[1];
                                Invoke(new Action(() =>
                                {
                                    RemoveInvitationFromList(invitationId);
                                    lblStatus.Text = "Lời mời đã bị hủy";
                                }));
                            }
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
                // Format: INVITATION_RECEIVED:invitationId:senderEndPoint
                string[] parts = data.Split(':', 3); // Split thành tối đa 3 phần
                if (parts.Length >= 3)
                {
                    string invitationId = parts[1];
                    string senderEndPoint = parts[2];

                    Invoke(new Action(() =>
                    {
                        // Lưu lời mời
                        if (!receivedInvitations.ContainsKey(invitationId))
                        {
                            receivedInvitations[invitationId] = senderEndPoint;
                            invitationTimestamps[invitationId] = DateTime.Now;

                            // Thêm vào danh sách hiển thị
                            lstRequests.Items.Add($"{senderEndPoint} (ID: {invitationId})");
                            lblStatus.Text = $"Nhận lời mời từ {senderEndPoint}";
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý lời mời: {ex.Message}");
            }
        }

        // ✅ Xóa lời mời khỏi danh sách
        private void RemoveInvitationFromList(string invitationId)
        {
            if (receivedInvitations.ContainsKey(invitationId))
            {
                string senderEndPoint = receivedInvitations[invitationId];
                receivedInvitations.Remove(invitationId);
                invitationTimestamps.Remove(invitationId);

                // Xóa khỏi ListBox
                for (int i = 0; i < lstRequests.Items.Count; i++)
                {
                    string item = lstRequests.Items[i].ToString();
                    if (item.Contains(invitationId))
                    {
                        lstRequests.Items.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // ✅ Cập nhật danh sách client với trạng thái
        private void UpdateClientList(string[] clients)
        {
            lstClients.Items.Clear();
            
            if (clients.Length == 0 || string.IsNullOrEmpty(clients[0]))
            {
                return;
            }

            // Phân loại client: available và busy
            List<string> availableClients = new List<string>();
            List<string> busyClients = new List<string>();

            foreach (string client in clients)
            {
                if (string.IsNullOrWhiteSpace(client))
                    continue;

                // ✅ Loại bỏ chính mình khỏi danh sách
                string cleanEndpoint = client.Replace("|BUSY", "");
                if (cleanEndpoint == myEndPoint)
                {
                    continue; // Bỏ qua chính mình
                }

                if (client.Contains("|BUSY"))
                {
                    // Client đang bận - loại bỏ suffix và thêm prefix
                    string endpoint = client.Replace("|BUSY", "");
                    busyClients.Add($"[BUSY] {endpoint}");
                }
                else
                {
                    availableClients.Add(client);
                }
            }

            // Thêm client available trước
            foreach (string client in availableClients)
            {
                lstClients.Items.Add(client);
            }

            // Thêm client busy sau (xuống cuối danh sách)
            foreach (string client in busyClients)
            {
                lstClients.Items.Add(client);
            }
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

                // Gửi lời mời
                socket.Send($"SEND_INVITATION:{selectedClient}");
                lblStatus.Text = $"Đang gửi lời mời đến {selectedClient}...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi lời mời: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Xử lý nút chấp nhận
        private void btnAccept_Click(object sender, EventArgs e)
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

            if (lstRequests.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một lời mời để chấp nhận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string selectedRequest = lstRequests.SelectedItem.ToString();
                
                // Lấy invitationId từ chuỗi hiển thị
                int idIndex = selectedRequest.IndexOf("(ID: ");
                if (idIndex >= 0)
                {
                    string invitationId = selectedRequest.Substring(idIndex + 5).TrimEnd(')');

                    // Kiểm tra xem lời mời còn hợp lệ không
                    if (!receivedInvitations.ContainsKey(invitationId))
                    {
                        MessageBox.Show("Lời mời không còn hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        lstRequests.Items.Remove(selectedRequest);
                        return;
                    }

                    // Gửi chấp nhận
                    socket.Send($"ACCEPT_INVITATION:{invitationId}");
                    lblStatus.Text = "Đang chấp nhận lời mời...";

                    // Xóa lời mời khỏi danh sách
                    RemoveInvitationFromList(invitationId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chấp nhận lời mời: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Có thể thêm xử lý khi chọn lời mời nếu cần
        }
    }
}
