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
    public partial class frmWelcome : Form
    {
        private EnviroSettings mySettings;
        private AVRProject myProject;

        public frmWelcome(EnviroSettings mySettings, AVRProject myProject)
        {
            InitializeComponent();

            this.mySettings = mySettings;
            this.mySettings.FillListBox(listRecentFiles);

            this.myProject = myProject;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listRecentFiles.SelectedIndex >= 0)
            {
                string file = Program.CleanFilePath(mySettings.FilePathFromListBoxIndex(listRecentFiles.SelectedIndex));
                if (myProject.Open(file))
                {
                    mySettings.AddFileAsMostRecent(file);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error, Could Not Load Project");
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SaveResult res = myProject.CreateNew();
            if (res == SaveResult.Successful)
            {
                mySettings.AddFileAsMostRecent(myProject.FilePath);
                this.Close();
            }
            else if (res == SaveResult.Failed)
            {
                MessageBox.Show("Error While Saving Project Configuration");
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = Program.CleanFilePath(openFileDialog1.FileName);
                if (myProject.Open(file))
                {
                    mySettings.AddFileAsMostRecent(file);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error, Could Not Load Project");
                }
            }
        }

        private void listRecentFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listRecentFiles.SelectedIndex >= 0)
            {
                string file = Program.CleanFilePath(mySettings.FilePathFromListBoxIndex(listRecentFiles.SelectedIndex));
                if (myProject.Open(file))
                {
                    mySettings.AddFileAsMostRecent(file);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error, Could Not Load Project");
                }
            }
        }

        private void frmWelcome_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (myProject.IsReady)
            {
                if (mySettings.SaveRecentList() == false)
                {
                    MessageBox.Show("Error, Could Not Save Recent File List");
                }
            }
        }
    }
}
