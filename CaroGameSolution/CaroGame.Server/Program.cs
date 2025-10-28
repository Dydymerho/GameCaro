using System.Net;
using System.Net.Sockets;
using CaroGame.Share; // Quan trọng: Thêm reference đến NetworkHelper

Console.Title = "SERVER - Caro Game";

try
{
    TcpListener server = new TcpListener(IPAddress.Any, CaroConstants.DEFAULT_PORT);
    server.Start();
    Console.WriteLine($"Server da khoi dong tai {CaroConstants.DEFAULT_IP}:{CaroConstants.DEFAULT_PORT}");
    Console.WriteLine("Dang cho Client ket noi...");

    // Chấp nhận kết nối (Blocking call)
    TcpClient client = server.AcceptTcpClient();

    Console.WriteLine("Client da ket noi thanh cong!");
    Console.WriteLine($"Thong tin Client: {client.Client.RemoteEndPoint}");

    // --- Bổ sung Logic Nhận/Gửi Dữ liệu ---

    // 1. Nhận tin nhắn đầu tiên từ Client
    Console.WriteLine("Dang cho tin nhan tu Client...");
    string clientMessage = NetworkHelper.ReceiveMessage(client);

    if (clientMessage != null)
    {
        Console.WriteLine($"\n<< Client noi: {clientMessage}");

        // 2. Gửi phản hồi lại Client
        string serverResponse = "Chao mung ban da den voi Server Caro!";
        NetworkHelper.SendMessage(client, serverResponse);
        Console.WriteLine($">> Server noi: {serverResponse}");
    }
    else
    {
        Console.WriteLine("Client da ngat ket noi truoc khi gui tin nhan.");
    }

    // --- Kết thúc Logic Nhận/Gửi ---

    // Đợi một chút trước khi đóng (để Client kịp nhận tin nhắn)
    Thread.Sleep(1000);
    client.Close();
    server.Stop();
}
catch (Exception ex)
{
    Console.WriteLine($"\n[ERROR] Co loi xay ra phia Server: {ex.Message}");
}

Console.WriteLine("Nhan Enter de ket thuc Server.");
Console.ReadLine();