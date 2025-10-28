using System.Net.Sockets;
using System.Windows.Forms;
using CaroGame.Share; // Quan trọng: Thêm reference đến NetworkHelper

namespace CaroGame.Client
{
    public partial class Form1 : Form
    {
        // Giữ nguyên khai báo
        private TcpClient client;

        public Form1()
        {
            InitializeComponent();
            lblStatus.Text = "Chua ket noi";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Dang ket noi...";
                Application.DoEvents(); // Cập nhật giao diện ngay lập tức

                client = new TcpClient();

                // Dùng hằng số từ Share
                client.Connect(CaroConstants.DEFAULT_IP, CaroConstants.DEFAULT_PORT);

                if (client.Connected)
                {
                    MessageBox.Show("Ket noi thanh cong!");
                    lblStatus.Text = "Da ket noi Server!";
                    btnConnect.Enabled = false; // Vô hiệu hóa nút sau khi kết nối

                    // --- Bổ sung Logic Gửi/Nhận Dữ liệu ---

                    // 1. Gửi tin nhắn đầu tiên lên Server
                    string clientMessage = "Toi la Client [PLAYER A], xin chao!";
                    NetworkHelper.SendMessage(client, clientMessage);
                    txtStatus.Text += $"Gui len Server: {clientMessage}\r\n";

                    // 2. Nhận phản hồi từ Server
                    string serverResponse = NetworkHelper.ReceiveMessage(client);

                    if (serverResponse != null)
                    {
                        txtStatus.Text += $"Nhan tu Server: {serverResponse}\r\n";
                    }
                    else
                    {
                        txtStatus.Text += "Server da ngat ket noi.\r\n";
                    }

                    // --- Kết thúc Logic Gửi/Nhận ---

                    // Đóng kết nối tạm thời sau khi giao tiếp xong
                    client.Close();
                }
            }
            catch (SocketException ex)
            {
                lblStatus.Text = "Ket noi that bai!";
                MessageBox.Show($"Khong the ket noi Server: {ex.Message}", "Loi Ket Noi");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi chung: {ex.Message}");
            }
        }
    }
}