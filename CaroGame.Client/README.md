# CaroGame.Client

Ứng dụng **client** của game Caro Online, cho phép người chơi kết nối đến máy chủ và tham gia ván đấu trực tuyến.

## Chức năng chính
- Kết nối đến server qua TCP.
- Giao diện người chơi (WinForms hoặc WPF).
- Hiển thị bàn cờ, lượt chơi, kết quả thắng/thua.
- Gửi và nhận nước đi qua mạng.
- (Tuỳ chọn) Hỗ trợ chat và restart trận đấu.

## Thư mục chính
- `GameBoard.cs` – Quản lý bàn cờ và hiển thị giao diện.
- `NetworkManager.cs` – Xử lý kết nối mạng.
- `MessageHandler.cs` – Gửi/nhận và phân tích dữ liệu JSON.
***