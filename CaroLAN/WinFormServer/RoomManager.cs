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
        private ConcurrentDictionary<Socket, string> playerRooms; 

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

            if (room.Players.Count == 1)
            {
                Socket remaining = room.Players[0];
                playerRooms.TryRemove(remaining, out _);
                room.RemovePlayer(remaining); 
            }

            // Nếu phòng trống → xóa phòng
            if (room.IsEmpty())
            {
                rooms.TryRemove(roomId, out _);
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
                                player.Send(data);
                            }
                            catch (Exception ex)
                            {
                                // Bỏ qua lỗi
                            }
                        }
                    }
            }
        }

        // Gửi tin nhắn từ sender tới các client khác trong cùng phòng
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
                            // Bỏ qua lỗi
                        }
                    }
                }
            }
            return true;
        }

        // Gửi tin nhắn riêng tới một client xác định bởi endpoint string 
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
                // Bỏ qua lỗi
                return false;
            }
        }
    }
}

