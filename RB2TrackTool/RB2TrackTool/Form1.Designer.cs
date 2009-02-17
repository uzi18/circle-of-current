namespace RB2TrackTool
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
            this.LoadFolderButton = new System.Windows.Forms.Button();
            this.FileListBox = new System.Windows.Forms.ListBox();
            this.SongStatusLabel = new System.Windows.Forms.Label();
            this.Checker = new System.Windows.Forms.Timer(this.components);
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this.CaliButton = new System.Windows.Forms.Button();
            this.TimeTakenLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SongLengthLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PlayProgBar = new System.Windows.Forms.ProgressBar();
            this.AbortButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.PortStatusLabel = new System.Windows.Forms.Label();
            this.FolderPathText = new System.Windows.Forms.TextBox();
            this.FolderBrowseButton = new System.Windows.Forms.Button();
            this.SerPort = new System.IO.Ports.SerialPort(this.components);
            this.Player = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.TimeTakenToPlayLabel = new System.Windows.Forms.Label();
            this.GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadFolderButton
            // 
            this.LoadFolderButton.Location = new System.Drawing.Point(12, 12);
            this.LoadFolderButton.Name = "LoadFolderButton";
            this.LoadFolderButton.Size = new System.Drawing.Size(75, 23);
            this.LoadFolderButton.TabIndex = 0;
            this.LoadFolderButton.Text = "Load Folder";
            this.LoadFolderButton.UseVisualStyleBackColor = true;
            this.LoadFolderButton.Click += new System.EventHandler(this.LoadFolderButton_Click);
            // 
            // FileListBox
            // 
            this.FileListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.FileListBox.FormattingEnabled = true;
            this.FileListBox.Location = new System.Drawing.Point(12, 41);
            this.FileListBox.Name = "FileListBox";
            this.FileListBox.Size = new System.Drawing.Size(184, 446);
            this.FileListBox.TabIndex = 2;
            this.FileListBox.SelectedIndexChanged += new System.EventHandler(this.FileListBox_SelectedIndexChanged);
            // 
            // SongStatusLabel
            // 
            this.SongStatusLabel.AutoSize = true;
            this.SongStatusLabel.Location = new System.Drawing.Point(6, 16);
            this.SongStatusLabel.Name = "SongStatusLabel";
            this.SongStatusLabel.Size = new System.Drawing.Size(68, 13);
            this.SongStatusLabel.TabIndex = 3;
            this.SongStatusLabel.Text = "Song Status:";
            // 
            // Checker
            // 
            this.Checker.Enabled = true;
            this.Checker.Tick += new System.EventHandler(this.Checker_Tick);
            // 
            // GroupBox
            // 
            this.GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox.Controls.Add(this.CaliButton);
            this.GroupBox.Controls.Add(this.TimeTakenToPlayLabel);
            this.GroupBox.Controls.Add(this.TimeTakenLabel);
            this.GroupBox.Controls.Add(this.label3);
            this.GroupBox.Controls.Add(this.label2);
            this.GroupBox.Controls.Add(this.SongLengthLabel);
            this.GroupBox.Controls.Add(this.label1);
            this.GroupBox.Controls.Add(this.PlayProgBar);
            this.GroupBox.Controls.Add(this.AbortButton);
            this.GroupBox.Controls.Add(this.PlayButton);
            this.GroupBox.Controls.Add(this.PortStatusLabel);
            this.GroupBox.Controls.Add(this.SongStatusLabel);
            this.GroupBox.Enabled = false;
            this.GroupBox.Location = new System.Drawing.Point(202, 41);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(272, 452);
            this.GroupBox.TabIndex = 4;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "Drum Bot Control";
            // 
            // CaliButton
            // 
            this.CaliButton.Location = new System.Drawing.Point(171, 65);
            this.CaliButton.Name = "CaliButton";
            this.CaliButton.Size = new System.Drawing.Size(75, 23);
            this.CaliButton.TabIndex = 12;
            this.CaliButton.Text = "Calibrate";
            this.CaliButton.UseVisualStyleBackColor = true;
            this.CaliButton.Click += new System.EventHandler(this.CalBut_Click);
            // 
            // TimeTakenLabel
            // 
            this.TimeTakenLabel.AutoSize = true;
            this.TimeTakenLabel.Location = new System.Drawing.Point(139, 164);
            this.TimeTakenLabel.Name = "TimeTakenLabel";
            this.TimeTakenLabel.Size = new System.Drawing.Size(13, 13);
            this.TimeTakenLabel.TabIndex = 11;
            this.TimeTakenLabel.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Time Taken To Calibrate:";
            // 
            // SongLengthLabel
            // 
            this.SongLengthLabel.AutoSize = true;
            this.SongLengthLabel.Location = new System.Drawing.Point(139, 135);
            this.SongLengthLabel.Name = "SongLengthLabel";
            this.SongLengthLabel.Size = new System.Drawing.Size(13, 13);
            this.SongLengthLabel.TabIndex = 9;
            this.SongLengthLabel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Length of Song:";
            // 
            // PlayProgBar
            // 
            this.PlayProgBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayProgBar.Location = new System.Drawing.Point(9, 94);
            this.PlayProgBar.Name = "PlayProgBar";
            this.PlayProgBar.Size = new System.Drawing.Size(257, 23);
            this.PlayProgBar.TabIndex = 7;
            // 
            // AbortButton
            // 
            this.AbortButton.Location = new System.Drawing.Point(90, 65);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 6;
            this.AbortButton.Text = "Abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(9, 65);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 23);
            this.PlayButton.TabIndex = 5;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // PortStatusLabel
            // 
            this.PortStatusLabel.AutoSize = true;
            this.PortStatusLabel.Location = new System.Drawing.Point(6, 38);
            this.PortStatusLabel.Name = "PortStatusLabel";
            this.PortStatusLabel.Size = new System.Drawing.Size(62, 13);
            this.PortStatusLabel.TabIndex = 4;
            this.PortStatusLabel.Text = "Port Status:";
            // 
            // FolderPathText
            // 
            this.FolderPathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FolderPathText.Location = new System.Drawing.Point(93, 14);
            this.FolderPathText.Name = "FolderPathText";
            this.FolderPathText.Size = new System.Drawing.Size(300, 20);
            this.FolderPathText.TabIndex = 5;
            // 
            // FolderBrowseButton
            // 
            this.FolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FolderBrowseButton.Location = new System.Drawing.Point(399, 12);
            this.FolderBrowseButton.Name = "FolderBrowseButton";
            this.FolderBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.FolderBrowseButton.TabIndex = 6;
            this.FolderBrowseButton.Text = "Find";
            this.FolderBrowseButton.UseVisualStyleBackColor = true;
            this.FolderBrowseButton.Click += new System.EventHandler(this.FolderBrowseButton_Click);
            // 
            // Player
            // 
            this.Player.Enabled = true;
            this.Player.Interval = 10;
            this.Player.Tick += new System.EventHandler(this.Player_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Time Taken To Play:";
            // 
            // TimeTakenToPlayLabel
            // 
            this.TimeTakenToPlayLabel.AutoSize = true;
            this.TimeTakenToPlayLabel.Location = new System.Drawing.Point(139, 191);
            this.TimeTakenToPlayLabel.Name = "TimeTakenToPlayLabel";
            this.TimeTakenToPlayLabel.Size = new System.Drawing.Size(13, 13);
            this.TimeTakenToPlayLabel.TabIndex = 11;
            this.TimeTakenToPlayLabel.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 504);
            this.Controls.Add(this.FolderBrowseButton);
            this.Controls.Add(this.FolderPathText);
            this.Controls.Add(this.GroupBox);
            this.Controls.Add(this.FileListBox);
            this.Controls.Add(this.LoadFolderButton);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Rock Band 2 Track Tool";
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadFolderButton;
        private System.Windows.Forms.ListBox FileListBox;
        private System.Windows.Forms.Label SongStatusLabel;
        private System.Windows.Forms.Timer Checker;
        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.TextBox FolderPathText;
        private System.Windows.Forms.Button FolderBrowseButton;
        private System.Windows.Forms.Label PortStatusLabel;
        private System.IO.Ports.SerialPort SerPort;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Timer Player;
        private System.Windows.Forms.ProgressBar PlayProgBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TimeTakenLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label SongLengthLabel;
        private System.Windows.Forms.Button CaliButton;
        private System.Windows.Forms.Label TimeTakenToPlayLabel;
        private System.Windows.Forms.Label label3;
    }
}

