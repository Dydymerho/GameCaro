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
        ///  Thiết lập giao diện game caro online
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlChessBoard = new System.Windows.Forms.Panel();
            this.lblRoom = new System.Windows.Forms.Label();
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnResign = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // 
            // pnlChessBoard
            // 
            this.pnlChessBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChessBoard.Location = new System.Drawing.Point(25, 60);
            this.pnlChessBoard.Name = "pnlChessBoard";
            this.pnlChessBoard.Size = new System.Drawing.Size(450, 450);
            this.pnlChessBoard.TabIndex = 0;

            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblRoom.Location = new System.Drawing.Point(25, 20);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(93, 23);
            this.lblRoom.TabIndex = 1;
            this.lblRoom.Text = "Phòng: ---";

            // 
            // lblTurn
            // 
            this.lblTurn.AutoSize = true;
            this.lblTurn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTurn.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblTurn.Location = new System.Drawing.Point(230, 20);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(132, 23);
            this.lblTurn.TabIndex = 2;
            this.lblTurn.Text = "Lượt: Đang xử lý";

            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTimer.ForeColor = System.Drawing.Color.DarkRed;
            this.lblTimer.Location = new System.Drawing.Point(400, 20);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(88, 23);
            this.lblTimer.TabIndex = 3;
            this.lblTimer.Text = "Thời gian";

            // 
            // btnResign
            // 
            this.btnResign.BackColor = System.Drawing.Color.LightCoral;
            this.btnResign.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnResign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnResign.ForeColor = System.Drawing.Color.White;
            this.btnResign.Location = new System.Drawing.Point(500, 60);
            this.btnResign.Name = "btnResign";
            this.btnResign.Size = new System.Drawing.Size(100, 40);
            this.btnResign.TabIndex = 4;
            this.btnResign.Text = "Đầu hàng";
            this.btnResign.UseVisualStyleBackColor = false;
            this.btnResign.Click += new System.EventHandler(this.btnResign_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(630, 540);
            this.Controls.Add(this.btnResign);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.pnlChessBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caro LAN - Phòng đấu";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
