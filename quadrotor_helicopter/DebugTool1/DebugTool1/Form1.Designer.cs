namespace DebugTool1
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
            this.DataList = new System.Windows.Forms.ListView();
            this.AddrColHead = new System.Windows.Forms.ColumnHeader();
            this.DataColHead = new System.Windows.Forms.ColumnHeader();
            this.AddrLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.WatchMin = new System.Windows.Forms.NumericUpDown();
            this.WatchMax = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.WatchBar = new System.Windows.Forms.TrackBar();
            this.FormChecker = new System.Windows.Forms.Timer(this.components);
            this.PortProcessor = new System.Windows.Forms.Timer(this.components);
            this.SerPort = new System.IO.Ports.SerialPort(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.WatchMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WatchMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WatchBar)).BeginInit();
            this.SuspendLayout();
            // 
            // DataList
            // 
            this.DataList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.DataList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AddrColHead,
            this.DataColHead});
            this.DataList.FullRowSelect = true;
            this.DataList.Location = new System.Drawing.Point(12, 12);
            this.DataList.MultiSelect = false;
            this.DataList.Name = "DataList";
            this.DataList.Size = new System.Drawing.Size(673, 430);
            this.DataList.TabIndex = 0;
            this.DataList.UseCompatibleStateImageBehavior = false;
            this.DataList.View = System.Windows.Forms.View.Details;
            this.DataList.SelectedIndexChanged += new System.EventHandler(this.DataList_SelectedIndexChanged);
            // 
            // AddrColHead
            // 
            this.AddrColHead.Text = "Addr";
            // 
            // DataColHead
            // 
            this.DataColHead.Text = "Data";
            this.DataColHead.Width = 150;
            // 
            // AddrLabel
            // 
            this.AddrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddrLabel.AutoSize = true;
            this.AddrLabel.Location = new System.Drawing.Point(9, 457);
            this.AddrLabel.Name = "AddrLabel";
            this.AddrLabel.Size = new System.Drawing.Size(32, 13);
            this.AddrLabel.TabIndex = 1;
            this.AddrLabel.Text = "Addr:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 475);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min:";
            // 
            // WatchMin
            // 
            this.WatchMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WatchMin.Location = new System.Drawing.Point(42, 473);
            this.WatchMin.Name = "WatchMin";
            this.WatchMin.Size = new System.Drawing.Size(72, 20);
            this.WatchMin.TabIndex = 3;
            // 
            // WatchMax
            // 
            this.WatchMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.WatchMax.Location = new System.Drawing.Point(613, 475);
            this.WatchMax.Name = "WatchMax";
            this.WatchMax.Size = new System.Drawing.Size(72, 20);
            this.WatchMax.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(580, 480);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max:";
            // 
            // WatchBar
            // 
            this.WatchBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.WatchBar.Location = new System.Drawing.Point(120, 448);
            this.WatchBar.Name = "WatchBar";
            this.WatchBar.Size = new System.Drawing.Size(454, 45);
            this.WatchBar.TabIndex = 4;
            this.WatchBar.TickFrequency = 10;
            this.WatchBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // FormChecker
            // 
            this.FormChecker.Enabled = true;
            this.FormChecker.Tick += new System.EventHandler(this.FormChecker_Tick);
            // 
            // PortProcessor
            // 
            this.PortProcessor.Enabled = true;
            this.PortProcessor.Interval = 10;
            this.PortProcessor.Tick += new System.EventHandler(this.PortProcessor_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 505);
            this.Controls.Add(this.WatchBar);
            this.Controls.Add(this.WatchMax);
            this.Controls.Add(this.WatchMin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AddrLabel);
            this.Controls.Add(this.DataList);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.WatchMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WatchMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WatchBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView DataList;
        private System.Windows.Forms.ColumnHeader AddrColHead;
        private System.Windows.Forms.ColumnHeader DataColHead;
        private System.Windows.Forms.Label AddrLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown WatchMin;
        private System.Windows.Forms.NumericUpDown WatchMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar WatchBar;
        private System.Windows.Forms.Timer FormChecker;
        private System.Windows.Forms.Timer PortProcessor;
        private System.IO.Ports.SerialPort SerPort;
    }
}

