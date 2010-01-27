namespace netPongDebugClient
{
    partial class netPongClientSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(netPongClientSettingsForm));
            this.netPongServerIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.netPongServerPort = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.login = new System.Windows.Forms.Button();
            this.LogOutput = new System.Windows.Forms.TextBox();
            this.playerListView = new System.Windows.Forms.ListView();
            this.playerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playerImageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chatWindowbutton = new System.Windows.Forms.Button();
            this.gamesettingsButton = new System.Windows.Forms.Button();
            this.playerContextMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // netPongServerIP
            // 
            this.netPongServerIP.Location = new System.Drawing.Point(15, 27);
            this.netPongServerIP.Name = "netPongServerIP";
            this.netPongServerIP.Size = new System.Drawing.Size(95, 20);
            this.netPongServerIP.TabIndex = 0;
            this.netPongServerIP.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "netPong Server IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(133, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "netPong Server Port";
            // 
            // netPongServerPort
            // 
            this.netPongServerPort.Location = new System.Drawing.Point(136, 25);
            this.netPongServerPort.Name = "netPongServerPort";
            this.netPongServerPort.Size = new System.Drawing.Size(47, 20);
            this.netPongServerPort.TabIndex = 3;
            this.netPongServerPort.Text = "2410";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(9, 77);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(227, 20);
            this.username.TabIndex = 4;
            this.username.Text = "dashi2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "username";
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(9, 115);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(97, 26);
            this.login.TabIndex = 6;
            this.login.Text = "login";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // LogOutput
            // 
            this.LogOutput.Location = new System.Drawing.Point(7, 18);
            this.LogOutput.Multiline = true;
            this.LogOutput.Name = "LogOutput";
            this.LogOutput.ReadOnly = true;
            this.LogOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogOutput.Size = new System.Drawing.Size(321, 210);
            this.LogOutput.TabIndex = 7;
            // 
            // playerListView
            // 
            this.playerListView.ContextMenuStrip = this.playerContextMenuStrip;
            this.playerListView.LargeImageList = this.playerImageList;
            this.playerListView.Location = new System.Drawing.Point(15, 17);
            this.playerListView.MultiSelect = false;
            this.playerListView.Name = "playerListView";
            this.playerListView.Size = new System.Drawing.Size(354, 348);
            this.playerListView.SmallImageList = this.playerImageList;
            this.playerListView.TabIndex = 9;
            this.playerListView.UseCompatibleStateImageBehavior = false;
            // 
            // playerContextMenuStrip
            // 
            this.playerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem,
            this.sendMessageToolStripMenuItem});
            this.playerContextMenuStrip.Name = "playerContextMenuStrip";
            this.playerContextMenuStrip.Size = new System.Drawing.Size(155, 48);
            // 
            // toolStripMenuItem
            // 
            this.toolStripMenuItem.Name = "toolStripMenuItem";
            this.toolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem.Text = "&Invite";
            this.toolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // sendMessageToolStripMenuItem
            // 
            this.sendMessageToolStripMenuItem.Name = "sendMessageToolStripMenuItem";
            this.sendMessageToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.sendMessageToolStripMenuItem.Text = "&Send Message";
            this.sendMessageToolStripMenuItem.Click += new System.EventHandler(this.sendMessageToolStripMenuItem_Click);
            // 
            // playerImageList
            // 
            this.playerImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("playerImageList.ImageStream")));
            this.playerImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.playerImageList.Images.SetKeyName(0, "hemp.gif");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.playerListView);
            this.groupBox1.Location = new System.Drawing.Point(354, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 380);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "online players";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LogOutput);
            this.groupBox2.Location = new System.Drawing.Point(7, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 243);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "client log";
            // 
            // chatWindowbutton
            // 
            this.chatWindowbutton.Enabled = false;
            this.chatWindowbutton.Location = new System.Drawing.Point(112, 115);
            this.chatWindowbutton.Name = "chatWindowbutton";
            this.chatWindowbutton.Size = new System.Drawing.Size(103, 26);
            this.chatWindowbutton.TabIndex = 13;
            this.chatWindowbutton.Text = "open chat window";
            this.chatWindowbutton.UseVisualStyleBackColor = true;
            this.chatWindowbutton.Click += new System.EventHandler(this.chatWindowbutton_Click);
            // 
            // gamesettingsButton
            // 
            this.gamesettingsButton.Location = new System.Drawing.Point(221, 115);
            this.gamesettingsButton.Name = "gamesettingsButton";
            this.gamesettingsButton.Size = new System.Drawing.Size(98, 25);
            this.gamesettingsButton.TabIndex = 14;
            this.gamesettingsButton.Text = "game settings";
            this.gamesettingsButton.UseVisualStyleBackColor = true;
            this.gamesettingsButton.Click += new System.EventHandler(this.gamesettingsButton_Click);
            // 
            // netPongClientSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 396);
            this.Controls.Add(this.gamesettingsButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chatWindowbutton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.login);
            this.Controls.Add(this.netPongServerIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.netPongServerPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.username);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "netPongClientSettingsForm";
            this.Text = "netPong settings debug dummy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.netPongClientSettingsForm_FormClosed);
            this.playerContextMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox netPongServerIP;
        public System.Windows.Forms.TextBox username;
        public System.Windows.Forms.TextBox netPongServerPort;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button login;
        public System.Windows.Forms.TextBox LogOutput;
        private System.Windows.Forms.ListView playerListView;
        private System.Windows.Forms.ImageList playerImageList;
        private System.Windows.Forms.ContextMenuStrip playerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem sendMessageToolStripMenuItem;
        public System.Windows.Forms.Button chatWindowbutton;
        private System.Windows.Forms.Button gamesettingsButton;
    }
}

