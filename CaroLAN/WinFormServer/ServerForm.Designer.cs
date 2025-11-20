namespace WinFormServer
{
    partial class ServerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtLog = new TextBox();
            lstClients = new ListBox();
            btnStart = new Button();
            btnStop = new Button();
            lblStatus = new Label();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            groupBoxControl = new GroupBox();
            lblStatusTitle = new Label();
            groupBoxLog = new GroupBox();
            groupBoxClients = new GroupBox();
            groupBoxControl.SuspendLayout();
            groupBoxLog.SuspendLayout();
            groupBoxClients.SuspendLayout();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.WhiteSmoke;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(15, 25);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(530, 370);
            txtLog.TabIndex = 0;
            txtLog.TextChanged += txtLog_TextChanged;
            // 
            // lstClients
            // 
            lstClients.BackColor = Color.WhiteSmoke;
            lstClients.Font = new Font("Segoe UI", 9F);
            lstClients.FormattingEnabled = true;
            lstClients.Location = new Point(15, 60);
            lstClients.Name = "lstClients";
            lstClients.ScrollAlwaysVisible = true;
            lstClients.Size = new Size(330, 384);
            lstClients.TabIndex = 1;
            lstClients.SelectedIndexChanged += lstClients_SelectedIndexChanged;
            // 
            // btnStart
            // 
            btnStart.BackColor = SystemColors.ButtonHighlight;
            btnStart.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStart.Location = new Point(20, 30);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(150, 35);
            btnStart.TabIndex = 2;
            btnStart.Text = "Bật Server";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = SystemColors.ButtonHighlight;
            btnStop.Enabled = false;
            btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStop.Location = new Point(190, 30);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(150, 35);
            btnStop.TabIndex = 3;
            btnStop.Text = "Dừng Server";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.DarkBlue;
            lblStatus.Location = new Point(110, 65);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(18, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(15, 30);
            label1.Name = "label1";
            label1.Size = new Size(129, 20);
            label1.TabIndex = 5;
            label1.Text = "Danh sách Client:";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ButtonHighlight;
            button1.Enabled = false;
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button1.Location = new Point(15, 460);
            button1.Name = "button1";
            button1.Size = new Size(150, 35);
            button1.TabIndex = 6;
            button1.Text = "Làm mới";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ButtonHighlight;
            button2.Enabled = false;
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button2.Location = new Point(195, 460);
            button2.Name = "button2";
            button2.Size = new Size(150, 35);
            button2.TabIndex = 7;
            button2.Text = "Ngắt kết nối";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // groupBoxControl
            // 
            groupBoxControl.Controls.Add(lblStatusTitle);
            groupBoxControl.Controls.Add(lblStatus);
            groupBoxControl.Controls.Add(btnStart);
            groupBoxControl.Controls.Add(btnStop);
            groupBoxControl.Location = new Point(20, 20);
            groupBoxControl.Name = "groupBoxControl";
            groupBoxControl.Size = new Size(560, 100);
            groupBoxControl.TabIndex = 8;
            groupBoxControl.TabStop = false;
            groupBoxControl.Text = "Điều khiển Server";
            // 
            // lblStatusTitle
            // 
            lblStatusTitle.AutoSize = true;
            lblStatusTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatusTitle.Location = new Point(20, 65);
            lblStatusTitle.Name = "lblStatusTitle";
            lblStatusTitle.Size = new Size(84, 20);
            lblStatusTitle.TabIndex = 8;
            lblStatusTitle.Text = "Trạng thái:";
            // 
            // groupBoxLog
            // 
            groupBoxLog.Controls.Add(txtLog);
            groupBoxLog.Location = new Point(20, 130);
            groupBoxLog.Name = "groupBoxLog";
            groupBoxLog.Size = new Size(560, 410);
            groupBoxLog.TabIndex = 9;
            groupBoxLog.TabStop = false;
            groupBoxLog.Text = "Nhật ký hoạt động";
            // 
            // groupBoxClients
            // 
            groupBoxClients.Controls.Add(button2);
            groupBoxClients.Controls.Add(button1);
            groupBoxClients.Controls.Add(lstClients);
            groupBoxClients.Controls.Add(label1);
            groupBoxClients.Location = new Point(600, 20);
            groupBoxClients.Name = "groupBoxClients";
            groupBoxClients.Size = new Size(360, 520);
            groupBoxClients.TabIndex = 10;
            groupBoxClients.TabStop = false;
            groupBoxClients.Text = "Quản lý Client";
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(980, 560);
            Controls.Add(groupBoxClients);
            Controls.Add(groupBoxLog);
            Controls.Add(groupBoxControl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ServerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Caro - Server Manager";
            groupBoxControl.ResumeLayout(false);
            groupBoxControl.PerformLayout();
            groupBoxLog.ResumeLayout(false);
            groupBoxLog.PerformLayout();
            groupBoxClients.ResumeLayout(false);
            groupBoxClients.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtLog;
        private ListBox lstClients;
        private Button btnStart;
        private Button btnStop;
        private Label lblStatus;
        private Label label1;
        private Button button1;
        private Button button2;
        private GroupBox groupBoxControl;
        private GroupBox groupBoxLog;
        private GroupBox groupBoxClients;
        private Label lblStatusTitle;
    }
}
