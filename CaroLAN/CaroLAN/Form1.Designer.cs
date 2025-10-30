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
            this.pnlChessBoard = new System.Windows.Forms.Panel();
            this.btnCreateServer = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            
            // pnlChessBoard
            this.pnlChessBoard.Location = new System.Drawing.Point(12, 60);
            this.pnlChessBoard.Size = new System.Drawing.Size(500, 500);
            this.pnlChessBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
           
            // btnCreateServer
            this.btnCreateServer.Location = new System.Drawing.Point(530, 60);
            this.btnCreateServer.Size = new System.Drawing.Size(120, 40);
            this.btnCreateServer.Text = "Tạo Server";
            this.btnCreateServer.Click += new System.EventHandler(this.btnCreateServer_Click);
            // btnConnect
            this.btnConnect.Location = new System.Drawing.Point(530, 150);
            this.btnConnect.Size = new System.Drawing.Size(120, 40);
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
         
            // txtIP
            this.txtIP.Location = new System.Drawing.Point(530, 120);
            this.txtIP.Size = new System.Drawing.Size(120, 23);
            this.txtIP.Text = "127.0.0.1";
            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(530, 200);
            this.lblStatus.AutoSize = true;
            this.lblStatus.Text = "Trạng thái: Chưa kết nối";
            // btnRestart
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnRestart.Location = new System.Drawing.Point(530, 250);
            this.btnRestart.Size = new System.Drawing.Size(120, 40);
            this.btnRestart.Text = "Chơi lại";
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            this.Controls.Add(this.btnRestart);

            // Form1
            this.ClientSize = new System.Drawing.Size(670, 580);
            this.Controls.Add(this.pnlChessBoard);
            this.Controls.Add(this.btnCreateServer);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblStatus);
            this.Text = "Caro LAN";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
