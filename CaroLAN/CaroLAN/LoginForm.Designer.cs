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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLogin = new System.Windows.Forms.TabPage();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtLoginPassword = new System.Windows.Forms.TextBox();
            this.lblLoginPassword = new System.Windows.Forms.Label();
            this.txtLoginUsername = new System.Windows.Forms.TextBox();
            this.lblLoginUsername = new System.Windows.Forms.Label();
            this.tabPageRegister = new System.Windows.Forms.TabPage();
            this.btnRegister = new System.Windows.Forms.Button();
            this.txtRegisterEmail = new System.Windows.Forms.TextBox();
            this.lblRegisterEmail = new System.Windows.Forms.Label();
            this.txtRegisterConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblRegisterConfirmPassword = new System.Windows.Forms.Label();
            this.txtRegisterPassword = new System.Windows.Forms.TextBox();
            this.lblRegisterPassword = new System.Windows.Forms.Label();
            this.txtRegisterUsername = new System.Windows.Forms.TextBox();
            this.lblRegisterUsername = new System.Windows.Forms.Label();
            this.lblServerIP = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageLogin.SuspendLayout();
            this.tabPageRegister.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLogin);
            this.tabControl1.Controls.Add(this.tabPageRegister);
            this.tabControl1.Location = new System.Drawing.Point(12, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(400, 320);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageLogin
            // 
            this.tabPageLogin.Controls.Add(this.btnLogin);
            this.tabPageLogin.Controls.Add(this.txtLoginPassword);
            this.tabPageLogin.Controls.Add(this.lblLoginPassword);
            this.tabPageLogin.Controls.Add(this.txtLoginUsername);
            this.tabPageLogin.Controls.Add(this.lblLoginUsername);
            this.tabPageLogin.Location = new System.Drawing.Point(4, 29);
            this.tabPageLogin.Name = "tabPageLogin";
            this.tabPageLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogin.Size = new System.Drawing.Size(392, 287);
            this.tabPageLogin.TabIndex = 0;
            this.tabPageLogin.Text = "Đăng nhập";
            this.tabPageLogin.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(20, 170);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(350, 45);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtLoginPassword
            // 
            this.txtLoginPassword.Location = new System.Drawing.Point(20, 120);
            this.txtLoginPassword.Name = "txtLoginPassword";
            this.txtLoginPassword.PasswordChar = '*';
            this.txtLoginPassword.Size = new System.Drawing.Size(350, 27);
            this.txtLoginPassword.TabIndex = 3;
            // 
            // lblLoginPassword
            // 
            this.lblLoginPassword.AutoSize = true;
            this.lblLoginPassword.Location = new System.Drawing.Point(20, 95);
            this.lblLoginPassword.Name = "lblLoginPassword";
            this.lblLoginPassword.Size = new System.Drawing.Size(73, 20);
            this.lblLoginPassword.TabIndex = 2;
            this.lblLoginPassword.Text = "Password:";
            // 
            // txtLoginUsername
            // 
            this.txtLoginUsername.Location = new System.Drawing.Point(20, 55);
            this.txtLoginUsername.Name = "txtLoginUsername";
            this.txtLoginUsername.Size = new System.Drawing.Size(350, 27);
            this.txtLoginUsername.TabIndex = 1;
            // 
            // lblLoginUsername
            // 
            this.lblLoginUsername.AutoSize = true;
            this.lblLoginUsername.Location = new System.Drawing.Point(20, 30);
            this.lblLoginUsername.Name = "lblLoginUsername";
            this.lblLoginUsername.Size = new System.Drawing.Size(75, 20);
            this.lblLoginUsername.TabIndex = 0;
            this.lblLoginUsername.Text = "Username:";
            // 
            // tabPageRegister
            // 
            this.tabPageRegister.Controls.Add(this.btnRegister);
            this.tabPageRegister.Controls.Add(this.txtRegisterEmail);
            this.tabPageRegister.Controls.Add(this.lblRegisterEmail);
            this.tabPageRegister.Controls.Add(this.txtRegisterConfirmPassword);
            this.tabPageRegister.Controls.Add(this.lblRegisterConfirmPassword);
            this.tabPageRegister.Controls.Add(this.txtRegisterPassword);
            this.tabPageRegister.Controls.Add(this.lblRegisterPassword);
            this.tabPageRegister.Controls.Add(this.txtRegisterUsername);
            this.tabPageRegister.Controls.Add(this.lblRegisterUsername);
            this.tabPageRegister.Location = new System.Drawing.Point(4, 29);
            this.tabPageRegister.Name = "tabPageRegister";
            this.tabPageRegister.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRegister.Size = new System.Drawing.Size(392, 287);
            this.tabPageRegister.TabIndex = 1;
            this.tabPageRegister.Text = "Đăng ký";
            this.tabPageRegister.UseVisualStyleBackColor = true;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(20, 240);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(350, 45);
            this.btnRegister.TabIndex = 8;
            this.btnRegister.Text = "Đăng ký";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // txtRegisterEmail
            // 
            this.txtRegisterEmail.Location = new System.Drawing.Point(20, 200);
            this.txtRegisterEmail.Name = "txtRegisterEmail";
            this.txtRegisterEmail.Size = new System.Drawing.Size(350, 27);
            this.txtRegisterEmail.TabIndex = 7;
            // 
            // lblRegisterEmail
            // 
            this.lblRegisterEmail.AutoSize = true;
            this.lblRegisterEmail.Location = new System.Drawing.Point(20, 175);
            this.lblRegisterEmail.Name = "lblRegisterEmail";
            this.lblRegisterEmail.Size = new System.Drawing.Size(49, 20);
            this.lblRegisterEmail.TabIndex = 6;
            this.lblRegisterEmail.Text = "Email:";
            // 
            // txtRegisterConfirmPassword
            // 
            this.txtRegisterConfirmPassword.Location = new System.Drawing.Point(20, 145);
            this.txtRegisterConfirmPassword.Name = "txtRegisterConfirmPassword";
            this.txtRegisterConfirmPassword.PasswordChar = '*';
            this.txtRegisterConfirmPassword.Size = new System.Drawing.Size(350, 27);
            this.txtRegisterConfirmPassword.TabIndex = 5;
            // 
            // lblRegisterConfirmPassword
            // 
            this.lblRegisterConfirmPassword.AutoSize = true;
            this.lblRegisterConfirmPassword.Location = new System.Drawing.Point(20, 120);
            this.lblRegisterConfirmPassword.Name = "lblRegisterConfirmPassword";
            this.lblRegisterConfirmPassword.Size = new System.Drawing.Size(140, 20);
            this.lblRegisterConfirmPassword.TabIndex = 4;
            this.lblRegisterConfirmPassword.Text = "Xác nhận Password:";
            // 
            // txtRegisterPassword
            // 
            this.txtRegisterPassword.Location = new System.Drawing.Point(20, 95);
            this.txtRegisterPassword.Name = "txtRegisterPassword";
            this.txtRegisterPassword.PasswordChar = '*';
            this.txtRegisterPassword.Size = new System.Drawing.Size(350, 27);
            this.txtRegisterPassword.TabIndex = 3;
            // 
            // lblRegisterPassword
            // 
            this.lblRegisterPassword.AutoSize = true;
            this.lblRegisterPassword.Location = new System.Drawing.Point(20, 70);
            this.lblRegisterPassword.Name = "lblRegisterPassword";
            this.lblRegisterPassword.Size = new System.Drawing.Size(73, 20);
            this.lblRegisterPassword.TabIndex = 2;
            this.lblRegisterPassword.Text = "Password:";
            // 
            // txtRegisterUsername
            // 
            this.txtRegisterUsername.Location = new System.Drawing.Point(20, 45);
            this.txtRegisterUsername.Name = "txtRegisterUsername";
            this.txtRegisterUsername.Size = new System.Drawing.Size(350, 27);
            this.txtRegisterUsername.TabIndex = 1;
            // 
            // lblRegisterUsername
            // 
            this.lblRegisterUsername.AutoSize = true;
            this.lblRegisterUsername.Location = new System.Drawing.Point(20, 20);
            this.lblRegisterUsername.Name = "lblRegisterUsername";
            this.lblRegisterUsername.Size = new System.Drawing.Size(75, 20);
            this.lblRegisterUsername.TabIndex = 0;
            this.lblRegisterUsername.Text = "Username:";
            // 
            // lblServerIP
            // 
            this.lblServerIP.AutoSize = true;
            this.lblServerIP.Location = new System.Drawing.Point(12, 20);
            this.lblServerIP.Name = "lblServerIP";
            this.lblServerIP.Size = new System.Drawing.Size(70, 20);
            this.lblServerIP.TabIndex = 1;
            this.lblServerIP.Text = "Server IP:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(88, 17);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(160, 27);
            this.txtServerIP.TabIndex = 2;
            this.txtServerIP.Text = "127.0.0.1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(254, 16);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 30);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 60);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(79, 20);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Chưa kết nối";
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Location = new System.Drawing.Point(12, 80);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(0, 20);
            this.lblUserInfo.TabIndex = 5;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 430);
            this.Controls.Add(this.lblUserInfo);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.lblServerIP);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập - GameCaro";
            this.tabControl1.ResumeLayout(false);
            this.tabPageLogin.ResumeLayout(false);
            this.tabPageLogin.PerformLayout();
            this.tabPageRegister.ResumeLayout(false);
            this.tabPageRegister.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
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
