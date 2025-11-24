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
        private CancellationTokenSource cancellationTokenSource; // ✅ Thêm CancellationToken

        private string roomId;
        private bool isMyTurn = false;
        private int timeLeft = 20;
        private System.Windows.Forms.Timer turnTimer;

        public Form1(string roomId, SocketManager socket, bool startFirst)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            this.roomId = roomId;
            this.socket = socket;
            this.isMyTurn = startFirst;

            // ✅ Khởi tạo CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();

            // ✅ Tạo bàn cờ với constructor mới - truyền thẳng startFirst
            // startFirst = true  → Player.One → X (màu xanh) → đi trước
            // startFirst = false → Player.Two → O (màu đỏ) → đi sau
            chessBoard = new ChessBoardManager(pnlChessBoard, startFirst);
            chessBoard.PlayerClicked += ChessBoard_PlayerClicked;
            chessBoard.GameEnded += ChessBoard_GameEnded;

            lblRoom.Text = $"Phòng: {roomId}";
            
            // ✅ Hiển thị vai trò rõ ràng
            if (startFirst)
            {
                lblTurn.Text = "Lượt của bạn - Bạn là X (đi trước)";
            }
            else
            {
                lblTurn.Text = "Lượt của đối thủ - Bạn là O (đi sau)";
            }
            
            lblTimer.Text = "";

            InitTimer();

            StartListening();
        }

        // ✅ Bộ đếm thời gian 20s mỗi lượt
        private void InitTimer()
        {
            turnTimer = new System.Windows.Forms.Timer();
            turnTimer.Interval = 1000;
            turnTimer.Tick += (s, e) =>
            {
                if (!chessBoard.isGameOver && isMyTurn)
                {
                    timeLeft--;
                    lblTimer.Text = $"Thời gian: {timeLeft}s";

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
            lblTimer.Text = $"Thời gian: {timeLeft}s";
            turnTimer.Start();
        }

        private void StopTurnTimer()
        {
            turnTimer.Stop();
        }

        private void EndGameDueToTimeout()
        {
            MessageBox.Show("Hết thời gian! Bạn đã thua lượt này.", "Thời gian hết", MessageBoxButtons.OK, MessageBoxIcon.Information);
            socket.Send("RESIGN"); // gửi tín hiệu đầu hàng do hết thời gian
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
                                    MessageBox.Show("Mất kết nối tới server!", "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        // ✅ Nhận nước đi từ đối thủ
                        if (data.StartsWith("GAME_MOVE:"))
                        {
                            string[] parts = data.Substring(10).Split(',');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Invoke(new Action(() =>
                                {
                                    chessBoard.OtherPlayerMove(new Point(x, y));
                                    isMyTurn = true;
                                    lblTurn.Text = "Lượt của bạn";
                                    StartTurnTimer();
                                }));
                            }
                        }

                        // ✅ Nhận tín hiệu đầu hàng
                        if (data == "RESIGN")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("Đối thủ đã đầu hàng!");
                            }));
                        }

                        // ✅ Khi đối thủ rời phòng
                        if (data == "OPPONENT_LEFT")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("Đối thủ đã thoát khỏi phòng.");
                            }));
                        }
                        // ✅ Nhận thông báo đối thủ thắng (mình thua)
                        if (data.StartsWith("OPPONENT_WON:"))
                        {
                            string moveData = data.Substring("OPPONENT_WON:".Length);
                            string[] parts = moveData.Split(',');

                            // Vẫn phải hiển thị nước đi cuối của đối thủ (đó chính là nước đi thắng)
                            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Invoke(new Action(() =>
                                {
                                    chessBoard.OtherPlayerMove(new Point(x, y)); // Hiển thị nước đi thắng
                                    EndGame("Bạn đã thua trận đấu này!");
                                }));
                            }
                        }

                        // ✅ Nhận thông báo mình thắng
                        if (data == "YOU_WON")
                        {
                            Invoke(new Action(() =>
                            {
                                EndGame("Chúc mừng, bạn đã thắng trận đấu!");
                            }));
                        }
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
                MessageBox.Show("Chưa đến lượt bạn!");
                return;
            }
            bool isWinner = chessBoard.CheckWin(e.X, e.Y);
            string messageToSend = isWinner ? $"GAME_WIN:{e.X},{e.Y}" : $"GAME_MOVE:{e.X},{e.Y}";
            socket.Send(messageToSend);

            StopTurnTimer();

            lblTurn.Text = "Lượt của đối thủ";
        }

        private void ChessBoard_GameEnded(object sender, Player winner)
        {
            StopTurnTimer();
        }

        private void btnResign_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn đầu hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                socket.Send("RESIGN");
                EndGame("Bạn đã đầu hàng!");
            }
        }

        private void EndGame(string reason)
        {
            StopTurnTimer();
            chessBoard.isGameOver = true;
            MessageBox.Show(reason, "Kết thúc ván", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                StopTurnTimer();
                
                // ✅ Gửi tín hiệu rời phòng cho server
                if (socket != null && socket.IsConnected)
                {
                    try
                    {
                        socket.Send("LEAVE_ROOM");
                        Thread.Sleep(100); // Đợi message được gửi đi
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi gửi
                    }
                }

                // ✅ Hủy thread an toàn bằng CancellationToken
                cancellationTokenSource?.Cancel();

                // ✅ Đợi thread kết thúc (tối đa 1 giây)
                if (listenThread != null && listenThread.IsAlive)
                {
                    if (!listenThread.Join(1000))
                    {
                        System.Diagnostics.Debug.WriteLine("Listen thread không dừng trong thời gian chờ");
                    }
                }

                // ✅ Dispose các tài nguyên
                cancellationTokenSource?.Dispose();
                turnTimer?.Dispose();
            }
            catch
            {
                // Bỏ qua lỗi khi đóng form
            }
            
            base.OnFormClosing(e);
        }
    }
}
