namespace FuseBurner
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
            this.WindowSplitter = new System.Windows.Forms.SplitContainer();
            this.FuseCalcWeb = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.OptTxt = new System.Windows.Forms.TextBox();
            this.Forbid = new System.Windows.Forms.CheckBox();
            this.OutputTxt = new System.Windows.Forms.TextBox();
            this.ReadBtn = new System.Windows.Forms.Button();
            this.BurnBtn = new System.Windows.Forms.Button();
            this.Checker = new System.Windows.Forms.Timer(this.components);
            this.WindowSplitter.Panel1.SuspendLayout();
            this.WindowSplitter.Panel2.SuspendLayout();
            this.WindowSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowSplitter
            // 
            this.WindowSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WindowSplitter.Location = new System.Drawing.Point(0, 0);
            this.WindowSplitter.Name = "WindowSplitter";
            this.WindowSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // WindowSplitter.Panel1
            // 
            this.WindowSplitter.Panel1.Controls.Add(this.FuseCalcWeb);
            this.WindowSplitter.Panel1MinSize = 100;
            // 
            // WindowSplitter.Panel2
            // 
            this.WindowSplitter.Panel2.Controls.Add(this.label1);
            this.WindowSplitter.Panel2.Controls.Add(this.OptTxt);
            this.WindowSplitter.Panel2.Controls.Add(this.Forbid);
            this.WindowSplitter.Panel2.Controls.Add(this.OutputTxt);
            this.WindowSplitter.Panel2.Controls.Add(this.ReadBtn);
            this.WindowSplitter.Panel2.Controls.Add(this.BurnBtn);
            this.WindowSplitter.Panel2MinSize = 100;
            this.WindowSplitter.Size = new System.Drawing.Size(492, 473);
            this.WindowSplitter.SplitterDistance = 250;
            this.WindowSplitter.TabIndex = 0;
            // 
            // FuseCalcWeb
            // 
            this.FuseCalcWeb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FuseCalcWeb.Location = new System.Drawing.Point(0, 0);
            this.FuseCalcWeb.MinimumSize = new System.Drawing.Size(20, 20);
            this.FuseCalcWeb.Name = "FuseCalcWeb";
            this.FuseCalcWeb.Size = new System.Drawing.Size(492, 250);
            this.FuseCalcWeb.TabIndex = 0;
            this.FuseCalcWeb.Url = new System.Uri("", System.UriKind.Relative);
            this.FuseCalcWeb.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.FuseCalcWeb_Navigated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(297, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Options:";
            // 
            // OptTxt
            // 
            this.OptTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OptTxt.Location = new System.Drawing.Point(349, 5);
            this.OptTxt.Name = "OptTxt";
            this.OptTxt.Size = new System.Drawing.Size(140, 20);
            this.OptTxt.TabIndex = 3;
            // 
            // Forbid
            // 
            this.Forbid.AutoSize = true;
            this.Forbid.Location = new System.Drawing.Point(190, 8);
            this.Forbid.Name = "Forbid";
            this.Forbid.Size = new System.Drawing.Size(83, 17);
            this.Forbid.TabIndex = 2;
            this.Forbid.Text = "Forbid Write";
            this.Forbid.UseVisualStyleBackColor = true;
            // 
            // OutputTxt
            // 
            this.OutputTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTxt.Location = new System.Drawing.Point(1, 31);
            this.OutputTxt.Multiline = true;
            this.OutputTxt.Name = "OutputTxt";
            this.OutputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputTxt.Size = new System.Drawing.Size(490, 187);
            this.OutputTxt.TabIndex = 1;
            this.OutputTxt.WordWrap = false;
            // 
            // ReadBtn
            // 
            this.ReadBtn.Location = new System.Drawing.Point(93, 3);
            this.ReadBtn.Name = "ReadBtn";
            this.ReadBtn.Size = new System.Drawing.Size(75, 23);
            this.ReadBtn.TabIndex = 0;
            this.ReadBtn.Text = "Read";
            this.ReadBtn.UseVisualStyleBackColor = true;
            this.ReadBtn.Click += new System.EventHandler(this.ReadBtn_Click);
            // 
            // BurnBtn
            // 
            this.BurnBtn.Location = new System.Drawing.Point(12, 3);
            this.BurnBtn.Name = "BurnBtn";
            this.BurnBtn.Size = new System.Drawing.Size(75, 23);
            this.BurnBtn.TabIndex = 0;
            this.BurnBtn.Text = "Burn";
            this.BurnBtn.UseVisualStyleBackColor = true;
            this.BurnBtn.Click += new System.EventHandler(this.BurnBtn_Click);
            // 
            // Checker
            // 
            this.Checker.Enabled = true;
            this.Checker.Tick += new System.EventHandler(this.Checker_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 473);
            this.Controls.Add(this.WindowSplitter);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fuse Burner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.WindowSplitter.Panel1.ResumeLayout(false);
            this.WindowSplitter.Panel2.ResumeLayout(false);
            this.WindowSplitter.Panel2.PerformLayout();
            this.WindowSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer WindowSplitter;
        private System.Windows.Forms.WebBrowser FuseCalcWeb;
        private System.Windows.Forms.TextBox OutputTxt;
        private System.Windows.Forms.Button ReadBtn;
        private System.Windows.Forms.Button BurnBtn;
        private System.Windows.Forms.TextBox OptTxt;
        private System.Windows.Forms.CheckBox Forbid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer Checker;
    }
}

