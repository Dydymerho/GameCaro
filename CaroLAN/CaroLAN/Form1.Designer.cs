namespace CaroLAN
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlChessBoard;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnResign;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Label lblGameTitle;
        private System.Windows.Forms.Panel pnlBoardContainer;
        private System.Windows.Forms.Label lblPlayerInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlPlayerX;
        private System.Windows.Forms.Label lblPlayerX;
        private System.Windows.Forms.Label lblPlayerXStatus;
        private System.Windows.Forms.Panel pnlPlayerO;
        private System.Windows.Forms.Label lblPlayerO;
        private System.Windows.Forms.Label lblPlayerOStatus;
        private System.Windows.Forms.PictureBox picPlayerX;
        private System.Windows.Forms.PictureBox picPlayerO;
        private System.Windows.Forms.Panel pnlChat;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.TextBox txtChatInput;
        private System.Windows.Forms.Button btnSendChat;

        /// <summary>
        ///  Dọn tài nguyên.
        /// </summary>
        /// <param name="disposing">true nếu muốn giải phóng tài nguyên.</param>
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
        ///  Thiết lập giao diện game caro online theo phong cách gaming
        /// </summary>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblGameTitle = new Label();
            lblRoom = new Label();
            pnlBoardContainer = new Panel();
            pnlChessBoard = new Panel();
            pnlChat = new Panel();
            rtbChat = new RichTextBox();
            txtChatInput = new TextBox();
            btnSendChat = new Button();
            pnlSidebar = new Panel();
            lblStatus = new Label();
            pnlPlayerX = new Panel();
            picPlayerX = new PictureBox();
            lblPlayerX = new Label();
            lblPlayerXStatus = new Label();
            pnlPlayerO = new Panel();
            picPlayerO = new PictureBox();
            lblPlayerO = new Label();
            lblPlayerOStatus = new Label();
            lblTimer = new Label();
            btnResign = new Button();
            pnlBoardContainer.SuspendLayout();
            pnlChat.SuspendLayout();
            pnlSidebar.SuspendLayout();
            pnlPlayerX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerX).BeginInit();
            pnlPlayerO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerO).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Location = new Point(11, 11);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(176, 54);
            pnlHeader.TabIndex = 4;
            pnlHeader.Paint += pnlHeader_Paint;
            // 
            // lblGameTitle
            // 
            lblGameTitle.Location = new Point(0, 0);
            lblGameTitle.Name = "lblGameTitle";
            lblGameTitle.Size = new Size(100, 23);
            lblGameTitle.TabIndex = 0;
            // 
            // lblRoom
            // 
            lblRoom.Location = new Point(0, 0);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(100, 23);
            lblRoom.TabIndex = 0;
            // 
            // pnlBoardContainer
            // 
            pnlBoardContainer.Controls.Add(pnlChessBoard);
            pnlBoardContainer.Location = new Point(287, 12);
            pnlBoardContainer.Name = "pnlBoardContainer";
            pnlBoardContainer.Padding = new Padding(15);
            pnlBoardContainer.Size = new Size(494, 490);
            pnlBoardContainer.TabIndex = 1;
            pnlBoardContainer.Paint += pnlBoardContainer_Paint;
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = Color.FromArgb(250, 250, 250);
            pnlChessBoard.BorderStyle = BorderStyle.FixedSingle;
            pnlChessBoard.Dock = DockStyle.Fill;
            pnlChessBoard.Location = new Point(15, 15);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(464, 460);
            pnlChessBoard.TabIndex = 0;
            // 
            // pnlChat
            // 
            pnlChat.BackColor = Color.FromArgb(245, 245, 245);
            pnlChat.BorderStyle = BorderStyle.FixedSingle;
            pnlChat.Controls.Add(pnlHeader);
            pnlChat.Controls.Add(rtbChat);
            pnlChat.Controls.Add(txtChatInput);
            pnlChat.Controls.Add(btnSendChat);
            pnlChat.Location = new Point(787, 12);
            pnlChat.Name = "pnlChat";
            pnlChat.Padding = new Padding(8);
            pnlChat.Size = new Size(200, 490);
            pnlChat.TabIndex = 3;
            // 
            // rtbChat
            // 
            rtbChat.BackColor = Color.White;
            rtbChat.BorderStyle = BorderStyle.FixedSingle;
            rtbChat.Location = new Point(11, 85);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(176, 354);
            rtbChat.TabIndex = 0;
            rtbChat.Text = "";
            // 
            // txtChatInput
            // 
            txtChatInput.Location = new Point(17, 448);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.Size = new Size(124, 27);
            txtChatInput.TabIndex = 1;
            // 
            // btnSendChat
            // 
            btnSendChat.Location = new Point(147, 446);
            btnSendChat.Name = "btnSendChat";
            btnSendChat.Size = new Size(40, 28);
            btnSendChat.TabIndex = 2;
            btnSendChat.Text = "→";
            btnSendChat.UseVisualStyleBackColor = true;
            btnSendChat.Click += btnSendChat_Click;
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.WhiteSmoke;
            pnlSidebar.Controls.Add(lblStatus);
            pnlSidebar.Controls.Add(pnlPlayerX);
            pnlSidebar.Controls.Add(pnlPlayerO);
            pnlSidebar.Controls.Add(lblTimer);
            pnlSidebar.Controls.Add(btnResign);
            pnlSidebar.Location = new Point(12, 12);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(269, 490);
            pnlSidebar.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(70, 130, 180);
            lblStatus.Location = new Point(9, 19);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(250, 30);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "⚔️ NGƯỜI CHƠI";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlPlayerX
            // 
            pnlPlayerX.BackColor = Color.White;
            pnlPlayerX.BorderStyle = BorderStyle.FixedSingle;
            pnlPlayerX.Controls.Add(picPlayerX);
            pnlPlayerX.Controls.Add(lblPlayerX);
            pnlPlayerX.Controls.Add(lblPlayerXStatus);
            pnlPlayerX.Location = new Point(9, 59);
            pnlPlayerX.Name = "pnlPlayerX";
            pnlPlayerX.Size = new Size(250, 100);
            pnlPlayerX.TabIndex = 1;
            // 
            // picPlayerX
            // 
            picPlayerX.BackColor = Color.FromArgb(70, 130, 180);
            picPlayerX.Location = new Point(15, 15);
            picPlayerX.Name = "picPlayerX";
            picPlayerX.Size = new Size(70, 70);
            picPlayerX.TabIndex = 0;
            picPlayerX.TabStop = false;
            // 
            // lblPlayerX
            // 
            lblPlayerX.AutoSize = true;
            lblPlayerX.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlayerX.ForeColor = Color.FromArgb(70, 130, 180);
            lblPlayerX.Location = new Point(100, 15);
            lblPlayerX.Name = "lblPlayerX";
            lblPlayerX.Size = new Size(108, 32);
            lblPlayerX.TabIndex = 1;
            lblPlayerX.Text = "Player X";
            // 
            // lblPlayerXStatus
            // 
            lblPlayerXStatus.Font = new Font("Segoe UI", 10F);
            lblPlayerXStatus.ForeColor = Color.Gray;
            lblPlayerXStatus.Location = new Point(100, 50);
            lblPlayerXStatus.Name = "lblPlayerXStatus";
            lblPlayerXStatus.Size = new Size(140, 35);
            lblPlayerXStatus.TabIndex = 2;
            lblPlayerXStatus.Text = "⏳ Đang chờ...";
            // 
            // pnlPlayerO
            // 
            pnlPlayerO.BackColor = Color.White;
            pnlPlayerO.BorderStyle = BorderStyle.FixedSingle;
            pnlPlayerO.Controls.Add(picPlayerO);
            pnlPlayerO.Controls.Add(lblPlayerO);
            pnlPlayerO.Controls.Add(lblPlayerOStatus);
            pnlPlayerO.Location = new Point(9, 181);
            pnlPlayerO.Name = "pnlPlayerO";
            pnlPlayerO.Size = new Size(250, 100);
            pnlPlayerO.TabIndex = 2;
            // 
            // picPlayerO
            // 
            picPlayerO.BackColor = Color.FromArgb(220, 20, 60);
            picPlayerO.Location = new Point(15, 15);
            picPlayerO.Name = "picPlayerO";
            picPlayerO.Size = new Size(70, 70);
            picPlayerO.TabIndex = 0;
            picPlayerO.TabStop = false;
            // 
            // lblPlayerO
            // 
            lblPlayerO.AutoSize = true;
            lblPlayerO.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlayerO.ForeColor = Color.FromArgb(220, 20, 60);
            lblPlayerO.Location = new Point(100, 15);
            lblPlayerO.Name = "lblPlayerO";
            lblPlayerO.Size = new Size(110, 32);
            lblPlayerO.TabIndex = 1;
            lblPlayerO.Text = "Player O";
            // 
            // lblPlayerOStatus
            // 
            lblPlayerOStatus.Font = new Font("Segoe UI", 10F);
            lblPlayerOStatus.ForeColor = Color.Gray;
            lblPlayerOStatus.Location = new Point(100, 50);
            lblPlayerOStatus.Name = "lblPlayerOStatus";
            lblPlayerOStatus.Size = new Size(140, 35);
            lblPlayerOStatus.TabIndex = 2;
            lblPlayerOStatus.Text = "⏳ Đang chờ...";
            // 
            // lblTimer
            // 
            lblTimer.BackColor = Color.White;
            lblTimer.BorderStyle = BorderStyle.FixedSingle;
            lblTimer.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTimer.ForeColor = Color.FromArgb(70, 130, 180);
            lblTimer.Location = new Point(58, 296);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(160, 80);
            lblTimer.TabIndex = 3;
            lblTimer.Text = "⏰ --";
            lblTimer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnResign
            // 
            btnResign.BackColor = Color.FromArgb(220, 20, 60);
            btnResign.Cursor = Cursors.Hand;
            btnResign.FlatAppearance.BorderSize = 0;
            btnResign.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 20, 50);
            btnResign.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 20, 55);
            btnResign.FlatStyle = FlatStyle.Flat;
            btnResign.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnResign.ForeColor = Color.White;
            btnResign.Location = new Point(32, 430);
            btnResign.Name = "btnResign";
            btnResign.Size = new Size(209, 50);
            btnResign.TabIndex = 4;
            btnResign.Text = "🏳️ ĐẦU HÀNG";
            btnResign.UseVisualStyleBackColor = false;
            btnResign.Click += btnResign_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(991, 517);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlBoardContainer);
            Controls.Add(pnlChat);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "🎮 Caro Battle - Gaming Arena";
            Load += Form1_Load;
            pnlBoardContainer.ResumeLayout(false);
            pnlChat.ResumeLayout(false);
            pnlChat.PerformLayout();
            pnlSidebar.ResumeLayout(false);
            pnlPlayerX.ResumeLayout(false);
            pnlPlayerX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerX).EndInit();
            pnlPlayerO.ResumeLayout(false);
            pnlPlayerO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerO).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
