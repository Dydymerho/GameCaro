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
            dgvMyHistory = new DataGridView();
            colRoomId = new DataGridViewTextBoxColumn();
            colOpponent = new DataGridViewTextBoxColumn();
            colResult = new DataGridViewTextBoxColumn();
            colDate = new DataGridViewTextBoxColumn();
            panelHistoryControls = new Panel();
            lblHistoryStats = new Label();
            btnRefreshMy = new Button();
            panelHeader = new Panel();
            lblTitle = new Label();
            panelSoundControls = new Panel();
            btnToggleMusic = new Button();
            btnToggleSfx = new Button();
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
            ((System.ComponentModel.ISupportInitialize)dgvMyHistory).BeginInit();
            panelHistoryControls.SuspendLayout();
            panelHeader.SuspendLayout();
            panelSoundControls.SuspendLayout();
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
            label1.Location = new Point(18, 11);
            label1.Name = "label1";
            label1.Size = new Size(206, 21);
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
            lstClients.ItemHeight = 17;
            lstClients.Location = new Point(18, 38);
            lstClients.Margin = new Padding(3, 2, 3, 2);
            lstClients.Name = "lstClients";
            lstClients.ScrollAlwaysVisible = true;
            lstClients.Size = new Size(455, 104);
            lstClients.TabIndex = 1;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(46, 204, 113);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(118, 20);
            btnConnect.Margin = new Padding(3, 2, 3, 2);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(101, 20);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click_1;
            // 
            // btnFindServers
            // 
            btnFindServers.BackColor = Color.FromArgb(230, 126, 34);
            btnFindServers.FlatAppearance.BorderSize = 0;
            btnFindServers.FlatStyle = FlatStyle.Flat;
            btnFindServers.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnFindServers.ForeColor = Color.White;
            btnFindServers.Location = new Point(96, 20);
            btnFindServers.Margin = new Padding(3, 2, 3, 2);
            btnFindServers.Name = "btnFindServers";
            btnFindServers.Size = new Size(18, 20);
            btnFindServers.TabIndex = 4;
            btnFindServers.Text = "🔍";
            btnFindServers.UseVisualStyleBackColor = false;
            btnFindServers.Click += btnFindServers_Click;
            // 
            // txtIP
            // 
            txtIP.BackColor = Color.White;
            txtIP.BorderStyle = BorderStyle.FixedSingle;
            txtIP.Font = new Font("Segoe UI", 9F);
            txtIP.Location = new Point(9, 20);
            txtIP.Margin = new Padding(3, 2, 3, 2);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(83, 23);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            txtIP.TextAlign = HorizontalAlignment.Center;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Font = new Font("Segoe UI", 10F);
            lblStatus.ForeColor = Color.FromArgb(52, 73, 94);
            lblStatus.Location = new Point(18, 6);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(4, 0, 0, 0);
            lblStatus.Size = new Size(182, 19);
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
            button3.Location = new Point(22, 49);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(175, 41);
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
            label2.Location = new Point(210, 60);
            label2.Name = "label2";
            label2.Size = new Size(219, 15);
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
            btnRequest.Location = new Point(18, 146);
            btnRequest.Margin = new Padding(3, 2, 3, 2);
            btnRequest.Name = "btnRequest";
            btnRequest.Size = new Size(455, 30);
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
            lstRequests.ItemHeight = 17;
            lstRequests.Location = new Point(18, 38);
            lstRequests.Margin = new Padding(3, 2, 3, 2);
            lstRequests.Name = "lstRequests";
            lstRequests.ScrollAlwaysVisible = true;
            lstRequests.Size = new Size(455, 104);
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
            btnAccept.Location = new Point(18, 146);
            btnAccept.Margin = new Padding(3, 2, 3, 2);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(455, 30);
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
            label3.Location = new Point(18, 11);
            label3.Name = "label3";
            label3.Size = new Size(127, 21);
            label3.TabIndex = 0;
            label3.Text = "📨 Lời mời đấu";
            // 
            // tabHistory
            // 
            tabHistory.Controls.Add(tabMyHistory);
            tabHistory.Font = new Font("Segoe UI", 9F);
            tabHistory.Location = new Point(18, 38);
            tabHistory.Margin = new Padding(3, 2, 3, 2);
            tabHistory.Name = "tabHistory";
            tabHistory.SelectedIndex = 0;
            tabHistory.Size = new Size(962, 172);
            tabHistory.TabIndex = 1;
            // 
            // tabMyHistory
            // 
            tabMyHistory.BackColor = Color.FromArgb(250, 251, 252);
            tabMyHistory.Controls.Add(dgvMyHistory);
            tabMyHistory.Controls.Add(panelHistoryControls);
            tabMyHistory.Location = new Point(4, 24);
            tabMyHistory.Margin = new Padding(3, 2, 3, 2);
            tabMyHistory.Name = "tabMyHistory";
            tabMyHistory.Padding = new Padding(9, 8, 9, 8);
            tabMyHistory.Size = new Size(954, 144);
            tabMyHistory.TabIndex = 1;
            tabMyHistory.Text = "⭐ Lịch sử của tôi";
            // 
            // dgvMyHistory
            // 
            dgvMyHistory.AllowUserToAddRows = false;
            dgvMyHistory.AllowUserToDeleteRows = false;
            dgvMyHistory.AllowUserToResizeRows = false;
            dgvMyHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMyHistory.BackgroundColor = Color.White;
            dgvMyHistory.BorderStyle = BorderStyle.None;
            dgvMyHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMyHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvMyHistory.ColumnHeadersHeight = 35;
            dgvMyHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvMyHistory.Columns.AddRange(new DataGridViewColumn[] { colRoomId, colOpponent, colResult, colDate });
            dgvMyHistory.Dock = DockStyle.Fill;
            dgvMyHistory.EnableHeadersVisualStyles = false;
            dgvMyHistory.GridColor = Color.FromArgb(230, 230, 230);
            dgvMyHistory.Location = new Point(9, 42);
            dgvMyHistory.Margin = new Padding(3, 2, 3, 2);
            dgvMyHistory.MultiSelect = false;
            dgvMyHistory.Name = "dgvMyHistory";
            dgvMyHistory.ReadOnly = true;
            dgvMyHistory.RowHeadersVisible = false;
            dgvMyHistory.RowHeadersWidth = 51;
            dgvMyHistory.RowTemplate.Height = 30;
            dgvMyHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyHistory.Size = new Size(936, 94);
            dgvMyHistory.TabIndex = 1;
            // 
            // colRoomId
            // 
            colRoomId.FillWeight = 20F;
            colRoomId.HeaderText = "Phòng";
            colRoomId.MinimumWidth = 6;
            colRoomId.Name = "colRoomId";
            colRoomId.ReadOnly = true;
            // 
            // colOpponent
            // 
            colOpponent.FillWeight = 30F;
            colOpponent.HeaderText = "Đối thủ";
            colOpponent.MinimumWidth = 6;
            colOpponent.Name = "colOpponent";
            colOpponent.ReadOnly = true;
            // 
            // colResult
            // 
            colResult.FillWeight = 20F;
            colResult.HeaderText = "Kết quả";
            colResult.MinimumWidth = 6;
            colResult.Name = "colResult";
            colResult.ReadOnly = true;
            // 
            // colDate
            // 
            colDate.FillWeight = 30F;
            colDate.HeaderText = "Thời gian";
            colDate.MinimumWidth = 6;
            colDate.Name = "colDate";
            colDate.ReadOnly = true;
            // 
            // panelHistoryControls
            // 
            panelHistoryControls.BackColor = Color.White;
            panelHistoryControls.Controls.Add(lblHistoryStats);
            panelHistoryControls.Controls.Add(btnRefreshMy);
            panelHistoryControls.Dock = DockStyle.Top;
            panelHistoryControls.Location = new Point(9, 8);
            panelHistoryControls.Margin = new Padding(3, 2, 3, 2);
            panelHistoryControls.Name = "panelHistoryControls";
            panelHistoryControls.Padding = new Padding(4);
            panelHistoryControls.Size = new Size(936, 34);
            panelHistoryControls.TabIndex = 0;
            // 
            // lblHistoryStats
            // 
            lblHistoryStats.AutoSize = true;
            lblHistoryStats.Dock = DockStyle.Left;
            lblHistoryStats.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblHistoryStats.ForeColor = Color.FromArgb(52, 73, 94);
            lblHistoryStats.Location = new Point(4, 4);
            lblHistoryStats.Name = "lblHistoryStats";
            lblHistoryStats.Padding = new Padding(4, 6, 0, 0);
            lblHistoryStats.Size = new Size(225, 25);
            lblHistoryStats.TabIndex = 0;
            lblHistoryStats.Text = "Tổng: 0 trận | Thắng: 0 | Thua: 0";
            // 
            // btnRefreshMy
            // 
            btnRefreshMy.BackColor = Color.FromArgb(46, 204, 113);
            btnRefreshMy.Dock = DockStyle.Right;
            btnRefreshMy.FlatAppearance.BorderSize = 0;
            btnRefreshMy.FlatStyle = FlatStyle.Flat;
            btnRefreshMy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefreshMy.ForeColor = Color.White;
            btnRefreshMy.Location = new Point(844, 4);
            btnRefreshMy.Margin = new Padding(4);
            btnRefreshMy.Name = "btnRefreshMy";
            btnRefreshMy.Size = new Size(88, 26);
            btnRefreshMy.TabIndex = 1;
            btnRefreshMy.Text = "🔄";
            btnRefreshMy.UseVisualStyleBackColor = false;
            btnRefreshMy.Click += btnRefreshMy_Click;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(panelSoundControls);
            panelHeader.Controls.Add(panelConnection);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(3, 2, 3, 2);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(18, 11, 18, 11);
            panelHeader.Size = new Size(1050, 75);
            panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 11);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 9, 0, 0);
            lblTitle.Size = new Size(289, 56);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🎮 GAME CARO";
            // 
            // panelSoundControls
            // 
            panelSoundControls.BackColor = Color.FromArgb(52, 152, 219);
            panelSoundControls.Controls.Add(btnToggleMusic);
            panelSoundControls.Controls.Add(btnToggleSfx);
            panelSoundControls.Dock = DockStyle.Right;
            panelSoundControls.Location = new Point(576, 11);
            panelSoundControls.Margin = new Padding(3, 2, 3, 2);
            panelSoundControls.Name = "panelSoundControls";
            panelSoundControls.Padding = new Padding(9, 4, 9, 4);
            panelSoundControls.Size = new Size(228, 53);
            panelSoundControls.TabIndex = 2;
            // 
            // btnToggleMusic
            // 
            btnToggleMusic.BackColor = Color.FromArgb(46, 204, 113);
            btnToggleMusic.FlatAppearance.BorderSize = 0;
            btnToggleMusic.FlatStyle = FlatStyle.Flat;
            btnToggleMusic.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnToggleMusic.ForeColor = Color.White;
            btnToggleMusic.Location = new Point(9, 8);
            btnToggleMusic.Margin = new Padding(3, 2, 3, 2);
            btnToggleMusic.Name = "btnToggleMusic";
            btnToggleMusic.Size = new Size(101, 38);
            btnToggleMusic.TabIndex = 0;
            btnToggleMusic.Text = "🎵 Nhạc: BẬT";
            btnToggleMusic.UseVisualStyleBackColor = false;
            btnToggleMusic.Click += btnToggleMusic_Click;
            // 
            // btnToggleSfx
            // 
            btnToggleSfx.BackColor = Color.FromArgb(46, 204, 113);
            btnToggleSfx.FlatAppearance.BorderSize = 0;
            btnToggleSfx.FlatStyle = FlatStyle.Flat;
            btnToggleSfx.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnToggleSfx.ForeColor = Color.White;
            btnToggleSfx.Location = new Point(118, 8);
            btnToggleSfx.Margin = new Padding(3, 2, 3, 2);
            btnToggleSfx.Name = "btnToggleSfx";
            btnToggleSfx.Size = new Size(101, 38);
            btnToggleSfx.TabIndex = 1;
            btnToggleSfx.Text = "🔊 SFX: BẬT";
            btnToggleSfx.UseVisualStyleBackColor = false;
            btnToggleSfx.Click += btnToggleSfx_Click;
            // 
            // panelConnection
            // 
            panelConnection.BackColor = Color.FromArgb(52, 152, 219);
            panelConnection.Controls.Add(lblConnectionTitle);
            panelConnection.Controls.Add(txtIP);
            panelConnection.Controls.Add(btnFindServers);
            panelConnection.Controls.Add(btnConnect);
            panelConnection.Dock = DockStyle.Right;
            panelConnection.Location = new Point(804, 11);
            panelConnection.Margin = new Padding(3, 2, 3, 2);
            panelConnection.Name = "panelConnection";
            panelConnection.Padding = new Padding(9, 4, 9, 4);
            panelConnection.Size = new Size(228, 53);
            panelConnection.TabIndex = 1;
            // 
            // lblConnectionTitle
            // 
            lblConnectionTitle.AutoSize = true;
            lblConnectionTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblConnectionTitle.ForeColor = Color.White;
            lblConnectionTitle.Location = new Point(9, 4);
            lblConnectionTitle.Name = "lblConnectionTitle";
            lblConnectionTitle.Size = new Size(65, 13);
            lblConnectionTitle.TabIndex = 0;
            lblConnectionTitle.Text = "🔌 KẾT NỐI";
            // 
            // panelStatusBar
            // 
            panelStatusBar.BackColor = Color.FromArgb(236, 240, 241);
            panelStatusBar.Controls.Add(lblStatus);
            panelStatusBar.Dock = DockStyle.Top;
            panelStatusBar.Location = new Point(0, 75);
            panelStatusBar.Margin = new Padding(3, 2, 3, 2);
            panelStatusBar.Name = "panelStatusBar";
            panelStatusBar.Padding = new Padding(18, 6, 18, 6);
            panelStatusBar.Size = new Size(1050, 30);
            panelStatusBar.TabIndex = 1;
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(245, 247, 250);
            panelMain.Controls.Add(panelHistory);
            panelMain.Controls.Add(panelPlayersAndInvites);
            panelMain.Controls.Add(panelQuickMatch);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 105);
            panelMain.Margin = new Padding(3, 2, 3, 2);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(26, 15, 26, 15);
            panelMain.Size = new Size(1050, 551);
            panelMain.TabIndex = 2;
            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.White;
            panelHistory.Controls.Add(lblHistoryTitle);
            panelHistory.Controls.Add(tabHistory);
            panelHistory.Dock = DockStyle.Top;
            panelHistory.Location = new Point(26, 322);
            panelHistory.Margin = new Padding(3, 2, 3, 2);
            panelHistory.Name = "panelHistory";
            panelHistory.Padding = new Padding(18, 11, 18, 11);
            panelHistory.Size = new Size(998, 221);
            panelHistory.TabIndex = 2;
            // 
            // lblHistoryTitle
            // 
            lblHistoryTitle.AutoSize = true;
            lblHistoryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblHistoryTitle.Location = new Point(18, 11);
            lblHistoryTitle.Name = "lblHistoryTitle";
            lblHistoryTitle.Size = new Size(155, 21);
            lblHistoryTitle.TabIndex = 0;
            lblHistoryTitle.Text = "📊 Lịch sử gần đây";
            // 
            // panelPlayersAndInvites
            // 
            panelPlayersAndInvites.BackColor = Color.Transparent;
            panelPlayersAndInvites.Controls.Add(panelInvitations);
            panelPlayersAndInvites.Controls.Add(panelPlayers);
            panelPlayersAndInvites.Dock = DockStyle.Top;
            panelPlayersAndInvites.Location = new Point(26, 127);
            panelPlayersAndInvites.Margin = new Padding(3, 2, 3, 2);
            panelPlayersAndInvites.Name = "panelPlayersAndInvites";
            panelPlayersAndInvites.Padding = new Padding(0, 11, 0, 0);
            panelPlayersAndInvites.Size = new Size(998, 195);
            panelPlayersAndInvites.TabIndex = 1;
            // 
            // panelInvitations
            // 
            panelInvitations.BackColor = Color.White;
            panelInvitations.Controls.Add(label3);
            panelInvitations.Controls.Add(lstRequests);
            panelInvitations.Controls.Add(btnAccept);
            panelInvitations.Dock = DockStyle.Right;
            panelInvitations.Location = new Point(508, 11);
            panelInvitations.Margin = new Padding(3, 2, 3, 2);
            panelInvitations.Name = "panelInvitations";
            panelInvitations.Padding = new Padding(18, 11, 18, 11);
            panelInvitations.Size = new Size(490, 184);
            panelInvitations.TabIndex = 1;
            // 
            // panelPlayers
            // 
            panelPlayers.BackColor = Color.White;
            panelPlayers.Controls.Add(label1);
            panelPlayers.Controls.Add(lstClients);
            panelPlayers.Controls.Add(btnRequest);
            panelPlayers.Dock = DockStyle.Left;
            panelPlayers.Location = new Point(0, 11);
            panelPlayers.Margin = new Padding(3, 2, 3, 2);
            panelPlayers.Name = "panelPlayers";
            panelPlayers.Padding = new Padding(18, 11, 18, 11);
            panelPlayers.Size = new Size(490, 184);
            panelPlayers.TabIndex = 0;
            // 
            // panelQuickMatch
            // 
            panelQuickMatch.BackColor = Color.White;
            panelQuickMatch.Controls.Add(lblQuickMatchTitle);
            panelQuickMatch.Controls.Add(button3);
            panelQuickMatch.Controls.Add(label2);
            panelQuickMatch.Dock = DockStyle.Top;
            panelQuickMatch.Location = new Point(26, 15);
            panelQuickMatch.Margin = new Padding(3, 2, 3, 2);
            panelQuickMatch.Name = "panelQuickMatch";
            panelQuickMatch.Padding = new Padding(22, 15, 22, 15);
            panelQuickMatch.Size = new Size(998, 112);
            panelQuickMatch.TabIndex = 0;
            // 
            // lblQuickMatchTitle
            // 
            lblQuickMatchTitle.AutoSize = true;
            lblQuickMatchTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblQuickMatchTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblQuickMatchTitle.Location = new Point(22, 15);
            lblQuickMatchTitle.Name = "lblQuickMatchTitle";
            lblQuickMatchTitle.Size = new Size(194, 30);
            lblQuickMatchTitle.TabIndex = 0;
            lblQuickMatchTitle.Text = "⚡ CHƠI NHANH";
            // 
            // sanhCho
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1050, 656);
            Controls.Add(panelMain);
            Controls.Add(panelStatusBar);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "sanhCho";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Caro - Sảnh Chờ";
            Load += sanhCho_Load;
            tabHistory.ResumeLayout(false);
            tabMyHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMyHistory).EndInit();
            panelHistoryControls.ResumeLayout(false);
            panelHistoryControls.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelSoundControls.ResumeLayout(false);
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
        private DataGridView dgvMyHistory;
        private DataGridViewTextBoxColumn colRoomId;
        private DataGridViewTextBoxColumn colOpponent;
        private DataGridViewTextBoxColumn colResult;
        private DataGridViewTextBoxColumn colDate;
        private Panel panelHistoryControls;
        private Label lblHistoryStats;
        private Button btnRefreshMy;
        private Panel panelSoundControls;
        private Button btnToggleMusic;
        private Button btnToggleSfx;
    }
}