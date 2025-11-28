using System;
using System.Drawing;
using System.Net.Sockets;

namespace WinFormServer
{
    /// Quản lý toàn bộ logic game: xử lý nước đi, kiểm tra thắng thua, cập nhật stats
 
    internal class GameEngine
    {
        private readonly UserManager userManager;
        private readonly Action<string>? logAction;

        public GameEngine(UserManager userManager, Action<string>? logAction = null)
        {
            this.userManager = userManager;
            this.logAction = logAction;
        }
        public GameMoveResult ProcessMove(
            GameBoardState boardState,
            GameRoom room,
            Socket playerSocket,
            int row,
            int col,
            Func<Socket, User?> getUserFunc)
        {
            var result = new GameMoveResult
            {
                LastMove = new Point(row, col)
            };

            // Xác định người chơi (player 1 = X = 1, player 2 = O = 2)
            int playerValue = room.Players[0] == playerSocket ? 1 : 2;

            // Kiểm tra ô đã được đánh chưa
            if (boardState.GetCell(row, col) != 0)
            {
                result.ErrorMessage = "Ô này đã được đánh";
                return result;
            }

            // Đánh dấu nước đi
            if (!boardState.SetCell(row, col, playerValue))
            {
                result.ErrorMessage = "Không thể đánh nước này";
                return result;
            }

            // Kiểm tra thắng thua
            bool isWinner = boardState.CheckWin(row, col, playerValue);
            bool isDraw = !isWinner && boardState.IsBoardFull();

            if (isWinner)
            {
                result.IsGameOver = true;
                result.EndReason = GameEndReason.FiveInRow;
                result.Winner = playerSocket;
                result.Loser = room.GetOpponent(playerSocket);

                // Cập nhật stats và lịch sử
                UpdateGameStats(room, result.Winner, result.Loser, getUserFunc);
            }
            else if (isDraw)
            {
                result.IsGameOver = true;
                result.EndReason = GameEndReason.Draw;
                // Trong trường hợp hòa, không có winner/loser
                logAction?.Invoke($"🤝 Trận đấu trong phòng {room.RoomId} hòa");
            }

            return result;
        }

        /// Xử lý khi người chơi đầu hàng
        public GameEndResult ProcessResign(
            GameRoom room,
            Socket resignerSocket,
            Func<Socket, User?> getUserFunc)
        {
            Socket opponentSocket = room.GetOpponent(resignerSocket);

            var result = new GameEndResult(
                winner: opponentSocket,
                loser: resignerSocket,
                reason: GameEndReason.Resign,
                roomId: room.RoomId
            );

            // Cập nhật stats
            UpdateGameStats(room, result.Winner, result.Loser, getUserFunc);

            logAction?.Invoke($"💀 {GetUsername(resignerSocket, getUserFunc)} đầu hàng trong phòng {room.RoomId}");

            return result;
        }

        /// Xử lý khi người chơi rời phòng (ngắt kết nối)
        public GameEndResult? ProcessDisconnect(
            GameRoom room,
            Socket disconnectedSocket,
            Func<Socket, User?> getUserFunc)
        {
            if (!room.IsGameStarted || room.Players.Count < 2)
            {
                return null; // Không xử lý nếu game chưa bắt đầu
            }

            Socket opponentSocket = room.GetOpponent(disconnectedSocket);

            var result = new GameEndResult(
                winner: opponentSocket,
                loser: disconnectedSocket,
                reason: GameEndReason.Disconnect,
                roomId: room.RoomId
            );

            // Cập nhật stats
            UpdateGameStats(room, result.Winner, result.Loser, getUserFunc);

            logAction?.Invoke($"👋 {GetUsername(disconnectedSocket, getUserFunc)} rời phòng {room.RoomId} - đối thủ thắng");

            return result;
        }

        /// Cập nhật thống kê và lưu lịch sử cho cả hai người chơi
        private void UpdateGameStats(
            GameRoom room,
            Socket? winnerSocket,
            Socket? loserSocket,
            Func<Socket, User?> getUserFunc)
        {
            User? winner = winnerSocket != null ? getUserFunc(winnerSocket) : null;
            User? loser = loserSocket != null ? getUserFunc(loserSocket) : null;

            if (winner != null)
            {
                userManager.UpdateGameStats(winner.Id, true);
                logAction?.Invoke($"🏆 {winner.Username} thắng");
            }

            if (loser != null)
            {
                userManager.UpdateGameStats(loser.Id, false);
                logAction?.Invoke($"💀 {loser.Username} thua");
            }

            // Lưu lịch sử đấu - sử dụng trực tiếp winner/loser thay vì room.Players
            // vì room.Players có thể đã thay đổi khi disconnect
            if (winner != null && loser != null)
            {
                int winnerId = winner.Id;
                int loserId = loser.Id;

                if (winnerId > 0 && loserId > 0)
                {
                    // player1 = winner, player2 = loser
                    bool saved = userManager.SaveMatchHistory(room.RoomId, winnerId, loserId, winnerId);
                    if (saved)
                    {
                        logAction?.Invoke($"📝 Đã lưu lịch sử đấu: {room.RoomId} (Winner: {winner.Username}, Loser: {loser.Username})");
                    }
                    else
                    {
                        logAction?.Invoke($"❌ Lỗi lưu lịch sử đấu: {room.RoomId}");
                    }
                }
                else
                {
                    logAction?.Invoke($"⚠️ Không thể lưu lịch sử: winnerId={winnerId}, loserId={loserId}");
                }
            }
            else
            {
                logAction?.Invoke($"⚠️ Không thể lưu lịch sử: winner={winner?.Username ?? "null"}, loser={loser?.Username ?? "null"}");
            }
        }

        /// Lấy username từ socket
        private string GetUsername(Socket socket, Func<Socket, User?> getUserFunc)
        {
            var user = getUserFunc(socket);
            return user?.Username ?? socket.RemoteEndPoint?.ToString() ?? "Unknown";
        }
    }
}
