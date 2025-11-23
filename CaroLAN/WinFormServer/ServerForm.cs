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
            bool dbInitialized = UserManager.InitializeDatabase(DB_SERVER, DB_DATABASE, DB_USER, DB_PASSWORD, LogToTextBox);
            
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
            socket.CreateServer(LogToTextBox, UpdateClientList);
            lblStatus.Text = "Đang chờ kết nối...";

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
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
            catch
            {
                txtLog.AppendText("Lỗi khi dừng server.\n");
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
            UpdateClientList();
            LogToTextBox("Cập nhật danh sách client...");
            List<string> connectedClients = socket.GetConnectedClients();
            LogToTextBox($"Number of connected clients: {connectedClients.Count}");
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
                string selectedClient = lstClients.SelectedItem.ToString();
                socket.DisconnectClient(selectedClient, LogToTextBox);
                UpdateClientList();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một client để ngắt kết nối.");
            }
        }
    }
}
