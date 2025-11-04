namespace CaroLAN
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlChessBoard;
        private System.Windows.Forms.Button btnCreateServer;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRestart;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlChessBoard = new Panel();
            btnCreateServer = new Button();
            btnConnect = new Button();
            txtIP = new TextBox();
            lblStatus = new Label();
            btnRestart = new Button();
            SuspendLayout();
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BorderStyle = BorderStyle.FixedSingle;
            pnlChessBoard.Location = new Point(12, 60);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(500, 500);
            pnlChessBoard.TabIndex = 1;
            // 
            // btnCreateServer
            // 
            btnCreateServer.Location = new Point(530, 60);
            btnCreateServer.Name = "btnCreateServer";
            btnCreateServer.Size = new Size(120, 40);
            btnCreateServer.TabIndex = 2;
            btnCreateServer.Text = "Tạo Server";
            btnCreateServer.Click += btnCreateServer_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(530, 150);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 40);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Kết nối";
            btnConnect.Click += btnConnect_Click;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(530, 120);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(120, 27);
            txtIP.TabIndex = 4;
            txtIP.Text = "127.0.0.1";
            txtIP.TextChanged += txtIP_TextChanged;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(530, 200);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(165, 20);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Trạng thái: Chưa kết nối";
            lblStatus.Click += lblStatus_Click;
            // 
            // btnRestart
            // 
            btnRestart.Location = new Point(530, 250);
            btnRestart.Name = "btnRestart";
            btnRestart.Size = new Size(120, 40);
            btnRestart.TabIndex = 0;
            btnRestart.Text = "Chơi lại";
            btnRestart.Click += btnRestart_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(670, 580);
            Controls.Add(btnRestart);
            Controls.Add(pnlChessBoard);
            Controls.Add(btnCreateServer);
            Controls.Add(btnConnect);
            Controls.Add(txtIP);
            Controls.Add(lblStatus);
            Name = "Form1";
            Text = "Caro LAN";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
