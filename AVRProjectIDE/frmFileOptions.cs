using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVRProjectIDE
{
    public partial class frmFileOptions : Form
    {
        private ProjectFile myFile;

        public frmFileOptions(ProjectFile myFile)
        {
            InitializeComponent();

            this.myFile = myFile;

            this.Text = "Set Compiler Options for " + myFile.FileName;

            txtFileOpts.Text = myFile.Options;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            myFile.Options = txtFileOpts.Text.Trim();
            this.Close();
        }
    }
}
