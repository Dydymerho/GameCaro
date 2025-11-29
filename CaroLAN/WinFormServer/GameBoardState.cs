using System;

namespace WinFormServer
{
    internal class GameBoardState
    {
        private const int BOARD_SIZE = 15;
        private readonly int[,] matrix;

        public GameBoardState()
        {
            matrix = new int[BOARD_SIZE, BOARD_SIZE];
        }

        public int GetCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                return -1;
            return matrix[row, col];
        }

        public bool SetCell(int row, int col, int value)
        {
            if (!IsValidPosition(row, col))
                return false;

            if (matrix[row, col] != 0) // Ô đã có quân
                return false;

            matrix[row, col] = value;
            return true;
        }

        public bool CheckWin(int row, int col, int player)
        {
            if (!IsValidPosition(row, col) || matrix[row, col] != player)
                return false;

            int[][] directions = new int[][]
            {
                new int[]{0, 1},   // Ngang
                new int[]{1, 0},   // Dọc
                new int[]{1, 1},   // Chéo chính
                new int[]{1, -1}   // Chéo phụ
            };

            foreach (var dir in directions)
            {
                int count = 1; // Đếm ô hiện tại
                count += CountInDirection(row, col, dir[0], dir[1], player);
                count += CountInDirection(row, col, -dir[0], -dir[1], player);

                if (count >= 5)
                    return true;
            }

            return false;
        }

        public bool IsBoardFull()
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (matrix[i, j] == 0)
                        return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            Array.Clear(matrix, 0, matrix.Length);
        }

        private int CountInDirection(int row, int col, int deltaRow, int deltaCol, int player)
        {
            int count = 0;
            for (int i = 1; i < 5; i++) // Kiểm tra tối đa 4 ô
            {
                int newRow = row + deltaRow * i;
                int newCol = col + deltaCol * i;

                if (!IsValidPosition(newRow, newCol))
                    break;

                if (matrix[newRow, newCol] == player)
                    count++;
                else
                    break;
            }
            return count;
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE;
        }

        public int[,] GetSnapshot()
        {
            return (int[,])matrix.Clone();
        }

        public int GetBoardSize()
        {
            return BOARD_SIZE;
        }
    }
}
