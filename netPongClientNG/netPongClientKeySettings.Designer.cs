namespace netPongClientNG
{
    partial class netPongClientKeySettings
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
            this.paddleLeftButton = new System.Windows.Forms.Button();
            this.paddleRightButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.worldMoveDownButton = new System.Windows.Forms.Button();
            this.worldMoveUpButton = new System.Windows.Forms.Button();
            this.resetCameraButtom = new System.Windows.Forms.Button();
            this.worldBackwardsButton = new System.Windows.Forms.Button();
            this.worldForwardsButton = new System.Windows.Forms.Button();
            this.worldRightButton = new System.Windows.Forms.Button();
            this.worldLeftButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rotateZMinusButton = new System.Windows.Forms.Button();
            this.rotateZPlusButton = new System.Windows.Forms.Button();
            this.rotateYMinusButton = new System.Windows.Forms.Button();
            this.rotateYPlusButton = new System.Windows.Forms.Button();
            this.rotateXMinusButton = new System.Windows.Forms.Button();
            this.rotateXPlusButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.keyBindingListView = new System.Windows.Forms.ListView();
            this.columnKeyActionHeader = new System.Windows.Forms.ColumnHeader();
            this.columnKeyNameHeader = new System.Windows.Forms.ColumnHeader();
            this.resetDefaultButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // paddleLeftButton
            // 
            this.paddleLeftButton.Location = new System.Drawing.Point(65, 29);
            this.paddleLeftButton.Name = "paddleLeftButton";
            this.paddleLeftButton.Size = new System.Drawing.Size(108, 20);
            this.paddleLeftButton.TabIndex = 0;
            this.paddleLeftButton.Text = "move left";
            this.paddleLeftButton.UseVisualStyleBackColor = true;
            this.paddleLeftButton.Click += new System.EventHandler(this.paddleLeftButton_Click);
            // 
            // paddleRightButton
            // 
            this.paddleRightButton.Location = new System.Drawing.Point(204, 29);
            this.paddleRightButton.Name = "paddleRightButton";
            this.paddleRightButton.Size = new System.Drawing.Size(108, 20);
            this.paddleRightButton.TabIndex = 1;
            this.paddleRightButton.Text = "move right";
            this.paddleRightButton.UseVisualStyleBackColor = true;
            this.paddleRightButton.Click += new System.EventHandler(this.paddleRightButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.paddleRightButton);
            this.groupBox1.Controls.Add(this.paddleLeftButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 69);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "paddle movement";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.worldMoveDownButton);
            this.groupBox2.Controls.Add(this.worldMoveUpButton);
            this.groupBox2.Controls.Add(this.resetCameraButtom);
            this.groupBox2.Controls.Add(this.worldBackwardsButton);
            this.groupBox2.Controls.Add(this.worldForwardsButton);
            this.groupBox2.Controls.Add(this.worldRightButton);
            this.groupBox2.Controls.Add(this.worldLeftButton);
            this.groupBox2.Location = new System.Drawing.Point(7, 85);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(379, 145);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "world movement";
            // 
            // worldMoveDownButton
            // 
            this.worldMoveDownButton.Location = new System.Drawing.Point(242, 45);
            this.worldMoveDownButton.Name = "worldMoveDownButton";
            this.worldMoveDownButton.Size = new System.Drawing.Size(108, 20);
            this.worldMoveDownButton.TabIndex = 6;
            this.worldMoveDownButton.Text = "move down";
            this.worldMoveDownButton.UseVisualStyleBackColor = true;
            this.worldMoveDownButton.Click += new System.EventHandler(this.worldMoveDownButton_Click);
            // 
            // worldMoveUpButton
            // 
            this.worldMoveUpButton.Location = new System.Drawing.Point(242, 19);
            this.worldMoveUpButton.Name = "worldMoveUpButton";
            this.worldMoveUpButton.Size = new System.Drawing.Size(108, 20);
            this.worldMoveUpButton.TabIndex = 5;
            this.worldMoveUpButton.Text = "move up";
            this.worldMoveUpButton.UseVisualStyleBackColor = true;
            this.worldMoveUpButton.Click += new System.EventHandler(this.worldMoveUpButton_Click);
            // 
            // resetCameraButtom
            // 
            this.resetCameraButtom.Location = new System.Drawing.Point(13, 19);
            this.resetCameraButtom.Name = "resetCameraButtom";
            this.resetCameraButtom.Size = new System.Drawing.Size(110, 20);
            this.resetCameraButtom.TabIndex = 4;
            this.resetCameraButtom.Text = "reset camera";
            this.resetCameraButtom.UseVisualStyleBackColor = true;
            this.resetCameraButtom.Click += new System.EventHandler(this.resetCameraButtom_Click);
            // 
            // worldBackwardsButton
            // 
            this.worldBackwardsButton.Location = new System.Drawing.Point(128, 114);
            this.worldBackwardsButton.Name = "worldBackwardsButton";
            this.worldBackwardsButton.Size = new System.Drawing.Size(108, 20);
            this.worldBackwardsButton.TabIndex = 3;
            this.worldBackwardsButton.Text = "move backwords";
            this.worldBackwardsButton.UseVisualStyleBackColor = true;
            this.worldBackwardsButton.Click += new System.EventHandler(this.worldBackwardsButton_Click);
            // 
            // worldForwardsButton
            // 
            this.worldForwardsButton.Location = new System.Drawing.Point(128, 88);
            this.worldForwardsButton.Name = "worldForwardsButton";
            this.worldForwardsButton.Size = new System.Drawing.Size(108, 20);
            this.worldForwardsButton.TabIndex = 2;
            this.worldForwardsButton.Text = "move forwards";
            this.worldForwardsButton.UseVisualStyleBackColor = true;
            this.worldForwardsButton.Click += new System.EventHandler(this.worldForwardsButton_Click);
            // 
            // worldRightButton
            // 
            this.worldRightButton.Location = new System.Drawing.Point(242, 103);
            this.worldRightButton.Name = "worldRightButton";
            this.worldRightButton.Size = new System.Drawing.Size(108, 20);
            this.worldRightButton.TabIndex = 1;
            this.worldRightButton.Text = "move right";
            this.worldRightButton.UseVisualStyleBackColor = true;
            this.worldRightButton.Click += new System.EventHandler(this.worldRightButton_Click);
            // 
            // worldLeftButton
            // 
            this.worldLeftButton.Location = new System.Drawing.Point(16, 103);
            this.worldLeftButton.Name = "worldLeftButton";
            this.worldLeftButton.Size = new System.Drawing.Size(106, 20);
            this.worldLeftButton.TabIndex = 0;
            this.worldLeftButton.Text = "move left";
            this.worldLeftButton.UseVisualStyleBackColor = true;
            this.worldLeftButton.Click += new System.EventHandler(this.worldLeftButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rotateZMinusButton);
            this.groupBox3.Controls.Add(this.rotateZPlusButton);
            this.groupBox3.Controls.Add(this.rotateYMinusButton);
            this.groupBox3.Controls.Add(this.rotateYPlusButton);
            this.groupBox3.Controls.Add(this.rotateXMinusButton);
            this.groupBox3.Controls.Add(this.rotateXPlusButton);
            this.groupBox3.Location = new System.Drawing.Point(7, 236);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(379, 92);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "world rotation";
            // 
            // rotateZMinusButton
            // 
            this.rotateZMinusButton.Location = new System.Drawing.Point(250, 52);
            this.rotateZMinusButton.Name = "rotateZMinusButton";
            this.rotateZMinusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateZMinusButton.TabIndex = 7;
            this.rotateZMinusButton.Text = "rotate z -";
            this.rotateZMinusButton.UseVisualStyleBackColor = true;
            this.rotateZMinusButton.Click += new System.EventHandler(this.rotateZMinusButton_Click);
            // 
            // rotateZPlusButton
            // 
            this.rotateZPlusButton.Location = new System.Drawing.Point(250, 25);
            this.rotateZPlusButton.Name = "rotateZPlusButton";
            this.rotateZPlusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateZPlusButton.TabIndex = 6;
            this.rotateZPlusButton.Text = "rotate z +";
            this.rotateZPlusButton.UseVisualStyleBackColor = true;
            this.rotateZPlusButton.Click += new System.EventHandler(this.rotateZPlusButton_Click);
            // 
            // rotateYMinusButton
            // 
            this.rotateYMinusButton.Location = new System.Drawing.Point(129, 52);
            this.rotateYMinusButton.Name = "rotateYMinusButton";
            this.rotateYMinusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateYMinusButton.TabIndex = 5;
            this.rotateYMinusButton.Text = "rotate y -";
            this.rotateYMinusButton.UseVisualStyleBackColor = true;
            this.rotateYMinusButton.Click += new System.EventHandler(this.rotateYMinusButton_Click);
            // 
            // rotateYPlusButton
            // 
            this.rotateYPlusButton.Location = new System.Drawing.Point(128, 25);
            this.rotateYPlusButton.Name = "rotateYPlusButton";
            this.rotateYPlusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateYPlusButton.TabIndex = 4;
            this.rotateYPlusButton.Text = "rotate y +";
            this.rotateYPlusButton.UseVisualStyleBackColor = true;
            this.rotateYPlusButton.Click += new System.EventHandler(this.rotateYPlusButton_Click);
            // 
            // rotateXMinusButton
            // 
            this.rotateXMinusButton.Location = new System.Drawing.Point(12, 52);
            this.rotateXMinusButton.Name = "rotateXMinusButton";
            this.rotateXMinusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateXMinusButton.TabIndex = 3;
            this.rotateXMinusButton.Text = "rotate x -";
            this.rotateXMinusButton.UseVisualStyleBackColor = true;
            this.rotateXMinusButton.Click += new System.EventHandler(this.rotateXMinusButton_Click);
            // 
            // rotateXPlusButton
            // 
            this.rotateXPlusButton.Location = new System.Drawing.Point(11, 25);
            this.rotateXPlusButton.Name = "rotateXPlusButton";
            this.rotateXPlusButton.Size = new System.Drawing.Size(108, 20);
            this.rotateXPlusButton.TabIndex = 2;
            this.rotateXPlusButton.Text = "rotate x +";
            this.rotateXPlusButton.UseVisualStyleBackColor = true;
            this.rotateXPlusButton.Click += new System.EventHandler(this.rotateXPlusButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.keyBindingListView);
            this.groupBox4.Location = new System.Drawing.Point(394, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(462, 292);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "current controls";
            // 
            // keyBindingListView
            // 
            this.keyBindingListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnKeyActionHeader,
            this.columnKeyNameHeader});
            this.keyBindingListView.FullRowSelect = true;
            this.keyBindingListView.GridLines = true;
            this.keyBindingListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.keyBindingListView.Location = new System.Drawing.Point(16, 22);
            this.keyBindingListView.MultiSelect = false;
            this.keyBindingListView.Name = "keyBindingListView";
            this.keyBindingListView.Size = new System.Drawing.Size(434, 251);
            this.keyBindingListView.TabIndex = 0;
            this.keyBindingListView.UseCompatibleStateImageBehavior = false;
            this.keyBindingListView.View = System.Windows.Forms.View.Details;
            // 
            // columnKeyActionHeader
            // 
            this.columnKeyActionHeader.Text = "action";
            this.columnKeyActionHeader.Width = 260;
            // 
            // columnKeyNameHeader
            // 
            this.columnKeyNameHeader.Text = "key";
            this.columnKeyNameHeader.Width = 170;
            // 
            // resetDefaultButton
            // 
            this.resetDefaultButton.Location = new System.Drawing.Point(394, 306);
            this.resetDefaultButton.Name = "resetDefaultButton";
            this.resetDefaultButton.Size = new System.Drawing.Size(116, 22);
            this.resetDefaultButton.TabIndex = 6;
            this.resetDefaultButton.Text = "reset defaults";
            this.resetDefaultButton.UseVisualStyleBackColor = true;
            this.resetDefaultButton.Click += new System.EventHandler(this.resetDefaultButton_Click);
            // 
            // netPongClientKeySettings
            // 
            this.ClientSize = new System.Drawing.Size(868, 336);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.resetDefaultButton);
            this.MaximizeBox = false;
            this.Name = "netPongClientKeySettings";
            this.ShowIcon = false;
            this.Text = "netPongClientNG: controls";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.netPongClientKeySettings_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button paddleLeftButton;
        private System.Windows.Forms.Button paddleRightButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button worldBackwardsButton;
        private System.Windows.Forms.Button worldForwardsButton;
        private System.Windows.Forms.Button worldRightButton;
        private System.Windows.Forms.Button worldLeftButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button rotateXMinusButton;
        private System.Windows.Forms.Button rotateXPlusButton;
        private System.Windows.Forms.Button rotateZMinusButton;
        private System.Windows.Forms.Button rotateZPlusButton;
        private System.Windows.Forms.Button rotateYMinusButton;
        private System.Windows.Forms.Button rotateYPlusButton;
        private System.Windows.Forms.Button resetCameraButtom;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView keyBindingListView;
        private System.Windows.Forms.Button resetDefaultButton;
        private System.Windows.Forms.ColumnHeader columnKeyActionHeader;
        private System.Windows.Forms.ColumnHeader columnKeyNameHeader;
        private System.Windows.Forms.Button worldMoveDownButton;
        private System.Windows.Forms.Button worldMoveUpButton;
    }
}