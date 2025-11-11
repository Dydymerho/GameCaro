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

        private string roomId; // ✅ tên phòng hiện tại
        private bool isMyTurn = false; // ✅ xác định lượt
        private int timeLeft = 20; // ✅ thời gian mỗi lượt
        private System.Windows.Forms.Timer turnTimer;

        public Form1(string roomId, SocketManager socket, bool startFirst)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            this.roomId = roomId;
            this.socket = socket;
            this.isMyTurn = startFirst;

            // ✅ tạo bàn cờ
            chessBoard = new ChessBoardManager(pnlChessBoard);
            chessBoard.PlayerClicked += ChessBoard_PlayerClicked;
            chessBoard.GameEnded += ChessBoard_GameEnded;

            lblRoom.Text = $"Phòng: {roomId}";
            lblTurn.Text = startFirst ? "Lượt của bạn (X)" : "Lượt của đối thủ (O)";
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
                while (true)
                {
                    if (!socket.IsConnected)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Mất kết nối tới server!", "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Close();
                        }));
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
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void ChessBoard_PlayerClicked(object sender, Point e)
        {
            if (!isMyTurn || chessBoard.isGameOver) return;

            chessBoard.isPlayerTurn = false;
            StopTurnTimer();

            // Gửi nước đi
            socket.Send($"GAME_MOVE:{e.X},{e.Y}");

            // Chuyển lượt
            isMyTurn = false;
            lblTurn.Text = "Lượt của đối thủ";
        }

        private void ChessBoard_GameEnded(object sender, Player winner)
        {
            StopTurnTimer();
            string result = (winner == Player.One) ? "Bạn thắng!" : "Bạn thua!";
            MessageBox.Show(result, "Kết thúc trận đấu");
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
            StopTurnTimer();
            if (listenThread != null && listenThread.IsAlive) listenThread.Abort();
            base.OnFormClosing(e);
        }
    }
}
