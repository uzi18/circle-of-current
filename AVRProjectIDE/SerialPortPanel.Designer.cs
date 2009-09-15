namespace AVRProjectIDE
{
    partial class SerialPortPanel
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
            KillThread();

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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.dropPorts = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dropBaud = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRx = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtTx = new System.Windows.Forms.TextBox();
            this.timerStatusChecker = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.bgRxWorker = new System.ComponentModel.BackgroundWorker();
            this.barRxStatus = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.barSerPortTick = new System.Windows.Forms.ProgressBar();
            this.timerTextBoxUpdater = new System.Windows.Forms.Timer(this.components);
            this.bgTxWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dropPorts
            // 
            this.dropPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dropPorts.FormattingEnabled = true;
            this.dropPorts.Location = new System.Drawing.Point(39, 8);
            this.dropPorts.Name = "dropPorts";
            this.dropPorts.Size = new System.Drawing.Size(75, 21);
            this.dropPorts.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(667, 6);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(586, 6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Baud:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // dropBaud
            // 
            this.dropBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dropBaud.FormattingEnabled = true;
            this.dropBaud.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600",
            "76800",
            "115200",
            "230400"});
            this.dropBaud.Location = new System.Drawing.Point(165, 8);
            this.dropBaud.Name = "dropBaud";
            this.dropBaud.Size = new System.Drawing.Size(80, 21);
            this.dropBaud.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtRx);
            this.groupBox2.Location = new System.Drawing.Point(3, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(742, 318);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // txtRx
            // 
            this.txtRx.BackColor = System.Drawing.Color.Black;
            this.txtRx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRx.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRx.ForeColor = System.Drawing.Color.White;
            this.txtRx.Location = new System.Drawing.Point(3, 16);
            this.txtRx.Multiline = true;
            this.txtRx.Name = "txtRx";
            this.txtRx.ReadOnly = true;
            this.txtRx.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRx.Size = new System.Drawing.Size(736, 299);
            this.txtRx.TabIndex = 8;
            this.txtRx.WordWrap = false;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(666, 354);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(76, 23);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtTx
            // 
            this.txtTx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTx.Location = new System.Drawing.Point(59, 355);
            this.txtTx.Name = "txtTx";
            this.txtTx.Size = new System.Drawing.Size(601, 20);
            this.txtTx.TabIndex = 5;
            this.txtTx.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTx_KeyUp);
            // 
            // timerStatusChecker
            // 
            this.timerStatusChecker.Enabled = true;
            this.timerStatusChecker.Tick += new System.EventHandler(this.timerStatusChecker_Tick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 359);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Transmit:";
            // 
            // bgRxWorker
            // 
            this.bgRxWorker.WorkerSupportsCancellation = true;
            this.bgRxWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundRxWorker_DoWork);
            // 
            // barRxStatus
            // 
            this.barRxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.barRxStatus.Location = new System.Drawing.Point(358, 7);
            this.barRxStatus.Name = "barRxStatus";
            this.barRxStatus.Size = new System.Drawing.Size(222, 23);
            this.barRxStatus.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(265, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Rx Buffer Status:";
            // 
            // barSerPortTick
            // 
            this.barSerPortTick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.barSerPortTick.Location = new System.Drawing.Point(358, 7);
            this.barSerPortTick.MarqueeAnimationSpeed = 10;
            this.barSerPortTick.Name = "barSerPortTick";
            this.barSerPortTick.Size = new System.Drawing.Size(222, 23);
            this.barSerPortTick.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.barSerPortTick.TabIndex = 5;
            this.barSerPortTick.Visible = false;
            // 
            // timerTextBoxUpdater
            // 
            this.timerTextBoxUpdater.Enabled = true;
            this.timerTextBoxUpdater.Interval = 250;
            this.timerTextBoxUpdater.Tick += new System.EventHandler(this.timerTextBoxUpdater_Tick);
            // 
            // bgTxWorker
            // 
            this.bgTxWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgTxWorker_DoWork);
            // 
            // SerialPortPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Controls.Add(this.barSerPortTick);
            this.Controls.Add(this.barRxStatus);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dropBaud);
            this.Controls.Add(this.dropPorts);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtTx);
            this.Name = "SerialPortPanel";
            this.Size = new System.Drawing.Size(748, 382);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox dropPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox dropBaud;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtRx;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtTx;
        private System.Windows.Forms.Timer timerStatusChecker;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgRxWorker;
        private System.Windows.Forms.ProgressBar barRxStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar barSerPortTick;
        private System.Windows.Forms.Timer timerTextBoxUpdater;
        private System.ComponentModel.BackgroundWorker bgTxWorker;
    }
}
