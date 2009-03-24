namespace CSharpBootloaderUtility
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
            this.PortList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PortStatus = new System.Windows.Forms.Label();
            this.SerPort = new System.IO.Ports.SerialPort(this.components);
            this.ProcTimer = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.FilePathTxt = new System.Windows.Forms.TextBox();
            this.BrowseBtn = new System.Windows.Forms.Button();
            this.StartBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.LoadProgress = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LogTxt = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PortList
            // 
            this.PortList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PortList.FormattingEnabled = true;
            this.PortList.Location = new System.Drawing.Point(78, 12);
            this.PortList.Name = "PortList";
            this.PortList.Size = new System.Drawing.Size(121, 21);
            this.PortList.TabIndex = 0;
            this.PortList.SelectedIndexChanged += new System.EventHandler(this.PortList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port Name:";
            // 
            // PortStatus
            // 
            this.PortStatus.AutoSize = true;
            this.PortStatus.Location = new System.Drawing.Point(205, 15);
            this.PortStatus.Name = "PortStatus";
            this.PortStatus.Size = new System.Drawing.Size(52, 13);
            this.PortStatus.TabIndex = 2;
            this.PortStatus.Text = "Initializing";
            // 
            // SerPort
            // 
            this.SerPort.BaudRate = 19200;
            // 
            // ProcTimer
            // 
            this.ProcTimer.Enabled = true;
            this.ProcTimer.Interval = 10;
            this.ProcTimer.Tick += new System.EventHandler(this.ProcTimer_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "File Path:";
            // 
            // FilePathTxt
            // 
            this.FilePathTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FilePathTxt.Location = new System.Drawing.Point(78, 39);
            this.FilePathTxt.Name = "FilePathTxt";
            this.FilePathTxt.Size = new System.Drawing.Size(310, 20);
            this.FilePathTxt.TabIndex = 4;
            // 
            // BrowseBtn
            // 
            this.BrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseBtn.Location = new System.Drawing.Point(394, 37);
            this.BrowseBtn.Name = "BrowseBtn";
            this.BrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.BrowseBtn.TabIndex = 5;
            this.BrowseBtn.Text = "Browse";
            this.BrowseBtn.UseVisualStyleBackColor = true;
            this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(78, 65);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 23);
            this.StartBtn.TabIndex = 6;
            this.StartBtn.Text = "Bootload";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Enabled = false;
            this.CancelBtn.Location = new System.Drawing.Point(159, 65);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 7;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // LoadProgress
            // 
            this.LoadProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadProgress.Location = new System.Drawing.Point(12, 94);
            this.LoadProgress.Name = "LoadProgress";
            this.LoadProgress.Size = new System.Drawing.Size(457, 23);
            this.LoadProgress.Step = 1;
            this.LoadProgress.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.LogTxt);
            this.groupBox1.Location = new System.Drawing.Point(12, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 297);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // LogTxt
            // 
            this.LogTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTxt.Location = new System.Drawing.Point(3, 16);
            this.LogTxt.Multiline = true;
            this.LogTxt.Name = "LogTxt";
            this.LogTxt.ReadOnly = true;
            this.LogTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTxt.Size = new System.Drawing.Size(451, 278);
            this.LogTxt.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 432);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LoadProgress);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.BrowseBtn);
            this.Controls.Add(this.FilePathTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PortStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PortList);
            this.Name = "Form1";
            this.Text = "Bootloader Utility";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox PortList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label PortStatus;
        private System.IO.Ports.SerialPort SerPort;
        private System.Windows.Forms.Timer ProcTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilePathTxt;
        private System.Windows.Forms.Button BrowseBtn;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.ProgressBar LoadProgress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox LogTxt;
    }
}

