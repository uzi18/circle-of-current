namespace BaseConverter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.NotiIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.notifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copy2StripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dec2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hex2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bin2toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.DetectedLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.HexLabel = new System.Windows.Forms.Label();
            this.DecLabel = new System.Windows.Forms.Label();
            this.BinLabel = new System.Windows.Forms.Label();
            this.ProcessTimer = new System.Windows.Forms.Timer(this.components);
            this.HideButton = new System.Windows.Forms.Button();
            this.EnableNoti = new System.Windows.Forms.CheckBox();
            this.KillButton = new System.Windows.Forms.Button();
            this.CopyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RightClickMenu.SuspendLayout();
            this.CopyMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotiIcon
            // 
            this.NotiIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.NotiIcon.ContextMenuStrip = this.RightClickMenu;
            this.NotiIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotiIcon.Icon")));
            this.NotiIcon.Text = "Base Converter";
            this.NotiIcon.Visible = true;
            this.NotiIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotiIcon_MouseClick);
            this.NotiIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotiIcon_MouseDoubleClick);
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notifyToolStripMenuItem,
            this.copy2StripMenuItem,
            this.quitToolStripMenuItem});
            this.RightClickMenu.Name = "RightClickMenu";
            this.RightClickMenu.Size = new System.Drawing.Size(153, 92);
            this.RightClickMenu.Text = "Menu";
            // 
            // notifyToolStripMenuItem
            // 
            this.notifyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onToolStripMenuItem,
            this.offToolStripMenuItem});
            this.notifyToolStripMenuItem.Name = "notifyToolStripMenuItem";
            this.notifyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.notifyToolStripMenuItem.Text = "Notify?";
            // 
            // onToolStripMenuItem
            // 
            this.onToolStripMenuItem.Name = "onToolStripMenuItem";
            this.onToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.onToolStripMenuItem.Text = "On";
            this.onToolStripMenuItem.Click += new System.EventHandler(this.onToolStripMenuItem_Click);
            // 
            // offToolStripMenuItem
            // 
            this.offToolStripMenuItem.Name = "offToolStripMenuItem";
            this.offToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.offToolStripMenuItem.Text = "Off";
            this.offToolStripMenuItem.Click += new System.EventHandler(this.offToolStripMenuItem_Click);
            // 
            // copy2StripMenuItem
            // 
            this.copy2StripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dec2ToolStripMenuItem,
            this.hex2ToolStripMenuItem,
            this.bin2toolStripMenuItem});
            this.copy2StripMenuItem.Name = "copy2StripMenuItem";
            this.copy2StripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copy2StripMenuItem.Text = "Copy";
            // 
            // dec2ToolStripMenuItem
            // 
            this.dec2ToolStripMenuItem.Name = "dec2ToolStripMenuItem";
            this.dec2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dec2ToolStripMenuItem.Text = "Decimal";
            this.dec2ToolStripMenuItem.Click += new System.EventHandler(this.decimalToolStripMenuItem_Click);
            // 
            // hex2ToolStripMenuItem
            // 
            this.hex2ToolStripMenuItem.Name = "hex2ToolStripMenuItem";
            this.hex2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hex2ToolStripMenuItem.Text = "Hex";
            this.hex2ToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // bin2toolStripMenuItem
            // 
            this.bin2toolStripMenuItem.Name = "bin2toolStripMenuItem";
            this.bin2toolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.bin2toolStripMenuItem.Text = "Binary";
            this.bin2toolStripMenuItem.Click += new System.EventHandler(this.binaryToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.KillButton_Click);
            // 
            // InputBox
            // 
            this.InputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InputBox.Location = new System.Drawing.Point(12, 12);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(191, 20);
            this.InputBox.TabIndex = 0;
            this.InputBox.Text = "0";
            // 
            // DetectedLabel
            // 
            this.DetectedLabel.AutoSize = true;
            this.DetectedLabel.Location = new System.Drawing.Point(83, 35);
            this.DetectedLabel.Name = "DetectedLabel";
            this.DetectedLabel.Size = new System.Drawing.Size(0, 13);
            this.DetectedLabel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Decimal: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Detected as:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Hex:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Binary:";
            // 
            // HexLabel
            // 
            this.HexLabel.AutoSize = true;
            this.HexLabel.Location = new System.Drawing.Point(65, 61);
            this.HexLabel.Name = "HexLabel";
            this.HexLabel.Size = new System.Drawing.Size(13, 13);
            this.HexLabel.TabIndex = 6;
            this.HexLabel.Text = "0";
            // 
            // DecLabel
            // 
            this.DecLabel.AutoSize = true;
            this.DecLabel.Location = new System.Drawing.Point(65, 48);
            this.DecLabel.Name = "DecLabel";
            this.DecLabel.Size = new System.Drawing.Size(13, 13);
            this.DecLabel.TabIndex = 7;
            this.DecLabel.Text = "0";
            // 
            // BinLabel
            // 
            this.BinLabel.AutoSize = true;
            this.BinLabel.Location = new System.Drawing.Point(65, 74);
            this.BinLabel.Name = "BinLabel";
            this.BinLabel.Size = new System.Drawing.Size(13, 13);
            this.BinLabel.TabIndex = 8;
            this.BinLabel.Text = "0";
            // 
            // ProcessTimer
            // 
            this.ProcessTimer.Enabled = true;
            this.ProcessTimer.Tick += new System.EventHandler(this.ProcessTimer_Tick);
            // 
            // HideButton
            // 
            this.HideButton.Location = new System.Drawing.Point(12, 90);
            this.HideButton.Name = "HideButton";
            this.HideButton.Size = new System.Drawing.Size(48, 23);
            this.HideButton.TabIndex = 9;
            this.HideButton.Text = "Hide";
            this.HideButton.UseVisualStyleBackColor = true;
            this.HideButton.Click += new System.EventHandler(this.HideButton_Click);
            // 
            // EnableNoti
            // 
            this.EnableNoti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EnableNoti.AutoSize = true;
            this.EnableNoti.Checked = true;
            this.EnableNoti.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableNoti.Location = new System.Drawing.Point(132, 94);
            this.EnableNoti.Name = "EnableNoti";
            this.EnableNoti.Size = new System.Drawing.Size(71, 17);
            this.EnableNoti.TabIndex = 10;
            this.EnableNoti.Text = "Notify Me";
            this.EnableNoti.UseVisualStyleBackColor = true;
            // 
            // KillButton
            // 
            this.KillButton.Location = new System.Drawing.Point(66, 90);
            this.KillButton.Name = "KillButton";
            this.KillButton.Size = new System.Drawing.Size(48, 23);
            this.KillButton.TabIndex = 9;
            this.KillButton.Text = "Kill";
            this.KillButton.UseVisualStyleBackColor = true;
            this.KillButton.Click += new System.EventHandler(this.KillButton_Click);
            // 
            // CopyMenu
            // 
            this.CopyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.CopyMenu.Name = "CopyMenu";
            this.CopyMenu.Size = new System.Drawing.Size(99, 26);
            this.CopyMenu.Text = "Copy Menu";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decimalToolStripMenuItem,
            this.hexToolStripMenuItem,
            this.binaryToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // decimalToolStripMenuItem
            // 
            this.decimalToolStripMenuItem.Name = "decimalToolStripMenuItem";
            this.decimalToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.decimalToolStripMenuItem.Text = "Decimal";
            this.decimalToolStripMenuItem.Click += new System.EventHandler(this.decimalToolStripMenuItem_Click);
            // 
            // hexToolStripMenuItem
            // 
            this.hexToolStripMenuItem.Name = "hexToolStripMenuItem";
            this.hexToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.hexToolStripMenuItem.Text = "Hex";
            this.hexToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // binaryToolStripMenuItem
            // 
            this.binaryToolStripMenuItem.Name = "binaryToolStripMenuItem";
            this.binaryToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.binaryToolStripMenuItem.Text = "Binary";
            this.binaryToolStripMenuItem.Click += new System.EventHandler(this.binaryToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 125);
            this.ContextMenuStrip = this.CopyMenu;
            this.Controls.Add(this.EnableNoti);
            this.Controls.Add(this.KillButton);
            this.Controls.Add(this.HideButton);
            this.Controls.Add(this.BinLabel);
            this.Controls.Add(this.DecLabel);
            this.Controls.Add(this.HexLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DetectedLabel);
            this.Controls.Add(this.InputBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Base Converter";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.RightClickMenu.ResumeLayout(false);
            this.CopyMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotiIcon;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.Label DetectedLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label HexLabel;
        private System.Windows.Forms.Label DecLabel;
        private System.Windows.Forms.Label BinLabel;
        private System.Windows.Forms.Timer ProcessTimer;
        private System.Windows.Forms.Button HideButton;
        private System.Windows.Forms.CheckBox EnableNoti;
        private System.Windows.Forms.Button KillButton;
        private System.Windows.Forms.ContextMenuStrip RightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip CopyMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copy2StripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dec2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hex2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bin2toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offToolStripMenuItem;
    }
}

