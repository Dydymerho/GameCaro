using CaroGame.Share;

namespace CaroGame.Core
{
    public class CaroGameManager
    {
        private int[,] Board { get; set; }
        public int CurrentPlayer { get; private set; } = 1; // Bắt đầu bằng Player 1

        public CaroGameManager()
        {
            Board = new int[CaroConstants.BOARD_SIZE, CaroConstants.BOARD_SIZE];
            // Khởi tạo bàn cờ với tất cả các ô là 0 (trống)
        }

        public bool MakeMove(int row, int col, int player)
        {
            if (row < 0 || row >= CaroConstants.BOARD_SIZE || col < 0 || col >= CaroConstants.BOARD_SIZE)
                return false; // Nước đi ngoài phạm vi

            if (Board[row, col] != 0)
                return false; // Ô đã có quân cờ

            if (player != CurrentPlayer)
                return false; // Không phải lượt của người chơi này

            // Đặt quân cờ
            Board[row, col] = player;

            // Chuyển lượt
            CurrentPlayer = (player == 1) ? 2 : 1;

            return true; // Nước đi hợp lệ
        }

        // TODO: Viết hàm kiểm tra thắng thua ở đây
        public bool CheckWin(int lastRow, int lastCol, int player)
        {
            // Logic kiểm tra 5 quân liên tiếp (ngang, dọc, chéo) sẽ được viết ở đây.
            // ... (Phần này sẽ phức tạp hơn)
            return false;
        }

        // Hàm lấy trạng thái bàn cờ (dùng cho Client hiển thị)
        public int[,] GetBoardState()
        {
            return Board;
        }
    }
}