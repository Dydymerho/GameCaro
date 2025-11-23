using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormServer
{
    public partial class ServerForm : Form
    {
        ServerSocketManager socket;
        UserManager userManager;

        // database config localhost
        private const string DB_SERVER = "localhost";
        private const string DB_DATABASE = "gamecaro";
        private const string DB_USER = "root";
        private const string DB_PASSWORD = "";



        public ServerForm()
        {
            InitializeComponent();
            
            // Kiểm tra và tạo database nếu chưa có
            LogToTextBox("Đang kiểm tra database...");
            bool dbInitialized = false;

            try
            {
                dbInitialized = UserManager.InitializeDatabase(DB_SERVER, DB_DATABASE, DB_USER, DB_PASSWORD, LogToTextBox);
            }
            catch (Exception ex)
            {
                LogToTextBox($"Lỗi khi khởi tạo database: {ex.Message}");
            }


            if (dbInitialized)
            {
                // Khởi tạo UserManager
                userManager = new UserManager(DB_SERVER, DB_DATABASE, DB_USER, DB_PASSWORD);
                socket = new ServerSocketManager(userManager);
                LogToTextBox("Server đã sẵn sàng. Nhấn 'Bat server' để bắt đầu server.");
            }
            else
            {
                LogToTextBox("Lỗi: Không thể khởi tạo database. Vui lòng kiểm tra kết nối MySQL.");
            }
        }
        //comment

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void lstClients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                socket.CreateServer(LogToTextBox, UpdateClientList);
                lblStatus.Text = "Đang chờ kết nối...";
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }
            catch (Exception ex)
            {
                LogToTextBox($"Lỗi khi bật server: {ex.Message}");
                MessageBox.Show("Không thể bật server. Vui lòng kiểm tra PORT hoặc Firewall.");
            }
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                socket.stopServer(LogToTextBox);

                lblStatus.Text = "Server đã dừng.";
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            catch (Exception ex)
            {
                LogToTextBox($"Lỗi khi dừng server: {ex.Message}");
                MessageBox.Show("Có lỗi xảy ra khi dừng server!");
            }
        }


        private void LogToTextBox(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action(() =>
                {
                    txtLog.AppendText(message + Environment.NewLine);
                }));
            }
            else
            {
                txtLog.AppendText(message + Environment.NewLine);
            }
        }

        //nut refresh danh sach client
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateClientList();
                LogToTextBox("Cập nhật danh sách client...");
                List<string> connectedClients = socket.GetConnectedClients();
                LogToTextBox($"Số client đang kết nối: {connectedClients.Count}");
            }
            catch (Exception ex)
            {
                LogToTextBox($"Lỗi khi cập nhật danh sách client: {ex.Message}");
            }
        }


        private void UpdateClientList()
        {
            if (lstClients.InvokeRequired)
            {
                lstClients.Invoke(new Action(UpdateClientList));
                return;
            }
                
            //LogToTextBox("Cập nhật danh sách client...");
            List<string> connectedClients = socket.GetConnectedClients();
            //LogToTextBox($"Number of connected clients: {connectedClients.Count}");

            // Cập nhật lstClients trên giao diện
            lstClients.BeginUpdate();
            lstClients.Items.Clear();
            foreach (string client in connectedClients)
            {
                lstClients.Items.Add(client);
            }
            lstClients.EndUpdate();
        }

        // btn ngắt kết nối client
        private void button2_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItem != null)
            {
                try
                {
                    string selectedClient = lstClients.SelectedItem.ToString();
                    socket.DisconnectClient(selectedClient, LogToTextBox);
                    UpdateClientList();
                }
                catch (Exception ex)
                {
                    LogToTextBox($"Lỗi khi ngắt client: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một client để ngắt kết nối.");
            }
        }

    }
}
