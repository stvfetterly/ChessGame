namespace Chess.Networking
{
    partial class NetworkingWindow
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtIPAddress4 = new System.Windows.Forms.MaskedTextBox();
            this.txtIPAddress3 = new System.Windows.Forms.MaskedTextBox();
            this.txtIPAddress2 = new System.Windows.Forms.MaskedTextBox();
            this.txtPortNum = new System.Windows.Forms.MaskedTextBox();
            this.txtIPAddress1 = new System.Windows.Forms.MaskedTextBox();
            this.buttStartStop = new System.Windows.Forms.Button();
            this.radioJoin = new System.Windows.Forms.RadioButton();
            this.radioHost = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMain = new System.Windows.Forms.RichTextBox();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.buttSend = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.processingTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtIPAddress4);
            this.groupBox1.Controls.Add(this.txtIPAddress3);
            this.groupBox1.Controls.Add(this.txtIPAddress2);
            this.groupBox1.Controls.Add(this.txtPortNum);
            this.groupBox1.Controls.Add(this.txtIPAddress1);
            this.groupBox1.Controls.Add(this.buttStartStop);
            this.groupBox1.Controls.Add(this.radioJoin);
            this.groupBox1.Controls.Add(this.radioHost);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(414, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 158);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ntework Settings";
            // 
            // txtIPAddress4
            // 
            this.txtIPAddress4.Location = new System.Drawing.Point(126, 22);
            this.txtIPAddress4.Name = "txtIPAddress4";
            this.txtIPAddress4.Size = new System.Drawing.Size(28, 20);
            this.txtIPAddress4.TabIndex = 10;
            this.txtIPAddress4.Text = "1";
            this.txtIPAddress4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress4_keydown);
            this.txtIPAddress4.Leave += new System.EventHandler(this.txtIPAddress4_leave);
            // 
            // txtIPAddress3
            // 
            this.txtIPAddress3.Location = new System.Drawing.Point(92, 22);
            this.txtIPAddress3.Name = "txtIPAddress3";
            this.txtIPAddress3.Size = new System.Drawing.Size(28, 20);
            this.txtIPAddress3.TabIndex = 9;
            this.txtIPAddress3.Text = "0";
            this.txtIPAddress3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress3_keydown);
            this.txtIPAddress3.Leave += new System.EventHandler(this.txtIPAddress3_leave);
            // 
            // txtIPAddress2
            // 
            this.txtIPAddress2.Location = new System.Drawing.Point(58, 22);
            this.txtIPAddress2.Name = "txtIPAddress2";
            this.txtIPAddress2.Size = new System.Drawing.Size(28, 20);
            this.txtIPAddress2.TabIndex = 8;
            this.txtIPAddress2.Text = "0";
            this.txtIPAddress2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress2_keydown);
            this.txtIPAddress2.Leave += new System.EventHandler(this.txtIPAddress2_leave);
            // 
            // txtPortNum
            // 
            this.txtPortNum.Location = new System.Drawing.Point(24, 48);
            this.txtPortNum.Name = "txtPortNum";
            this.txtPortNum.Size = new System.Drawing.Size(130, 20);
            this.txtPortNum.TabIndex = 11;
            this.txtPortNum.Text = "1981";
            this.txtPortNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPortNum_keydown);
            this.txtPortNum.Leave += new System.EventHandler(this.txtPortNum_keydown);
            // 
            // txtIPAddress1
            // 
            this.txtIPAddress1.Location = new System.Drawing.Point(24, 22);
            this.txtIPAddress1.Name = "txtIPAddress1";
            this.txtIPAddress1.Size = new System.Drawing.Size(28, 20);
            this.txtIPAddress1.TabIndex = 7;
            this.txtIPAddress1.Text = "127";
            this.txtIPAddress1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress1_keydown);
            this.txtIPAddress1.Leave += new System.EventHandler(this.txtIPAddress1_leave);
            // 
            // buttStartStop
            // 
            this.buttStartStop.Location = new System.Drawing.Point(24, 125);
            this.buttStartStop.Name = "buttStartStop";
            this.buttStartStop.Size = new System.Drawing.Size(75, 23);
            this.buttStartStop.TabIndex = 14;
            this.buttStartStop.Text = "Start";
            this.buttStartStop.UseVisualStyleBackColor = true;
            this.buttStartStop.Click += new System.EventHandler(this.buttStartStop_Click);
            // 
            // radioJoin
            // 
            this.radioJoin.AutoSize = true;
            this.radioJoin.Location = new System.Drawing.Point(100, 88);
            this.radioJoin.Name = "radioJoin";
            this.radioJoin.Size = new System.Drawing.Size(44, 17);
            this.radioJoin.TabIndex = 13;
            this.radioJoin.Text = "Join";
            this.radioJoin.UseVisualStyleBackColor = true;
            // 
            // radioHost
            // 
            this.radioHost.AutoSize = true;
            this.radioHost.Checked = true;
            this.radioHost.Location = new System.Drawing.Point(24, 88);
            this.radioHost.Name = "radioHost";
            this.radioHost.Size = new System.Drawing.Size(47, 17);
            this.radioHost.TabIndex = 12;
            this.radioHost.TabStop = true;
            this.radioHost.Text = "Host";
            this.radioHost.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address";
            // 
            // txtMain
            // 
            this.txtMain.Enabled = false;
            this.txtMain.Location = new System.Drawing.Point(11, 4);
            this.txtMain.Name = "txtMain";
            this.txtMain.Size = new System.Drawing.Size(397, 236);
            this.txtMain.TabIndex = 1;
            this.txtMain.Text = "";
            this.txtMain.TextChanged += new System.EventHandler(this.txtMain_TextChanged);
            // 
            // txtSend
            // 
            this.txtSend.Enabled = false;
            this.txtSend.Location = new System.Drawing.Point(11, 246);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(330, 20);
            this.txtSend.TabIndex = 2;
            this.txtSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSend_KeyPress);
            // 
            // buttSend
            // 
            this.buttSend.Enabled = false;
            this.buttSend.Location = new System.Drawing.Point(349, 246);
            this.buttSend.Name = "buttSend";
            this.buttSend.Size = new System.Drawing.Size(59, 20);
            this.buttSend.TabIndex = 3;
            this.buttSend.Text = "Send";
            this.buttSend.UseVisualStyleBackColor = true;
            this.buttSend.Click += new System.EventHandler(this.buttSend_Click);
            // 
            // processingTimer
            // 
            this.processingTimer.Tick += new System.EventHandler(this.processingTimer_Tick);
            // 
            // NetworkingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 271);
            this.ControlBox = false;
            this.Controls.Add(this.buttSend);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.txtMain);
            this.Controls.Add(this.groupBox1);
            this.Name = "NetworkingWindow";
            this.ShowInTaskbar = false;
            this.Text = "ChessNetworking";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttStartStop;
        private System.Windows.Forms.RadioButton radioJoin;
        private System.Windows.Forms.RadioButton radioHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtPortNum;
        private System.Windows.Forms.MaskedTextBox txtIPAddress1;
        private System.Windows.Forms.RichTextBox txtMain;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.Button buttSend;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MaskedTextBox txtIPAddress4;
        private System.Windows.Forms.MaskedTextBox txtIPAddress3;
        private System.Windows.Forms.MaskedTextBox txtIPAddress2;
        private System.Windows.Forms.Timer processingTimer;
    }
}