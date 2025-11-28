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
        public bool isPlayerTurn = false;
        public bool isGameOver = false;

        public event EventHandler<Point> PlayerClicked;
        public event EventHandler<Player> GameEnded;

        public Player currentPlayer = Player.One;

        public ChessBoardManager(Panel chessBoard, bool isFirstPlayer)
        {
            board = new Button[BOARD_SIZE, BOARD_SIZE];
            matrix = new int[BOARD_SIZE, BOARD_SIZE];
            
            currentPlayer = isFirstPlayer ? Player.One : Player.Two;
            isPlayerTurn = isFirstPlayer;
            
            DrawBoard(chessBoard);
        }

        public void DrawBoard(Panel panel)
        {

            isGameOver = false;
            if (panel.Controls.Count == 0)
            {
                isPlayerTurn = (currentPlayer == Player.One);
            }

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

            PlayerClicked?.Invoke(this, point);

            isPlayerTurn = false;
        }

        public void OtherPlayerMove(Point point)
        {
            if (matrix[point.X, point.Y] != 0) return;
            
            if (currentPlayer == Player.One)
            {
                board[point.X, point.Y].Text = "O";
                board[point.X, point.Y].ForeColor = Color.Red;
                matrix[point.X, point.Y] = 2;
            }
            else
            {
                board[point.X, point.Y].Text = "X";
                board[point.X, point.Y].ForeColor = Color.Blue;
                matrix[point.X, point.Y] = 1;
            }

            isPlayerTurn = true;
        }

        public void ResetBoard()
        {
            isGameOver = false;
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
