namespace WebSocketGUI
{
    partial class MainForm
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblMessagesCount = new System.Windows.Forms.Label();
            this.lblLastMsgTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnListen = new System.Windows.Forms.Button();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.tbLog = new System.Windows.Forms.RichTextBox();
            this.panelTop.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblMessagesCount);
            this.panelTop.Controls.Add(this.lblLastMsgTime);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.panelLeft);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(656, 87);
            this.panelTop.TabIndex = 0;
            // 
            // lblMessagesCount
            // 
            this.lblMessagesCount.AutoSize = true;
            this.lblMessagesCount.Location = new System.Drawing.Point(400, 21);
            this.lblMessagesCount.Name = "lblMessagesCount";
            this.lblMessagesCount.Size = new System.Drawing.Size(10, 13);
            this.lblMessagesCount.TabIndex = 8;
            this.lblMessagesCount.Text = "-";
            // 
            // lblLastMsgTime
            // 
            this.lblLastMsgTime.AutoSize = true;
            this.lblLastMsgTime.Location = new System.Drawing.Point(400, 5);
            this.lblLastMsgTime.Name = "lblLastMsgTime";
            this.lblLastMsgTime.Size = new System.Drawing.Size(10, 13);
            this.lblLastMsgTime.TabIndex = 7;
            this.lblLastMsgTime.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Сообщений получено";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(271, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Последнее сообщение";
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.btnListen);
            this.panelLeft.Controls.Add(this.tbPort);
            this.panelLeft.Controls.Add(this.btnConnect);
            this.panelLeft.Controls.Add(this.tbHost);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(265, 87);
            this.panelLeft.TabIndex = 4;
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(151, 29);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(108, 23);
            this.btnListen.TabIndex = 7;
            this.btnListen.Text = "слушать";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(7, 31);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(138, 20);
            this.tbPort.TabIndex = 6;
            this.tbPort.Text = "19006";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(151, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(108, 23);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "подключиться";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(7, 5);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(138, 20);
            this.tbHost.TabIndex = 4;
            this.tbHost.Text = "ws://127.0.0.1:19006";
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(0, 87);
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(656, 260);
            this.tbLog.TabIndex = 1;
            this.tbLog.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 347);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.panelTop);
            this.Name = "MainForm";
            this.Text = "WebSocket GUI";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblMessagesCount;
        private System.Windows.Forms.Label lblLastMsgTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.RichTextBox tbLog;
    }
}

