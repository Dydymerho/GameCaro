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

        // ✅ Biến phân biệt người chơi - sử dụng để xác định hiển thị X hay O
        public Player currentPlayer = Player.One;

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

            // ✅ Hiển thị X hoặc O dựa vào currentPlayer
            if (currentPlayer == Player.One)
            {
                btn.Text = "X";
                btn.ForeColor = Color.Blue;
                matrix[point.X, point.Y] = 1;
            }
            else
            {
                btn.Text = "O";
                btn.ForeColor = Color.Red;
                matrix[point.X, point.Y] = 2;
            }

            // Gửi tọa độ nước đi ra ngoài
            PlayerClicked?.Invoke(this, point);

            // ✅ Kiểm tra thắng cho chính người đang đi
            if (CheckWin(point.X, point.Y))
            {
                isGameOver = true;
                GameEnded?.Invoke(this, currentPlayer);
            }

            isPlayerTurn = false;
        }

        public void OtherPlayerMove(Point point)
        {
            if (matrix[point.X, point.Y] != 0) return;
            
            // ✅ Hiển thị X hoặc O ngược lại với currentPlayer
            if (currentPlayer == Player.One)
            {
                // Nếu mình là X, đối thủ là O
                board[point.X, point.Y].Text = "O";
                board[point.X, point.Y].ForeColor = Color.Red;
                matrix[point.X, point.Y] = 2;
            }
            else
            {
                // Nếu mình là O, đối thủ là X
                board[point.X, point.Y].Text = "X";
                board[point.X, point.Y].ForeColor = Color.Blue;
                matrix[point.X, point.Y] = 1;
            }

            // ✅ Kiểm tra thắng cho đối thủ
            if (CheckWin(point.X, point.Y))
            {
                isGameOver = true;
                // Đối thủ thắng = người không phải currentPlayer
                Player opponent = (currentPlayer == Player.One) ? Player.Two : Player.One;
                GameEnded?.Invoke(this, opponent);
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
            // ✅ Đặt lại lượt về người chơi tương ứng
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
