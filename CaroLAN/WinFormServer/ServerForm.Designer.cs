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
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new Point(40, 110);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(343, 344);
            txtLog.TabIndex = 0;
            txtLog.TextChanged += txtLog_TextChanged;
            // 
            // lstClients
            // 
            lstClients.FormattingEnabled = true;
            lstClients.Location = new Point(478, 110);
            lstClients.Name = "lstClients";
            lstClients.Size = new Size(282, 344);
            lstClients.TabIndex = 1;
            lstClients.SelectedIndexChanged += lstClients_SelectedIndexChanged;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(40, 28);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(150, 29);
            btnStart.TabIndex = 2;
            btnStart.Text = "Bat dau Server";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(207, 28);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(147, 29);
            btnStop.TabIndex = 3;
            btnStop.Text = "Dung Server";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(403, 36);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(18, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(478, 87);
            label1.Name = "label1";
            label1.Size = new Size(117, 20);
            label1.TabIndex = 5;
            label1.Text = "Client da ket noi";
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 494);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(lstClients);
            Controls.Add(txtLog);
            Name = "ServerForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtLog;
        private ListBox lstClients;
        private Button btnStart;
        private Button btnStop;
        private Label lblStatus;
        private Label label1;
    }
}
