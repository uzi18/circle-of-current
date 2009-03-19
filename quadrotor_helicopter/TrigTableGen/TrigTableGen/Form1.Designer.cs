namespace TrigTableGen
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
            this.TableBox = new System.Windows.Forms.TextBox();
            this.NumOfEntries = new System.Windows.Forms.NumericUpDown();
            this.GenAtanBut = new System.Windows.Forms.Button();
            this.GenProgress = new System.Windows.Forms.ProgressBar();
            this.GenAsinBut = new System.Windows.Forms.Button();
            this.DegreeSwitch = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumOfEntries)).BeginInit();
            this.SuspendLayout();
            // 
            // TableBox
            // 
            this.TableBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TableBox.Location = new System.Drawing.Point(12, 12);
            this.TableBox.Multiline = true;
            this.TableBox.Name = "TableBox";
            this.TableBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TableBox.Size = new System.Drawing.Size(609, 424);
            this.TableBox.TabIndex = 0;
            // 
            // NumOfEntries
            // 
            this.NumOfEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumOfEntries.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumOfEntries.Location = new System.Drawing.Point(12, 445);
            this.NumOfEntries.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NumOfEntries.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumOfEntries.Name = "NumOfEntries";
            this.NumOfEntries.Size = new System.Drawing.Size(120, 20);
            this.NumOfEntries.TabIndex = 1;
            this.NumOfEntries.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // GenAtanBut
            // 
            this.GenAtanBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenAtanBut.Location = new System.Drawing.Point(138, 443);
            this.GenAtanBut.Name = "GenAtanBut";
            this.GenAtanBut.Size = new System.Drawing.Size(75, 23);
            this.GenAtanBut.TabIndex = 2;
            this.GenAtanBut.Text = "Atan Table";
            this.GenAtanBut.UseVisualStyleBackColor = true;
            this.GenAtanBut.Click += new System.EventHandler(this.GenAtan);
            // 
            // GenProgress
            // 
            this.GenProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenProgress.Location = new System.Drawing.Point(415, 443);
            this.GenProgress.Name = "GenProgress";
            this.GenProgress.Size = new System.Drawing.Size(206, 23);
            this.GenProgress.TabIndex = 4;
            // 
            // GenAsinBut
            // 
            this.GenAsinBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenAsinBut.Location = new System.Drawing.Point(219, 443);
            this.GenAsinBut.Name = "GenAsinBut";
            this.GenAsinBut.Size = new System.Drawing.Size(75, 23);
            this.GenAsinBut.TabIndex = 2;
            this.GenAsinBut.Text = "Asin Table";
            this.GenAsinBut.UseVisualStyleBackColor = true;
            this.GenAsinBut.Click += new System.EventHandler(this.GenAsin);
            // 
            // DegreeSwitch
            // 
            this.DegreeSwitch.AutoSize = true;
            this.DegreeSwitch.Location = new System.Drawing.Point(324, 448);
            this.DegreeSwitch.Name = "DegreeSwitch";
            this.DegreeSwitch.Size = new System.Drawing.Size(52, 17);
            this.DegreeSwitch.TabIndex = 5;
            this.DegreeSwitch.Text = "Deg?";
            this.DegreeSwitch.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 474);
            this.Controls.Add(this.DegreeSwitch);
            this.Controls.Add(this.GenProgress);
            this.Controls.Add(this.GenAsinBut);
            this.Controls.Add(this.GenAtanBut);
            this.Controls.Add(this.NumOfEntries);
            this.Controls.Add(this.TableBox);
            this.Name = "Form1";
            this.Text = "TrigTableGen";
            ((System.ComponentModel.ISupportInitialize)(this.NumOfEntries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TableBox;
        private System.Windows.Forms.NumericUpDown NumOfEntries;
        private System.Windows.Forms.Button GenAtanBut;
        private System.Windows.Forms.ProgressBar GenProgress;
        private System.Windows.Forms.Button GenAsinBut;
        private System.Windows.Forms.CheckBox DegreeSwitch;
    }
}

