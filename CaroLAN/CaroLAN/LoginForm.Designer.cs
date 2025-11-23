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
            btnLogin = new Button();
            txtLoginPassword = new TextBox();
            lblLoginPassword = new Label();
            txtLoginUsername = new TextBox();
            lblLoginUsername = new Label();
            tabPageRegister = new TabPage();
            btnRegister = new Button();
            txtRegisterEmail = new TextBox();
            lblRegisterEmail = new Label();
            txtRegisterConfirmPassword = new TextBox();
            lblRegisterConfirmPassword = new Label();
            txtRegisterPassword = new TextBox();
            lblRegisterPassword = new Label();
            txtRegisterUsername = new TextBox();
            lblRegisterUsername = new Label();
            lblServerIP = new Label();
            txtServerIP = new TextBox();
            btnConnect = new Button();
            lblStatus = new Label();
            lblUserInfo = new Label();
            tabControl1.SuspendLayout();
            tabPageLogin.SuspendLayout();
            tabPageRegister.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageLogin);
            tabControl1.Controls.Add(tabPageRegister);
            tabControl1.Location = new Point(10, 75);
            tabControl1.Margin = new Padding(3, 2, 3, 2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(350, 240);
            tabControl1.TabIndex = 0;
            // 
            // tabPageLogin
            // 
            tabPageLogin.Controls.Add(btnLogin);
            tabPageLogin.Controls.Add(txtLoginPassword);
            tabPageLogin.Controls.Add(lblLoginPassword);
            tabPageLogin.Controls.Add(txtLoginUsername);
            tabPageLogin.Controls.Add(lblLoginUsername);
            tabPageLogin.Location = new Point(4, 24);
            tabPageLogin.Margin = new Padding(3, 2, 3, 2);
            tabPageLogin.Name = "tabPageLogin";
            tabPageLogin.Padding = new Padding(3, 2, 3, 2);
            tabPageLogin.Size = new Size(342, 212);
            tabPageLogin.TabIndex = 0;
            tabPageLogin.Text = "Đăng nhập";
            tabPageLogin.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(0, 122, 204);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(18, 128);
            btnLogin.Margin = new Padding(3, 2, 3, 2);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(306, 34);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtLoginPassword
            // 
            txtLoginPassword.Location = new Point(18, 90);
            txtLoginPassword.Margin = new Padding(3, 2, 3, 2);
            txtLoginPassword.Name = "txtLoginPassword";
            txtLoginPassword.PasswordChar = '*';
            txtLoginPassword.Size = new Size(307, 23);
            txtLoginPassword.TabIndex = 3;
            // 
            // lblLoginPassword
            // 
            lblLoginPassword.AutoSize = true;
            lblLoginPassword.Location = new Point(18, 71);
            lblLoginPassword.Name = "lblLoginPassword";
            lblLoginPassword.Size = new Size(60, 15);
            lblLoginPassword.TabIndex = 2;
            lblLoginPassword.Text = "Password:";
            // 
            // txtLoginUsername
            // 
            txtLoginUsername.Location = new Point(18, 41);
            txtLoginUsername.Margin = new Padding(3, 2, 3, 2);
            txtLoginUsername.Name = "txtLoginUsername";
            txtLoginUsername.Size = new Size(307, 23);
            txtLoginUsername.TabIndex = 1;
            // 
            // lblLoginUsername
            // 
            lblLoginUsername.AutoSize = true;
            lblLoginUsername.Location = new Point(18, 22);
            lblLoginUsername.Name = "lblLoginUsername";
            lblLoginUsername.Size = new Size(63, 15);
            lblLoginUsername.TabIndex = 0;
            lblLoginUsername.Text = "Username:";
            // 
            // tabPageRegister
            // 
            tabPageRegister.Controls.Add(btnRegister);
            tabPageRegister.Controls.Add(txtRegisterEmail);
            tabPageRegister.Controls.Add(lblRegisterEmail);
            tabPageRegister.Controls.Add(txtRegisterConfirmPassword);
            tabPageRegister.Controls.Add(lblRegisterConfirmPassword);
            tabPageRegister.Controls.Add(txtRegisterPassword);
            tabPageRegister.Controls.Add(lblRegisterPassword);
            tabPageRegister.Controls.Add(txtRegisterUsername);
            tabPageRegister.Controls.Add(lblRegisterUsername);
            tabPageRegister.Location = new Point(4, 24);
            tabPageRegister.Margin = new Padding(3, 2, 3, 2);
            tabPageRegister.Name = "tabPageRegister";
            tabPageRegister.Padding = new Padding(3, 2, 3, 2);
            tabPageRegister.Size = new Size(342, 212);
            tabPageRegister.TabIndex = 1;
            tabPageRegister.Text = "Đăng ký";
            tabPageRegister.UseVisualStyleBackColor = true;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.FromArgb(40, 167, 69);
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(18, 180);
            btnRegister.Margin = new Padding(3, 2, 3, 2);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(306, 34);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "Đăng ký";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // txtRegisterEmail
            // 
            txtRegisterEmail.Location = new Point(18, 150);
            txtRegisterEmail.Margin = new Padding(3, 2, 3, 2);
            txtRegisterEmail.Name = "txtRegisterEmail";
            txtRegisterEmail.Size = new Size(307, 23);
            txtRegisterEmail.TabIndex = 7;
            // 
            // lblRegisterEmail
            // 
            lblRegisterEmail.AutoSize = true;
            lblRegisterEmail.Location = new Point(18, 131);
            lblRegisterEmail.Name = "lblRegisterEmail";
            lblRegisterEmail.Size = new Size(39, 15);
            lblRegisterEmail.TabIndex = 6;
            lblRegisterEmail.Text = "Email:";
            // 
            // txtRegisterConfirmPassword
            // 
            txtRegisterConfirmPassword.Location = new Point(18, 109);
            txtRegisterConfirmPassword.Margin = new Padding(3, 2, 3, 2);
            txtRegisterConfirmPassword.Name = "txtRegisterConfirmPassword";
            txtRegisterConfirmPassword.PasswordChar = '*';
            txtRegisterConfirmPassword.Size = new Size(307, 23);
            txtRegisterConfirmPassword.TabIndex = 5;
            // 
            // lblRegisterConfirmPassword
            // 
            lblRegisterConfirmPassword.AutoSize = true;
            lblRegisterConfirmPassword.Location = new Point(18, 90);
            lblRegisterConfirmPassword.Name = "lblRegisterConfirmPassword";
            lblRegisterConfirmPassword.Size = new Size(112, 15);
            lblRegisterConfirmPassword.TabIndex = 4;
            lblRegisterConfirmPassword.Text = "Xác nhận Password:";
            // 
            // txtRegisterPassword
            // 
            txtRegisterPassword.Location = new Point(18, 71);
            txtRegisterPassword.Margin = new Padding(3, 2, 3, 2);
            txtRegisterPassword.Name = "txtRegisterPassword";
            txtRegisterPassword.PasswordChar = '*';
            txtRegisterPassword.Size = new Size(307, 23);
            txtRegisterPassword.TabIndex = 3;
            // 
            // lblRegisterPassword
            // 
            lblRegisterPassword.AutoSize = true;
            lblRegisterPassword.Location = new Point(18, 52);
            lblRegisterPassword.Name = "lblRegisterPassword";
            lblRegisterPassword.Size = new Size(60, 15);
            lblRegisterPassword.TabIndex = 2;
            lblRegisterPassword.Text = "Password:";
            // 
            // txtRegisterUsername
            // 
            txtRegisterUsername.Location = new Point(18, 34);
            txtRegisterUsername.Margin = new Padding(3, 2, 3, 2);
            txtRegisterUsername.Name = "txtRegisterUsername";
            txtRegisterUsername.Size = new Size(307, 23);
            txtRegisterUsername.TabIndex = 1;
            // 
            // lblRegisterUsername
            // 
            lblRegisterUsername.AutoSize = true;
            lblRegisterUsername.Location = new Point(18, 15);
            lblRegisterUsername.Name = "lblRegisterUsername";
            lblRegisterUsername.Size = new Size(63, 15);
            lblRegisterUsername.TabIndex = 0;
            lblRegisterUsername.Text = "Username:";
            // 
            // lblServerIP
            // 
            lblServerIP.AutoSize = true;
            lblServerIP.Location = new Point(10, 15);
            lblServerIP.Name = "lblServerIP";
            lblServerIP.Size = new Size(55, 15);
            lblServerIP.TabIndex = 1;
            lblServerIP.Text = "Server IP:";
            // 
            // txtServerIP
            // 
            txtServerIP.Location = new Point(77, 13);
            txtServerIP.Margin = new Padding(3, 2, 3, 2);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(140, 23);
            txtServerIP.TabIndex = 2;
            txtServerIP.Text = "127.0.0.1";
            txtServerIP.TextChanged += txtServerIP_TextChanged;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(222, 12);
            btnConnect.Margin = new Padding(3, 2, 3, 2);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(105, 22);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(10, 45);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(74, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Chưa kết nối";
            // 
            // lblUserInfo
            // 
            lblUserInfo.AutoSize = true;
            lblUserInfo.Location = new Point(10, 60);
            lblUserInfo.Name = "lblUserInfo";
            lblUserInfo.Size = new Size(0, 15);
            lblUserInfo.TabIndex = 5;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 322);
            Controls.Add(lblUserInfo);
            Controls.Add(lblStatus);
            Controls.Add(btnConnect);
            Controls.Add(txtServerIP);
            Controls.Add(lblServerIP);
            Controls.Add(tabControl1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập - GameCaro";
            tabControl1.ResumeLayout(false);
            tabPageLogin.ResumeLayout(false);
            tabPageLogin.PerformLayout();
            tabPageRegister.ResumeLayout(false);
            tabPageRegister.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtLoginPassword;
        private System.Windows.Forms.Label lblLoginPassword;
        private System.Windows.Forms.TextBox txtLoginUsername;
        private System.Windows.Forms.Label lblLoginUsername;
        private System.Windows.Forms.TabPage tabPageRegister;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TextBox txtRegisterEmail;
        private System.Windows.Forms.Label lblRegisterEmail;
        private System.Windows.Forms.TextBox txtRegisterConfirmPassword;
        private System.Windows.Forms.Label lblRegisterConfirmPassword;
        private System.Windows.Forms.TextBox txtRegisterPassword;
        private System.Windows.Forms.Label lblRegisterPassword;
        private System.Windows.Forms.TextBox txtRegisterUsername;
        private System.Windows.Forms.Label lblRegisterUsername;
        private System.Windows.Forms.Label lblServerIP;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUserInfo;
    }
}
