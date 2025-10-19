# CaroGame.Core

Thư viện **xử lý logic trò chơi** của Caro Online, chứa toàn bộ quy tắc và thuật toán kiểm tra kết quả.

## Chức năng chính
- Quản lý trạng thái bàn cờ (mảng 2 chiều).
- Xử lý lượt chơi và đánh dấu ô cờ.
- Kiểm tra điều kiện thắng/thua hoặc hòa.
- Hỗ trợ khởi tạo và đặt lại ván chơi.

## Thư mục chính
- `Board.cs` – Mô hình bàn cờ và dữ liệu quân cờ.
- `GameLogic.cs` – Thuật toán kiểm tra thắng.
- `Player.cs` – Thông tin người chơi và lượt đánh.
