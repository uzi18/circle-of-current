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
            this.NumOfAtanEntries = new System.Windows.Forms.NumericUpDown();
            this.GenBut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumOfAsinEntries = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.MATH_MULTIPLIER = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NumOfAtanEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumOfAsinEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MATH_MULTIPLIER)).BeginInit();
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
            this.TableBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TableBox.Size = new System.Drawing.Size(609, 424);
            this.TableBox.TabIndex = 0;
            this.TableBox.WordWrap = false;
            // 
            // NumOfAtanEntries
            // 
            this.NumOfAtanEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumOfAtanEntries.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumOfAtanEntries.Location = new System.Drawing.Point(85, 445);
            this.NumOfAtanEntries.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NumOfAtanEntries.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumOfAtanEntries.Name = "NumOfAtanEntries";
            this.NumOfAtanEntries.Size = new System.Drawing.Size(62, 20);
            this.NumOfAtanEntries.TabIndex = 1;
            this.NumOfAtanEntries.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // GenBut
            // 
            this.GenBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenBut.Location = new System.Drawing.Point(546, 442);
            this.GenBut.Name = "GenBut";
            this.GenBut.Size = new System.Drawing.Size(75, 23);
            this.GenBut.TabIndex = 2;
            this.GenBut.Text = "Generate";
            this.GenBut.UseVisualStyleBackColor = true;
            this.GenBut.Click += new System.EventHandler(this.GenTable);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 448);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Atan Entries:";
            // 
            // NumOfAsinEntries
            // 
            this.NumOfAsinEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumOfAsinEntries.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumOfAsinEntries.Location = new System.Drawing.Point(243, 445);
            this.NumOfAsinEntries.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NumOfAsinEntries.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumOfAsinEntries.Name = "NumOfAsinEntries";
            this.NumOfAsinEntries.Size = new System.Drawing.Size(62, 20);
            this.NumOfAsinEntries.TabIndex = 1;
            this.NumOfAsinEntries.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 448);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Asin Entries:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(328, 448);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Multiplier:";
            // 
            // MATH_MULTIPLIER
            // 
            this.MATH_MULTIPLIER.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MATH_MULTIPLIER.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MATH_MULTIPLIER.Location = new System.Drawing.Point(385, 445);
            this.MATH_MULTIPLIER.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MATH_MULTIPLIER.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MATH_MULTIPLIER.Name = "MATH_MULTIPLIER";
            this.MATH_MULTIPLIER.Size = new System.Drawing.Size(62, 20);
            this.MATH_MULTIPLIER.TabIndex = 1;
            this.MATH_MULTIPLIER.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 474);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MATH_MULTIPLIER);
            this.Controls.Add(this.NumOfAsinEntries);
            this.Controls.Add(this.GenBut);
            this.Controls.Add(this.NumOfAtanEntries);
            this.Controls.Add(this.TableBox);
            this.Name = "Form1";
            this.Text = "TrigTableGen";
            ((System.ComponentModel.ISupportInitialize)(this.NumOfAtanEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumOfAsinEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MATH_MULTIPLIER)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TableBox;
        private System.Windows.Forms.NumericUpDown NumOfAtanEntries;
        private System.Windows.Forms.Button GenBut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumOfAsinEntries;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MATH_MULTIPLIER;
    }
}

