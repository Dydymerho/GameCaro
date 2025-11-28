using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CaroLAN
{
    public partial class Form1 : Form
    {
        ChessBoardManager chessBoard;
        SocketManager socket;
        Thread listenThread;
        private CancellationTokenSource cancellationTokenSource;

        private string roomId;
        private bool isMyTurn = false;
        private int timeLeft = 20;
        private System.Windows.Forms.Timer turnTimer;
        private bool iAmPlayerX;

        // Màu sắc chính
        private readonly Color ColorX = Color.FromArgb(70, 130, 180); // Steel Blue
        private readonly Color ColorO = Color.FromArgb(220, 20, 60);  // Crimson
        private readonly Color ColorActive = Color.FromArgb(240, 248, 255); // Alice Blue - màu active
        private readonly Color ColorInactive = Color.White; // Trắng - màu inactive

        public Form1(string roomId, SocketManager socket, bool startFirst)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            this.roomId = roomId;
            this.socket = socket;
            this.isMyTurn = startFirst;
            this.iAmPlayerX = startFirst;

            cancellationTokenSource = new CancellationTokenSource();

            chessBoard = new ChessBoardManager(pnlChessBoard, startFirst);
            chessBoard.PlayerClicked += ChessBoard_PlayerClicked;
            chessBoard.GameEnded += ChessBoard_GameEnded;

            // Vẽ icon X và O trong PictureBox
            DrawXIcon();
            DrawOIcon();

            // ✅ Cập nhật UI cho 2 người chơi
            lblRoom.Text = $"🎯 Phòng: {roomId}";

            if (iAmPlayerX)
            {
                // Tôi là X - đi trước
                lblPlayerX.Text = "Bạn";
                lblPlayerO.Text = "Đối thủ";
                lblPlayerXStatus.Text = "⚡ Đang chơi";
                lblPlayerOStatus.Text = "⏳ Chờ lượt";
                pnlPlayerX.BackColor = ColorActive;
            }
            else
            {
                // Tôi là O - đi sau
                lblPlayerX.Text = "Đối thủ";
                lblPlayerO.Text = "Bạn";
                lblPlayerXStatus.Text = "⚡ Đang chơi";
                lblPlayerOStatus.Text = "⏳ Chờ lượt";
                pnlPlayerX.BackColor = ColorActive; // X đi trước
            }

            lblTimer.Text = "⏰ --";

            InitTimer();
            StartListening();

            // Chat input enter handler
            try
            {
                txtChatInput.KeyDown += TxtChatInput_KeyDown;
            }
            catch { }
        }

        // Vẽ hình X trong PictureBox
        private void DrawXIcon()
        {
            Bitmap bmp = new Bitmap(picPlayerX.Width, picPlayerX.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(ColorX);

                using (Pen pen = new Pen(Color.White, 8))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    // Vẽ X
                    g.DrawLine(pen, 15, 15, 55, 55);
                    g.DrawLine(pen, 55, 15, 15, 55);
                }
            }
            picPlayerX.Image = bmp;
        }

        // Vẽ hình O trong PictureBox
        private void DrawOIcon()
        {
            Bitmap bmp = new Bitmap(picPlayerO.Width, picPlayerO.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(ColorO);

                using (Pen pen = new Pen(Color.White, 8))
                {
                    // Vẽ O
                    g.DrawEllipse(pen, 15, 15, 40, 40);
                }
            }
            picPlayerO.Image = bmp;
        }

        // Cập nhật trạng thái người chơi
        private void UpdatePlayerStatus(bool isXTurn)
        {
            if (isXTurn)
            {
                // Lượt của X
                pnlPlayerX.BackColor = ColorActive;
                pnlPlayerO.BackColor = ColorInactive;

                if (iAmPlayerX)
                {
                    lblPlayerXStatus.Text = "⚡ Đang chơi";
                    lblPlayerOStatus.Text = "⏳ Chờ lượt";
                }
                else
                {
                    lblPlayerXStatus.Text = "⚡ Đang chơi";
                    lblPlayerOStatus.Text = "⏳ Chờ lượt";
                }
            }
            else
            {
                // Lượt của O
                pnlPlayerX.BackColor = ColorInactive;
                pnlPlayerO.BackColor = ColorActive;

                if (iAmPlayerX)
                {
                    lblPlayerXStatus.Text = "⏳ Chờ lượt";
                    lblPlayerOStatus.Text = "⚡ Đang chơi";
                }
                else
                {
                    lblPlayerXStatus.Text = "⏳ Chờ lượt";
                    lblPlayerOStatus.Text = "⚡ Đang chơi";
                }
            }
        }

        private void InitTimer()
        {
            turnTimer = new System.Windows.Forms.Timer();
            turnTimer.Interval = 1000;
            turnTimer.Tick += (s, e) =>
            {
                if (!chessBoard.isGameOver && isMyTurn)
                {
                    timeLeft--;
                    lblTimer.Text = $"⏰ {timeLeft}s";

                    // Chỉ đổi màu chữ khi còn ít thời gian
                    if (timeLeft <= 5)
                    {
                        lblTimer.ForeColor = ColorO; // Đỏ
                    }
                    else
                    {
                        lblTimer.ForeColor = ColorX; // Xanh
                    }

                    if (timeLeft <= 0)
                    {
                        turnTimer.Stop();
                        EndGameDueToTimeout();
                    }
                }
            };
            if (isMyTurn) StartTurnTimer();
        }

        private void StartTurnTimer()
        {
            timeLeft = 20;
            lblTimer.Text = $"⏰ {timeLeft}s";
            lblTimer.ForeColor = ColorX;
            turnTimer.Start();
        }

        private void StopTurnTimer()
        {
            turnTimer.Stop();
            lblTimer.Text = "⏰ --";
            lblTimer.ForeColor = ColorX;
        }

        private void EndGameDueToTimeout()
        {
            MessageBox.Show("⏰ Hết thời gian! Bạn đã thua lượt này.", "Thời gian hết", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            socket.Send("RESIGN");
            EndGame("Thua do hết thời gian");
        }

        private void StartListening()
        {
            listenThread = new Thread(() =>
            {
                var token = cancellationTokenSource.Token;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!socket.IsConnected)
                        {
                            if (!token.IsCancellationRequested)
                            {
                                Invoke(new Action(() =>
                                {
                                    MessageBox.Show("❌ Mất kết nối tới server!", "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Close();
                                }));
                            }
                            break;
                        }

                        string data = socket.Receive();
                        if (string.IsNullOrEmpty(data))
                        {
                            Thread.Sleep(20);
                            continue;
                        }

                        if (data.StartsWith("GAME_MOVE:"))
                        {
                            string[] parts = data.Substring(10).Split(',');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Invoke(new Action(() =>
                                {
                                    chessBoard.OtherPlayerMove(new Point(x, y));
                                    isMyTurn = true;

                                    // Cập nhật trạng thái: bây giờ là lượt của tôi
                                    UpdatePlayerStatus(iAmPlayerX); // true nếu tôi là X
                                    StartTurnTimer();
                                }));
                            }
                        }

                        if (data == "RESIGN")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("🏆 Đối thủ đã đầu hàng! Bạn thắng!");
                            }));
                        }

                        // ✅ XỬ LÝ KHI ĐỐI THỦ ĐẦU HÀNG (message từ server)
                        if (data == "OPPONENT_RESIGNED")
                        {
                            Invoke(new Action(() =>
                            {
                                StopTurnTimer();
                                chessBoard.isGameOver = true;
                                MessageBox.Show(
                                    "🏳️ Đối thủ đã đầu hàng!\n\n🏆 Bạn chiến thắng!",
                                    "Chiến thắng",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                                
                                // Tự động đóng form sau 2 giây
                                System.Threading.Timer? closeTimer = null;
                                closeTimer = new System.Threading.Timer((state) =>
                                {
                                    try
                                    {
                                        Invoke(new Action(() => this.Close()));
                                    }
                                    catch { }
                                    finally
                                    {
                                        closeTimer?.Dispose();
                                    }
                                }, null, 2000, System.Threading.Timeout.Infinite);
                            }));
                        }

                        if (data == "OPPONENT_LEFT")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("🚪 Đối thủ đã thoát khỏi phòng.");
                            }));
                        }

                        if (data.StartsWith("OPPONENT_WON:"))
                        {
                            string moveData = data.Substring("OPPONENT_WON:".Length);
                            string[] parts = moveData.Split(',');

                            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Invoke(new Action(() =>
                                {
                                    chessBoard.OtherPlayerMove(new Point(x, y));
                                    EndGame("😢 Bạn đã thua trận đấu này!");
                                }));
                            }
                        }

                        if (data == "YOU_WON")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("🎉 Chúc mừng, bạn đã thắng trận đấu!");
                            }));
                        }

                        // Chat messages from opponent (broadcasted by server)
                        if (data.StartsWith("CHAT_FROM:"))
                        {
                            // Format: CHAT_FROM:username:message
                            string payload = data.Substring("CHAT_FROM:".Length);
                            int idx = payload.IndexOf(':');
                            if (idx > 0)
                            {
                                string from = payload.Substring(0, idx);
                                string body = payload.Substring(idx + 1);
                                Invoke(new Action(() =>
                                {
                                    AppendChatMessage(from, body, true);
                                }));
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            try
                            {
                                Invoke(new Action(() =>
                                {
                                    MessageBox.Show("❌ Mất kết nối tới server!", "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Close();
                                }));
                            }
                            catch { }
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void ChessBoard_PlayerClicked(object sender, Point e)
        {
            if (!isMyTurn || chessBoard.isGameOver)
            {
                MessageBox.Show("⚠️ Chưa đến lượt bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // ✅ Luôn gửi GAME_MOVE, server sẽ kiểm tra thắng thua
            string messageToSend = $"GAME_MOVE:{e.X},{e.Y}";

            try
            {
                socket.Send(messageToSend);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi gửi nước đi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EndGame("Mất kết nối với server");
                return;
            }

            StopTurnTimer();

            // Cập nhật trạng thái: bây giờ là lượt của đối thủ
            isMyTurn = false;
            UpdatePlayerStatus(!iAmPlayerX); // Lượt của người kia
        }

        private void ChessBoard_GameEnded(object sender, Player winner)
        {
            StopTurnTimer();
        }

        private void btnResign_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("🏳️ Bạn có chắc muốn đầu hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    socket.Send("RESIGN");
                }
                catch (Exception ex)
                {
                    // Bỏ qua lỗi gửi
                }
                EndGame("🏳️ Bạn đã đầu hàng!");
            }
        }

        private void EndGame(string reason)
        {
            StopTurnTimer();
            chessBoard.isGameOver = true;
            MessageBox.Show(reason, "🎮 Kết thúc ván", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                StopTurnTimer();

                if (socket != null && socket.IsConnected)
                {
                    try
                    {
                        socket.Send("LEAVE_ROOM");
                        Thread.Sleep(100);
                    }
                    catch { }
                }

                cancellationTokenSource?.Cancel();

                if (listenThread != null && listenThread.IsAlive)
                {
                    if (!listenThread.Join(1000))
                    {
                        // Listen thread không dừng trong thời gian chờ
                    }
                }

                cancellationTokenSource?.Dispose();
                turnTimer?.Dispose();
            }
            catch { }

            base.OnFormClosing(e);
        }

        // Append a chat message to the chat box
        private void AppendChatMessage(string sender, string message, bool incoming)
        {
            try
            {
                string time = DateTime.Now.ToString("HH:mm");
                string prefix = incoming ? sender : "Bạn";
                rtbChat.AppendText($"[{time}] {prefix}: {message}{Environment.NewLine}");
                rtbChat.SelectionStart = rtbChat.Text.Length;
                rtbChat.ScrollToCaret();
            }
            catch { }
        }

        private void btnSendChat_Click(object sender, EventArgs e)
        {
            string text = txtChatInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                socket.Send("CHAT:" + text);
                AppendChatMessage("Bạn", text, false);
                txtChatInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi gửi tin nhắn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtChatInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSendChat_Click(this, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pnlBoardContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
