using System;

namespace WinFormServer
{
    public class MatchHistory
    {
        public int Id { get; set; }
        public string RoomId { get; set; }
        public int Player1Id { get; set; }
        public string Player1Username { get; set; }
        public int Player2Id { get; set; }
        public string Player2Username { get; set; }
        public int? WinnerId { get; set; }
        public string WinnerUsername { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public MatchHistory()
        {
            RoomId = string.Empty;
            Player1Username = string.Empty;
            Player2Username = string.Empty;
            WinnerUsername = string.Empty;
        }
    }
}

