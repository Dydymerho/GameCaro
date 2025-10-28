using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CaroGame.Share
{
    public static class NetworkHelper
    {
        // Hàm gửi chuỗi (string) qua NetworkStream
        public static void SendMessage(TcpClient client, string message)
        {
            if (client == null || !client.Connected) return;

            // Lấy Stream để đọc/ghi
            NetworkStream stream = client.GetStream();

            // Sử dụng StreamWriter để dễ dàng ghi chuỗi
            // true cho autoFlush đảm bảo tin nhắn được gửi đi ngay lập tức
            using (var writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            // KHÔNG đóng writer ở đây (vì leaveOpen: true), nếu không sẽ đóng luôn NetworkStream
        }

        // Hàm nhận chuỗi (string) từ NetworkStream
        public static string ReceiveMessage(TcpClient client)
        {
            if (client == null || !client.Connected) return null;

            // Lấy Stream
            NetworkStream stream = client.GetStream();

            // Sử dụng StreamReader để dễ dàng đọc chuỗi
            // leaveOpen: true để không đóng NetworkStream khi đóng reader
            using (var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
            {
                // ReadLine() sẽ chờ (blocking) cho đến khi nhận được một dòng
                // (kết thúc bằng ký tự newline) hoặc hết stream.
                return reader.ReadLine();
            }
        }
    }
}