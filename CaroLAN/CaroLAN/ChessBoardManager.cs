using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroLAN
{
    public class ChessBoardManager
    {
        public const int BOARD_SIZE = 15;
        public Button[,] board;
        public int[,] matrix;
        public bool isPlayerTurn = true;
        public bool isGameOver = false;

        public event EventHandler<Point> PlayerClicked;
        public event EventHandler<Player> GameEnded;

        // ✅ Thêm biến phân biệt người chơi
        public Player currentPlayer = Player.One; // mặc định: người chơi này là Player.One (X)

        public ChessBoardManager(Panel chessBoard)
        {
            board = new Button[BOARD_SIZE, BOARD_SIZE];
            matrix = new int[BOARD_SIZE, BOARD_SIZE];
            DrawBoard(chessBoard);
        }

        public void DrawBoard(Panel panel)
        {
            panel.Controls.Clear();
            int btnSize = 30;

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    Button btn = new Button()
                    {
                        Width = btnSize,
                        Height = btnSize,
                        Location = new Point(j * btnSize, i * btnSize),
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Tag = new Point(i, j)
                    };
                    btn.Click += Btn_Click;
                    panel.Controls.Add(btn);
                    board[i, j] = btn;
                    matrix[i, j] = 0;
                }
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (!isPlayerTurn || isGameOver) return;

            Button btn = sender as Button;
            Point point = (Point)btn.Tag;
            if (matrix[point.X, point.Y] != 0) return;

            btn.Text = "X";
            btn.ForeColor = Color.Blue;
            matrix[point.X, point.Y] = 1;

            // Gửi tọa độ nước đi ra ngoài
            PlayerClicked?.Invoke(this, point);

            // ✅ Chỉ kiểm tra thắng cho chính người đang đi
            if (CheckWin(point.X, point.Y))
            {
                isGameOver = true;
                GameEnded?.Invoke(this, Player.One); // chỉ bên Player.One hiển thị
            }

            isPlayerTurn = false;
        }

        public void OtherPlayerMove(Point point)
        {
            if (matrix[point.X, point.Y] != 0) return;
            board[point.X, point.Y].Text = "O";
            board[point.X, point.Y].ForeColor = Color.Red;
            matrix[point.X, point.Y] = 2;

            // ✅ Kiểm tra thắng nhưng chỉ thực hiện ở bên nhận, không hiển thị trùng
            if (CheckWin(point.X, point.Y))
            {
                isGameOver = true;
                GameEnded?.Invoke(this, Player.Two);
            }

            isPlayerTurn = true;
        }

        private bool CheckWin(int row, int col)
        {
            int player = matrix[row, col];
            int[][] dirs = new int[][]
            {
                new int[]{0,1}, new int[]{1,0}, new int[]{1,1}, new int[]{1,-1}
            };

            foreach (var d in dirs)
            {
                int count = 1;
                count += Count(row, col, d[0], d[1], player);
                count += Count(row, col, -d[0], -d[1], player);
                if (count >= 5) return true;
            }
            return false;
        }

        private int Count(int r, int c, int dr, int dc, int player)
        {
            int cnt = 0;
            for (int i = 1; i < 5; i++)
            {
                int nr = r + dr * i, nc = c + dc * i;
                if (nr < 0 || nc < 0 || nr >= BOARD_SIZE || nc >= BOARD_SIZE) break;
                if (matrix[nr, nc] == player) cnt++;
                else break;
            }
            return cnt;
        }

        public void ResetBoard()
        {
            isGameOver = false;
            // ✅ Đặt lại lượt về người chơi One (hoặc bạn có thể luân phiên)
            isPlayerTurn = (currentPlayer == Player.One);
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    matrix[i, j] = 0;
                    board[i, j].Text = "";
                    board[i, j].BackColor = Color.White;
                    board[i, j].Enabled = true;
                }
            }
        }
    }

    public enum Player
    {
        One,
        Two
    }
}
