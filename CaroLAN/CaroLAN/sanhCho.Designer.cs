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
            tabHistory.SuspendLayout();
            tabMyHistory.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(286, 38);
            label1.Name = "label1";
            label1.Size = new Size(189, 20);
            label1.TabIndex = 9;
            label1.Text = "Nguoi choi dang truc tuyen";
            // 
            // lstClients
            // 
            lstClients.FormattingEnabled = true;
            lstClients.Location = new Point(286, 61);
            lstClients.Name = "lstClients";
            lstClients.ScrollAlwaysVisible = true;
            lstClients.Size = new Size(272, 300);
            lstClients.TabIndex = 8;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(33, 61);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 40);
            btnConnect.TabIndex = 10;
            btnConnect.Text = "Kết nối";
            btnConnect.Click += btnConnect_Click_1;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(33, 31);
            txtIP.Name = "txtIP";
            txtIP.ReadOnly = true;
            txtIP.Size = new Size(120, 27);
            txtIP.TabIndex = 11;
            txtIP.Text = "127.0.0.1";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(33, 111);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(165, 20);
            lblStatus.TabIndex = 12;
            lblStatus.Text = "Trạng thái: Chưa kết nối";
            // 
            // button3
            // 
            button3.Location = new Point(78, 251);
            button3.Name = "button3";
            button3.Size = new Size(120, 44);
            button3.TabIndex = 15;
            button3.Text = "Bat dau";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(63, 309);
            label2.Name = "label2";
            label2.Size = new Size(158, 20);
            label2.TabIndex = 16;
            label2.Text = "Vao phong ngau nhien";
            // 
            // btnRequest
            // 
            btnRequest.Location = new Point(286, 370);
            btnRequest.Name = "btnRequest";
            btnRequest.Size = new Size(120, 35);
            btnRequest.TabIndex = 17;
            btnRequest.Text = "Mời chơi";
            btnRequest.UseVisualStyleBackColor = true;
            btnRequest.Click += btnRequest_Click;
            // 
            // lstRequests
            // 
            lstRequests.FormattingEnabled = true;
            lstRequests.Location = new Point(614, 61);
            lstRequests.Name = "lstRequests";
            lstRequests.ScrollAlwaysVisible = true;
            lstRequests.Size = new Size(272, 300);
            lstRequests.TabIndex = 18;
            lstRequests.SelectedIndexChanged += lstRequests_SelectedIndexChanged;
            // 
            // btnAccept
            // 
            btnAccept.Location = new Point(766, 370);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(120, 35);
            btnAccept.TabIndex = 19;
            btnAccept.Text = "Chấp nhận";
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.Click += btnAccept_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(614, 31);
            label3.Name = "label3";
            label3.Size = new Size(224, 20);
            label3.TabIndex = 20;
            label3.Text = "Loi moi so tai tu nguoi choi khac";
            // 
            // tabHistory
            // 
            tabHistory.Controls.Add(tabMyHistory);
            tabHistory.Location = new Point(33, 410);
            tabHistory.Name = "tabHistory";
            tabHistory.SelectedIndex = 0;
            tabHistory.Size = new Size(853, 130);
            tabHistory.TabIndex = 21;
            // 
            // tabMyHistory
            // 
            tabMyHistory.Controls.Add(lstMyHistory);
            tabMyHistory.Controls.Add(btnRefreshMy);
            tabMyHistory.Location = new Point(4, 29);
            tabMyHistory.Name = "tabMyHistory";
            tabMyHistory.Padding = new Padding(3);
            tabMyHistory.Size = new Size(845, 97);
            tabMyHistory.TabIndex = 1;
            tabMyHistory.Text = "Lịch sử của tôi";
            tabMyHistory.UseVisualStyleBackColor = true;
            // 
            // lstMyHistory
            // 
            lstMyHistory.FormattingEnabled = true;
            lstMyHistory.Location = new Point(6, 6);
            lstMyHistory.Name = "lstMyHistory";
            lstMyHistory.Size = new Size(733, 91);
            lstMyHistory.TabIndex = 0;
            // 
            // btnRefreshMy
            // 
            btnRefreshMy.Location = new Point(745, 6);
            btnRefreshMy.Name = "btnRefreshMy";
            btnRefreshMy.Size = new Size(94, 29);
            btnRefreshMy.TabIndex = 1;
            btnRefreshMy.Text = "Làm mới";
            btnRefreshMy.UseVisualStyleBackColor = true;
            btnRefreshMy.Click += btnRefreshMy_Click;
            // 
            // sanhCho
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(929, 545);
            Controls.Add(tabHistory);
            Controls.Add(label3);
            Controls.Add(btnAccept);
            Controls.Add(lstRequests);
            Controls.Add(btnRequest);
            Controls.Add(label2);
            Controls.Add(button3);
            Controls.Add(btnConnect);
            Controls.Add(txtIP);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Controls.Add(lstClients);
            Name = "sanhCho";
            Text = "sanhCho";
            Load += sanhCho_Load;
            tabHistory.ResumeLayout(false);
            tabMyHistory.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox lstClients;
        private Button btnConnect;
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