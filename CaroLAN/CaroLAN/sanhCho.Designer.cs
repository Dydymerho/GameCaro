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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(516, 53);
            label1.Name = "label1";
            label1.Size = new Size(117, 20);
            label1.TabIndex = 9;
            label1.Text = "Client da ket noi";
            // 
            // lstClients
            // 
            lstClients.FormattingEnabled = true;
            lstClients.Location = new Point(516, 76);
            lstClients.Name = "lstClients";
            lstClients.Size = new Size(272, 344);
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
            // sanhCho
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 450);
            Controls.Add(btnConnect);
            Controls.Add(txtIP);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Controls.Add(lstClients);
            Name = "sanhCho";
            Text = "sanhCho";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox lstClients;
        private Button btnConnect;
        private TextBox txtIP;
        private Label lblStatus;
    }
}