using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVRProjectIDE
{
  

    public partial class frmProjIDE : Form
    {
        #region Fields and Properties

        private SerialPortPanel serialPortControlPanel;
        private EditorTabsPanel myEditorTabsPanel;
        private AVRProject myProject;
        private EnviroSettings mySettings;
        
        private ProjectBuilder myBuilder;
        private ProjectBurner myBurner;

        public ListView ListSearchResults
        {
            get { return listSearchResults; }
        }

        public TabControl TabCtrlMessages
        {
            get { return tabCtrlMessages; }
        }

        #endregion

        public frmProjIDE(AVRProject myProject, EnviroSettings mySettings)
        {
            this.myProject = myProject;
            this.mySettings = mySettings;

            InitializeComponent();

            myBuilder = new ProjectBuilder(myProject, txtOutputMsg, listErrWarn);
            myBuilder.DoneWork += new ProjectBuilder.EventHandler(myBuilder_DoneWork);

            myBurner = new ProjectBurner(myProject);

            serialPortControlPanel = new SerialPortPanel("com0", 9600);
            tabSerialPort.Controls.Add(serialPortControlPanel);
            serialPortControlPanel.SerialPortException += new SerialPortPanel.SerialPortErrorHandler(serialPortControlPanel_SerialPortException);

            myEditorTabsPanel = new EditorTabsPanel(myProject, mySettings);
            splitFileTreeEditorTabs.Panel2.Controls.Add(myEditorTabsPanel);

            FillRecentProjects();

            RefreshFileTree();
        }

        #region Message Box Related

        private delegate void MessageTextBoxChange(TextBox textBox, string text, TextBoxChangeMode mode);

        private void TextBoxModify(TextBox textBox, string text, TextBoxChangeMode mode)
        {
            if (InvokeRequired)
            {
                Invoke(new MessageTextBoxChange(TextBoxModify), new object[] { textBox, text, mode, });
            }
            else
            {
                if (mode == TextBoxChangeMode.Append)
                    textBox.Text += text;
                else if (mode == TextBoxChangeMode.AppendNewLine)
                    textBox.Text += "\r\n" + text;
                else if (mode == TextBoxChangeMode.Prepend)
                    textBox.Text = text + textBox.Text;
                else if (mode == TextBoxChangeMode.PrependNewLine)
                    textBox.Text = text + "\r\n" + textBox.Text;
                else if (mode == TextBoxChangeMode.Set)
                    textBox.Text = text;
                else if (mode == TextBoxChangeMode.SetNewLine)
                    textBox.Text = text + "\r\n";
            }
        }

        void serialPortControlPanel_SerialPortException(Exception ex)
        {

        }

        #endregion

        #region Misc Form Window Related

        private void frmProjIDE_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myEditorTabsPanel.HasChanged)
            {
                DialogResult res = MessageBox.Show("You have unsaved changes. Do you want to save?", "Unsaved Project", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    myEditorTabsPanel.SaveAll();
                }
                else if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void frmProjIDE_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (KeyValuePair<string, ProjectFile> i in myProject.FileList)
            {
                i.Value.DeleteBackup();
            }
        }

        private void mbtnAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void mbtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public FileEditorTab GetActiveEditor()
        {
            string fname = myEditorTabsPanel.MyTabControl.SelectedTab.Text.TrimEnd('*').Trim();
            FileEditorTab fet;
            if (myEditorTabsPanel.MyListOfEditors.TryGetValue(fname, out fet))
            {
                return fet;
            }
            return null;
        }

        private void tbtnNewOrig_Click(object sender, EventArgs e)
        {
            myEditorTabsPanel.MyTabControl.SelectedIndex = 0;
        }

        private void tbtnOpenOrig_Click(object sender, EventArgs e)
        {
            myEditorTabsPanel.MyTabControl.SelectedIndex = 0;
        }        

        #endregion

        #region File Tree Functions and Fields

        private TreeNode rootNode;
        private TreeNode sourceNode;
        private TreeNode headerNode;

        public void RefreshFileTree()
        {
            rootNode = new TreeNode(myProject.FileName);
            sourceNode = new TreeNode("Source Files (.c or .S)");
            headerNode = new TreeNode("Header Files (.h)");

            rootNode.Checked = true;
            sourceNode.Checked = true;
            headerNode.Checked = false;

            rootNode.ToolTipText = "Double Click Me To Open Project Folder";
            sourceNode.ToolTipText = "Only C and Assembly Source Files";
            headerNode.ToolTipText = "Only Header Files";

            foreach (KeyValuePair<string, ProjectFile> f in myProject.FileList)
            {
                string fn = f.Value.FileName;
                TreeNode tn = new TreeNode(fn);

                tn.ToolTipText = f.Value.FileRelPath;

                if (f.Value.Exists == false)
                {
                    tn.ImageKey = "missing.ico";
                    tn.SelectedImageKey = "missing.ico";
                    tn.StateImageKey = "missing.ico";
                }
                else
                {
                    tn.ImageKey = "file.ico";
                    tn.SelectedImageKey = "file.ico";
                    tn.StateImageKey = "file.ico";
                }

                string ext = fn.ToLower();
                if (ext.EndsWith(".s") || ext.EndsWith(".c"))
                {
                    if (f.Value.ToCompile)
                    {
                        tn.Checked = true;
                    }

                    sourceNode.Nodes.Add(tn);
                }
                else if (ext.EndsWith(".h"))
                {
                    tn.Checked = false;
                    headerNode.Nodes.Add(tn);
                }

                if (f.Value.Exists)
                {
                    FileEditorTab fet;
                    if (myEditorTabsPanel.MyListOfEditors.TryGetValue(fn, out fet) == false)
                    {
                        fet = new FileEditorTab(f.Value, tn);
                        myEditorTabsPanel.MyListOfEditors.Add(fn, fet);
                        fet.FileChanged += new FileEditorTab.FileChangedHandler(myEditorTabsPanel.FileEditorTab_FileChanged);
                        if (myEditorTabsPanel.MyTabControl.TabPages.Contains(fet.MyTabPage) == false)
                        {
                            myEditorTabsPanel.MyTabControl.Controls.Add(fet.MyTabPage);
                            fet.WatchingForChange = true;
                        }
                    }
                }
            }

            rootNode.Nodes.Add(sourceNode);
            rootNode.Nodes.Add(headerNode);

            rootNode.ImageKey = "folder2.png";
            rootNode.SelectedImageKey = "folder2.png";
            rootNode.StateImageKey = "folder2.png";

            sourceNode.ImageKey = "folder.png";
            sourceNode.SelectedImageKey = "folder.png";
            sourceNode.StateImageKey = "folder.png";

            headerNode.ImageKey = "folder.png";
            headerNode.SelectedImageKey = "folder.png";
            headerNode.StateImageKey = "folder.png";

            treeFiles.Nodes.Clear();
            treeFiles.Nodes.Add(rootNode);

            treeFiles.ExpandAll();
        }

        private void treeFiles_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == rootNode || e.Node == sourceNode || e.Node == headerNode)
            {
                e.CancelEdit = true;
                return;
            }
        }

        private void treeFiles_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == rootNode)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = true;
        }

        private void treeFiles_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Node == rootNode || e.Node == sourceNode || e.Node == headerNode)
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Node.Parent == sourceNode && (e.Label.ToLower().EndsWith(".c") || e.Label.ToLower().EndsWith(".s")) == false)
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Node.Parent == headerNode && e.Label.ToLower().EndsWith(".h") == false)
            {
                e.CancelEdit = true;
                return;
            }

            string oldName = e.Node.Text;
            string newName = e.Label;
            newName = newName.Replace("\\", " ").Trim().Replace(" ", "_");
            if (oldName == newName)
            {
                e.CancelEdit = true;
                return;
            }
            if (newName.Length < 3)
            {
                e.CancelEdit = true;
                return;
            }
            if (newName.ToLower().EndsWith(".c") == false && newName.ToLower().EndsWith(".h") == false && newName.ToLower().EndsWith(".s") == false)
            {
                e.CancelEdit = true;
                return;
            }
            if (oldName.ToLower().EndsWith(".c") && newName.ToLower().EndsWith(".c") == false)
            {
                e.CancelEdit = true;
                return;
            }
            if (oldName.ToLower().EndsWith(".h") && newName.ToLower().EndsWith(".h") == false)
            {
                e.CancelEdit = true;
                return;
            }
            if (oldName.ToLower().EndsWith(".s") && newName.ToLower().EndsWith(".s") == false)
            {
                e.CancelEdit = true;
                return;
            }

            bool success = true;
            ProjectFile f;
            if (myProject.FileList.TryGetValue(oldName, out f))
            {
                try
                {
                    File.Move(f.FileAbsPath, f.FileDir + "\\" + newName);
                    f.FileAbsPath = f.FileDir + "\\" + newName;
                    e.Node.Text = newName;
                    myProject.FileList.Remove(oldName);
                    myProject.FileList.Add(newName, f);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error While Renaming " + oldName + "\r\n" + ex.ToString());
                    success = false;
                }
            }
            if (success)
            {
                FileEditorTab fet;
                if (myEditorTabsPanel.MyListOfEditors.TryGetValue(oldName, out fet))
                {
                    fet.MyTabPage.Text = newName;
                    fet.ExternChangeWatcher.Filter = newName;
                    myEditorTabsPanel.MyListOfEditors.Remove(oldName);
                    myEditorTabsPanel.MyListOfEditors.Add(newName, fet);
                }
            }
            else
            {
                e.Node.Text = oldName;
            }
        }

        private void treeFiles_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == sourceNode || e.Node == headerNode)
            {
                e.Cancel = true;
                return;
            }
        }

        private void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == sourceNode || e.Node == headerNode)
            {
                return;
            }
        }

        private void treeFiles_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == rootNode || e.Node == sourceNode || e.Node == headerNode)
            {
                e.Cancel = true;
                return;
            }

            ProjectFile f;
            if (myProject.FileList.TryGetValue(e.Node.Text, out f))
            {
                if (f.FileExt == "h")
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void treeFiles_DoubleClick(object sender, EventArgs e)
        {
            TreeNode n = treeFiles.SelectedNode;
            if (n == null)
                return;
            if (n == rootNode)
            {
                System.Diagnostics.Process.Start(myProject.DirPath);
                return;
            }
            else
            {
                myEditorTabsPanel.SwitchTo(n.Text);
            }
        }

        private void treeFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            TreeNode n = treeFiles.SelectedNode;
            if (n == null)
                return;

            ProjectFile file;
            if (myProject.FileList.TryGetValue(n.Text, out file))
            {
                if (file.FileExt != "h")
                {
                    frmFileOptions optDialog = new frmFileOptions(file);
                    optDialog.ShowDialog();
                }
            }
        }

        private void treeFiles_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ProjectFile f;
            if (myProject.FileList.TryGetValue(e.Node.Text, out f))
            {
                if (f.FileExt != "h")
                {
                    f.ToCompile = e.Node.Checked;
                }
                else
                {
                    f.ToCompile = false;
                }
            }
            else
            {
                MessageBox.Show("Error While Setting ToCompile for " + e.Node.Text);
            }
        }

        private void treeFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (
                    treeFiles.SelectedNode == rootNode ||
                    treeFiles.SelectedNode == sourceNode ||
                    treeFiles.SelectedNode == headerNode
                    )
                {
                    return;
                }
                else if (treeFiles.SelectedNode != null)
                {
                    TreeNode n = treeFiles.SelectedNode;
                    DialogResult res = MessageBox.Show("Do you want to remove (not delete)" + n.Name + "?", "Remove from Project", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                        return;
                    ProjectFile f;
                    if (myProject.FileList.TryGetValue(n.Text, out f))
                    {
                        myProject.FileList.Remove(n.Text);
                    }
                    FileEditorTab fet;
                    if (myEditorTabsPanel.MyListOfEditors.TryGetValue(n.Text, out fet))
                    {
                        myEditorTabsPanel.MyListOfEditors.Remove(n.Text);
                        myEditorTabsPanel.MyTabControl.Controls.Remove(fet.MyTabPage);
                        fet.FileChanged -= new FileEditorTab.FileChangedHandler(myEditorTabsPanel.FileEditorTab_FileChanged);
                    }

                    if (f.Exists)
                    {
                        res = MessageBox.Show("Do you want to delete" + n.Name + "?", "Delete from Disk", MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            try
                            {
                                File.Delete(f.FileAbsPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error, Cannot Delete File\r\n" + ex.ToString());
                            }
                        }
                    }

                    RefreshFileTree();
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                if (treeFiles.SelectedNode != null)
                {
                    treeFiles.SelectedNode.BeginEdit();
                }
            }
        }

        #endregion

        #region Advanced Editing Functions

        private void mbtnComment_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockComment();
            }
        }

        private void tbtnBlockComment_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockComment();
            }
        }

        private void mbtnUncomment_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockUncomment();
            }
        }

        private void tbtnBlockUncomment_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockUncomment();
            }
        }

        private void tbtnBlockTab_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockIndent();
            }
        }

        private void mbtnIndent_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockIndent();
            }
        }

        private void mbtnUnindent_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockUnindent();
            }
        }

        private void tbtnBlockUntab_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.BlockUnindent();
            }
        }

        private void mbtnClearBookmarks_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Markers.DeleteAll();
                fet.MyScint.FindReplace.ClearAllHighlights();
            }
        }

        #endregion

        #region Save / Open Button Functions

        private void mbtnSaveAllFiles_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void mbtnSaveFileAs_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.SaveAs();
            }
        }

        private void mbtnSaveConfigAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "AVR Project (*.avrproj)|*.avrproj";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (myProject.Save(sfd.FileName) == false)
                {
                    MessageBox.Show("Error While Saving Project Configuration File");
                }
            }
        }

        private void mbtnOpenProject_Click(object sender, EventArgs e)
        {
            if (myEditorTabsPanel.HasChanged)
            {
                DialogResult res = MessageBox.Show("You have unsaved changes. Do you want to save?", "Unsaved Project", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    myEditorTabsPanel.SaveAll();
                }
                else if (res == DialogResult.Cancel)
                {
                    return;
                }
            }

            string originalProject = myProject.FilePath;

            foreach (KeyValuePair<string, FileEditorTab> i in myEditorTabsPanel.MyListOfEditors)
            {
                myEditorTabsPanel.MyTabControl.TabPages.Remove(i.Value.MyTabPage);
            }
            myEditorTabsPanel.MyListOfEditors.Clear();
            treeFiles.Nodes.Clear();
            splitFileTreeEditorTabs.Panel2.Controls.Clear();
            myProject.Reset();

            while (true)
            {
                frmWelcome newWelcome = new frmWelcome(mySettings, myProject);
                newWelcome.ShowDialog();

                if (myProject.IsReady == false)
                {
                    if (myProject.Open(originalProject) == false)
                    {
                        MessageBox.Show("Error Reopening Original Project");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            myEditorTabsPanel = new EditorTabsPanel(myProject, mySettings);
            splitFileTreeEditorTabs.Panel2.Controls.Add(myEditorTabsPanel);

            FillRecentProjects();

            RefreshFileTree();
        }

        private void FillRecentProjects()
        {
            mbtnRecentProjects.DropDownItems.Clear();
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            foreach (string i in mySettings.RecentFileList)
            {
                ToolStripMenuItem recentMenuItem = new ToolStripMenuItem(i.Substring(i.LastIndexOf('\\') + 1));
                recentMenuItem.Click += new EventHandler(recentMenuItem_Click);
                items.Add(recentMenuItem);
            }
            foreach (ToolStripMenuItem i in items)
            {
                mbtnRecentProjects.DropDownItems.Add(i);
            }
        }

        void recentMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            string originalProject = myProject.FilePath;

            foreach (KeyValuePair<string, FileEditorTab> i in myEditorTabsPanel.MyListOfEditors)
            {
                myEditorTabsPanel.MyTabControl.TabPages.Remove(i.Value.MyTabPage);
            }
            myEditorTabsPanel.MyListOfEditors.Clear();
            treeFiles.Nodes.Clear();
            splitFileTreeEditorTabs.Panel2.Controls.Clear();
            myProject.Reset();

            string recentPath = "";
            foreach (string i in mySettings.RecentFileList)
            {
                if (i.EndsWith(item.Text))
                {
                    recentPath = i;
                    break;
                }
            }

            if (myProject.Open(recentPath) == false)
            {
                MessageBox.Show("Error Opening Recent Project");

                if (myProject.Open(originalProject) == false)
                {
                    MessageBox.Show("Error Reopening Original Project");
                    MessageBox.Show("This should never have happened, I do not know how to handle this situation, please restart the IDE");
                    this.Close();
                    return;
                }
            }

            myEditorTabsPanel = new EditorTabsPanel(myProject, mySettings);
            splitFileTreeEditorTabs.Panel2.Controls.Add(myEditorTabsPanel);

            FillRecentProjects();

            RefreshFileTree();
        }

        private void tbtnSaveOrig_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.Save();
            }
        }

        private void mbtnSaveCurFile_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.Save();
            }
        }

        private void tbtnSaveAll_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void SaveAll()
        {
            SaveResult res = myProject.Save();
            if (res == SaveResult.Failed)
            {
                MessageBox.Show("Error While Saving Project Configuration");
            }

            if (myEditorTabsPanel.SaveAll() == false)
            {
                MessageBox.Show("Error While Saving a Project File");
            }
        }

        #endregion        

        #region Basic Editing Button Functions

        private void mbtnUndo_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.UndoRedo.Undo();
            }
        }

        private void mbtnRedo_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.UndoRedo.Redo();
            }
        }

        private void tbtnUndo_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.UndoRedo.Undo();
            }
        }

        private void tbtnRedo_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.UndoRedo.Redo();
            }
        }

        private void mbtnCut_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Cut();
            }
        }

        private void tbtnCutOrig_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Cut();
            }
        }

        private void tbtnCopyOrig_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Copy();
            }
        }

        private void mbtnCopy_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Copy();
            }
        }

        private void mbtnPaste_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Paste();
            }
        }
        
        private void tbtnPasteOrig_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Clipboard.Paste();
            }
        }

        private void mbtnSelectAll_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.Selection.SelectAll();
            }
        }

        #endregion

        #region Find and Replace Buttons

        private void mbtnFindReplace_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.FindReplace.ShowFind();
            }
        }
        
        private void tbtnFind_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                fet.MyScint.FindReplace.ShowFind();
            }
        }

        private void mbtnFindNext_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                string last = fet.MyScint.FindReplace.LastFindString;
                fet.MyScint.FindReplace.FindNext(last, true, fet.MyScint.FindReplace.Flags);
            }
        }

        private void tbtnFindNext_Click(object sender, EventArgs e)
        {
            FileEditorTab fet = GetActiveEditor();
            if (fet != null)
            {
                string last = fet.MyScint.FindReplace.LastFindString;
                if (last != null)
                    fet.MyScint.FindReplace.FindNext(last, true, fet.MyScint.FindReplace.Flags);
            }
        }

        private void mbtnSearchAll_Click(object sender, EventArgs e)
        {
            myEditorTabsPanel.MyTabControl.SelectedIndex = 0;
        }

        private void tbtnSearch_Click(object sender, EventArgs e)
        {
            myEditorTabsPanel.MyTabControl.SelectedIndex = 0;
        }

        private void listSearchResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listSearchResults.SelectedItems[0] == null)
                return;
            if (string.IsNullOrEmpty(listSearchResults.SelectedItems[0].SubItems[1].Text))
                return;
            if (string.IsNullOrEmpty(listSearchResults.SelectedItems[0].SubItems[2].Text))
                return;
            if (string.IsNullOrEmpty(listSearchResults.SelectedItems[0].SubItems[3].Text))
                return;
            if (string.IsNullOrEmpty(listSearchResults.SelectedItems[0].SubItems[4].Text))
                return;
            string fname = listSearchResults.SelectedItems[0].SubItems[1].Text;
            int line = int.Parse(listSearchResults.SelectedItems[0].SubItems[2].Text);
            int start = int.Parse(listSearchResults.SelectedItems[0].SubItems[3].Text);
            int end = int.Parse(listSearchResults.SelectedItems[0].SubItems[4].Text);

            myEditorTabsPanel.SwitchTo(fname, start, end);
        }

        private void listErrWarn_DoubleClick(object sender, EventArgs e)
        {
            if (listErrWarn.SelectedItems.Count >= 1)
            {
                ListViewItem item = listErrWarn.SelectedItems[0];
                if (string.IsNullOrEmpty(item.SubItems[0].Text) || string.IsNullOrEmpty(item.SubItems[1].Text))
                    return;

                string file = item.SubItems[0].Text;
                int line = int.Parse(item.SubItems[1].Text) - 1;
                myEditorTabsPanel.SwitchTo(file, line);
            }
        }

        #endregion

        #region Actions of Config, Compile, and Burn Buttons

        private void mbtnConfig_Click(object sender, EventArgs e)
        {
            frmProjConfig wnd = new frmProjConfig(myProject, mySettings);
            wnd.ShowDialog();
        }

        private void tbtnConfig_Click(object sender, EventArgs e)
        {
            frmProjConfig wnd = new frmProjConfig(myProject, mySettings);
            wnd.ShowDialog();
        }

        private void mbtnCompile_Click(object sender, EventArgs e)
        {
            myBuilder.StartBuild();
        }

        private void tbtnCompile_Click(object sender, EventArgs e)
        {
            myBuilder.StartBuild();
        }

        void myBuilder_DoneWork(bool success)
        {
            if (success)
            {
                TextBoxModify(txtOutputMsg, "Build Succeeded", TextBoxChangeMode.AppendNewLine);
            }
            else
            {
                TextBoxModify(txtOutputMsg, "Build Failed", TextBoxChangeMode.AppendNewLine);
            }

            txtOutputMsg.Select(txtOutputMsg.TextLength - 1, 0);
            txtOutputMsg.ScrollToCaret();
        }

        private void tbtnBurn_Click(object sender, EventArgs e)
        {
            myBurner.Burn(false);
        }

        private void mbtnBurn_Click(object sender, EventArgs e)
        {
            myBurner.Burn(false);
        }

        private void mbtnExportMakefile_Click(object sender, EventArgs e)
        {
            if (Makefile.Generate(myProject))
            {
                MessageBox.Show("Makefile Generated in Your Project Directory");
            }
            else
            {
                MessageBox.Show("Error, failed to generate a makefile");
            }
        }

        #endregion

        private void mbtnHelpTopics_Click(object sender, EventArgs e)
        {
            
        }
    }
}
