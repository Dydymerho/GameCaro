using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinFormServer
{
    internal class RoomManager
    {
        private ConcurrentDictionary<string, GameRoom> rooms;
        private ConcurrentDictionary<Socket, string> playerRooms; // Mapping player -> roomId

        public RoomManager()
        {
            rooms = new ConcurrentDictionary<string, GameRoom>();
            playerRooms = new ConcurrentDictionary<Socket, string>();
        }

        public string CreateRoom()
        {
            string roomId = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var room = new GameRoom(roomId);
            rooms.TryAdd(roomId, room);
            return roomId;
        }

        public bool JoinRoom(Socket player, string roomId = null)
        {
            // Nếu không chỉ định roomId, tìm phòng có sẵn hoặc tạo mới
            if (string.IsNullOrEmpty(roomId))
            {
                // Tìm phòng chưa đầy người
                var availableRoom = rooms.Values.FirstOrDefault(r => !r.IsFull() && !r.IsGameStarted);

                if (availableRoom != null)
                {
                    roomId = availableRoom.RoomId;
                    Console.WriteLine($"[RoomManager] Người chơi ghép vào phòng sẵn có: {roomId}");
                }
                else
                {
                    // Tạo phòng mới
                    roomId = CreateRoom();
                }
            }

            // Kiểm tra phòng có tồn tại không
            if (!rooms.TryGetValue(roomId, out GameRoom room))
                return false;

            // Thêm người chơi vào phòng
            if (room.AddPlayer(player))
            {
                playerRooms.TryAdd(player, roomId);
                Console.WriteLine($"[RoomManager] Người chơi {player.RemoteEndPoint} vào phòng {roomId} ({room.Players.Count}/2)");
                return true;
            }

            return false;
        }

        public void LeaveRoom(Socket player)
        {
            if (!playerRooms.TryRemove(player, out string roomId))
                return;

            if (!rooms.TryGetValue(roomId, out GameRoom room))
                return;

            // Xóa player khỏi room
            room.RemovePlayer(player);
            Console.WriteLine($"[RoomManager] Người chơi {player.RemoteEndPoint} rời phòng {roomId}");

            // Nếu còn một người → xóa mapping của người còn lại để họ trở thành rảnh
            if (room.Players.Count == 1)
            {
                Socket remaining = room.Players[0];
                playerRooms.TryRemove(remaining, out _);  // 
                Console.WriteLine($"[RoomManager] Người chơi còn lại {remaining.RemoteEndPoint} được giải phóng khỏi phòng");
            }

            // Nếu phòng trống → xóa phòng
            if (room.IsEmpty())
            {
                rooms.TryRemove(roomId, out _);
                Console.WriteLine($"[RoomManager] 🗑️ Phòng {roomId} đã bị xóa (trống)");
            }
        }


        public GameRoom GetPlayerRoom(Socket player)
        {
            if (playerRooms.TryGetValue(player, out string roomId))
            {
                rooms.TryGetValue(roomId, out GameRoom room);
                return room;
            }
            return null;
        }

        public List<GameRoom> GetAllRooms()
        {
            return rooms.Values.ToList();
        }

        public void BroadcastToRoom(string roomId, string message, Socket sender = null)
        {
            if (rooms.TryGetValue(roomId, out GameRoom room))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                lock (room.Players)
                    foreach (var player in room.Players)
                    {
                        if (player != sender && player.Connected)
                        {
                            try
                            {
                                // MessageBox.Show("start");
                                player.Send(data);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi gửi dữ liệu tới {player.RemoteEndPoint}: {ex.Message}");
                            }
                        }
                    }
            }
        }

    }
}

