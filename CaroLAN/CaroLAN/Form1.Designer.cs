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
            pnlHeader.SuspendLayout();
            pnlBoardContainer.SuspendLayout();
            pnlSidebar.SuspendLayout();
            pnlPlayerX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerX).BeginInit();
            pnlPlayerO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayerO).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(41, 128, 185);
            pnlHeader.Controls.Add(lblGameTitle);
            pnlHeader.Controls.Add(lblRoom);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(900, 80);
            pnlHeader.TabIndex = 0;
            // 
            // lblGameTitle
            // 
            lblGameTitle.AutoSize = true;
            lblGameTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblGameTitle.ForeColor = Color.White;
            lblGameTitle.Location = new Point(20, 15);
            lblGameTitle.Name = "lblGameTitle";
            lblGameTitle.Size = new Size(278, 46);
            lblGameTitle.TabIndex = 0;
            lblGameTitle.Text = "🎮 GAME CARO";
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRoom.ForeColor = Color.White;
            lblRoom.Location = new Point(700, 28);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(107, 28);
            lblRoom.TabIndex = 1;
            lblRoom.Text = "Phòng: ---";
            // 
            // pnlBoardContainer
            // 
            pnlBoardContainer.BackColor = Color.White;
            pnlBoardContainer.Controls.Add(pnlChat);
            pnlBoardContainer.Controls.Add(pnlChessBoard);
            pnlBoardContainer.Location = new Point(30, 110);
            pnlBoardContainer.Name = "pnlBoardContainer";
            pnlBoardContainer.Padding = new Padding(15);
            pnlBoardContainer.Size = new Size(530, 530);
            pnlBoardContainer.TabIndex = 1;
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = Color.FromArgb(250, 250, 250);
            pnlChessBoard.BorderStyle = BorderStyle.FixedSingle;
            pnlChessBoard.Dock = DockStyle.Fill;
            pnlChessBoard.Location = new Point(15, 15);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(500, 500);
            pnlChessBoard.TabIndex = 0;
            // 
            // pnlChat
            // 
            pnlChat.BackColor = Color.FromArgb(245, 245, 245);
            pnlChat.BorderStyle = BorderStyle.FixedSingle;
            pnlChat.Dock = DockStyle.Right;
            pnlChat.Location = new Point(515, 15);
            pnlChat.Name = "pnlChat";
            pnlChat.Size = new Size(200, 500);
            pnlChat.TabIndex = 1;
            pnlChat.Padding = new Padding(8);
            // 
            // rtbChat
            // 
            rtbChat.Dock = DockStyle.Top;
            rtbChat.Height = 380;
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.White;
            rtbChat.BorderStyle = BorderStyle.FixedSingle;
            rtbChat.Name = "rtbChat";
            rtbChat.TabIndex = 0;
            // 
            // txtChatInput
            // 
            txtChatInput.Location = new Point(12, 400);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.Size = new Size(140, 24);
            txtChatInput.TabIndex = 1;
            // 
            // btnSendChat
            // 
            btnSendChat.Location = new Point(158, 398);
            btnSendChat.Name = "btnSendChat";
            btnSendChat.Size = new Size(28, 28);
            btnSendChat.TabIndex = 2;
            btnSendChat.Text = "→";
            btnSendChat.UseVisualStyleBackColor = true;
            btnSendChat.Click += btnSendChat_Click;
            
            pnlChat.Controls.Add(rtbChat);
            pnlChat.Controls.Add(txtChatInput);
            pnlChat.Controls.Add(btnSendChat);
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.WhiteSmoke;
            pnlSidebar.Controls.Add(lblStatus);
            pnlSidebar.Controls.Add(pnlPlayerX);
            pnlSidebar.Controls.Add(pnlPlayerO);
            pnlSidebar.Controls.Add(lblTimer);
            pnlSidebar.Controls.Add(btnResign);
            pnlSidebar.Location = new Point(580, 110);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(290, 530);
            pnlSidebar.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(70, 130, 180);
            lblStatus.Location = new Point(20, 20);
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
            pnlPlayerX.Location = new Point(20, 60);
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
            pnlPlayerO.Location = new Point(20, 180);
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
            lblTimer.Location = new Point(20, 300);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(250, 80);
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
            btnResign.Location = new Point(20, 420);
            btnResign.Name = "btnResign";
            btnResign.Size = new Size(250, 60);
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
            ClientSize = new Size(900, 670);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlBoardContainer);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "🎮 Caro Battle - Gaming Arena";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBoardContainer.ResumeLayout(false);
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
