namespace CaroGame.Share
{
    public static class CaroConstants
    {
        public const int BOARD_SIZE = 20; // Bàn cờ 20x20
        public const int WIN_COUNT = 5;    // Cần 5 quân liên tiếp để thắng
        public const int DEFAULT_PORT = 9999;
        public const string DEFAULT_IP = "127.0.0.1";
    }

    // Định nghĩa các loại tin nhắn/lệnh
    public enum Command
    {
        CONNECT_REQUEST, // Yêu cầu kết nối
        CONNECT_SUCCESS, // Kết nối thành công
        CHALLENGE_REQUEST, // Yêu cầu thách đấu
        MOVE,              // Gửi nước đi
        GAME_OVER,         // Trận đấu kết thúc
        ERROR              // Tin nhắn lỗi
    }

    // Định nghĩa cấu trúc nước đi
    public class MoveData
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; } // 1 hoặc 2
    }
}