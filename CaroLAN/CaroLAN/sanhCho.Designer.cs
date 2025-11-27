namespace CaroLAN
{
    partial class sanhCho
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            lstClients = new ListBox();
            btnConnect = new Button();
            btnFindServers = new Button();
            txtIP = new TextBox();
            lblStatus = new Label();
            button3 = new Button();
            label2 = new Label();
            btnRequest = new Button();
            lstRequests = new ListBox();
            btnAccept = new Button();
            label3 = new Label();
            tabHistory = new TabControl();
            tabMyHistory = new TabPage();
            lstMyHistory = new ListBox();
            btnRefreshMy = new Button();
            panelHeader = new Panel();
            lblTitle = new Label();
            panelConnection = new Panel();
            lblConnectionTitle = new Label();
            panelStatusBar = new Panel();
            panelMain = new Panel();
            panelHistory = new Panel();
            lblHistoryTitle = new Label();
            panelPlayersAndInvites = new Panel();
            panelInvitations = new Panel();
            panelPlayers = new Panel();
            panelQuickMatch = new Panel();
            lblQuickMatchTitle = new Label();
            tabHistory.SuspendLayout();
            tabMyHistory.SuspendLayout();
            panelHeader.SuspendLayout();
            panelConnection.SuspendLayout();
            panelStatusBar.SuspendLayout();
            panelMain.SuspendLayout();
            panelHistory.SuspendLayout();
            panelPlayersAndInvites.SuspendLayout();
            panelInvitations.SuspendLayout();
            panelPlayers.SuspendLayout();
            panelQuickMatch.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(52, 73, 94);
            label1.Location = new Point(20, 15);
            label1.Name = "label1";
            label1.Size = new Size(258, 28);
            label1.TabIndex = 0;
            label1.Text = "👥 Người chơi trực tuyến";
            // 
            // lstClients
            // 
            lstClients.BackColor = Color.FromArgb(250, 251, 252);
            lstClients.BorderStyle = BorderStyle.FixedSingle;
            lstClients.Font = new Font("Segoe UI", 10F);
            lstClients.ForeColor = Color.FromArgb(44, 62, 80);
            lstClients.FormattingEnabled = true;
            lstClients.ItemHeight = 23;
            lstClients.Location = new Point(20, 50);
            lstClients.Name = "lstClients";
            lstClients.ScrollAlwaysVisible = true;
            lstClients.Size = new Size(520, 140);
            lstClients.TabIndex = 1;
            // 
            // txtIP
            // 
            txtIP.BackColor = Color.White;
            txtIP.BorderStyle = BorderStyle.FixedSingle;
            txtIP.Font = new Font("Segoe UI", 9F);
            txtIP.Location = new Point(10, 27);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(95, 27);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            txtIP.TextAlign = HorizontalAlignment.Center;
            // 
            // btnFindServers
            // 
            btnFindServers.BackColor = Color.FromArgb(230, 126, 34);
            btnFindServers.FlatAppearance.BorderSize = 0;
            btnFindServers.FlatStyle = FlatStyle.Flat;
            btnFindServers.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnFindServers.ForeColor = Color.White;
            btnFindServers.Location = new Point(110, 27);
            btnFindServers.Name = "btnFindServers";
            btnFindServers.Size = new Size(20, 27);
            btnFindServers.TabIndex = 4;
            btnFindServers.Text = "🔍";
            btnFindServers.UseVisualStyleBackColor = false;
            btnFindServers.Click += btnFindServers_Click;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(46, 204, 113);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(135, 27);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(115, 27);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click_1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Font = new Font("Segoe UI", 10F);
            lblStatus.ForeColor = Color.FromArgb(52, 73, 94);
            lblStatus.Location = new Point(20, 8);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(5, 0, 0, 0);
            lblStatus.Size = new Size(226, 23);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "⚪ Trạng thái: Chưa kết nối";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(52, 152, 219);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            button3.ForeColor = Color.White;
            button3.Location = new Point(25, 65);
            button3.Name = "button3";
            button3.Size = new Size(200, 55);
            button3.TabIndex = 1;
            button3.Text = "🎯 BẮT ĐẦU";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F);
            label2.ForeColor = Color.FromArgb(149, 165, 166);
            label2.Location = new Point(240, 80);
            label2.Name = "label2";
            label2.Size = new Size(272, 20);
            label2.TabIndex = 2;
            label2.Text = "Tìm đối thủ ngẫu nhiên và bắt đầu chơi!";
            // 
            // btnRequest
            // 
            btnRequest.BackColor = Color.FromArgb(52, 152, 219);
            btnRequest.FlatAppearance.BorderSize = 0;
            btnRequest.FlatStyle = FlatStyle.Flat;
            btnRequest.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRequest.ForeColor = Color.White;
            btnRequest.Location = new Point(20, 195);
            btnRequest.Name = "btnRequest";
            btnRequest.Size = new Size(520, 40);
            btnRequest.TabIndex = 2;
            btnRequest.Text = "📧 MỜI CHƠI";
            btnRequest.UseVisualStyleBackColor = false;
            btnRequest.Click += btnRequest_Click;
            // 
            // lstRequests
            // 
            lstRequests.BackColor = Color.FromArgb(250, 251, 252);
            lstRequests.BorderStyle = BorderStyle.FixedSingle;
            lstRequests.Font = new Font("Segoe UI", 10F);
            lstRequests.ForeColor = Color.FromArgb(44, 62, 80);
            lstRequests.FormattingEnabled = true;
            lstRequests.ItemHeight = 23;
            lstRequests.Location = new Point(20, 50);
            lstRequests.Name = "lstRequests";
            lstRequests.ScrollAlwaysVisible = true;
            lstRequests.Size = new Size(520, 140);
            lstRequests.TabIndex = 1;
            lstRequests.SelectedIndexChanged += lstRequests_SelectedIndexChanged;
            // 
            // btnAccept
            // 
            btnAccept.BackColor = Color.FromArgb(46, 204, 113);
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.FlatStyle = FlatStyle.Flat;
            btnAccept.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAccept.ForeColor = Color.White;
            btnAccept.Location = new Point(20, 195);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(520, 40);
            btnAccept.TabIndex = 2;
            btnAccept.Text = "✅ CHẤP NHẬN";
            btnAccept.UseVisualStyleBackColor = false;
            btnAccept.Click += btnAccept_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(52, 73, 94);
            label3.Location = new Point(20, 15);
            label3.Name = "label3";
            label3.Size = new Size(160, 28);
            label3.TabIndex = 0;
            label3.Text = "📨 Lời mời đấu";
            // 
            // tabHistory
            // 
            tabHistory.Controls.Add(tabMyHistory);
            tabHistory.Font = new Font("Segoe UI", 9F);
            tabHistory.Location = new Point(20, 50);
            tabHistory.Name = "tabHistory";
            tabHistory.SelectedIndex = 0;
            tabHistory.Size = new Size(1100, 70);
            tabHistory.TabIndex = 1;
            // 
            // tabMyHistory
            // 
            tabMyHistory.BackColor = Color.FromArgb(250, 251, 252);
            tabMyHistory.Controls.Add(lstMyHistory);
            tabMyHistory.Controls.Add(btnRefreshMy);
            tabMyHistory.Location = new Point(4, 29);
            tabMyHistory.Name = "tabMyHistory";
            tabMyHistory.Padding = new Padding(3);
            tabMyHistory.Size = new Size(1092, 37);
            tabMyHistory.TabIndex = 1;
            tabMyHistory.Text = "⭐ Của tôi";
            // 
            // lstMyHistory
            // 
            lstMyHistory.BackColor = Color.White;
            lstMyHistory.BorderStyle = BorderStyle.FixedSingle;
            lstMyHistory.Font = new Font("Segoe UI", 9F);
            lstMyHistory.ForeColor = Color.FromArgb(44, 62, 80);
            lstMyHistory.FormattingEnabled = true;
            lstMyHistory.Location = new Point(6, 6);
            lstMyHistory.Name = "lstMyHistory";
            lstMyHistory.Size = new Size(1000, 22);
            lstMyHistory.TabIndex = 0;
            // 
            // btnRefreshMy
            // 
            btnRefreshMy.BackColor = Color.FromArgb(46, 204, 113);
            btnRefreshMy.FlatAppearance.BorderSize = 0;
            btnRefreshMy.FlatStyle = FlatStyle.Flat;
            btnRefreshMy.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnRefreshMy.ForeColor = Color.White;
            btnRefreshMy.Location = new Point(1012, 6);
            btnRefreshMy.Name = "btnRefreshMy";
            btnRefreshMy.Size = new Size(74, 25);
            btnRefreshMy.TabIndex = 1;
            btnRefreshMy.Text = "🔄 Mới";
            btnRefreshMy.UseVisualStyleBackColor = false;
            btnRefreshMy.Click += btnRefreshMy_Click;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(panelConnection);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(20, 15, 20, 15);
            panelHeader.Size = new Size(1200, 100);
            panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 12, 0, 0);
            lblTitle.Size = new Size(357, 72);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🎮 GAME CARO";
            // 
            // panelConnection
            // 
            panelConnection.BackColor = Color.FromArgb(52, 152, 219);
            panelConnection.Controls.Add(lblConnectionTitle);
            panelConnection.Controls.Add(txtIP);
            panelConnection.Controls.Add(btnFindServers);
            panelConnection.Controls.Add(btnConnect);
            panelConnection.Dock = DockStyle.Right;
            panelConnection.Location = new Point(920, 15);
            panelConnection.Name = "panelConnection";
            panelConnection.Padding = new Padding(10, 5, 10, 5);
            panelConnection.Size = new Size(260, 70);
            panelConnection.TabIndex = 1;
            // 
            // lblConnectionTitle
            // 
            lblConnectionTitle.AutoSize = true;
            lblConnectionTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblConnectionTitle.ForeColor = Color.White;
            lblConnectionTitle.Location = new Point(10, 5);
            lblConnectionTitle.Name = "lblConnectionTitle";
            lblConnectionTitle.Size = new Size(87, 19);
            lblConnectionTitle.TabIndex = 0;
            lblConnectionTitle.Text = "🔌 KẾT NỐI";
            // 
            // panelStatusBar
            // 
            panelStatusBar.BackColor = Color.FromArgb(236, 240, 241);
            panelStatusBar.Controls.Add(lblStatus);
            panelStatusBar.Dock = DockStyle.Top;
            panelStatusBar.Location = new Point(0, 100);
            panelStatusBar.Name = "panelStatusBar";
            panelStatusBar.Padding = new Padding(20, 8, 20, 8);
            panelStatusBar.Size = new Size(1200, 40);
            panelStatusBar.TabIndex = 1;
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(245, 247, 250);
            panelMain.Controls.Add(panelHistory);
            panelMain.Controls.Add(panelPlayersAndInvites);
            panelMain.Controls.Add(panelQuickMatch);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 140);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(30, 20, 30, 20);
            panelMain.Size = new Size(1200, 580);
            panelMain.TabIndex = 2;
            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.White;
            panelHistory.Controls.Add(lblHistoryTitle);
            panelHistory.Controls.Add(tabHistory);
            panelHistory.Dock = DockStyle.Top;
            panelHistory.Location = new Point(30, 430);
            panelHistory.Name = "panelHistory";
            panelHistory.Padding = new Padding(20, 15, 20, 15);
            panelHistory.Size = new Size(1140, 130);
            panelHistory.TabIndex = 2;
            // 
            // lblHistoryTitle
            // 
            lblHistoryTitle.AutoSize = true;
            lblHistoryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblHistoryTitle.Location = new Point(20, 15);
            lblHistoryTitle.Name = "lblHistoryTitle";
            lblHistoryTitle.Size = new Size(154, 28);
            lblHistoryTitle.TabIndex = 0;
            lblHistoryTitle.Text = "📊 Lịch sử đấu";
            // 
            // panelPlayersAndInvites
            // 
            panelPlayersAndInvites.BackColor = Color.Transparent;
            panelPlayersAndInvites.Controls.Add(panelInvitations);
            panelPlayersAndInvites.Controls.Add(panelPlayers);
            panelPlayersAndInvites.Dock = DockStyle.Top;
            panelPlayersAndInvites.Location = new Point(30, 170);
            panelPlayersAndInvites.Name = "panelPlayersAndInvites";
            panelPlayersAndInvites.Padding = new Padding(0, 15, 0, 0);
            panelPlayersAndInvites.Size = new Size(1140, 260);
            panelPlayersAndInvites.TabIndex = 1;
            // 
            // panelInvitations
            // 
            panelInvitations.BackColor = Color.White;
            panelInvitations.Controls.Add(label3);
            panelInvitations.Controls.Add(lstRequests);
            panelInvitations.Controls.Add(btnAccept);
            panelInvitations.Dock = DockStyle.Right;
            panelInvitations.Location = new Point(580, 15);
            panelInvitations.Name = "panelInvitations";
            panelInvitations.Padding = new Padding(20, 15, 20, 15);
            panelInvitations.Size = new Size(560, 245);
            panelInvitations.TabIndex = 1;
            // 
            // panelPlayers
            // 
            panelPlayers.BackColor = Color.White;
            panelPlayers.Controls.Add(label1);
            panelPlayers.Controls.Add(lstClients);
            panelPlayers.Controls.Add(btnRequest);
            panelPlayers.Dock = DockStyle.Left;
            panelPlayers.Location = new Point(0, 15);
            panelPlayers.Name = "panelPlayers";
            panelPlayers.Padding = new Padding(20, 15, 20, 15);
            panelPlayers.Size = new Size(560, 245);
            panelPlayers.TabIndex = 0;
            // 
            // panelQuickMatch
            // 
            panelQuickMatch.BackColor = Color.White;
            panelQuickMatch.Controls.Add(lblQuickMatchTitle);
            panelQuickMatch.Controls.Add(button3);
            panelQuickMatch.Controls.Add(label2);
            panelQuickMatch.Dock = DockStyle.Top;
            panelQuickMatch.Location = new Point(30, 20);
            panelQuickMatch.Name = "panelQuickMatch";
            panelQuickMatch.Padding = new Padding(25, 20, 25, 20);
            panelQuickMatch.Size = new Size(1140, 150);
            panelQuickMatch.TabIndex = 0;
            // 
            // lblQuickMatchTitle
            // 
            lblQuickMatchTitle.AutoSize = true;
            lblQuickMatchTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblQuickMatchTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblQuickMatchTitle.Location = new Point(25, 20);
            lblQuickMatchTitle.Name = "lblQuickMatchTitle";
            lblQuickMatchTitle.Size = new Size(240, 37);
            lblQuickMatchTitle.TabIndex = 0;
            lblQuickMatchTitle.Text = "⚡ CHƠI NHANH";
            // 
            // sanhCho
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1200, 720);
            Controls.Add(panelMain);
            Controls.Add(panelStatusBar);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "sanhCho";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Caro - Sảnh Chờ";
            Load += sanhCho_Load;
            tabHistory.ResumeLayout(false);
            tabMyHistory.ResumeLayout(false);
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelConnection.ResumeLayout(false);
            panelConnection.PerformLayout();
            panelStatusBar.ResumeLayout(false);
            panelStatusBar.PerformLayout();
            panelMain.ResumeLayout(false);
            panelHistory.ResumeLayout(false);
            panelHistory.PerformLayout();
            panelPlayersAndInvites.ResumeLayout(false);
            panelInvitations.ResumeLayout(false);
            panelInvitations.PerformLayout();
            panelPlayers.ResumeLayout(false);
            panelPlayers.PerformLayout();
            panelQuickMatch.ResumeLayout(false);
            panelQuickMatch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelConnection;
        private Label lblConnectionTitle;
        private Panel panelStatusBar;
        private Panel panelMain;
        private Panel panelQuickMatch;
        private Label lblQuickMatchTitle;
        private Panel panelPlayersAndInvites;
        private Panel panelPlayers;
        private Panel panelInvitations;
        private Panel panelHistory;
        private Label lblHistoryTitle;
        private Label label1;
        private ListBox lstClients;
        private Button btnConnect;
        private Button btnFindServers;
        private TextBox txtIP;
        private Label lblStatus;
        private Button button3;
        private Label label2;
        private Button btnRequest;
        private ListBox lstRequests;
        private Button btnAccept;
        private Label label3;
        private TabControl tabHistory;
        private TabPage tabMyHistory;
        private ListBox lstMyHistory;
        private Button btnRefreshMy;
    }
}