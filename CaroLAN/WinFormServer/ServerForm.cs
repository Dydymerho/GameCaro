using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormServer
{
    public partial class ServerForm : Form
    {
        ServerSocketManager socket = new ServerSocketManager();
        public ServerForm()
        {
            InitializeComponent();
        }


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
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                socket.stopServer(LogToTextBox);

                lblStatus.Text = "Server đã dừng.";
                btnStart.Enabled = true;
                btnStop.Enabled = false;
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
        }

        private void UpdateClientList()
        {
            if (lstClients.InvokeRequired)
            {
                lstClients.Invoke(new Action(UpdateClientList));
                return;
            }

            LogToTextBox("Cập nhật danh sách client...");
            List<string> connectedClients = socket.GetConnectedClients();
            LogToTextBox($"Number of connected clients: {connectedClients.Count}");

            // Cập nhật lstClients trên giao diện
            lstClients.BeginUpdate();
            lstClients.Items.Clear();
            foreach (string client in connectedClients)
            {
                lstClients.Items.Add(client);
            }
            lstClients.EndUpdate();

            LogToTextBox("Client list updated.");
        }

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
