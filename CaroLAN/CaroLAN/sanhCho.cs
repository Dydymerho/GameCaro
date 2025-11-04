using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroLAN
{
    public partial class sanhCho : Form
    {
        ChessBoardManager chessBoard;
        SocketManager socket;
        Thread listenThread;
        public sanhCho()
        {
            InitializeComponent();
            socket = new SocketManager();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            string serverIP = txtIP.Text.Trim();
            if (socket.ConnectToServer(serverIP))
            {
                lblStatus.Text = "Đã kết nối đến server";
                lobbyListening();
            }
            else
            {
                lblStatus.Text = "Không kết nối được server!";
            }
        }

        private void lobbyListening()
        {
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        // Nhận dữ liệu từ server
                        string data = socket.Receive();
                        if (string.IsNullOrEmpty(data))
                            continue;

                        // ✅ Nếu nhận danh sách client
                        if (data.StartsWith("CLIENT_LIST:"))
                        {
                            string[] clients = data.Substring("CLIENT_LIST:".Length).Split(',');
                            Invoke(new Action(() =>
                            {
                                lstClients.Items.Clear();
                                lstClients.Items.AddRange(clients);
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Lỗi khi nhận dữ liệu từ server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                        break;
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

    }
}
