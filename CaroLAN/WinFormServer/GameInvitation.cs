using System;
using System.Net.Sockets;

namespace WinFormServer
{
    internal class GameInvitation
    {
        public string InvitationId { get; private set; }
        public Socket Sender { get; set; }
        public Socket Receiver { get; set; }
        public string SenderEndPoint { get; set; }
        public string ReceiverEndPoint { get; set; }
        public DateTime CreatedTime { get; private set; }
        public bool IsAccepted { get; set; }
        public bool IsExpired { get; set; }

        public GameInvitation(Socket sender, Socket receiver)
        {
            InvitationId = Guid.NewGuid().ToString("N")[..8].ToUpper();
            Sender = sender;
            Receiver = receiver;
            SenderEndPoint = sender.RemoteEndPoint?.ToString() ?? "Unknown";
            ReceiverEndPoint = receiver.RemoteEndPoint?.ToString() ?? "Unknown";
            CreatedTime = DateTime.Now;
            IsAccepted = false;
            IsExpired = false;
        }

        public bool IsValid()
        {
            return !IsExpired && (DateTime.Now - CreatedTime).TotalSeconds < 10;
        }
    }
}
