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
                //StartListening();
            }
            else
            {
                lblStatus.Text = "Không kết nối được server!";
            }
        }

        private void StartListening()
        {
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    string data = socket.Receive();
                    if (string.IsNullOrEmpty(data))
                        continue;

                    // ✅ Nếu nhận tín hiệu chơi lại
                    if (data == "RESTART")
                    {
                        // Đặt lại bàn cờ
                        Invoke(new Action(() =>
                        {
                            chessBoard.ResetBoard();
                            lblStatus.Text = "Đối thủ đã khởi động lại ván!";
                        }));
                        continue;
                    }

                    // ✅ Nhận nước đi từ đối thủ (dạng "x,y")
                    string[] parts = data.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        Invoke(new Action(() =>
                        {
                            chessBoard.OtherPlayerMove(new Point(x, y));
                        }));
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }
    }
}
