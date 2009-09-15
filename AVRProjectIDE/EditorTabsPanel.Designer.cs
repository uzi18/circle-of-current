namespace AVRProjectIDE
{
    partial class EditorTabsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.myTabControl = new System.Windows.Forms.TabControl();
            this.tabFileManage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkEscape = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkWordStart = new System.Windows.Forms.CheckBox();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFindFile = new System.Windows.Forms.Button();
            this.btnNewFile = new System.Windows.Forms.Button();
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.timerBackup = new System.Windows.Forms.Timer(this.components);
            this.myTabControl.SuspendLayout();
            this.tabFileManage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myTabControl
            // 
            this.myTabControl.Controls.Add(this.tabFileManage);
            this.myTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTabControl.Location = new System.Drawing.Point(0, 0);
            this.myTabControl.Name = "myTabControl";
            this.myTabControl.SelectedIndex = 0;
            this.myTabControl.Size = new System.Drawing.Size(738, 430);
            this.myTabControl.TabIndex = 0;
            // 
            // tabFileManage
            // 
            this.tabFileManage.Controls.Add(this.groupBox2);
            this.tabFileManage.Controls.Add(this.groupBox1);
            this.tabFileManage.Location = new System.Drawing.Point(4, 22);
            this.tabFileManage.Name = "tabFileManage";
            this.tabFileManage.Padding = new System.Windows.Forms.Padding(3);
            this.tabFileManage.Size = new System.Drawing.Size(730, 404);
            this.tabFileManage.TabIndex = 0;
            this.tabFileManage.Text = "New / Manage Files";
            this.tabFileManage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkEscape);
            this.groupBox2.Controls.Add(this.chkMatchCase);
            this.groupBox2.Controls.Add(this.chkWordStart);
            this.groupBox2.Controls.Add(this.chkWholeWord);
            this.groupBox2.Controls.Add(this.txtSearch);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Location = new System.Drawing.Point(6, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(718, 70);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search in All Files";
            // 
            // chkEscape
            // 
            this.chkEscape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEscape.AutoSize = true;
            this.chkEscape.Location = new System.Drawing.Point(466, 46);
            this.chkEscape.Name = "chkEscape";
            this.chkEscape.Size = new System.Drawing.Size(246, 17);
            this.chkEscape.TabIndex = 10;
            this.chkEscape.Text = "Use Escape Sequence ( \\\\ \\t \\r \\n Supported)";
            this.chkEscape.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(122, 45);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkMatchCase.TabIndex = 8;
            this.chkMatchCase.Text = "Match Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkWordStart
            // 
            this.chkWordStart.AutoSize = true;
            this.chkWordStart.Location = new System.Drawing.Point(228, 46);
            this.chkWordStart.Name = "chkWordStart";
            this.chkWordStart.Size = new System.Drawing.Size(77, 17);
            this.chkWordStart.TabIndex = 9;
            this.chkWordStart.Text = "Word Start";
            this.chkWordStart.UseVisualStyleBackColor = true;
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.Location = new System.Drawing.Point(6, 45);
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.Size = new System.Drawing.Size(86, 17);
            this.chkWholeWord.TabIndex = 7;
            this.chkWholeWord.Text = "Whole Word";
            this.chkWholeWord.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(6, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(625, 20);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(637, 17);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearchAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFindFile);
            this.groupBox1.Controls.Add(this.btnNewFile);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(172, 52);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add File To Project";
            // 
            // btnFindFile
            // 
            this.btnFindFile.Location = new System.Drawing.Point(87, 19);
            this.btnFindFile.Name = "btnFindFile";
            this.btnFindFile.Size = new System.Drawing.Size(75, 23);
            this.btnFindFile.TabIndex = 3;
            this.btnFindFile.Text = "Find";
            this.btnFindFile.UseVisualStyleBackColor = true;
            this.btnFindFile.Click += new System.EventHandler(this.btnFindFile_Click);
            // 
            // btnNewFile
            // 
            this.btnNewFile.Location = new System.Drawing.Point(6, 19);
            this.btnNewFile.Name = "btnNewFile";
            this.btnNewFile.Size = new System.Drawing.Size(75, 23);
            this.btnNewFile.TabIndex = 2;
            this.btnNewFile.Text = "New";
            this.btnNewFile.UseVisualStyleBackColor = true;
            this.btnNewFile.Click += new System.EventHandler(this.btnNewFile_Click);
            // 
            // timerCheck
            // 
            this.timerCheck.Enabled = true;
            this.timerCheck.Interval = 5000;
            this.timerCheck.Tick += new System.EventHandler(this.timerCheck_Tick);
            // 
            // timerBackup
            // 
            this.timerBackup.Enabled = true;
            this.timerBackup.Interval = 30000;
            this.timerBackup.Tick += new System.EventHandler(this.timerBackup_Tick);
            // 
            // EditorTabsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Controls.Add(this.myTabControl);
            this.Name = "EditorTabsPanel";
            this.Size = new System.Drawing.Size(738, 430);
            this.myTabControl.ResumeLayout(false);
            this.tabFileManage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl myTabControl;
        private System.Windows.Forms.Timer timerCheck;
        private System.Windows.Forms.Timer timerBackup;
        private System.Windows.Forms.TabPage tabFileManage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnNewFile;
        private System.Windows.Forms.Button btnFindFile;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox chkEscape;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.CheckBox chkWholeWord;
        private System.Windows.Forms.CheckBox chkWordStart;
    }
}
