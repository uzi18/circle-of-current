namespace SettingsGeneratorTool
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
            this.ListGrid = new System.Windows.Forms.DataGridView();
            this.SaveListBut = new System.Windows.Forms.Button();
            this.LoadListBut = new System.Windows.Forms.Button();
            this.OutputText = new System.Windows.Forms.TextBox();
            this.GenBut = new System.Windows.Forms.Button();
            this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefaultValueCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeCol = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ListGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ListGrid
            // 
            this.ListGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.ListGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ListGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCol,
            this.DefaultValueCol,
            this.TypeCol});
            this.ListGrid.Location = new System.Drawing.Point(12, 12);
            this.ListGrid.Name = "ListGrid";
            this.ListGrid.Size = new System.Drawing.Size(381, 472);
            this.ListGrid.TabIndex = 0;
            // 
            // SaveListBut
            // 
            this.SaveListBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveListBut.Location = new System.Drawing.Point(12, 490);
            this.SaveListBut.Name = "SaveListBut";
            this.SaveListBut.Size = new System.Drawing.Size(75, 23);
            this.SaveListBut.TabIndex = 1;
            this.SaveListBut.Text = "Save List";
            this.SaveListBut.UseVisualStyleBackColor = true;
            this.SaveListBut.Click += new System.EventHandler(this.SaveListBut_Click);
            // 
            // LoadListBut
            // 
            this.LoadListBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LoadListBut.Location = new System.Drawing.Point(93, 490);
            this.LoadListBut.Name = "LoadListBut";
            this.LoadListBut.Size = new System.Drawing.Size(75, 23);
            this.LoadListBut.TabIndex = 2;
            this.LoadListBut.Text = "LoadList";
            this.LoadListBut.UseVisualStyleBackColor = true;
            this.LoadListBut.Click += new System.EventHandler(this.LoadBut_Click);
            // 
            // OutputText
            // 
            this.OutputText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputText.Location = new System.Drawing.Point(399, 12);
            this.OutputText.Multiline = true;
            this.OutputText.Name = "OutputText";
            this.OutputText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputText.Size = new System.Drawing.Size(441, 472);
            this.OutputText.TabIndex = 3;
            this.OutputText.Text = "Output Goes Here";
            this.OutputText.WordWrap = false;
            // 
            // GenBut
            // 
            this.GenBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenBut.Location = new System.Drawing.Point(318, 490);
            this.GenBut.Name = "GenBut";
            this.GenBut.Size = new System.Drawing.Size(75, 23);
            this.GenBut.TabIndex = 5;
            this.GenBut.Text = "Generate";
            this.GenBut.UseVisualStyleBackColor = true;
            this.GenBut.Click += new System.EventHandler(this.GenBut_Click);
            // 
            // NameCol
            // 
            this.NameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NameCol.HeaderText = "Name";
            this.NameCol.Name = "NameCol";
            // 
            // DefaultValueCol
            // 
            this.DefaultValueCol.HeaderText = "Default Value";
            this.DefaultValueCol.MinimumWidth = 100;
            this.DefaultValueCol.Name = "DefaultValueCol";
            this.DefaultValueCol.Width = 125;
            // 
            // TypeCol
            // 
            this.TypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TypeCol.HeaderText = "Type";
            this.TypeCol.Items.AddRange(new object[] {
            "d",
            "ul",
            "sl",
            "ui",
            "si",
            "uc",
            "sc"});
            this.TypeCol.MinimumWidth = 50;
            this.TypeCol.Name = "TypeCol";
            this.TypeCol.Width = 50;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 525);
            this.Controls.Add(this.GenBut);
            this.Controls.Add(this.OutputText);
            this.Controls.Add(this.LoadListBut);
            this.Controls.Add(this.SaveListBut);
            this.Controls.Add(this.ListGrid);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ListGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ListGrid;
        private System.Windows.Forms.Button SaveListBut;
        private System.Windows.Forms.Button LoadListBut;
        private System.Windows.Forms.TextBox OutputText;
        private System.Windows.Forms.Button GenBut;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefaultValueCol;
        private System.Windows.Forms.DataGridViewComboBoxColumn TypeCol;
    }
}

