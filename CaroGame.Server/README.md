# CaroGame.Server

Thành phần **máy chủ** của hệ thống Caro Online, chịu trách nhiệm quản lý kết nối và điều phối các trận đấu giữa người chơi.

## Chức năng chính
- Lắng nghe và chấp nhận kết nối TCP từ client.
- Quản lý danh sách người chơi và phòng chơi.
- Trung gian truyền nước đi giữa hai người chơi.
- Gửi thông báo thắng/thua, khởi tạo hoặc kết thúc ván đấu.

## Thư mục chính
- `ServerManager.cs` – Quản lý danh sách client và logic phòng chơi.
- `ClientHandler.cs` – Xử lý dữ liệu của từng client.
- `Message.cs` – Định nghĩa định dạng gói tin trao đổi.
