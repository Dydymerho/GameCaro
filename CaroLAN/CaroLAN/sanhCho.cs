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
                        if (!socket.IsConnected)
                        {
                            lblStatus.Text = "Kết nối đến server đã bị mất!";
                            lstClients.Items.Clear();
                            break;
                        }
                        else
                        {
                            string data = socket.Receive();
                            if (string.IsNullOrEmpty(data))
                                continue;

                            if (data == "SERVER_STOPPED")
                            {
                                Invoke(new Action(() =>
                                {
                                    lblStatus.Text = "Server đã dừng!";
                                    lstClients.Items.Clear();
                                }));
                                break; // Ngừng lắng nghe
                            }

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

        // button bat dau choi
        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
