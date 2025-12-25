# GameCaro - Cờ Caro Online LAN

Bài tập lớn môn Lập trình mạng - Trường Đại học Giao thông Vận tải TP.HCM

## 📝 Mô tả

Ứng dụng chơi cờ Caro trên mạng LAN, phát triển bằng C# WinForms với kiến trúc Client-Server. Người chơi có thể tạo phòng, tham gia phòng và chơi cờ caro với nhau qua mạng LAN.

## 🎯 Tính năng

### Client (CaroLAN)
- **Đăng nhập/Đăng ký**: Hệ thống xác thực người dùng với mã hóa mật khẩu
- **Tự động phát hiện Server**: Tìm kiếm server trên mạng LAN tự động
- **Sảnh chờ**: Xem danh sách phòng có sẵn, tạo phòng mới
- **Trò chơi Cờ Caro**: 
  - Bàn cờ 20x20
  - Đếm ngược thời gian mỗi lượt
  - Chat trong game
  - Hiệu ứng âm thanh
- **Thống kê**: Xem lịch sử trận đấu và thống kê cá nhân

### Server (WinFormServer)
- **Quản lý người dùng**: Đăng ký, đăng nhập, lưu trữ thông tin người dùng
- **Quản lý phòng**: Tạo, xóa, quản lý trạng thái các phòng chơi
- **Game Engine**: Xử lý logic trò chơi, kiểm tra thắng/thua
- **Broadcast Discovery**: Phát hiện tự động trên mạng LAN
- **Lưu lịch sử**: Lưu trữ kết quả trận đấu vào database MySQL

## 🏗️ Kiến trúc

```
GameCaro/
├── CaroLAN/                    # Client Application
│   ├── LoginForm.cs           # Form đăng nhập
│   ├── sanhCho.cs            # Sảnh chờ (Lobby)
│   ├── Form1.cs              # Form chơi game chính
│   ├── SocketManager.cs      # Quản lý kết nối socket client
│   ├── ServerDiscoveryClient.cs  # Tìm kiếm server
│   ├── ChessBoardManager.cs  # Quản lý bàn cờ
│   ├── SoundManager.cs       # Quản lý âm thanh
│   └── SecurityHelper.cs     # Mã hóa mật khẩu
│
└── WinFormServer/             # Server Application
    ├── ServerForm.cs          # Giao diện server
    ├── ServerSocketManager.cs # Quản lý kết nối socket server
    ├── UserManager.cs         # Quản lý người dùng
    ├── RoomManager.cs         # Quản lý phòng chơi
    ├── GameEngine.cs          # Logic trò chơi
    ├── GameModels.cs          # Các model dữ liệu
    ├── BroadcastDiscovery.cs  # Broadcast để client phát hiện
    └── database_schema.sql    # Schema database MySQL
```

## 🔧 Công nghệ sử dụng

- **Framework**: .NET 8.0 Windows Forms
- **Ngôn ngữ**: C#
- **Database**: MySQL
- **Network**: TCP/IP Socket, UDP Broadcast
- **Mã hóa**: SHA256 (password hashing)

## 📋 Yêu cầu hệ thống

- Windows 10/11
- .NET 8.0 Runtime
- MySQL Server 8.0 hoặc mới hơn
- Mạng LAN (để chơi multiplayer)

## 🚀 Cài đặt và Chạy

### 1. Cài đặt MySQL Database



### 2. Khởi chạy My SQL


### 3. Build Project

```powershell
# Mở terminal trong thư mục gốc
cd CaroLAN

# Build toàn bộ solution
dotnet build CaroLAN.sln
```

### 4. Chạy Server

```powershell
# Chạy server trước
cd WinFormServer
dotnet run
```

Hoặc mở `CaroLAN.sln` trong Visual Studio và chạy project **WinFormServer**.

### 5. Chạy Client

```powershell
# Mở terminal mới
cd CaroLAN
dotnet run
```

Hoặc chạy project **CaroLAN** từ Visual Studio.

Có thể chạy nhiều client instance để test multiplayer.

## 🎮 Hướng dẫn sử dụng

### Bước 1: Khởi động Server
1. Chạy ứng dụng WinFormServer
2. Server sẽ tự động lắng nghe kết nối trên port mặc định
3. Server phát broadcast để client có thể tìm thấy

### Bước 2: Đăng nhập Client
1. Chạy ứng dụng CaroLAN
2. Client sẽ tự động tìm server trên mạng LAN
3. Đăng ký tài khoản mới hoặc đăng nhập với tài khoản có sẵn

### Bước 3: Tạo hoặc tham gia phòng
1. Trong sảnh chờ, xem danh sách phòng có sẵn
2. Tạo phòng mới hoặc tham gia phòng đang chờ
3. Đợi đối thủ vào phòng

### Bước 4: Chơi game
1. Người chơi luân phiên đánh cờ
2. Mỗi lượt có thời gian giới hạn
3. Người đầu tiên tạo được 5 quân liên tiếp sẽ thắng
4. Có thể chat với đối thủ trong khi chơi

