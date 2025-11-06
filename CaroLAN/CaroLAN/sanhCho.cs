using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroLAN
{
    public partial class sanhCho : Form
    {
        ChessBoardManager chessBoard;
        SocketManager socket;
        Thread listenThread;
        private string currentRoomId; // ✅ Lưu ID phòng hiện tại
        private bool isInRoom = false; // ✅ Trạng thái có trong phòng hay không

        public sanhCho()
        {
            InitializeComponent();
            socket = new SocketManager();
        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            string serverIP = txtIP.Text.Trim();
            if (socket.ConnectToServer(serverIP))
            {
                lblStatus.Text = "Đã kết nối đến server";
                lobbyListening();
            }
            else
            {
                lblStatus.Text = "Không kết nối được server!";
            }
        }

        private void lobbyListening()
        {
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!socket.IsConnected)
                        {
                            lblStatus.Text = "Kết nối đến server đã bị mất!";
                            lstClients.Items.Clear();
                            break;
                        }
                        else
                        {
                            string data = socket.Receive();
                            if (string.IsNullOrEmpty(data))
                                continue;

                            if (data == "SERVER_STOPPED")
                            {
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = "Server đã dừng!";
                                    lstClients.Items.Clear();
                                }));
                                break; // Ngừng lắng nghe
                            }

                            //==================================
                            // ✅ Xử lý tham gia phòng thành công
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

                            // ✅ Xử lý bắt đầu game
                            if (data == "GAME_START")
                            {
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = $"Trận đấu trong phòng {currentRoomId} đã bắt đầu!";
                                    StartGame();
                                }));
                            }

                            // ✅ Xử lý đối thủ rời phòng
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

                            // ✅ Xử lý nước đi từ đối thủ
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

                            // ✅ Xử lý danh sách client (chỉ khi không trong phòng)
                            if (data.StartsWith("CLIENT_LIST:") && !isInRoom)
                            {
                                string[] clients = data.Substring("CLIENT_LIST:".Length).Split(',');
                                Invoke(new Action(() =>
                                {
                                    lstClients.Items.Clear();
                                    lstClients.Items.AddRange(clients);
                                }));
                            }

                            // ✅ Xử lý lỗi tham gia phòng
                            if (data == "ROOM_JOIN_FAILED")
                            {
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = "Không thể tham gia phòng!";
                                    MessageBox.Show("Không thể tham gia phòng. Vui lòng thử lại.", "Lỗi");
                                }));
                            }
                            //===================================

                            // ✅ Nếu nhận danh sách client
                            if (data.StartsWith("CLIENT_LIST:"))
                            {
                                string[] clients = data.Substring("CLIENT_LIST:".Length).Split(',');
                                Invoke(new Action(() =>
                                {
                                    lstClients.Items.Clear();
                                    lstClients.Items.AddRange(clients);
                                }));
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Lỗi khi nhận dữ liệu từ server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                        break;
                    }
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

        // ✅ Bắt đầu game - chuyển sang màn hình chơi
        private void StartGame()
        {
            try
            {
                // Khởi tạo hoặc reset bàn cờ
                if (chessBoard == null)
                {
                    // Giả sử có một panel để vẽ bàn cờ - bạn cần thêm panel này vào form
                    // chessBoard = new ChessBoardManager(pnlChessBoard);

                    // Hoặc mở form mới cho game
                    Form1 gameForm = new Form1();
                    gameForm.Show();
                    this.Hide();
                    //MessageBox.Show("da chay qua lenh form1.show");
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

        // ✅ Xử lý khi đóng form
        private void sanhCho_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isInRoom && socket.IsConnected)
                {
                    socket.Send("LEAVE_ROOM");
                }

                if (listenThread != null && listenThread.IsAlive)
                {
                    listenThread.Abort();
                }
            }
            catch
            {
                // Ignore errors when closing
            }
        }
    }
}
