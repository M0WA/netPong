namespace netPongClientNG
{
    partial class netPongClientLoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(netPongClientLoginForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.playerListView = new System.Windows.Forms.ListView();
            this.playerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playerImageList = new System.Windows.Forms.ImageList(this.components);
            this.login = new System.Windows.Forms.Button();
            this.netPongServerIP = new System.Windows.Forms.TextBox();
            this.netPongServerPort = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.keySettingsButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.playerContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.playerListView);
            this.groupBox1.Location = new System.Drawing.Point(6, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 169);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "online players";
            // 
            // playerListView
            // 
            this.playerListView.ContextMenuStrip = this.playerContextMenuStrip;
            this.playerListView.Location = new System.Drawing.Point(15, 19);
            this.playerListView.MultiSelect = false;
            this.playerListView.Name = "playerListView";
            this.playerListView.Size = new System.Drawing.Size(363, 131);
            this.playerListView.SmallImageList = this.playerImageList;
            this.playerListView.StateImageList = this.playerImageList;
            this.playerListView.TabIndex = 9;
            this.playerListView.UseCompatibleStateImageBehavior = false;
            this.playerListView.View = System.Windows.Forms.View.SmallIcon;
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
            // login
            // 
            this.login.Location = new System.Drawing.Point(195, 11);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(97, 20);
            this.login.TabIndex = 15;
            this.login.Text = "login";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // netPongServerIP
            // 
            this.netPongServerIP.Location = new System.Drawing.Point(12, 12);
            this.netPongServerIP.Name = "netPongServerIP";
            this.netPongServerIP.Size = new System.Drawing.Size(95, 20);
            this.netPongServerIP.TabIndex = 12;
            this.netPongServerIP.Text = "127.0.0.1";
            // 
            // netPongServerPort
            // 
            this.netPongServerPort.Location = new System.Drawing.Point(133, 11);
            this.netPongServerPort.Name = "netPongServerPort";
            this.netPongServerPort.Size = new System.Drawing.Size(47, 20);
            this.netPongServerPort.TabIndex = 13;
            this.netPongServerPort.Text = "2410";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(11, 40);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(169, 20);
            this.username.TabIndex = 14;
            this.username.Text = "dashi";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(304, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 20);
            this.button1.TabIndex = 17;
            this.button1.Text = "graphic settings";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // keySettingsButton
            // 
            this.keySettingsButton.Location = new System.Drawing.Point(304, 40);
            this.keySettingsButton.Name = "keySettingsButton";
            this.keySettingsButton.Size = new System.Drawing.Size(96, 20);
            this.keySettingsButton.TabIndex = 18;
            this.keySettingsButton.Text = "key settings";
            this.keySettingsButton.UseVisualStyleBackColor = true;
            this.keySettingsButton.Click += new System.EventHandler(this.keySettingsButton_Click);
            // 
            // netPongClientLoginForm
            // 
            this.ClientSize = new System.Drawing.Size(415, 263);
            this.Controls.Add(this.keySettingsButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.login);
            this.Controls.Add(this.netPongServerIP);
            this.Controls.Add(this.netPongServerPort);
            this.Controls.Add(this.username);
            this.MaximizeBox = false;
            this.Name = "netPongClientLoginForm";
            this.ShowIcon = false;
            this.Text = "netPongClientNG: login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.netPongClientLoginForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.playerContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView playerListView;
        public System.Windows.Forms.Button login;
        public System.Windows.Forms.TextBox netPongServerIP;
        public System.Windows.Forms.TextBox netPongServerPort;
        public System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList playerImageList;
        private System.Windows.Forms.ContextMenuStrip playerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendMessageToolStripMenuItem;
        private System.Windows.Forms.Button keySettingsButton;
    }
}