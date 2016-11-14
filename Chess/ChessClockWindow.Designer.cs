namespace Chess
{
    partial class ChessClockWindow
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
            this.components = new System.ComponentModel.Container();
            this.label_turn = new System.Windows.Forms.Label();
            this.label_curTime = new System.Windows.Forms.Label();
            this.label_othTime = new System.Windows.Forms.Label();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label_turn
            // 
            this.label_turn.AutoSize = true;
            this.label_turn.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_turn.Location = new System.Drawing.Point(12, 9);
            this.label_turn.Name = "label_turn";
            this.label_turn.Size = new System.Drawing.Size(236, 42);
            this.label_turn.TabIndex = 0;
            this.label_turn.Text = "White Move:";
            this.label_turn.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label_curTime
            // 
            this.label_curTime.AutoSize = true;
            this.label_curTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_curTime.Location = new System.Drawing.Point(287, 9);
            this.label_curTime.Name = "label_curTime";
            this.label_curTime.Size = new System.Drawing.Size(150, 42);
            this.label_curTime.TabIndex = 1;
            this.label_curTime.Text = "1:00:00";
            this.label_curTime.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label_othTime
            // 
            this.label_othTime.AutoSize = true;
            this.label_othTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_othTime.Location = new System.Drawing.Point(290, 75);
            this.label_othTime.Name = "label_othTime";
            this.label_othTime.Size = new System.Drawing.Size(69, 20);
            this.label_othTime.TabIndex = 2;
            this.label_othTime.Text = "1:00:00";
            this.label_othTime.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            // 
            // ChessClockWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 111);
            this.ControlBox = false;
            this.Controls.Add(this.label_othTime);
            this.Controls.Add(this.label_curTime);
            this.Controls.Add(this.label_turn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChessClockWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ChessClock";
            this.Load += new System.EventHandler(this.ChessClockWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_turn;
        private System.Windows.Forms.Label label_curTime;
        private System.Windows.Forms.Label label_othTime;
        private System.Windows.Forms.Timer refreshTimer;
    }
}