# CaroGame.Shared

Thư viện **chung** giữa Client và Server, chứa các lớp mô hình và tiện ích dùng chung.

## Nội dung chính
- `Models/GameMessage.cs` – Định nghĩa cấu trúc dữ liệu trao đổi giữa client và server (JSON message).
- `Utils/JsonHelper.cs` – Hỗ trợ chuyển đổi dữ liệu giữa đối tượng C# và JSON.
- Các hằng số và kiểu dữ liệu dùng chung cho toàn bộ hệ thống Caro Online.

## Mục đích
Giúp đồng bộ dữ liệu giữa client và server, đảm bảo định dạng gói tin thống nhất.
