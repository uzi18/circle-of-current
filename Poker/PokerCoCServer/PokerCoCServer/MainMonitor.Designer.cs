namespace PokerCoCServer
{
    partial class MainMonitor
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
            this.LobbyListView = new System.Windows.Forms.ListView();
            this.LobbyIDCol = new System.Windows.Forms.ColumnHeader();
            this.PlayerIPCol = new System.Windows.Forms.ColumnHeader();
            this.LobbyNameCol = new System.Windows.Forms.ColumnHeader();
            this.PlayerBankCol = new System.Windows.Forms.ColumnHeader();
            this.RefreshLobbyBtn = new System.Windows.Forms.Button();
            this.GameListView = new System.Windows.Forms.ListView();
            this.GameIdCol = new System.Windows.Forms.ColumnHeader();
            this.GameNameCol = new System.Windows.Forms.ColumnHeader();
            this.GamePlayerCol = new System.Windows.Forms.ColumnHeader();
            this.GameChipCol = new System.Windows.Forms.ColumnHeader();
            this.GamePlayerList = new System.Windows.Forms.ListView();
            this.PlayerIDCol = new System.Windows.Forms.ColumnHeader();
            this.PlayerNameCol = new System.Windows.Forms.ColumnHeader();
            this.PlayerChipsInPlayCol = new System.Windows.Forms.ColumnHeader();
            this.TimeoutTxt = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.ReqHandlerSleepTxt = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RoomManSleepTxt = new System.Windows.Forms.NumericUpDown();
            this.ConnCheckerSleepTxt = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.TimeoutTxt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReqHandlerSleepTxt)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RoomManSleepTxt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnCheckerSleepTxt)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LobbyListView
            // 
            this.LobbyListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.LobbyListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LobbyIDCol,
            this.PlayerIPCol,
            this.LobbyNameCol,
            this.PlayerBankCol});
            this.LobbyListView.FullRowSelect = true;
            this.LobbyListView.Location = new System.Drawing.Point(6, 6);
            this.LobbyListView.MultiSelect = false;
            this.LobbyListView.Name = "LobbyListView";
            this.LobbyListView.Size = new System.Drawing.Size(301, 302);
            this.LobbyListView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.LobbyListView.TabIndex = 1;
            this.LobbyListView.UseCompatibleStateImageBehavior = false;
            this.LobbyListView.View = System.Windows.Forms.View.Details;
            // 
            // LobbyIDCol
            // 
            this.LobbyIDCol.Text = "Index";
            // 
            // PlayerIPCol
            // 
            this.PlayerIPCol.Text = "IP";
            this.PlayerIPCol.Width = 103;
            // 
            // LobbyNameCol
            // 
            this.LobbyNameCol.Text = "Name";
            // 
            // PlayerBankCol
            // 
            this.PlayerBankCol.Text = "$$$";
            this.PlayerBankCol.Width = 49;
            // 
            // RefreshLobbyBtn
            // 
            this.RefreshLobbyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RefreshLobbyBtn.Location = new System.Drawing.Point(6, 314);
            this.RefreshLobbyBtn.Name = "RefreshLobbyBtn";
            this.RefreshLobbyBtn.Size = new System.Drawing.Size(75, 23);
            this.RefreshLobbyBtn.TabIndex = 2;
            this.RefreshLobbyBtn.Text = "Refresh";
            this.RefreshLobbyBtn.UseVisualStyleBackColor = true;
            this.RefreshLobbyBtn.Click += new System.EventHandler(this.RefreshLobbyBtn_Click);
            // 
            // GameListView
            // 
            this.GameListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.GameListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GameIdCol,
            this.GameNameCol,
            this.GamePlayerCol,
            this.GameChipCol});
            this.GameListView.FullRowSelect = true;
            this.GameListView.Location = new System.Drawing.Point(313, 6);
            this.GameListView.MultiSelect = false;
            this.GameListView.Name = "GameListView";
            this.GameListView.Size = new System.Drawing.Size(300, 302);
            this.GameListView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.GameListView.TabIndex = 1;
            this.GameListView.UseCompatibleStateImageBehavior = false;
            this.GameListView.View = System.Windows.Forms.View.Details;
            this.GameListView.SelectedIndexChanged += new System.EventHandler(this.GameListView_SelectedIndexChanged);
            // 
            // GameIdCol
            // 
            this.GameIdCol.Text = "ID";
            // 
            // GameNameCol
            // 
            this.GameNameCol.Text = "Name";
            this.GameNameCol.Width = 94;
            // 
            // GamePlayerCol
            // 
            this.GamePlayerCol.Text = "Players";
            this.GamePlayerCol.Width = 52;
            // 
            // GameChipCol
            // 
            this.GameChipCol.Text = "$$$";
            // 
            // GamePlayerList
            // 
            this.GamePlayerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.GamePlayerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PlayerIDCol,
            this.PlayerNameCol,
            this.PlayerChipsInPlayCol});
            this.GamePlayerList.FullRowSelect = true;
            this.GamePlayerList.Location = new System.Drawing.Point(619, 6);
            this.GamePlayerList.MultiSelect = false;
            this.GamePlayerList.Name = "GamePlayerList";
            this.GamePlayerList.Size = new System.Drawing.Size(258, 302);
            this.GamePlayerList.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.GamePlayerList.TabIndex = 1;
            this.GamePlayerList.UseCompatibleStateImageBehavior = false;
            this.GamePlayerList.View = System.Windows.Forms.View.Details;
            // 
            // PlayerIDCol
            // 
            this.PlayerIDCol.Text = "ID";
            // 
            // PlayerNameCol
            // 
            this.PlayerNameCol.Text = "Name";
            this.PlayerNameCol.Width = 115;
            // 
            // PlayerChipsInPlayCol
            // 
            this.PlayerChipsInPlayCol.Text = "$$$";
            this.PlayerChipsInPlayCol.Width = 51;
            // 
            // TimeoutTxt
            // 
            this.TimeoutTxt.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.TimeoutTxt.Location = new System.Drawing.Point(146, 14);
            this.TimeoutTxt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimeoutTxt.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.TimeoutTxt.Name = "TimeoutTxt";
            this.TimeoutTxt.Size = new System.Drawing.Size(120, 20);
            this.TimeoutTxt.TabIndex = 3;
            this.TimeoutTxt.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.TimeoutTxt.ValueChanged += new System.EventHandler(this.Performance_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Network Timeout:";
            // 
            // ReqHandlerSleepTxt
            // 
            this.ReqHandlerSleepTxt.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ReqHandlerSleepTxt.Location = new System.Drawing.Point(146, 40);
            this.ReqHandlerSleepTxt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ReqHandlerSleepTxt.Name = "ReqHandlerSleepTxt";
            this.ReqHandlerSleepTxt.Size = new System.Drawing.Size(120, 20);
            this.ReqHandlerSleepTxt.TabIndex = 3;
            this.ReqHandlerSleepTxt.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ReqHandlerSleepTxt.ValueChanged += new System.EventHandler(this.Performance_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Req Handler Sleep:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1069, 369);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.RoomManSleepTxt);
            this.tabPage2.Controls.Add(this.ConnCheckerSleepTxt);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.ReqHandlerSleepTxt);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.TimeoutTxt);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1061, 343);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RoomManSleepTxt
            // 
            this.RoomManSleepTxt.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.RoomManSleepTxt.Location = new System.Drawing.Point(146, 92);
            this.RoomManSleepTxt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.RoomManSleepTxt.Name = "RoomManSleepTxt";
            this.RoomManSleepTxt.Size = new System.Drawing.Size(120, 20);
            this.RoomManSleepTxt.TabIndex = 3;
            this.RoomManSleepTxt.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.RoomManSleepTxt.ValueChanged += new System.EventHandler(this.Performance_ValueChanged);
            // 
            // ConnCheckerSleepTxt
            // 
            this.ConnCheckerSleepTxt.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ConnCheckerSleepTxt.Location = new System.Drawing.Point(146, 66);
            this.ConnCheckerSleepTxt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ConnCheckerSleepTxt.Name = "ConnCheckerSleepTxt";
            this.ConnCheckerSleepTxt.Size = new System.Drawing.Size(120, 20);
            this.ConnCheckerSleepTxt.TabIndex = 3;
            this.ConnCheckerSleepTxt.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ConnCheckerSleepTxt.ValueChanged += new System.EventHandler(this.Performance_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Room Man Sleep:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Conn Checker Sleep:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.GamePlayerList);
            this.tabPage1.Controls.Add(this.GameListView);
            this.tabPage1.Controls.Add(this.LobbyListView);
            this.tabPage1.Controls.Add(this.RefreshLobbyBtn);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1061, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // MainMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 393);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainMonitor";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainMonitor_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainMonitor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.TimeoutTxt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReqHandlerSleepTxt)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RoomManSleepTxt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnCheckerSleepTxt)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LobbyListView;
        private System.Windows.Forms.ColumnHeader LobbyIDCol;
        private System.Windows.Forms.ColumnHeader PlayerIPCol;
        private System.Windows.Forms.ColumnHeader LobbyNameCol;
        private System.Windows.Forms.Button RefreshLobbyBtn;
        private System.Windows.Forms.ListView GameListView;
        private System.Windows.Forms.ColumnHeader GameNameCol;
        private System.Windows.Forms.ColumnHeader GamePlayerCol;
        private System.Windows.Forms.ColumnHeader GameChipCol;
        private System.Windows.Forms.ListView GamePlayerList;
        private System.Windows.Forms.ColumnHeader PlayerNameCol;
        private System.Windows.Forms.ColumnHeader PlayerChipsInPlayCol;
        private System.Windows.Forms.ColumnHeader GameIdCol;
        private System.Windows.Forms.ColumnHeader PlayerIDCol;
        private System.Windows.Forms.NumericUpDown TimeoutTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ReqHandlerSleepTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader PlayerBankCol;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown RoomManSleepTxt;
        private System.Windows.Forms.NumericUpDown ConnCheckerSleepTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}

