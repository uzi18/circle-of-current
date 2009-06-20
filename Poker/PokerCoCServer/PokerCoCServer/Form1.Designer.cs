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
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequestListener = new System.ComponentModel.BackgroundWorker();
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
            this.Index,
            this.PlayerIP,
            this.PlayerName});
            this.LobbyListGrid.Location = new System.Drawing.Point(12, 12);
            this.LobbyListGrid.Name = "LobbyListGrid";
            this.LobbyListGrid.RowHeadersVisible = false;
            this.LobbyListGrid.Size = new System.Drawing.Size(717, 240);
            this.LobbyListGrid.TabIndex = 0;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            // 
            // PlayerIP
            // 
            this.PlayerIP.HeaderText = "IP";
            this.PlayerIP.Name = "PlayerIP";
            this.PlayerIP.ReadOnly = true;
            // 
            // PlayerName
            // 
            this.PlayerName.HeaderText = "Name";
            this.PlayerName.Name = "PlayerName";
            this.PlayerName.ReadOnly = true;
            // 
            // RequestListener
            // 
            this.RequestListener.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RequestListener_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 264);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerName;
        private System.ComponentModel.BackgroundWorker RequestListener;
    }
}

