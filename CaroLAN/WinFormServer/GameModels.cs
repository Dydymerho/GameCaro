using System;
using System.Drawing;
using System.Net.Sockets;

namespace WinFormServer
{
    /// <summary>
    /// Lý do game kết thúc
    /// </summary>
    internal enum GameEndReason
    {
        FiveInRow,    // 5 quân liên tiếp
        Resign,       // Đầu hàng
        Disconnect,   // Ngắt kết nối
        Draw          // Hòa (bàn cờ đầy)
    }

    /// <summary>
    /// Kết quả của một nước đi trong game
    /// </summary>
    internal class GameMoveResult
    {
        public bool IsGameOver { get; set; }
        public GameEndReason? EndReason { get; set; }
        public Socket? Winner { get; set; }
        public Socket? Loser { get; set; }
        public Point LastMove { get; set; }
        public string? ErrorMessage { get; set; }
         
        public GameMoveResult()
        {
            IsGameOver = false;
            LastMove = Point.Empty;
        }
    }

    /// <summary>
    /// Kết quả khi game kết thúc
    /// </summary>
    internal class GameEndResult
    {
        public Socket Winner { get; set; }
        public Socket Loser { get; set; }
        public GameEndReason Reason { get; set; }
        public string RoomId { get; set; }

        public GameEndResult(Socket winner, Socket loser, GameEndReason reason, string roomId)
        {
            Winner = winner;
            Loser = loser;
            Reason = reason;
            RoomId = roomId;
        }
    }
}
