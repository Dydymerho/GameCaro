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

        private bool isLoggedIn;
        private string currentUsername = string.Empty;
        private string currentPassword = string.Empty; // ✅ Lưu password để tự động đăng nhập lại
        private int userId;
        private int totalGames;
        private int wins;
        private int losses;

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Nếu đã nhập sẵn IP thì tự động connect
            if (!string.IsNullOrWhiteSpace(txtServerIP.Text))
            {
                btnConnect_Click(null, null);  // Tự động kết nối khi form chạy
            }
        }


        public LoginForm()
        {
            InitializeComponent();
            socket = new SocketManager();
            cancellationTokenSource = new CancellationTokenSource();
            lblStatus.Text = "Chưa kết nối";

            this.Load += LoginForm_Load; // auto connect to localhost
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
                return;
            }

            try
            {
                lblStatus.Text = "Đang kết nối...";
                btnConnect.Enabled = false;
                Application.DoEvents();

                if (socket.ConnectToServer(serverIP))
                {
                    lblStatus.Text = "Đã kết nối đến server";
                    btnConnect.Text = "Đã kết nối";
                    btnConnect.Enabled = false;
                    txtServerIP.Enabled = false;
                    StartListening();
                }
                else
                {
                    lblStatus.Text = "Không kết nối được server";
                    btnConnect.Enabled = true;
                    MessageBox.Show("Không thể kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi kết nối";
                btnConnect.Enabled = true;
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
                socket.Send($"LOGIN:{username}:{password}");
                lblStatus.Text = "Đang đăng nhập...";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi gửi dữ liệu";
                MessageBox.Show($"Không thể gửi yêu cầu đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                socket.Send(registerMessage);
                lblStatus.Text = "Đang đăng ký...";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi gửi dữ liệu";
                MessageBox.Show($"Không thể gửi yêu cầu đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartListening()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            listenThread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!socket.IsConnected)
                        {
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
                cancellationTokenSource?.Cancel();
                if (listenThread != null && listenThread.IsAlive)
                {
                    listenThread.Join(1000);
                }
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

            cancellationTokenSource?.Dispose();
            base.OnFormClosing(e);
        }

        private void txtServerIP_TextChanged(object sender, EventArgs e)
        {

        }
    }
}