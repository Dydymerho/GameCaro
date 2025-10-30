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

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            chessBoard = new ChessBoardManager(pnlChessBoard);
            chessBoard.PlayerClicked += ChessBoard_PlayerClicked;
            chessBoard.GameEnded += ChessBoard_GameEnded;
            socket = new SocketManager();
        }

        private void btnCreateServer_Click(object sender, EventArgs e)
        {
            socket.CreateServer();
            lblStatus.Text = "Đang chờ kết nối...";
            StartListening();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (socket.ConnectToServer(txtIP.Text))
            {
                lblStatus.Text = "Đã kết nối đến server";
                StartListening();
                chessBoard.isPlayerTurn = false; // Client đi sau
            }
            else
            {
                lblStatus.Text = "Không kết nối được server!";
            }
        }
        private void btnRestart_Click(object sender, EventArgs e)
        {
            chessBoard.ResetBoard();

            // Nếu có kết nối LAN, gửi tín hiệu chơi lại cho đối thủ
            if (socket != null && socket.IsConnected)
            {
                socket.Send("RESTART");
            }

            lblStatus.Text = "Đã bắt đầu ván mới!";
        }


        private void StartListening()
        {
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    string data = socket.Receive();
                    if (string.IsNullOrEmpty(data))
                        continue;

                    // ✅ Nếu nhận tín hiệu chơi lại
                    if (data == "RESTART")
                    {
                        // Đặt lại bàn cờ
                        Invoke(new Action(() =>
                        {
                            chessBoard.ResetBoard();
                            lblStatus.Text = "Đối thủ đã khởi động lại ván!";
                        }));
                        continue;
                    }

                    // ✅ Nhận nước đi từ đối thủ (dạng "x,y")
                    string[] parts = data.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        Invoke(new Action(() =>
                        {
                            chessBoard.OtherPlayerMove(new Point(x, y));
                        }));
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }


        private void ChessBoard_PlayerClicked(object sender, Point e)
        {
            socket.Send($"{e.X},{e.Y}");
        }

        private void ChessBoard_GameEnded(object sender, Player winner)
        {
            MessageBox.Show($"Người chơi {winner} thắng!");
        }
    }
}
