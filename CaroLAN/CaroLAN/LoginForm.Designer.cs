namespace CaroLAN
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPageLogin = new TabPage();
            lblLoginUsername = new Label();
            txtLoginUsername = new TextBox();
            lblLoginPassword = new Label();
            txtLoginPassword = new TextBox();
            btnLogin = new Button();
            tabPageRegister = new TabPage();
            lblRegisterUsername = new Label();
            txtRegisterUsername = new TextBox();
            lblRegisterPassword = new Label();
            txtRegisterPassword = new TextBox();
            lblRegisterConfirmPassword = new Label();
            txtRegisterConfirmPassword = new TextBox();
            btnRegister = new Button();
            lblStatus = new Label();
            lblUserInfo = new Label();
            panelStatusBar = new Panel();
            panelMain = new Panel();
            panelConnection = new Panel();
            lblConnectionTitle = new Label();
            lblServerIP = new Label();
            txtServerIP = new TextBox();
            btnConnect = new Button();
            btnFindServers = new Button();
            panelHeader = new Panel();
            lblTitle = new Label();
            tabControl1.SuspendLayout();
            tabPageLogin.SuspendLayout();
            tabPageRegister.SuspendLayout();
            panelStatusBar.SuspendLayout();
            panelMain.SuspendLayout();
            panelConnection.SuspendLayout();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageLogin);
            tabControl1.Controls.Add(tabPageRegister);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Font = new Font("Segoe UI", 10F);
            tabControl1.Location = new Point(26, 15);
            tabControl1.Margin = new Padding(3, 2, 3, 2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(386, 210);
            tabControl1.TabIndex = 0;
            // 
            // tabPageLogin
            // 
            tabPageLogin.BackColor = Color.White;
            tabPageLogin.Controls.Add(lblLoginUsername);
            tabPageLogin.Controls.Add(txtLoginUsername);
            tabPageLogin.Controls.Add(lblLoginPassword);
            tabPageLogin.Controls.Add(txtLoginPassword);
            tabPageLogin.Controls.Add(btnLogin);
            tabPageLogin.Location = new Point(4, 26);
            tabPageLogin.Margin = new Padding(3, 2, 3, 2);
            tabPageLogin.Name = "tabPageLogin";
            tabPageLogin.Padding = new Padding(18, 15, 18, 15);
            tabPageLogin.Size = new Size(378, 180);
            tabPageLogin.TabIndex = 0;
            tabPageLogin.Text = "🔐 Đăng nhập";
            // 
            // lblLoginUsername
            // 
            lblLoginUsername.AutoSize = true;
            lblLoginUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLoginUsername.ForeColor = Color.FromArgb(52, 73, 94);
            lblLoginUsername.Location = new Point(18, 15);
            lblLoginUsername.Name = "lblLoginUsername";
            lblLoginUsername.Size = new Size(107, 19);
            lblLoginUsername.TabIndex = 0;
            lblLoginUsername.Text = "Tên đăng nhập";
            // 
            // txtLoginUsername
            // 
            txtLoginUsername.BackColor = Color.FromArgb(250, 251, 252);
            txtLoginUsername.BorderStyle = BorderStyle.FixedSingle;
            txtLoginUsername.Font = new Font("Segoe UI", 11F);
            txtLoginUsername.Location = new Point(18, 34);
            txtLoginUsername.Margin = new Padding(3, 2, 3, 2);
            txtLoginUsername.Name = "txtLoginUsername";
            txtLoginUsername.Size = new Size(343, 27);
            txtLoginUsername.TabIndex = 1;
            // 
            // lblLoginPassword
            // 
            lblLoginPassword.AutoSize = true;
            lblLoginPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLoginPassword.ForeColor = Color.FromArgb(52, 73, 94);
            lblLoginPassword.Location = new Point(18, 68);
            lblLoginPassword.Name = "lblLoginPassword";
            lblLoginPassword.Size = new Size(71, 19);
            lblLoginPassword.TabIndex = 2;
            lblLoginPassword.Text = "Mật khẩu";
            // 
            // txtLoginPassword
            // 
            txtLoginPassword.BackColor = Color.FromArgb(250, 251, 252);
            txtLoginPassword.BorderStyle = BorderStyle.FixedSingle;
            txtLoginPassword.Font = new Font("Segoe UI", 11F);
            txtLoginPassword.Location = new Point(18, 86);
            txtLoginPassword.Margin = new Padding(3, 2, 3, 2);
            txtLoginPassword.Name = "txtLoginPassword";
            txtLoginPassword.PasswordChar = '●';
            txtLoginPassword.Size = new Size(343, 27);
            txtLoginPassword.TabIndex = 3;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(52, 152, 219);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(18, 128);
            btnLogin.Margin = new Padding(3, 2, 3, 2);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(343, 34);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // tabPageRegister
            // 
            tabPageRegister.BackColor = Color.White;
            tabPageRegister.Controls.Add(lblRegisterUsername);
            tabPageRegister.Controls.Add(txtRegisterUsername);
            tabPageRegister.Controls.Add(lblRegisterPassword);
            tabPageRegister.Controls.Add(txtRegisterPassword);
            tabPageRegister.Controls.Add(lblRegisterConfirmPassword);
            tabPageRegister.Controls.Add(txtRegisterConfirmPassword);
            tabPageRegister.Controls.Add(btnRegister);
            tabPageRegister.Location = new Point(4, 26);
            tabPageRegister.Margin = new Padding(3, 2, 3, 2);
            tabPageRegister.Name = "tabPageRegister";
            tabPageRegister.Padding = new Padding(18, 11, 18, 11);
            tabPageRegister.Size = new Size(378, 180);
            tabPageRegister.TabIndex = 1;
            tabPageRegister.Text = "📝 Đăng ký";
            // 
            // lblRegisterUsername
            // 
            lblRegisterUsername.AutoSize = true;
            lblRegisterUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRegisterUsername.ForeColor = Color.FromArgb(52, 73, 94);
            lblRegisterUsername.Location = new Point(18, 11);
            lblRegisterUsername.Name = "lblRegisterUsername";
            lblRegisterUsername.Size = new Size(88, 15);
            lblRegisterUsername.TabIndex = 0;
            lblRegisterUsername.Text = "Tên đăng nhập";
            // 
            // txtRegisterUsername
            // 
            txtRegisterUsername.BackColor = Color.FromArgb(250, 251, 252);
            txtRegisterUsername.BorderStyle = BorderStyle.FixedSingle;
            txtRegisterUsername.Font = new Font("Segoe UI", 10F);
            txtRegisterUsername.Location = new Point(18, 28);
            txtRegisterUsername.Margin = new Padding(3, 2, 3, 2);
            txtRegisterUsername.Name = "txtRegisterUsername";
            txtRegisterUsername.Size = new Size(343, 25);
            txtRegisterUsername.TabIndex = 1;
            // 
            // lblRegisterPassword
            // 
            lblRegisterPassword.AutoSize = true;
            lblRegisterPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRegisterPassword.ForeColor = Color.FromArgb(52, 73, 94);
            lblRegisterPassword.Location = new Point(18, 52);
            lblRegisterPassword.Name = "lblRegisterPassword";
            lblRegisterPassword.Size = new Size(59, 15);
            lblRegisterPassword.TabIndex = 2;
            lblRegisterPassword.Text = "Mật khẩu";
            // 
            // txtRegisterPassword
            // 
            txtRegisterPassword.BackColor = Color.FromArgb(250, 251, 252);
            txtRegisterPassword.BorderStyle = BorderStyle.FixedSingle;
            txtRegisterPassword.Font = new Font("Segoe UI", 10F);
            txtRegisterPassword.Location = new Point(18, 68);
            txtRegisterPassword.Margin = new Padding(3, 2, 3, 2);
            txtRegisterPassword.Name = "txtRegisterPassword";
            txtRegisterPassword.PasswordChar = '●';
            txtRegisterPassword.Size = new Size(343, 25);
            txtRegisterPassword.TabIndex = 3;
            // 
            // lblRegisterConfirmPassword
            // 
            lblRegisterConfirmPassword.AutoSize = true;
            lblRegisterConfirmPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRegisterConfirmPassword.ForeColor = Color.FromArgb(52, 73, 94);
            lblRegisterConfirmPassword.Location = new Point(18, 93);
            lblRegisterConfirmPassword.Name = "lblRegisterConfirmPassword";
            lblRegisterConfirmPassword.Size = new Size(112, 15);
            lblRegisterConfirmPassword.TabIndex = 4;
            lblRegisterConfirmPassword.Text = "Xác nhận mật khẩu";
            // 
            // txtRegisterConfirmPassword
            // 
            txtRegisterConfirmPassword.BackColor = Color.FromArgb(250, 251, 252);
            txtRegisterConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtRegisterConfirmPassword.Font = new Font("Segoe UI", 10F);
            txtRegisterConfirmPassword.Location = new Point(18, 110);
            txtRegisterConfirmPassword.Margin = new Padding(3, 2, 3, 2);
            txtRegisterConfirmPassword.Name = "txtRegisterConfirmPassword";
            txtRegisterConfirmPassword.PasswordChar = '●';
            txtRegisterConfirmPassword.Size = new Size(343, 25);
            txtRegisterConfirmPassword.TabIndex = 5;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.FromArgb(46, 204, 113);
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(18, 146);
            btnRegister.Margin = new Padding(3, 2, 3, 2);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(343, 30);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "ĐĂNG KÝ";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(52, 73, 94);
            lblStatus.Location = new Point(18, 6);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(0, 2, 0, 0);
            lblStatus.Size = new Size(89, 17);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "⚪ Chưa kết nối";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblUserInfo
            // 
            lblUserInfo.AutoSize = true;
            lblUserInfo.Dock = DockStyle.Right;
            lblUserInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblUserInfo.ForeColor = Color.FromArgb(149, 165, 166);
            lblUserInfo.Location = new Point(420, 6);
            lblUserInfo.Name = "lblUserInfo";
            lblUserInfo.Padding = new Padding(0, 2, 0, 0);
            lblUserInfo.Size = new Size(0, 17);
            lblUserInfo.TabIndex = 1;
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelStatusBar
            // 
            panelStatusBar.BackColor = Color.FromArgb(236, 240, 241);
            panelStatusBar.Controls.Add(lblStatus);
            panelStatusBar.Controls.Add(lblUserInfo);
            panelStatusBar.Dock = DockStyle.Top;
            panelStatusBar.Location = new Point(0, 68);
            panelStatusBar.Margin = new Padding(3, 2, 3, 2);
            panelStatusBar.Name = "panelStatusBar";
            panelStatusBar.Padding = new Padding(18, 6, 18, 6);
            panelStatusBar.Size = new Size(438, 30);
            panelStatusBar.TabIndex = 1;
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(245, 247, 250);
            panelMain.Controls.Add(tabControl1);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 98);
            panelMain.Margin = new Padding(3, 2, 3, 2);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(26, 15, 26, 15);
            panelMain.Size = new Size(438, 240);
            panelMain.TabIndex = 2;
            // 
            // panelConnection
            // 
            panelConnection.BackColor = Color.FromArgb(52, 152, 219);
            panelConnection.Controls.Add(lblConnectionTitle);
            panelConnection.Controls.Add(lblServerIP);
            panelConnection.Controls.Add(txtServerIP);
            panelConnection.Controls.Add(btnConnect);
            panelConnection.Controls.Add(btnFindServers);
            panelConnection.Dock = DockStyle.Right;
            panelConnection.Location = new Point(210, 11);
            panelConnection.Margin = new Padding(3, 2, 3, 2);
            panelConnection.Name = "panelConnection";
            panelConnection.Padding = new Padding(9, 4, 9, 4);
            panelConnection.Size = new Size(210, 46);
            panelConnection.TabIndex = 1;
            // 
            // lblConnectionTitle
            // 
            lblConnectionTitle.AutoSize = true;
            lblConnectionTitle.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            lblConnectionTitle.ForeColor = Color.White;
            lblConnectionTitle.Location = new Point(9, 4);
            lblConnectionTitle.Name = "lblConnectionTitle";
            lblConnectionTitle.Size = new Size(57, 12);
            lblConnectionTitle.TabIndex = 0;
            lblConnectionTitle.Text = "🔌 SERVER";
            // 
            // lblServerIP
            // 
            lblServerIP.AutoSize = true;
            lblServerIP.Font = new Font("Segoe UI", 7F);
            lblServerIP.ForeColor = Color.White;
            lblServerIP.Location = new Point(9, 16);
            lblServerIP.Name = "lblServerIP";
            lblServerIP.Size = new Size(16, 12);
            lblServerIP.TabIndex = 1;
            lblServerIP.Text = "IP:";
            // 
            // txtServerIP
            // 
            txtServerIP.BackColor = Color.White;
            txtServerIP.BorderStyle = BorderStyle.FixedSingle;
            txtServerIP.Font = new Font("Segoe UI", 8F);
            txtServerIP.Location = new Point(26, 15);
            txtServerIP.Margin = new Padding(3, 2, 3, 2);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(62, 22);
            txtServerIP.TabIndex = 2;
            txtServerIP.Text = "127.0.0.1";
            txtServerIP.TextAlign = HorizontalAlignment.Center;
            txtServerIP.TextChanged += txtServerIP_TextChanged;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(46, 204, 113);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(114, 15);
            btnConnect.Margin = new Padding(3, 2, 3, 2);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(88, 19);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnFindServers
            // 
            btnFindServers.BackColor = Color.FromArgb(230, 126, 34);
            btnFindServers.FlatAppearance.BorderSize = 0;
            btnFindServers.FlatStyle = FlatStyle.Flat;
            btnFindServers.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnFindServers.ForeColor = Color.White;
            btnFindServers.Location = new Point(92, 15);
            btnFindServers.Margin = new Padding(3, 2, 3, 2);
            btnFindServers.Name = "btnFindServers";
            btnFindServers.Size = new Size(18, 19);
            btnFindServers.TabIndex = 4;
            btnFindServers.Text = "🔍";
            btnFindServers.UseVisualStyleBackColor = false;
            btnFindServers.Click += btnFindServers_Click;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(panelConnection);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(3, 2, 3, 2);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(18, 11, 18, 11);
            panelHeader.Size = new Size(438, 68);
            panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(176, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "GAME CARO";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(438, 338);
            Controls.Add(panelMain);
            Controls.Add(panelStatusBar);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Caro - Đăng nhập";
            tabControl1.ResumeLayout(false);
            tabPageLogin.ResumeLayout(false);
            tabPageLogin.PerformLayout();
            tabPageRegister.ResumeLayout(false);
            tabPageRegister.PerformLayout();
            panelStatusBar.ResumeLayout(false);
            panelStatusBar.PerformLayout();
            panelMain.ResumeLayout(false);
            panelConnection.ResumeLayout(false);
            panelConnection.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panelStatusBar;
        private Panel panelMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtLoginPassword;
        private System.Windows.Forms.Label lblLoginPassword;
        private System.Windows.Forms.TextBox txtLoginUsername;
        private System.Windows.Forms.Label lblLoginUsername;
        private System.Windows.Forms.TabPage tabPageRegister;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TextBox txtRegisterConfirmPassword;
        private System.Windows.Forms.Label lblRegisterConfirmPassword;
        private System.Windows.Forms.TextBox txtRegisterPassword;
        private System.Windows.Forms.Label lblRegisterPassword;
        private System.Windows.Forms.TextBox txtRegisterUsername;
        private System.Windows.Forms.Label lblRegisterUsername;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUserInfo;
        private Panel panelConnection;
        private Label lblConnectionTitle;
        private Label lblServerIP;
        private TextBox txtServerIP;
        private Button btnConnect;
        private Button btnFindServers;
        private Panel panelHeader;
        private Label lblTitle;
    }
}
