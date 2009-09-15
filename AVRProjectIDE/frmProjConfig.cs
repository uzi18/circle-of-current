using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace AVRProjectIDE
{
    public partial class frmProjConfig : Form
    {
        #region Fields and Properties

        private AVRProject myProj;
        private EnviroSettings mySettings;
        private bool doNotAllowClose;

        #endregion

        public frmProjConfig(AVRProject myProj, EnviroSettings mySettings)
        {
            InitializeComponent();

            this.myProj = myProj;
            this.mySettings = mySettings;

            myBurner = new ProjectBurner(myProj);

            for (int i = 0; i < dropPart.Items.Count; i++)
            {
                string str = (string)dropPart.Items[i];
                str = str.Substring(str.IndexOf('=') + 2);
                str = str.Substring(0, str.IndexOf(' '));
                dropPart.Items[i] = str.Trim();
            }

            for (int i = 0; i < dropProg.Items.Count; i++)
            {
                string str = (string)dropProg.Items[i];
                str = str.Substring(0, str.IndexOf('='));
                dropProg.Items[i] = str.Trim();
            }

            doNotAllowClose = false;

            PopulateForm();
        }

        #region Saving and Loading

        private void frmProjEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormToProj();
            e.Cancel = doNotAllowClose;
        }

        private SaveResult SaveProj()
        {
            if (string.IsNullOrEmpty(myProj.FilePath))
            {
                return SaveProjAs();
            }
            else
            {
                if (myProj.Save(myProj.FilePath))
                {
                    return SaveResult.Successful;
                }
                else
                {
                    MessageBox.Show("Save Failed");
                    return SaveResult.Failed;
                }
            }
        }

        private SaveResult SaveProjAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.Filter = "AVR Project (*.avrproj)|*.avrproj"; 
            if (string.IsNullOrEmpty(myProj.FilePath) == false)
                sfd.InitialDirectory = myProj.FilePath.Substring(0, myProj.FilePath.LastIndexOf('\\'));
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FormToProj();
                myProj.FilePath = sfd.FileName;

                if (myProj.Save(sfd.FileName))
                {
                    return SaveResult.Successful;
                }
                else
                {
                    MessageBox.Show("Save Failed");
                    return SaveResult.Failed;
                }
            }
            else
            {
                return SaveResult.Cancelled;
            }
        }

        private void FormToProj()
        {
            myProj.OutputDir = txtOutputPath.Text;
            myProj.Device = (string)dropDevices.Items[dropDevices.SelectedIndex];
            myProj.ClockFreq = numClockFreq.Value;
            myProj.LinkerOptions = txtLinkerOptions.Text;
            myProj.OtherOptions = txtOtherOptions.Text;

            myProj.PackStructs = chklistOptions.GetItemChecked(2);
            myProj.ShortEnums = chklistOptions.GetItemChecked(3);
            myProj.UnsignedBitfields = chklistOptions.GetItemChecked(1);
            myProj.UnsignedChars = chklistOptions.GetItemChecked(0);

            myProj.UseInitStack = chkUseInitStack.Checked;
            myProj.InitStackAddr = Convert.ToUInt32("0x" + txtInitStackAddr.Text, 16);

            myProj.BurnOptions = txtBurnOpt.Text;
            myProj.BurnPart = (string)dropPart.Items[dropPart.SelectedIndex];
            myProj.BurnProgrammer = (string)dropProg.Items[dropProg.SelectedIndex];

            myProj.IncludeDirList.Clear();
            foreach (DataGridViewRow i in dgvIncPaths.Rows)
            {
                if (string.IsNullOrEmpty((string)i.Cells[0].Value) == false)
                    myProj.IncludeDirList.Add(Program.CleanFilePath(((string)i.Cells[0].Value).Trim('"').Trim()));
            }

            myProj.LibraryDirList.Clear();
            foreach (DataGridViewRow i in dgvLibPaths.Rows)
            {
                if (string.IsNullOrEmpty((string)i.Cells[0].Value) == false)
                    myProj.LibraryDirList.Add(Program.CleanFilePath(((string)i.Cells[0].Value).Trim('"').Trim()));
            }

            myProj.LinkObjList.Clear();
            myProj.LinkLibList.Clear();
            foreach (object i in listLinkObj.Items)
            {
                string s = (string)i;
                if (string.IsNullOrEmpty(s) == false)
                {
                    if(s.ToLower().Trim().EndsWith(".o"))
                    {
                        myProj.LinkObjList.Add(Program.CleanFilePath(s).Trim('"').Trim());
                    }
                    else if (s.ToLower().Trim().EndsWith(".a"))
                    {
                        myProj.LinkLibList.Add(Program.CleanFilePath(s).Trim('"').Trim());
                    }
                }
            }

            myProj.MemorySegList.Clear();
            foreach (DataGridViewRow i in dgvMemory.Rows)
            {
                if (string.IsNullOrEmpty((string)i.Cells[1].Value) == false)
                {
                    myProj.MemorySegList.Add(new MemorySegment((string)i.Cells[0].Value, (string)i.Cells[1].Value, Convert.ToUInt32("0x" + (string)i.Cells[2].Value, 16)));
                }
            }
        }

        private void PopulateForm()
        {
            txtOutputPath.Text = myProj.OutputDir;

            dropDevices.SelectedIndex = 0;
            if (dropDevices.Items.Contains(myProj.Device))
                dropDevices.SelectedIndex = dropDevices.Items.IndexOf(myProj.Device);

            dropPart.SelectedIndex = 0;
            if (dropPart.Items.Contains(myProj.BurnPart))
                dropPart.SelectedIndex = dropPart.Items.IndexOf(myProj.BurnPart);

            dropProg.SelectedIndex = 0;
            if (dropProg.Items.Contains(myProj.BurnProgrammer))
                dropProg.SelectedIndex = dropProg.Items.IndexOf(myProj.BurnProgrammer);

            txtOtherOptions.Text = myProj.OtherOptions;
            txtLinkerOptions.Text = myProj.LinkerOptions;
            txtInitStackAddr.Text = Convert.ToString(myProj.InitStackAddr, 16).ToUpper();
            numClockFreq.Value = myProj.ClockFreq;
            chkUseInitStack.Checked = myProj.UseInitStack;
            listOptimization.SelectedIndex = listOptimization.Items.IndexOf(myProj.Optimization);

            txtBurnOpt.Text = myProj.BurnOptions;

            foreach (string i in dropPart.Items)
            {
                if (i.ToLower() == myProj.BurnPart.ToLower())
                {
                    dropPart.SelectedIndex = dropPart.Items.IndexOf(i);
                    break;
                }
            }

            foreach (string i in dropProg.Items)
            {
                if (i.ToLower() == myProj.BurnProgrammer.ToLower())
                {
                    dropProg.SelectedIndex = dropProg.Items.IndexOf(i);
                    break;
                }
            }

            chklistOptions.SetItemChecked(2, myProj.PackStructs);
            chklistOptions.SetItemChecked(3, myProj.ShortEnums);
            chklistOptions.SetItemChecked(1, myProj.UnsignedBitfields);
            chklistOptions.SetItemChecked(0, myProj.UnsignedChars);

            listLinkObj.Items.Clear();
            foreach (string i in myProj.LinkLibList)
            {
                listLinkObj.Items.Add(i);
            }
            foreach (string i in myProj.LinkObjList)
            {
                listLinkObj.Items.Add(i);
            }

            dgvIncPaths.Rows.Clear();
            foreach (string s in myProj.IncludeDirList)
            {
                int i = dgvIncPaths.Rows.Add(new DataGridViewRow());
                dgvIncPaths.Rows[i].Cells[0].Value = s;
            }

            dgvLibPaths.Rows.Clear();
            foreach (string s in myProj.LibraryDirList)
            {
                int i = dgvLibPaths.Rows.Add(new DataGridViewRow());
                dgvLibPaths.Rows[i].Cells[0].Value = s;
            }

            dgvMemory.Rows.Clear();
            foreach (MemorySegment m in myProj.MemorySegList)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                string[] memStr = new string[3];
                string s = "Flash";
                if (m.Type.ToLower().Contains("flash"))
                    s = "Flash";
                else if (m.Type.ToLower().Contains("eeprom"))
                    s = "EEPROM";
                else if (m.Type.ToLower().Contains("sram"))
                    s = "SRAM";
                memStr[0] = s;
                memStr[1] = m.Name;
                memStr[2] = Convert.ToString(m.Addr, 16).ToUpper();
                dgvr.CreateCells(dgvMemory, memStr);
                int i = dgvMemory.Rows.Add(dgvr);
            }
        }

        #endregion

        #region Validation

        private void txtInitStackAddr_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Convert.ToUInt32(txtInitStackAddr.Text, 16);
                doNotAllowClose = false;
            }
            catch
            {
                MessageBox.Show("Invalid Stack Address");
                e.Cancel = true;
                doNotAllowClose = true;
            }
        }

        private void dgvMemory_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            e.Cancel = doNotAllowClose;
        }

        private void dgvMemory_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvMemory.Rows.Count)
                return;

            try
            {
                if (Convert.ToUInt32((string)dgvMemory.Rows[e.RowIndex].Cells[2].Value, 16) == 0)
                {
                    if (string.IsNullOrEmpty((string)dgvMemory.Rows[e.RowIndex].Cells[2].Value))
                    {
                        dgvMemory.Rows[e.RowIndex].Cells[2].Value = "0";
                    }
                }

                doNotAllowClose = false;
            }
            catch
            {
                MessageBox.Show("Invalid Memory Address");
                e.Cancel = true;
                doNotAllowClose = true;
            }

            if (string.IsNullOrEmpty((string)dgvMemory.Rows[e.RowIndex].Cells[1].Value))
            {
                MessageBox.Show("Please Specify a Memory Name");
                e.Cancel = true;
                doNotAllowClose = true;
            }
            else if (((string)dgvMemory.Rows[e.RowIndex].Cells[1].Value).Contains(' '))
            {
                MessageBox.Show("No Spaces are Allowed in the Memory Name");
                e.Cancel = true;
                doNotAllowClose = true;
            }
            else
            {
                doNotAllowClose = doNotAllowClose | false;
            }

            if (string.IsNullOrEmpty((string)dgvMemory.Rows[e.RowIndex].Cells[0].Value))
            {
                dgvMemory.Rows[e.RowIndex].Cells[0].Value = "Flash";
            }
        }

        #endregion

        #region Library and Link Object Page

        private void btnAddLib_Click(object sender, EventArgs e)
        {
            if (listAvailLibs.SelectedIndex >= 0)
            {
                if (listLinkObj.Items.Contains(listAvailLibs.SelectedItem) == false)
                {
                    int i = listLinkObj.Items.Count;
                    if (listLinkObj.SelectedIndex >= 0)
                    {
                        i = listLinkObj.SelectedIndex + 1;
                    }
                    listLinkObj.Items.Insert(i, listAvailLibs.SelectedItem);
                }
            }
        }

        private void btnLibRemove_Click(object sender, EventArgs e)
        {
            if (listLinkObj.SelectedIndex >= 0)
            {
                listLinkObj.Items.RemoveAt(listLinkObj.SelectedIndex);
            }
        }

        private void btnAddLibFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Link Object (*.o)|*.o|Library (*.a)|*.a";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int i = listLinkObj.Items.Count;
                if (listLinkObj.SelectedIndex >= 0)
                {
                    i = listLinkObj.SelectedIndex + 1;
                }
                listLinkObj.Items.Insert(i, ofd.FileName);
            }
        }

        private void btnLibMoveUp_Click(object sender, EventArgs e)
        {
            if (listLinkObj.SelectedIndex >= 1)
            {
                int i = listLinkObj.SelectedIndex;
                string s = (string)listLinkObj.Items[i];
                listLinkObj.Items.RemoveAt(i);
                listLinkObj.Items.Insert(i - 1, s);
                listLinkObj.SelectedIndex = i - 1;
            }
        }

        private void btnLibMoveDown_Click(object sender, EventArgs e)
        {
            if (listLinkObj.SelectedIndex >= 0 && listLinkObj.SelectedIndex < listLinkObj.Items.Count - 1)
            {
                int i = listLinkObj.SelectedIndex;
                string s = (string)listLinkObj.Items[i];
                listLinkObj.Items.RemoveAt(i);
                listLinkObj.Items.Insert(i + 1, s);
                listLinkObj.SelectedIndex = i + 1;
            }
        }

        #endregion

        #region Included Directory Pages

        private void btnIncDirAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                int i = dgvIncPaths.Rows.Add(new DataGridViewRow());
                dgvIncPaths.Rows[i].Cells[0].Value = fbd.SelectedPath;
            }
        }

        private void btnLibDirAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                int i = dgvLibPaths.Rows.Add(new DataGridViewRow());
                dgvLibPaths.Rows[i].Cells[0].Value = fbd.SelectedPath;
            }
        }

        private void txtOutputPath_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutputPath.Text))
            {
                doNotAllowClose = true;
                e.Cancel = true;
            }
            else
            {
                txtOutputPath.Text = Program.CleanFilePath(txtOutputPath.Text).Replace(' ', '_');
                doNotAllowClose = false;
                e.Cancel = false;
            }
        }

        private void btnIncPathMoveUp_Click(object sender, EventArgs e)
        {
            int i = -1;
            if (dgvIncPaths.SelectedRows.Count == 1 && dgvIncPaths.Rows.Count > 2)
            {
                i = dgvIncPaths.SelectedRows[0].Index;
            }
            else if (dgvIncPaths.SelectedCells.Count == 1 && dgvIncPaths.Rows.Count > 2)
            {
                i = dgvIncPaths.SelectedCells[0].RowIndex;
            }
            if (i != -1)
            {
                if (i > 0)
                {
                    if (dgvIncPaths.SelectedRows.Count == 1)
                    {
                        dgvIncPaths.SelectedRows[0].Selected = false;
                    }
                    else if (dgvIncPaths.SelectedCells.Count == 1)
                    {
                        dgvIncPaths.SelectedCells[0].Selected = false;
                    }
                    string s1 = (string)dgvIncPaths.Rows[i].Cells[0].Value;
                    string s2 = (string)dgvIncPaths.Rows[i - 1].Cells[0].Value;
                    dgvIncPaths.Rows[i - 1].Cells[0].Value = s1;
                    dgvIncPaths.Rows[i].Cells[0].Value = s2;
                    dgvIncPaths.Rows[i - 1].Selected = true;
                }
            }
        }

        private void btnIncPathMoveDown_Click(object sender, EventArgs e)
        {
            int i = -1;
            if (dgvIncPaths.SelectedRows.Count == 1 && dgvIncPaths.Rows.Count > 2)
            {
                i = dgvIncPaths.SelectedRows[0].Index;
            }
            else if (dgvIncPaths.SelectedCells.Count == 1 && dgvIncPaths.Rows.Count > 2)
            {
                i = dgvIncPaths.SelectedCells[0].RowIndex;
            }
            if (i != -1)
            {
                if (i < dgvIncPaths.Rows.Count - 2)
                {
                    if (dgvIncPaths.SelectedRows.Count == 1)
                    {
                        dgvIncPaths.SelectedRows[0].Selected = false;
                    }
                    else if (dgvIncPaths.SelectedCells.Count == 1)
                    {
                        dgvIncPaths.SelectedCells[0].Selected = false;
                    }
                    string s1 = (string)dgvIncPaths.Rows[i].Cells[0].Value;
                    string s2 = (string)dgvIncPaths.Rows[i + 1].Cells[0].Value;
                    dgvIncPaths.Rows[i + 1].Cells[0].Value = s1;
                    dgvIncPaths.Rows[i].Cells[0].Value = s2;
                    dgvIncPaths.Rows[i + 1].Selected = true;
                }
            }
        }

        private void btnLibPathMoveUp_Click(object sender, EventArgs e)
        {
            int i = -1;
            if (dgvLibPaths.SelectedRows.Count == 1 && dgvLibPaths.Rows.Count > 2)
            {
                i = dgvLibPaths.SelectedRows[0].Index;
            }
            else if (dgvLibPaths.SelectedCells.Count == 1 && dgvLibPaths.Rows.Count > 2)
            {
                i = dgvLibPaths.SelectedCells[0].RowIndex;
            }
            if (i != -1)
            {
                if (i > 0)
                {
                    if (dgvLibPaths.SelectedRows.Count == 1)
                    {
                        dgvLibPaths.SelectedRows[0].Selected = false;
                    }
                    else if (dgvLibPaths.SelectedCells.Count == 1)
                    {
                        dgvLibPaths.SelectedCells[0].Selected = false;
                    }
                    string s1 = (string)dgvLibPaths.Rows[i].Cells[0].Value;
                    string s2 = (string)dgvLibPaths.Rows[i - 1].Cells[0].Value;
                    dgvLibPaths.Rows[i - 1].Cells[0].Value = s1;
                    dgvLibPaths.Rows[i].Cells[0].Value = s2;
                    dgvLibPaths.Rows[i - 1].Selected = true;
                }
            }
        }

        private void btnLibPathMoveDown_Click(object sender, EventArgs e)
        {
            int i = -1;
            if (dgvLibPaths.SelectedRows.Count == 1 && dgvLibPaths.Rows.Count > 2)
            {
                i = dgvLibPaths.SelectedRows[0].Index;
            }
            else if (dgvLibPaths.SelectedCells.Count == 1 && dgvLibPaths.Rows.Count > 2)
            {
                i = dgvLibPaths.SelectedCells[0].RowIndex;
            }
            if (i != -1)
            {
                if (i < dgvLibPaths.Rows.Count - 2)
                {
                    if (dgvLibPaths.SelectedRows.Count == 1)
                    {
                        dgvLibPaths.SelectedRows[0].Selected = false;
                    }
                    else if (dgvLibPaths.SelectedCells.Count == 1)
                    {
                        dgvLibPaths.SelectedCells[0].Selected = false;
                    }
                    string s1 = (string)dgvLibPaths.Rows[i].Cells[0].Value;
                    string s2 = (string)dgvLibPaths.Rows[i + 1].Cells[0].Value;
                    dgvLibPaths.Rows[i + 1].Cells[0].Value = s1;
                    dgvLibPaths.Rows[i].Cells[0].Value = s2;
                    dgvLibPaths.Rows[i + 1].Selected = true;
                }
            }
        }

        #endregion

        #region Burning

        private ProjectBurner myBurner;

        private void btnBurnOnlyOpt_Click(object sender, EventArgs e)
        {
            FormToProj();
            myBurner.Burn(true);
        }

        #endregion
    }
}
