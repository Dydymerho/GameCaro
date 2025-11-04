using CaroServerApp;
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
            socket.CreateServer(LogToTextBox);
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
                txtLog.Invoke(new Action(() =>
                {
                    txtLog.AppendText(message + Environment.NewLine);
                }));
            }
            else
            {
                txtLog.AppendText(message + Environment.NewLine);
            }
        }
    }
}
