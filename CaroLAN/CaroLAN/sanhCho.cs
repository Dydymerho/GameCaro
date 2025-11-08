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
        SocketManager socket;
        Thread listenThread;
        private CancellationTokenSource cancellationTokenSource; // ✅ Thêm CancellationTokenSource

        private string currentRoomId; // ✅ Lưu ID phòng hiện tại
        private bool isInRoom = false; // ✅ Trạng thái có trong phòng hay không

        public sanhCho()
        {
            InitializeComponent();
            socket = new SocketManager();
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
                            Thread.Sleep(10); // Tránh CPU 100%
                            continue;
                        }

                        // Xử lý server dừng
                        if (data == "SERVER_STOPPED")
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Server đã dừng!";
                                lstClients.Items.Clear();
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtIP.Enabled = true;
                                MessageBox.Show("Server đã dừng hoạt động!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                            break;
                        }

                        // Xử lý tham gia phòng thành công
                        if (data.StartsWith("ROOM_JOINED:"))
                        {
                            string[] parts = data.Split(':');
                            if (parts.Length >= 3)
                            {
                                currentRoomId = parts[1];
                                int playerCount = int.Parse(parts[2]);

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
                                lstClients.Items.Clear();
                                if (clients.Length > 0 && !string.IsNullOrEmpty(clients[0]))
                                {
                                    lstClients.Items.AddRange(clients);
                                }
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
                // Khởi tạo hoặc reset bàn cờ
                if (chessBoard == null)
                {
                    // Mở form mới cho game
                    Form1 gameForm = new Form1();
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
                else
                {
                    chessBoard.ResetBoard();
                }
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

                // Ngắt kết nối socket
                socket.Disconnect();
            }
            catch
            {
                // Ignore errors when closing
            }
        }

        private void sanhCho_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu
            lblStatus.Text = "Chưa kết nối";
            btnConnect.Text = "Kết nối";
        }

        /// <summary>
        /// Kết nối đến server
        /// </summary>
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

                    // Bắt đầu lắng nghe
                    lobbyListening();
                }
                else
                {
                    // Kết nối thất bại
                    lblStatus.Text = "Không kết nối được server!";
                    btnConnect.Enabled = true;
                    MessageBox.Show(
                        "Không thể kết nối đến server.\n\n" +
                        "Vui lòng kiểm tra:\n" +
                        "- Địa chỉ IP có đúng không?\n" +
                        "- Server có đang chạy không?\n" +
                        "- Firewall có chặn kết nối không?",
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
    }
}
