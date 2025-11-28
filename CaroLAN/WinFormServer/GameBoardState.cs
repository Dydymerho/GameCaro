using System;

namespace WinFormServer
{
    /// <summary>
    /// Quản lý trạng thái bàn cờ: ma trận, kiểm tra thắng thua 
    /// </summary>
    internal class GameBoardState
    {
        private const int BOARD_SIZE = 15;
        private readonly int[,] matrix;

        public GameBoardState()
        {
            matrix = new int[BOARD_SIZE, BOARD_SIZE];
        }

        /// <summary>
        /// Lấy giá trị ô tại vị trí (row, col)
        /// </summary>
        /// <returns>0 = trống, 1 = player 1 (X), 2 = player 2 (O), -1 = out of bounds</returns>
        public int GetCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                return -1;
            return matrix[row, col];
        }

        /// <summary>
        /// Đặt giá trị cho ô tại vị trí (row, col)
        /// </summary>
        /// <returns>true nếu đặt thành công, false nếu ô đã có quân hoặc vị trí không hợp lệ</returns>
        public bool SetCell(int row, int col, int value)
        {
            if (!IsValidPosition(row, col))
                return false;

            if (matrix[row, col] != 0) // Ô đã có quân
                return false;

            matrix[row, col] = value;
            return true;
        }

        /// <summary>
        /// Kiểm tra có 5 quân liên tiếp từ vị trí (row, col) không
        /// </summary>
        public bool CheckWin(int row, int col, int player)
        {
            if (!IsValidPosition(row, col) || matrix[row, col] != player)
                return false;

            int[][] directions = new int[][]
            {
                new int[]{0, 1},   // → Ngang
                new int[]{1, 0},   // ↓ Dọc
                new int[]{1, 1},   // ↘ Chéo chính
                new int[]{1, -1}   // ↙ Chéo phụ
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

        /// <summary>
        /// Kiểm tra bàn cờ đã đầy chưa (hòa)
        /// </summary>
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

        /// <summary>
        /// Reset bàn cờ về trạng thái ban đầu
        /// </summary>
        public void Reset()
        {
            Array.Clear(matrix, 0, matrix.Length);
        }

        /// <summary>
        /// Đếm số quân liên tiếp theo một hướng
        /// </summary>
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

        /// <summary>
        /// Kiểm tra vị trí có hợp lệ không
        /// </summary>
        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE;
        }

        /// <summary>
        /// Lấy snapshot của bàn cờ (để debug/logging)
        /// </summary>
        public int[,] GetSnapshot()
        {
            return (int[,])matrix.Clone();
        }

        /// <summary>
        /// Lấy kích thước bàn cờ
        /// </summary>
        public int GetBoardSize()
        {
            return BOARD_SIZE;
        }
    }
}
