namespace AVRTimerCalculator
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
            this.label1 = new System.Windows.Forms.Label();
            this.ClkFreqTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TimerResDD = new System.Windows.Forms.ComboBox();
            this.TotalTicksTB = new System.Windows.Forms.TextBox();
            this.OverflowsTB = new System.Windows.Forms.TextBox();
            this.RemainderTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.RealTimeTB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PrescalerDD = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.UseTotalTicksBut = new System.Windows.Forms.Button();
            this.UseOverflowRemainderBut = new System.Windows.Forms.Button();
            this.UseRealTimeBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Clk Freq (Hz):";
            // 
            // ClkFreqTB
            // 
            this.ClkFreqTB.Location = new System.Drawing.Point(89, 12);
            this.ClkFreqTB.Name = "ClkFreqTB";
            this.ClkFreqTB.Size = new System.Drawing.Size(125, 20);
            this.ClkFreqTB.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Timer Res:";
            // 
            // TimerResDD
            // 
            this.TimerResDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimerResDD.FormattingEnabled = true;
            this.TimerResDD.Items.AddRange(new object[] {
            "8 bit",
            "16 bit"});
            this.TimerResDD.Location = new System.Drawing.Point(89, 38);
            this.TimerResDD.Name = "TimerResDD";
            this.TimerResDD.Size = new System.Drawing.Size(75, 21);
            this.TimerResDD.TabIndex = 3;
            this.TimerResDD.SelectedIndexChanged += new System.EventHandler(this.TimerResDD_SelectedIndexChanged);
            // 
            // TotalTicksTB
            // 
            this.TotalTicksTB.Location = new System.Drawing.Point(89, 92);
            this.TotalTicksTB.Name = "TotalTicksTB";
            this.TotalTicksTB.Size = new System.Drawing.Size(125, 20);
            this.TotalTicksTB.TabIndex = 4;
            // 
            // OverflowsTB
            // 
            this.OverflowsTB.Location = new System.Drawing.Point(89, 118);
            this.OverflowsTB.Name = "OverflowsTB";
            this.OverflowsTB.Size = new System.Drawing.Size(125, 20);
            this.OverflowsTB.TabIndex = 4;
            // 
            // RemainderTB
            // 
            this.RemainderTB.Location = new System.Drawing.Point(89, 144);
            this.RemainderTB.Name = "RemainderTB";
            this.RemainderTB.Size = new System.Drawing.Size(125, 20);
            this.RemainderTB.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Total Ticks:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Overflows:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Remainder:";
            // 
            // RealTimeTB
            // 
            this.RealTimeTB.Location = new System.Drawing.Point(89, 170);
            this.RealTimeTB.Name = "RealTimeTB";
            this.RealTimeTB.Size = new System.Drawing.Size(125, 20);
            this.RealTimeTB.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Real Time:";
            // 
            // PrescalerDD
            // 
            this.PrescalerDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PrescalerDD.FormattingEnabled = true;
            this.PrescalerDD.Items.AddRange(new object[] {
            "(1) Clk/1",
            "(2) Clk/8",
            "(3) Clk/64",
            "(4) Clk/256",
            "(5) Clk/1024"});
            this.PrescalerDD.Location = new System.Drawing.Point(89, 65);
            this.PrescalerDD.Name = "PrescalerDD";
            this.PrescalerDD.Size = new System.Drawing.Size(125, 21);
            this.PrescalerDD.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Prescaler:";
            // 
            // UseTotalTicksBut
            // 
            this.UseTotalTicksBut.Location = new System.Drawing.Point(220, 92);
            this.UseTotalTicksBut.Name = "UseTotalTicksBut";
            this.UseTotalTicksBut.Size = new System.Drawing.Size(40, 20);
            this.UseTotalTicksBut.TabIndex = 11;
            this.UseTotalTicksBut.Text = "Use";
            this.UseTotalTicksBut.UseVisualStyleBackColor = true;
            this.UseTotalTicksBut.Click += new System.EventHandler(this.UseTotalTicksBut_Click);
            // 
            // UseOverflowRemainderBut
            // 
            this.UseOverflowRemainderBut.Location = new System.Drawing.Point(220, 118);
            this.UseOverflowRemainderBut.Name = "UseOverflowRemainderBut";
            this.UseOverflowRemainderBut.Size = new System.Drawing.Size(40, 46);
            this.UseOverflowRemainderBut.TabIndex = 11;
            this.UseOverflowRemainderBut.Text = "Use";
            this.UseOverflowRemainderBut.UseVisualStyleBackColor = true;
            this.UseOverflowRemainderBut.Click += new System.EventHandler(this.UseOverflowRemainderBut_Click);
            // 
            // UseRealTimeBut
            // 
            this.UseRealTimeBut.Location = new System.Drawing.Point(220, 170);
            this.UseRealTimeBut.Name = "UseRealTimeBut";
            this.UseRealTimeBut.Size = new System.Drawing.Size(40, 20);
            this.UseRealTimeBut.TabIndex = 11;
            this.UseRealTimeBut.Text = "Use";
            this.UseRealTimeBut.UseVisualStyleBackColor = true;
            this.UseRealTimeBut.Click += new System.EventHandler(this.UseRealTimeBut_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 204);
            this.Controls.Add(this.UseOverflowRemainderBut);
            this.Controls.Add(this.UseRealTimeBut);
            this.Controls.Add(this.UseTotalTicksBut);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.RealTimeTB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.RemainderTB);
            this.Controls.Add(this.OverflowsTB);
            this.Controls.Add(this.TotalTicksTB);
            this.Controls.Add(this.PrescalerDD);
            this.Controls.Add(this.TimerResDD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ClkFreqTB);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "AVR Timer Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ClkFreqTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox TimerResDD;
        private System.Windows.Forms.TextBox TotalTicksTB;
        private System.Windows.Forms.TextBox OverflowsTB;
        private System.Windows.Forms.TextBox RemainderTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox RealTimeTB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox PrescalerDD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button UseTotalTicksBut;
        private System.Windows.Forms.Button UseOverflowRemainderBut;
        private System.Windows.Forms.Button UseRealTimeBut;
    }
}

