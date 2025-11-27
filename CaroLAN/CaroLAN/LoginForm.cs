using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CaroLAN
{
    public partial class LoginForm : Form
    {
        private readonly SocketManager socket;
        private Thread? listenThread;
        private CancellationTokenSource cancellationTokenSource;
        private ServerDiscoveryClient? serverDiscovery; // ✅ Server discovery client
        private Queue<string> pendingMessages = new Queue<string>(); // ✅ Lưu message chưa xử lý

        private bool isLoggedIn;
        private string currentUsername = string.Empty;
        private string currentPassword = string.Empty; // ✅ Lưu password để tự động đăng nhập lại
        private int userId;
        private int totalGames;
        private int wins;
        private int losses;

        private void LoginForm_Load(object? sender, EventArgs? e)
        {
            // ✅ Khởi tạo trạng thái ban đầu
            UpdateConnectionState(false);
            
            //  Tự động tìm server
            AutoFindAndConnectServer();
        }


        public LoginForm()
        {
            InitializeComponent();
            socket = new SocketManager();
            cancellationTokenSource = new CancellationTokenSource();
            serverDiscovery = new ServerDiscoveryClient(); // ✅ Khởi tạo server discovery
            lblStatus.Text = "Chưa kết nối";

            this.Load += LoginForm_Load; // auto connect to localhost
        }

        // ✅ Method cập nhật trạng thái enable/disable các button dựa trên kết nối
        private void UpdateConnectionState(bool isConnected)
        {
            // Khi chưa kết nối: chỉ enable nút kết nối và tìm server
            // Khi đã kết nối: enable các tab đăng nhập/đăng ký
            btnLogin.Enabled = isConnected;
            btnRegister.Enabled = isConnected;
            tabControl1.Enabled = isConnected;
            
            if (!isConnected)
            {
                lblStatus.Text = "⚪ Chưa kết nối - Vui lòng kết nối server";
            }
        }

        /// ✅ Xử lý nút tìm server
        private void btnFindServers_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "🔍 Đang tìm server...";
                btnFindServers.Enabled = false;
                Application.DoEvents();

                List<DiscoveredServer> foundServers = new List<DiscoveredServer>();

                serverDiscovery?.StartDiscovery(
                    onServerFound: (server) =>
                    {
                        // Callback khi tìm thấy server mới
                        foundServers.Add(server);
                    },
                    onDiscoveryComplete: (servers) =>
                    {
                        // Callback khi quét xong
                        Invoke(new Action(() =>
                        {
                            btnFindServers.Enabled = true;

                            if (servers.Count == 0)
                            {
                                lblStatus.Text = "Không tìm thấy server nào";
                                MessageBox.Show("Không tìm thấy server trong mạng LAN.\n\nVui lòng đảm bảo:\n- Server đã được bật\n- Cả server và client trong cùng mạng LAN\n- Firewall không chặn kết nối",
                                    "Không tìm thấy server",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                            else if (servers.Count == 1)
                            {
                                // Chỉ có 1 server, tự động điền IP và kết nối
                                txtServerIP.Text = servers[0].IPAddress;
                                lblStatus.Text = $"✅ Tìm thấy: {servers[0].ServerName}";
                                
                                // ✅ Tự động kết nối
                                ConnectToSelectedServer();
                            }
                            else
                            {
                                // Nhiều server, cho phép chọn
                                ShowServerSelectionDialog(servers);
                            }
                        }));
                    }
                );
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi khi tìm server";
                btnFindServers.Enabled = true;
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// ✅ Hiển thị dialog để chọn server khi tìm thấy nhiều server
        private void ShowServerSelectionDialog(List<DiscoveredServer> servers)
        {
            Form selectionForm = new Form
            {
                Text = "Chọn server",
                Width = 400,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblInfo = new Label
            {
                Text = $"Tìm thấy {servers.Count} server trong mạng LAN:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            ListBox lstServers = new ListBox
            {
                Location = new Point(20, 50),
                Width = 340,
                Height = 150
            };

            foreach (var server in servers)
            {
                lstServers.Items.Add(server);
            }

            if (lstServers.Items.Count > 0)
            {
                lstServers.SelectedIndex = 0;
            }

            Button btnSelect = new Button
            {
                Text = "Chọn",
                Location = new Point(180, 215),
                Width = 80,
                DialogResult = DialogResult.OK
            };

            Button btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(270, 215),
                Width = 80,
                DialogResult = DialogResult.Cancel
            };

            selectionForm.Controls.Add(lblInfo);
            selectionForm.Controls.Add(lstServers);
            selectionForm.Controls.Add(btnSelect);
            selectionForm.Controls.Add(btnCancel);

            selectionForm.AcceptButton = btnSelect;
            selectionForm.CancelButton = btnCancel;

            if (selectionForm.ShowDialog() == DialogResult.OK && lstServers.SelectedItem != null)
            {
                DiscoveredServer selected = (DiscoveredServer)lstServers.SelectedItem;
                txtServerIP.Text = selected.IPAddress;
                lblStatus.Text = $"✅ Đã chọn: {selected.ServerName}";
                
                // ✅ Tự động kết nối sau khi chọn
                ConnectToSelectedServer();
            }
            else
            {
                lblStatus.Text = "Đã hủy chọn server";
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string serverIP = txtServerIP.Text.Trim();

            if (string.IsNullOrWhiteSpace(serverIP))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServerIP.Focus();
                return;
            }

            if (socket.IsConnected)
            {
                // Ngắt kết nối thủ công
                socket.Disconnect();
                lblStatus.Text = "Đã ngắt kết nối";
                btnConnect.Text = "Kết nối";
                btnConnect.Enabled = true;
                txtServerIP.Enabled = true;
                UpdateConnectionState(false); // ✅ Disable các button
                return;
            }

            try
            {
                lblStatus.Text = "Đang kết nối...";
                btnConnect.Enabled = false;
                Application.DoEvents();

                if (socket.ConnectToServer(serverIP))
                {
                    lblStatus.Text = "✅ Đã kết nối đến server";
                    btnConnect.Text = "Ngắt kết nối";
                    btnConnect.Enabled = true;
                    txtServerIP.Enabled = false;
                    UpdateConnectionState(true); // ✅ Enable các button
                }
                else
                {
                    lblStatus.Text = "❌ Không kết nối được server";
                    btnConnect.Enabled = true;
                    UpdateConnectionState(false); // ✅ Disable các button
                    MessageBox.Show("Không thể kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi kết nối";
                btnConnect.Enabled = true;
                UpdateConnectionState(false); // ✅ Disable các button
                MessageBox.Show($"Lỗi khi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// ✅ Tự động tìm server và kết nối khi form load
        private void AutoFindAndConnectServer()
        {
            try
            {
                lblStatus.Text = "🔍 Đang tìm server trong mạng LAN...";
                btnConnect.Enabled = false;
                btnFindServers.Enabled = false;
                Application.DoEvents();

                serverDiscovery?.StartDiscovery(
                    onServerFound: (server) =>
                    {
                        // Không cần xử lý gì khi tìm thấy từng server
                    },
                    onDiscoveryComplete: (servers) =>
                    {
                        // Callback khi quét xong
                        Invoke(new Action(() =>
                        {
                            btnConnect.Enabled = true;
                            btnFindServers.Enabled = true;

                            if (servers.Count == 0)
                            {
                                lblStatus.Text = "⚠️ Không tìm thấy server. Vui lòng nhập IP thủ công.";
                                txtServerIP.Focus();
                                UpdateConnectionState(false); // ✅ Disable các button
                            }
                            else if (servers.Count == 1)
                            {
                                // Chỉ có 1 server, tự động kết nối
                                txtServerIP.Text = servers[0].IPAddress;
                                lblStatus.Text = $"✅ Tìm thấy: {servers[0].ServerName}";
                                ConnectToSelectedServer();
                            }
                            else
                            {
                                // Nhiều server, cho phép chọn
                                lblStatus.Text = $"Tìm thấy {servers.Count} server. Vui lòng chọn.";
                                ShowServerSelectionDialog(servers);
                            }
                        }));
                    }
                );
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi khi tìm server";
                btnConnect.Enabled = true;
                btnFindServers.Enabled = true;
                UpdateConnectionState(false); // ✅ Disable các button
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///  Kết nối đến server đã chọn
        private void ConnectToSelectedServer()
        {
            string serverIP = txtServerIP.Text.Trim();

            if (string.IsNullOrWhiteSpace(serverIP))
            {
                lblStatus.Text = "Chưa có địa chỉ server";
                UpdateConnectionState(false); // ✅ Disable các button
                return;
            }

            try
            {
                lblStatus.Text = "Đang kết nối...";
                btnConnect.Enabled = false;
                Application.DoEvents();

                if (socket.ConnectToServer(serverIP))
                {
                    lblStatus.Text = "✅ Đã kết nối đến server";
                    btnConnect.Text = "Ngắt kết nối";
                    btnConnect.Enabled = true;
                    txtServerIP.Enabled = false;
                    UpdateConnectionState(true); // ✅ Enable các button
                }
                else
                {
                    lblStatus.Text = "❌ Không kết nối được server";
                    btnConnect.Enabled = true;
                    UpdateConnectionState(false); // ✅ Disable các button
                    MessageBox.Show("Không thể kết nối đến server!\nVui lòng kiểm tra:\n- Server đã bật\n- Địa chỉ IP đúng\n- Firewall không chặn", 
                        "Lỗi kết nối", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Lỗi kết nối";
                btnConnect.Enabled = true;
                UpdateConnectionState(false); // ✅ Disable các button
                MessageBox.Show($"Lỗi khi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!socket.IsConnected)
            {
                MessageBox.Show("Bạn chưa kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ✅ Lưu password để tự động đăng nhập lại khi reconnect
            currentPassword = password;

            try
            {
                System.Diagnostics.Debug.WriteLine($"📤 LoginForm gửi: LOGIN:{username}:***");
                socket.Send($"LOGIN:{username}:{password}");
                lblStatus.Text = "Đang đăng nhập...";
                
                // ✅ Đợi response từ server (tối đa 5 giây)
                string? response = WaitForResponse("LOGIN_", 5000);
                
                if (response != null && response.StartsWith("LOGIN_SUCCESS:"))
                {
                    var match = Regex.Match(response, @"^LOGIN_SUCCESS:(\d+):([^:]+):(\d+):(\d+):(\d+)");
                    if (match.Success)
                    {
                        userId = int.Parse(match.Groups[1].Value);
                        currentUsername = match.Groups[2].Value;
                        totalGames = int.Parse(match.Groups[3].Value);
                        wins = int.Parse(match.Groups[4].Value);
                        losses = int.Parse(match.Groups[5].Value);
                        
                        isLoggedIn = true;
                        lblStatus.Text = $"Đăng nhập thành công: {currentUsername}";
                        lblUserInfo.Text = $"Xin chào, {currentUsername}! | Thắng: {wins} | Thua: {losses} | Tổng: {totalGames}";
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else if (response != null && response.StartsWith("LOGIN_FAILED:"))
                {
                    string error = response.Substring("LOGIN_FAILED:".Length);
                    lblStatus.Text = "Đăng nhập thất bại";
                    MessageBox.Show(error, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblStatus.Text = "Không nhận được phản hồi từ server";
                    MessageBox.Show("Không nhận được phản hồi từ server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi gửi dữ liệu";
                MessageBox.Show($"Không thể gửi yêu cầu đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ✅ Đợi response từ server (blocking)
        /// </summary>
        private string? WaitForResponse(string prefix, int timeoutMs)
        {
            DateTime startTime = DateTime.Now;
            
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    if (!socket.IsConnected)
                    {
                        return null;
                    }
                    
                    string data = socket.Receive();
                    if (!string.IsNullOrEmpty(data))
                    {
                        if (data.StartsWith(prefix))
                        {
                            System.Diagnostics.Debug.WriteLine($"📥 WaitForResponse nhận đúng: {data.Substring(0, Math.Min(50, data.Length))}...");
                            return data;
                        }
                        else
                        {
                            // ✅ Lưu message không match để sanhCho xử lý sau
                            System.Diagnostics.Debug.WriteLine($"📦 WaitForResponse lưu message khác: {data.Substring(0, Math.Min(50, data.Length))}...");
                            lock (pendingMessages)
                            {
                                pendingMessages.Enqueue(data);
                            }
                        }
                    }
                    
                    Thread.Sleep(10);
                }
                catch
                {
                    return null;
                }
            }
            
            return null;
        }

        /// <summary>
        /// ✅ Lấy các message đang pending để sanhCho xử lý
        /// </summary>
        public Queue<string> GetPendingMessages()
        {
            lock (pendingMessages)
            {
                var messages = new Queue<string>(pendingMessages);
                pendingMessages.Clear();
                return messages;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtRegisterUsername.Text.Trim();
            string password = txtRegisterPassword.Text.Trim();
            string confirmPassword = txtRegisterConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!socket.IsConnected)
            {
                MessageBox.Show("Bạn chưa kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Bỏ email: gửi luôn REGISTER:{username}:{password}
            string registerMessage = $"REGISTER:{username}:{password}";

            try
            {
                System.Diagnostics.Debug.WriteLine($"📤 LoginForm gửi: REGISTER:{username}:***");
                socket.Send(registerMessage);
                lblStatus.Text = "Đang đăng ký...";
                
                // ✅ Đợi response từ server
                string? response = WaitForResponse("REGISTER_", 5000);
                
                if (response != null && response.StartsWith("REGISTER_SUCCESS:"))
                {
                    lblStatus.Text = "Đăng ký thành công! Vui lòng đăng nhập.";
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControl1.SelectedTab = tabPageLogin;
                    txtLoginUsername.Text = username;
                }
                else if (response != null && response.StartsWith("REGISTER_FAILED:"))
                {
                    string error = response.Substring("REGISTER_FAILED:".Length);
                    lblStatus.Text = "Đăng ký thất bại";
                    MessageBox.Show(error, "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblStatus.Text = "Không nhận được phản hồi từ server";
                    MessageBox.Show("Không nhận được phản hồi từ server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi gửi dữ liệu";
                MessageBox.Show($"Không thể gửi yêu cầu đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartListening()
        {
            System.Diagnostics.Debug.WriteLine("🎧 StartListening() được gọi");
            
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            listenThread = new Thread(() =>
            {
                System.Diagnostics.Debug.WriteLine("🎧 Listen thread đã bắt đầu");
                
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!socket.IsConnected)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ Socket disconnected trong listen thread");
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Mất kết nối đến server";
                                btnConnect.Text = "Kết nối";
                                btnConnect.Enabled = true;
                                txtServerIP.Enabled = true;
                            }));
                            break;
                        }

                        string data = socket.Receive();
                        if (string.IsNullOrEmpty(data))
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"📥 LoginForm nhận: {data}");

                        if (data.StartsWith("LOGIN_SUCCESS:"))
                        {
                            try
                            {
                                // Chỉ lấy phần LOGIN_SUCCESS, bỏ qua phần thừa phía sau
                                var match = Regex.Match(data, @"^LOGIN_SUCCESS:(\d+):([^:]+):(\d+):(\d+):(\d+)");

                                if (match.Success)
                                {
                                    userId = int.Parse(match.Groups[1].Value);
                                    currentUsername = match.Groups[2].Value;
                                    totalGames = int.Parse(match.Groups[3].Value);
                                    wins = int.Parse(match.Groups[4].Value);
                                    losses = int.Parse(match.Groups[5].Value);

                                    Invoke(new Action(() =>
                                    {
                                        isLoggedIn = true;
                                        lblStatus.Text = $"Đăng nhập thành công: {currentUsername}";
                                        lblUserInfo.Text = $"Xin chào, {currentUsername}! | Thắng: {wins} | Thua: {losses} | Tổng: {totalGames}";
                                        DialogResult = DialogResult.OK;
                                        Close();
                                    }));
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ LOGIN_SUCCESS không khớp pattern: {data}");
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Lỗi parse LOGIN_SUCCESS: {ex.Message}");
                            }
                        }
                        else if (data.StartsWith("LOGIN_FAILED:"))
                        {
                            string error = data.Substring("LOGIN_FAILED:".Length);
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Đăng nhập thất bại";
                                MessageBox.Show(error, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                        else if (data.StartsWith("REGISTER_SUCCESS:"))
                        {
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Đăng ký thành công! Vui lòng đăng nhập.";
                                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tabControl1.SelectedTab = tabPageLogin;
                                txtLoginUsername.Text = txtRegisterUsername.Text;
                            }));
                        }
                        else if (data.StartsWith("REGISTER_FAILED:"))
                        {
                            string error = data.Substring("REGISTER_FAILED:".Length);
                            Invoke(new Action(() =>
                            {
                                lblStatus.Text = "Đăng ký thất bại";
                                MessageBox.Show(error, "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                        else if (data.StartsWith("AUTH_REQUIRED:"))
                        {
                            // Không làm gì trong màn hình đăng nhập
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Form đã bị đóng hoặc Invoke không thể thực thi
                        if (!token.IsCancellationRequested)
                        {
                            try
                            {
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = "Mất kết nối";
                                }));
                            }
                            catch { }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            try
                            {
                                MessageBox.Show($"Lỗi khi nhận dữ liệu từ server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = $"Lỗi: {ex.Message}";
                                }));
                            }
                            catch { }
                        }
                        break;
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public string GetUsername() => currentUsername;
        public string GetPassword() => currentPassword; // ✅ Trả về password để tự động đăng nhập lại
        public int GetUserId() => userId;
        public bool IsLoggedIn() => isLoggedIn;
        public SocketManager GetSocket() => socket;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // ✅ Dừng server discovery nếu đang chạy
                serverDiscovery?.StopDiscovery();
            }
            catch
            {
                // ignore
            }

            if (!isLoggedIn && socket.IsConnected)
            {
                try
                {
                    socket.Send("DISCONNECT");
                }
                catch
                {
                }

                socket.Disconnect();
            }

            base.OnFormClosing(e);
        }

        private void txtServerIP_TextChanged(object sender, EventArgs e)
        {

        }
    }
}