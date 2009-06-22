namespace PokerCoCServer
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
            this.ConnectionAcceptingTimer = new System.Windows.Forms.Timer(this.components);
            this.LobbyListGrid = new System.Windows.Forms.DataGridView();
            this.Player1Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Player1IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Player1Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequestListener = new System.ComponentModel.BackgroundWorker();
            this.LobbyListView = new System.Windows.Forms.ListView();
            this.PlayerIndex = new System.Windows.Forms.ColumnHeader();
            this.PlayerIP = new System.Windows.Forms.ColumnHeader();
            this.PlayerName = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.LobbyListGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ConnectionAcceptingTimer
            // 
            this.ConnectionAcceptingTimer.Enabled = true;
            this.ConnectionAcceptingTimer.Tick += new System.EventHandler(this.ConnectionAcceptingTimer_Tick);
            // 
            // LobbyListGrid
            // 
            this.LobbyListGrid.AllowUserToAddRows = false;
            this.LobbyListGrid.AllowUserToDeleteRows = false;
            this.LobbyListGrid.AllowUserToResizeRows = false;
            this.LobbyListGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LobbyListGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Player1Index,
            this.Player1IP,
            this.Player1Name});
            this.LobbyListGrid.Location = new System.Drawing.Point(12, 12);
            this.LobbyListGrid.Name = "LobbyListGrid";
            this.LobbyListGrid.RowHeadersVisible = false;
            this.LobbyListGrid.Size = new System.Drawing.Size(305, 240);
            this.LobbyListGrid.TabIndex = 0;
            // 
            // Player1Index
            // 
            this.Player1Index.HeaderText = "Index";
            this.Player1Index.Name = "Player1Index";
            // 
            // Player1IP
            // 
            this.Player1IP.HeaderText = "IP";
            this.Player1IP.Name = "Player1IP";
            this.Player1IP.ReadOnly = true;
            // 
            // Player1Name
            // 
            this.Player1Name.HeaderText = "Name";
            this.Player1Name.Name = "Player1Name";
            this.Player1Name.ReadOnly = true;
            // 
            // RequestListener
            // 
            this.RequestListener.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RequestListener_DoWork);
            // 
            // LobbyListView
            // 
            this.LobbyListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PlayerIndex,
            this.PlayerIP,
            this.PlayerName});
            this.LobbyListView.FullRowSelect = true;
            this.LobbyListView.Location = new System.Drawing.Point(323, 12);
            this.LobbyListView.MultiSelect = false;
            this.LobbyListView.Name = "LobbyListView";
            this.LobbyListView.Size = new System.Drawing.Size(296, 240);
            this.LobbyListView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.LobbyListView.TabIndex = 1;
            this.LobbyListView.UseCompatibleStateImageBehavior = false;
            this.LobbyListView.View = System.Windows.Forms.View.Details;
            // 
            // PlayerIndex
            // 
            this.PlayerIndex.Text = "Index";
            // 
            // PlayerIP
            // 
            this.PlayerIP.Text = "IP";
            // 
            // PlayerName
            // 
            this.PlayerName.Text = "Name";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 264);
            this.Controls.Add(this.LobbyListView);
            this.Controls.Add(this.LobbyListGrid);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LobbyListGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer ConnectionAcceptingTimer;
        private System.Windows.Forms.DataGridView LobbyListGrid;
        private System.ComponentModel.BackgroundWorker RequestListener;
        private System.Windows.Forms.DataGridViewTextBoxColumn Player1Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Player1IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn Player1Name;
        private System.Windows.Forms.ListView LobbyListView;
        private System.Windows.Forms.ColumnHeader PlayerIndex;
        private System.Windows.Forms.ColumnHeader PlayerIP;
        private System.Windows.Forms.ColumnHeader PlayerName;
    }
}

