using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinFormServer
{
    internal class GameRoom
    {
        public string RoomId { get; private set; }
        public List<Socket> Players { get; private set; }
        public bool IsGameStarted { get; set; }
        public DateTime CreatedTime { get; private set; }
        public int MaxPlayers { get; } = 2;

        public GameRoom(string roomId)
        {
            RoomId = roomId;
            Players = new List<Socket>();
            IsGameStarted = false;
            CreatedTime = DateTime.Now;
        }

        public bool AddPlayer(Socket player)
        {
            if (Players.Count < MaxPlayers && !Players.Contains(player))
            {
                Players.Add(player);
                return true;
            }
            return false;
        }

        public void RemovePlayer(Socket player)
        {
            Players.Remove(player);
        }

        public bool IsFull()
        {
            return Players.Count >= MaxPlayers;
        }

        public bool IsEmpty()
        {
            return Players.Count == 0;
        }

        public Socket GetOpponent(Socket player)
        {
            return Players.FirstOrDefault(p => p != player);
        }
    }
}
