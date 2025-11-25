# Tự động cập nhật lịch sử đấu và Sửa lỗi crash nhiều client

## ⚠️ Sửa lỗi crash khi chạy nhiều client (Mới nhất)

### Vấn đề
Khi chạy 2 hoặc nhiều client cùng lúc trên cùng một máy, client thứ 2 trở đi bị crash.

### Nguyên nhân
1. **Timeout quá cao** (60 giây) làm thread bị block lâu khi receive
2. **Không có ReuseAddress**: Socket không cho phép nhiều client bind vào cùng interface
3. **Xử lý exception không đầy đủ**: Khi có lỗi network, app crash thay vì xử lý gracefully
4. **Receive() blocking**: Method Receive() block thread cho đến khi có data hoặc timeout

### Các sửa đổi trong SocketManager.cs

#### 1. Thêm Socket Options
```csharp
// ✅ Cho phép tái sử dụng địa chỉ (quan trọng khi chạy nhiều client trên cùng máy)
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

// ✅ Giảm timeout xuống 10 giây để tránh block quá lâu
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

// ✅ Tắt Nagle algorithm để giảm latency
socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
```

#### 2. Cải thiện Receive() - Non-blocking
```csharp
// ✅ Kiểm tra xem có dữ liệu sẵn sàng không trước khi receive
if (socket.Available == 0)
{
    // ✅ Poll với timeout ngắn để không block lâu
    if (!socket.Poll(100000, SelectMode.SelectRead)) // 100ms
    {
        return string.Empty;
    }
}
```

#### 3. Xử lý exception chi tiết hơn
```csharp
catch (SocketException ex)
{
    // ✅ Chỉ set isConnected = false nếu là lỗi nghiêm trọng
    if (ex.SocketErrorCode != SocketError.WouldBlock && 
        ex.SocketErrorCode != SocketError.TimedOut)
    {
        isConnected = false;
    }
    return string.Empty;
}
```

### Các sửa đổi trong LoginForm.cs và sanhCho.cs

#### 1. Thêm try-catch cho tất cả socket.Send()
```csharp
try
{
    socket.Send($"LOGIN:{username}:{password}");
    lblStatus.Text = "Đang đăng nhập...";
}
catch (Exception ex)
{
    lblStatus.Text = "Lỗi gửi dữ liệu";
    MessageBox.Show($"Không thể gửi yêu cầu: {ex.Message}", "Lỗi");
}
```

#### 2. Cải thiện xử lý trong StartListening()
```csharp
catch (InvalidOperationException)
{
    // Form đã bị đóng hoặc Invoke không thể thực thi
    if (!token.IsCancellationRequested)
    {
        try
        {
            Invoke(new Action(() => { lblStatus.Text = "Mất kết nối"; }));
        }
        catch { }
    }
    break;
}
```

### Các sửa đổi trong Form1.cs

#### 1. Try-catch khi gửi nước đi
```csharp
try
{
    socket.Send(messageToSend);
}
catch (Exception ex)
{
    MessageBox.Show($"Lỗi gửi nước đi: {ex.Message}", "Lỗi");
    EndGame("Mất kết nối với server");
    return;
}
```

### Kết quả sau khi sửa

✅ **Chạy được nhiều client cùng lúc** trên cùng một máy
✅ **Không crash** khi có lỗi network
✅ **Response nhanh hơn**: 60s → 10s timeout
✅ **Graceful error handling**: Hiển thị lỗi thay vì crash
✅ **Non-blocking receive**: Không block thread lâu

### Test đã thực hiện

| Trường hợp | Kết quả |
|------------|---------|
| Chạy 2 client cùng lúc | ✅ Hoạt động tốt |
| Client 2 crash khi client 1 đang chơi | ✅ Không xảy ra |
| Network bị lỗi giữa chừng | ✅ Hiển thị lỗi, không crash |
| Server stop đột ngột | ✅ Client detect và thông báo |

---

## Các cải tiến đã thực hiện (Trước đó)

### 1. Tự động cập nhật định kỳ
- **Timer tự động**: Giảm interval từ 30 giây xuống **5 giây** để cập nhật lịch sử nhanh hơn
- Hoạt động liên tục khi client đang kết nối và không trong phòng
- Tự động dừng khi ngắt kết nối hoặc đóng form

### 2. Cập nhật ngay sau khi kết thúc trận
- Khi game kết thúc và quay về sảnh chờ, hệ thống tự động:
  - Đợi **1 giây** để đảm bảo server đã lưu lịch sử
  - Tự động gọi `LoadHistory()` để cập nhật cả 2 tab lịch sử
  - Hiển thị thông báo "Đã cập nhật lịch sử đấu"

### 3. Broadcast thông báo match mới
- Server **broadcast** message `NEW_MATCH_RECORDED` đến tất cả client khi:
  - Có người thắng trận (`HandleGameWin`)
  - Có người đầu hàng (`HandleResign`)
  - Có người rời phòng khi game đang chơi (`HandleLeaveRoom`)

- Tất cả client nhận được thông báo sẽ **tự động tải lại lịch sử** ngay lập tức
- Chỉ cập nhật nếu client không đang trong phòng chơi

### 4. Tự động tải lịch sử khi kết nối
- Khi kết nối/đăng nhập lại thành công, client tự động:
  - Tải **tất cả lịch sử đấu** (`GET_ALL_HISTORY`)
  - Tải **lịch sử của user** nếu đã đăng nhập (`GET_MY_HISTORY`)

## Cách hoạt động

### Luồng cập nhật lịch sử

```
1. Game kết thúc (thắng/thua/đầu hàng/rời phòng)
   ↓
2. Server lưu vào database
   ↓
3. Server broadcast "NEW_MATCH_RECORDED" → Tất cả client
   ↓
4. Client nhận thông báo → Tự động LoadHistory()
   ↓
5. Server gửi HISTORY_ALL và HISTORY_MY
   ↓
6. Client cập nhật giao diện
```

### Timer backup (mỗi 5 giây)

```
Timer tick (5 giây)
   ↓
Kiểm tra: IsConnected && !IsInRoom
   ↓
Gọi LoadHistory()
   ↓
Cập nhật cả 2 tab lịch sử
```

## Các file đã chỉnh sửa

### 1. `sanhCho.cs` (Client)
- Giảm `historyRefreshTimer.Interval` từ 30000ms → **5000ms**
- Cải thiện logic cập nhật sau khi game kết thúc (dùng Timer thay vì Thread.Sleep)
- Thêm xử lý message `NEW_MATCH_RECORDED` để tự động tải lịch sử

### 2. `ServerSocketManager.cs` (Server)
- Thêm `Broadcast("NEW_MATCH_RECORDED", clients, logAction)` sau khi lưu lịch sử trong:
  - `HandleGameWin()`
  - `HandleResign()`
  - `HandleLeaveRoom()`

## Lợi ích

✅ **Cập nhật theo thời gian thực**: Tất cả client đều thấy match mới ngay sau 1 giây

✅ **Giảm độ trễ**: Từ 30 giây → 5 giây (timer backup)

✅ **Đồng bộ toàn server**: Mọi người đều thấy lịch sử giống nhau

✅ **UX tốt hơn**: Không cần bấm refresh thủ công

✅ **Tự động khôi phục**: Kết nối lại tự động tải lịch sử

## Test các trường hợp

### Test 1: Kết thúc game bình thường
1. Hai người chơi vào phòng
2. Một người thắng
3. Cả hai quay về sảnh chờ
4. **Kết quả mong đợi**: Lịch sử cập nhật ngay trong vòng 1-2 giây

### Test 2: Client thứ 3 không tham gia
1. Client A và B đang chơi
2. Client C đang ở sảnh chờ
3. A thắng B
4. **Kết quả mong đợi**: Client C cũng thấy match mới ngay lập tức

### Test 3: Đầu hàng
1. Hai người chơi đang chơi
2. Một người bấm "Đầu hàng"
3. **Kết quả mong đợi**: Lịch sử lưu và broadcast cho tất cả

### Test 4: Rời phòng khi đang chơi
1. Hai người đang chơi
2. Một người đóng form game
3. **Kết quả mong đợi**: Người còn lại thắng, lịch sử được lưu và broadcast

### Test 5: Mất kết nối rồi kết nối lại
1. Client ngắt kết nối
2. Client kết nối lại (tự động đăng nhập)
3. **Kết quả mong đợi**: Lịch sử tự động tải lại

## Cấu hình

Có thể điều chỉnh interval của timer trong `sanhCho.cs`:

```csharp
historyRefreshTimer.Interval = 5000;

```

**Khuyến nghị**: Giữ ở **5 giây** để cân bằng giữa real-time và tải server.

## Lưu ý

- Timer chỉ chạy khi **client không trong phòng chơi**
- Broadcast chỉ gửi khi **lịch sử đã lưu thành công** vào database
- Client chỉ cập nhật khi **đang kết nối** đến server
