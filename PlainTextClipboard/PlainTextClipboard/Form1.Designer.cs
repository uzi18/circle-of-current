namespace PlainTextClipboard
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
            this.SystrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowWinBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.ConvertBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.ConvertedTxt = new System.Windows.Forms.TextBox();
            this.RightClickMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // SystrayIcon
            // 
            this.SystrayIcon.ContextMenuStrip = this.RightClickMenu;
            this.SystrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("SystrayIcon.Icon")));
            this.SystrayIcon.Text = "PlainTextClipboard";
            this.SystrayIcon.Visible = true;
            this.SystrayIcon.DoubleClick += new System.EventHandler(this.SystrayIcon_DoubleClick);
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowWinBtn,
            this.ConvertBtn});
            this.RightClickMenu.Name = "contextMenuStrip1";
            this.RightClickMenu.Size = new System.Drawing.Size(171, 48);
            // 
            // ShowWinBtn
            // 
            this.ShowWinBtn.Name = "ShowWinBtn";
            this.ShowWinBtn.Size = new System.Drawing.Size(170, 22);
            this.ShowWinBtn.Text = "Show Window";
            this.ShowWinBtn.Click += new System.EventHandler(this.ShowWinBtn_Click);
            // 
            // ConvertBtn
            // 
            this.ConvertBtn.Name = "ConvertBtn";
            this.ConvertBtn.Size = new System.Drawing.Size(170, 22);
            this.ConvertBtn.Text = "Plain Text Convert";
            this.ConvertBtn.Click += new System.EventHandler(this.ConvertBtn_Click);
            // 
            // ConvertedTxt
            // 
            this.ConvertedTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertedTxt.Location = new System.Drawing.Point(0, 0);
            this.ConvertedTxt.Multiline = true;
            this.ConvertedTxt.Name = "ConvertedTxt";
            this.ConvertedTxt.ReadOnly = true;
            this.ConvertedTxt.Size = new System.Drawing.Size(284, 264);
            this.ConvertedTxt.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.ConvertedTxt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "PlainTextClipboard";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.RightClickMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon SystrayIcon;
        private System.Windows.Forms.ContextMenuStrip RightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem ShowWinBtn;
        private System.Windows.Forms.ToolStripMenuItem ConvertBtn;
        private System.Windows.Forms.TextBox ConvertedTxt;
    }
}

