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
            if (playerRooms.TryRemove(player, out string roomId))
            {
                if (rooms.TryGetValue(roomId, out GameRoom room))
                {
                    room.RemovePlayer(player);
                    Console.WriteLine($"[RoomManager] Người chơi {player.RemoteEndPoint} rời phòng {roomId}");

                    // Xóa phòng nếu trống
                    if (room.IsEmpty())
                    {
                        rooms.TryRemove(roomId, out _);
                        Console.WriteLine($"[RoomManager] 🗑️ Phòng {roomId} đã bị xóa (trống)");
                    }
                }
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
        public void BroadcastToRoom(string roomId, string message, Socket? sender = null)
        {
            if (!rooms.TryGetValue(roomId, out GameRoom? room))
                return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            lock (room.Players)
            {
                foreach (var player in room.Players)
                {
                    if (player != sender && player.Connected)
                    {
                        try
                        {
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

        // Gửi tin nhắn từ sender tới các client khác trong cùng phòng (chat phòng / relay)
        public bool RelayMessage(Socket sender, string message)
        {
            var room = GetPlayerRoom(sender);
            if (room == null) return false;

            byte[] data = Encoding.UTF8.GetBytes(message);
            lock (room.Players)
            {
                foreach (var player in room.Players)
                {
                    if (player != sender && player.Connected)
                    {
                        try
                        {
                            player.Send(data);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi gửi tin nhắn tới {player.RemoteEndPoint}: {ex.Message}");
                        }
                    }
                }
            }
            return true;
        }

        // Gửi tin nhắn riêng tới một client xác định bởi endpoint string (ví dụ "127.0.0.1:12345")
        public bool SendPrivateMessage(Socket sender, string recipientEndpointString, string message)
        {
            var room = GetPlayerRoom(sender);
            if (room == null) return false;

            Socket? recipient = null;
            lock (room.Players)
            {
                recipient = room.Players.FirstOrDefault(p => p.RemoteEndPoint?.ToString() == recipientEndpointString);
            }

            if (recipient == null || !recipient.Connected) return false;

            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                recipient.Send(data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi private message tới {recipient.RemoteEndPoint}: {ex.Message}");
                return false;
            }
        }
    }
}

