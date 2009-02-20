namespace RB2DrumBot
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            this.LoadFolderButton = new System.Windows.Forms.Button();
            this.FileListBox = new System.Windows.Forms.ListBox();
            this.SongStatusLabel = new System.Windows.Forms.Label();
            this.Checker = new System.Windows.Forms.Timer(this.components);
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this.StopPlayButton = new System.Windows.Forms.Button();
            this.TimeTakenToPlayLabel = new System.Windows.Forms.Label();
            this.TimeTakenLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SongLengthLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ProgBar = new System.Windows.Forms.ProgressBar();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.PortStatusLabel = new System.Windows.Forms.Label();
            this.FolderPathText = new System.Windows.Forms.TextBox();
            this.FolderBrowseButton = new System.Windows.Forms.Button();
            this.SerPort = new System.IO.Ports.SerialPort(this.components);
            this.PortChecker = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ListOfNotes = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutoAdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ManAdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RedBox = new System.Windows.Forms.DataGridViewImageColumn();
            this.YellowBox = new System.Windows.Forms.DataGridViewImageColumn();
            this.BlueBox = new System.Windows.Forms.DataGridViewImageColumn();
            this.GreenBox = new System.Windows.Forms.DataGridViewImageColumn();
            this.BassBox = new System.Windows.Forms.DataGridViewImageColumn();
            this.ScaleBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.SaveConfigButton = new System.Windows.Forms.Button();
            this.LoadConfigButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.ClearCaliBut = new System.Windows.Forms.Button();
            this.PercentAdjBar = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.PercentAdjLabel = new System.Windows.Forms.Label();
            this.GroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ListOfNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentAdjBar)).BeginInit();
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
            this.FileListBox.Size = new System.Drawing.Size(184, 602);
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
            this.GroupBox.Controls.Add(this.PercentAdjLabel);
            this.GroupBox.Controls.Add(this.label5);
            this.GroupBox.Controls.Add(this.PercentAdjBar);
            this.GroupBox.Controls.Add(this.ClearCaliBut);
            this.GroupBox.Controls.Add(this.ResetButton);
            this.GroupBox.Controls.Add(this.LoadConfigButton);
            this.GroupBox.Controls.Add(this.SaveConfigButton);
            this.GroupBox.Controls.Add(this.StopPlayButton);
            this.GroupBox.Controls.Add(this.TimeTakenToPlayLabel);
            this.GroupBox.Controls.Add(this.TimeTakenLabel);
            this.GroupBox.Controls.Add(this.label3);
            this.GroupBox.Controls.Add(this.label2);
            this.GroupBox.Controls.Add(this.SongLengthLabel);
            this.GroupBox.Controls.Add(this.label1);
            this.GroupBox.Controls.Add(this.ProgBar);
            this.GroupBox.Controls.Add(this.LoadFileButton);
            this.GroupBox.Controls.Add(this.PlayButton);
            this.GroupBox.Controls.Add(this.PortStatusLabel);
            this.GroupBox.Controls.Add(this.SongStatusLabel);
            this.GroupBox.Enabled = false;
            this.GroupBox.Location = new System.Drawing.Point(202, 41);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(252, 620);
            this.GroupBox.TabIndex = 4;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "Drum Bot Control";
            // 
            // StopPlayButton
            // 
            this.StopPlayButton.Location = new System.Drawing.Point(171, 65);
            this.StopPlayButton.Name = "StopPlayButton";
            this.StopPlayButton.Size = new System.Drawing.Size(75, 23);
            this.StopPlayButton.TabIndex = 12;
            this.StopPlayButton.Text = "Calibrate";
            this.StopPlayButton.UseVisualStyleBackColor = true;
            this.StopPlayButton.Click += new System.EventHandler(this.CalBut_Click);
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
            // TimeTakenLabel
            // 
            this.TimeTakenLabel.AutoSize = true;
            this.TimeTakenLabel.Location = new System.Drawing.Point(139, 164);
            this.TimeTakenLabel.Name = "TimeTakenLabel";
            this.TimeTakenLabel.Size = new System.Drawing.Size(13, 13);
            this.TimeTakenLabel.TabIndex = 11;
            this.TimeTakenLabel.Text = "0";
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
            // ProgBar
            // 
            this.ProgBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgBar.Location = new System.Drawing.Point(9, 94);
            this.ProgBar.Name = "ProgBar";
            this.ProgBar.Size = new System.Drawing.Size(237, 23);
            this.ProgBar.TabIndex = 7;
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Location = new System.Drawing.Point(90, 65);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(75, 23);
            this.LoadFileButton.TabIndex = 6;
            this.LoadFileButton.Text = "Abort";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
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
            this.FolderPathText.Size = new System.Drawing.Size(280, 20);
            this.FolderPathText.TabIndex = 5;
            // 
            // FolderBrowseButton
            // 
            this.FolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FolderBrowseButton.Location = new System.Drawing.Point(379, 12);
            this.FolderBrowseButton.Name = "FolderBrowseButton";
            this.FolderBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.FolderBrowseButton.TabIndex = 6;
            this.FolderBrowseButton.Text = "Find";
            this.FolderBrowseButton.UseVisualStyleBackColor = true;
            this.FolderBrowseButton.Click += new System.EventHandler(this.FolderBrowseButton_Click);
            // 
            // PortChecker
            // 
            this.PortChecker.Enabled = true;
            this.PortChecker.Interval = 10;
            this.PortChecker.Tick += new System.EventHandler(this.PortChecker_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ListOfNotes);
            this.groupBox1.Controls.Add(this.ScaleBar);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(460, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 650);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Note Chart Editor";
            // 
            // ListOfNotes
            // 
            this.ListOfNotes.AllowUserToAddRows = false;
            this.ListOfNotes.AllowUserToDeleteRows = false;
            this.ListOfNotes.AllowUserToResizeColumns = false;
            this.ListOfNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ListOfNotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ListOfNotes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Delay,
            this.AutoAdj,
            this.ManAdj,
            this.RedBox,
            this.YellowBox,
            this.BlueBox,
            this.GreenBox,
            this.BassBox});
            this.ListOfNotes.Location = new System.Drawing.Point(9, 54);
            this.ListOfNotes.Name = "ListOfNotes";
            this.ListOfNotes.Size = new System.Drawing.Size(389, 590);
            this.ListOfNotes.TabIndex = 2;
            this.ListOfNotes.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ListOfNotes_CellEndEdit);
            this.ListOfNotes.RowHeightChanged += new System.Windows.Forms.DataGridViewRowEventHandler(this.ListOfNotes_RowHeightChanged);
            // 
            // Index
            // 
            this.Index.Frozen = true;
            this.Index.HeaderText = "#";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Index.Width = 50;
            // 
            // Delay
            // 
            this.Delay.Frozen = true;
            this.Delay.HeaderText = "Delay";
            this.Delay.Name = "Delay";
            this.Delay.ReadOnly = true;
            this.Delay.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Delay.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Delay.Width = 50;
            // 
            // AutoAdj
            // 
            this.AutoAdj.Frozen = true;
            this.AutoAdj.HeaderText = "Auto Adj";
            this.AutoAdj.Name = "AutoAdj";
            this.AutoAdj.ReadOnly = true;
            this.AutoAdj.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AutoAdj.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AutoAdj.Width = 50;
            // 
            // ManAdj
            // 
            this.ManAdj.Frozen = true;
            this.ManAdj.HeaderText = "Manual Adj";
            this.ManAdj.Name = "ManAdj";
            this.ManAdj.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ManAdj.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ManAdj.Width = 50;
            // 
            // RedBox
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle11.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle11.NullValue")));
            this.RedBox.DefaultCellStyle = dataGridViewCellStyle11;
            this.RedBox.Frozen = true;
            this.RedBox.HeaderText = "R";
            this.RedBox.Name = "RedBox";
            this.RedBox.ReadOnly = true;
            this.RedBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RedBox.Width = 21;
            // 
            // YellowBox
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle12.NullValue")));
            this.YellowBox.DefaultCellStyle = dataGridViewCellStyle12;
            this.YellowBox.Frozen = true;
            this.YellowBox.HeaderText = "Y";
            this.YellowBox.Name = "YellowBox";
            this.YellowBox.ReadOnly = true;
            this.YellowBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.YellowBox.Width = 21;
            // 
            // BlueBox
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle13.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle13.NullValue")));
            this.BlueBox.DefaultCellStyle = dataGridViewCellStyle13;
            this.BlueBox.Frozen = true;
            this.BlueBox.HeaderText = "B";
            this.BlueBox.Name = "BlueBox";
            this.BlueBox.ReadOnly = true;
            this.BlueBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BlueBox.Width = 21;
            // 
            // GreenBox
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle14.NullValue")));
            this.GreenBox.DefaultCellStyle = dataGridViewCellStyle14;
            this.GreenBox.Frozen = true;
            this.GreenBox.HeaderText = "G";
            this.GreenBox.Name = "GreenBox";
            this.GreenBox.ReadOnly = true;
            this.GreenBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GreenBox.Width = 21;
            // 
            // BassBox
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle15.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle15.NullValue")));
            this.BassBox.DefaultCellStyle = dataGridViewCellStyle15;
            this.BassBox.Frozen = true;
            this.BassBox.HeaderText = "P";
            this.BassBox.Name = "BassBox";
            this.BassBox.ReadOnly = true;
            this.BassBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BassBox.Width = 21;
            // 
            // ScaleBar
            // 
            this.ScaleBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScaleBar.Location = new System.Drawing.Point(46, 16);
            this.ScaleBar.Maximum = 100;
            this.ScaleBar.Name = "ScaleBar";
            this.ScaleBar.Size = new System.Drawing.Size(352, 45);
            this.ScaleBar.TabIndex = 1;
            this.ScaleBar.TickFrequency = 5;
            this.ScaleBar.Value = 25;
            this.ScaleBar.Scroll += new System.EventHandler(this.ScaleBar_Scroll);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Scale";
            // 
            // SaveConfigButton
            // 
            this.SaveConfigButton.Location = new System.Drawing.Point(9, 353);
            this.SaveConfigButton.Name = "SaveConfigButton";
            this.SaveConfigButton.Size = new System.Drawing.Size(75, 23);
            this.SaveConfigButton.TabIndex = 13;
            this.SaveConfigButton.Text = "Save Config";
            this.SaveConfigButton.UseVisualStyleBackColor = true;
            this.SaveConfigButton.Click += new System.EventHandler(this.SaveConfigButton_Click);
            // 
            // LoadConfigButton
            // 
            this.LoadConfigButton.Location = new System.Drawing.Point(90, 353);
            this.LoadConfigButton.Name = "LoadConfigButton";
            this.LoadConfigButton.Size = new System.Drawing.Size(75, 23);
            this.LoadConfigButton.TabIndex = 14;
            this.LoadConfigButton.Text = "Load Config";
            this.LoadConfigButton.UseVisualStyleBackColor = true;
            this.LoadConfigButton.Click += new System.EventHandler(this.LoadConfigButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(171, 343);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 15;
            this.ResetButton.Text = "Reset Adj";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // ClearCaliBut
            // 
            this.ClearCaliBut.Location = new System.Drawing.Point(171, 365);
            this.ClearCaliBut.Name = "ClearCaliBut";
            this.ClearCaliBut.Size = new System.Drawing.Size(75, 23);
            this.ClearCaliBut.TabIndex = 16;
            this.ClearCaliBut.Text = "Reset Cali";
            this.ClearCaliBut.UseVisualStyleBackColor = true;
            this.ClearCaliBut.Click += new System.EventHandler(this.ClearCaliBut_Click);
            // 
            // PercentAdjBar
            // 
            this.PercentAdjBar.Location = new System.Drawing.Point(6, 266);
            this.PercentAdjBar.Maximum = 11000;
            this.PercentAdjBar.Minimum = 9000;
            this.PercentAdjBar.Name = "PercentAdjBar";
            this.PercentAdjBar.Size = new System.Drawing.Size(240, 45);
            this.PercentAdjBar.TabIndex = 17;
            this.PercentAdjBar.TickFrequency = 100;
            this.PercentAdjBar.Value = 10000;
            this.PercentAdjBar.Scroll += new System.EventHandler(this.PercentAdjBar_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 247);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Live Runtime % Adj:";
            // 
            // PercentAdjLabel
            // 
            this.PercentAdjLabel.AutoSize = true;
            this.PercentAdjLabel.Location = new System.Drawing.Point(116, 247);
            this.PercentAdjLabel.Name = "PercentAdjLabel";
            this.PercentAdjLabel.Size = new System.Drawing.Size(25, 13);
            this.PercentAdjLabel.TabIndex = 19;
            this.PercentAdjLabel.Text = "100";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 673);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.FolderBrowseButton);
            this.Controls.Add(this.FolderPathText);
            this.Controls.Add(this.GroupBox);
            this.Controls.Add(this.FileListBox);
            this.Controls.Add(this.LoadFolderButton);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Rock Band 2 Drum Bot Control Panel";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ListOfNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentAdjBar)).EndInit();
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
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Timer PortChecker;
        private System.Windows.Forms.ProgressBar ProgBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TimeTakenLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label SongLengthLabel;
        private System.Windows.Forms.Button StopPlayButton;
        private System.Windows.Forms.Label TimeTakenToPlayLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView ListOfNotes;
        private System.Windows.Forms.TrackBar ScaleBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Delay;
        private System.Windows.Forms.DataGridViewTextBoxColumn AutoAdj;
        private System.Windows.Forms.DataGridViewTextBoxColumn ManAdj;
        private System.Windows.Forms.DataGridViewImageColumn RedBox;
        private System.Windows.Forms.DataGridViewImageColumn YellowBox;
        private System.Windows.Forms.DataGridViewImageColumn BlueBox;
        private System.Windows.Forms.DataGridViewImageColumn GreenBox;
        private System.Windows.Forms.DataGridViewImageColumn BassBox;
        private System.Windows.Forms.Button LoadConfigButton;
        private System.Windows.Forms.Button SaveConfigButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button ClearCaliBut;
        private System.Windows.Forms.TrackBar PercentAdjBar;
        private System.Windows.Forms.Label PercentAdjLabel;
        private System.Windows.Forms.Label label5;
    }
}

