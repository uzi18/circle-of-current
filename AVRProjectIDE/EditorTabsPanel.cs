using System;
using ScintillaNet;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AVRProjectIDE
{
    public partial class EditorTabsPanel : UserControl
    {
        #region Fields and Properties

        private Dictionary<string, FileEditorTab> myListOfEditors = new Dictionary<string, FileEditorTab>();
        public Dictionary<string, FileEditorTab> MyListOfEditors
        {
            get { return myListOfEditors; }
            set { myListOfEditors = value; }
        }

        public bool HasChanged
        {
            get
            {
                foreach (KeyValuePair<string, FileEditorTab> i in myListOfEditors)
                {
                    if (i.Value.HasChanged)
                        return true;
                }
                return false;
            }
        }

        private AVRProject myProject;
        private EnviroSettings mySettings;

        public TabControl MyTabControl
        {
            get { return myTabControl; }
            //set { myTabControl = value; }
        }

        #endregion

        public EditorTabsPanel(AVRProject myProject, EnviroSettings mySettings)
        {
            this.myProject = myProject;
            this.mySettings = mySettings;

            InitializeComponent();

            this.BackColor = SystemColors.Control;
            this.Dock = DockStyle.Fill;
        }

        #region File Management

        public bool SaveAll()
        {
            bool success = true;
            myProject.Save();
            foreach (KeyValuePair<string, FileEditorTab> i in myListOfEditors)
            {
                if (i.Value.Save() == false)
                {
                    success = false;
                }
            }
            return success;
        }

        public void BackupAll()
        {
            foreach (KeyValuePair<string, FileEditorTab> i in myListOfEditors)
            {
                i.Value.Backup();
            }
        }

        public void FileEditorTab_FileChanged(FileChangeEventType type, string oldName, string newName)
        {
            if (type == FileChangeEventType.Renamed)
            {
                FileEditorTab fet;
                if (myListOfEditors.TryGetValue(oldName, out fet))
                {
                    myListOfEditors.Remove(oldName);
                    myListOfEditors.Remove(newName);
                    myListOfEditors.Add(newName, fet);
                }
                ProjectFile file;
                if (myProject.FileList.TryGetValue(oldName, out file))
                {
                    myProject.FileList.Remove(oldName);
                    myProject.FileList.Remove(newName);
                    myProject.FileList.Add(newName, file);
                }
            }
            else if (type == FileChangeEventType.Deleted)
            {
                DialogResult res = MessageBox.Show("File " + oldName + " has been deleted, remove it from the editor?", "File Deleted", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    myListOfEditors.Remove(oldName);
                    myProject.FileList.Remove(oldName);
                }
                else
                {
                    FileEditorTab fet;
                    if (myListOfEditors.TryGetValue(oldName, out fet))
                    {
                        if (fet.Save() == false)
                        {
                            MessageBox.Show("Error Saving " + fet.MyFile.FileName);
                        }
                    }
                }
            }
            else if (type == FileChangeEventType.Changed)
            {
                DialogResult res = MessageBox.Show("File " + oldName + " has been changed externally.\r\nDiscard Changes and Reload It??", "File Changed", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    FileEditorTab fet;
                    if (myListOfEditors.TryGetValue(oldName, out fet))
                    {
                        if (fet.Load() == false)
                        {
                            MessageBox.Show("Error Reloading " + fet.MyFile.FileName);
                        }
                    }
                }
            }
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, FileEditorTab> i in myListOfEditors)
            {
                if (i.Value.MyTabPage.Parent != null)
                {
                    i.Value.CheckIfChanged();
                }
            }
        }

        private void btnNewFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = false;
            sfd.Filter = "C Code (*.c)|*.c|Header File (*.h)|*.h|Assembly Code (*.S)|*.S";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fname = sfd.FileName.Substring(sfd.FileName.IndexOf('\\') + 1);
                if (fname.Contains(' ') == false)
                {
                    ProjectFile pf = new ProjectFile(sfd.FileName, myProject.DirPath);
                    if (myProject.FileList.TryGetValue(pf.FileName, out pf) == false)
                    {
                        pf = new ProjectFile(sfd.FileName, myProject.DirPath);
                        myProject.FileList.Add(pf.FileName, pf);
                        ((frmProjIDE)(this.ParentForm)).RefreshFileTree();
                    }
                    else
                    {
                        MessageBox.Show("Please No Duplicate File Names, " + fname + " is already used.");
                    }
                }
                else
                {
                    MessageBox.Show("Please No Spaces in File Name");
                }
            }
        }

        private void btnFindFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "C Code (*.c)|*.c|Header File (*.h)|*.h|Assembly Code (*.S)|*.S";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.SafeFileName.Contains(' ') == false)
                {
                    ProjectFile pf = new ProjectFile(ofd.FileName, myProject.DirPath);
                    if (myProject.FileList.TryGetValue(pf.FileName, out pf) == false)
                    {
                        pf = new ProjectFile(ofd.FileName, myProject.DirPath);
                        myProject.FileList.Add(pf.FileName, pf);
                        ((frmProjIDE)(this.ParentForm)).RefreshFileTree();
                    }
                    else if (pf.Exists == false)
                    {
                        pf = new ProjectFile(ofd.FileName, myProject.DirPath);
                        myProject.FileList.Add(pf.FileName, pf);
                        ((frmProjIDE)(this.ParentForm)).RefreshFileTree();
                    }
                    else
                    {
                        MessageBox.Show("Please No Duplicate File Names, " + ofd.SafeFileName + " is already used.");
                    }
                }
                else
                {
                    MessageBox.Show("Please No Spaces in File Name");
                }
            }
        }

        private void timerBackup_Tick(object sender, EventArgs e)
        {
            BackupAll();
        }

        #endregion

        #region Switch To Functions

        public void SwitchTo(string file)
        {
            FileEditorTab fet;
            if (myListOfEditors.TryGetValue(file, out fet))
            {
                myTabControl.SelectedIndex = myTabControl.TabPages.IndexOf(fet.MyTabPage);
                fet.MyScint.Focus();
            }
        }

        public void SwitchTo(string file, int line)
        {
            FileEditorTab fet;
            if (myListOfEditors.TryGetValue(file, out fet))
            {
                myTabControl.SelectedIndex = myTabControl.TabPages.IndexOf(fet.MyTabPage);
                fet.MyScint.Focus();
                if (line < fet.MyScint.Lines.Count)
                    fet.MyScint.Caret.Goto(fet.MyScint.Lines[line].StartPosition);
            }
        }

        public void SwitchTo(string file, int start, int end)
        {
            FileEditorTab fet;
            if (myListOfEditors.TryGetValue(file, out fet))
            {
                myTabControl.SelectedIndex = myTabControl.TabPages.IndexOf(fet.MyTabPage);
                fet.MyScint.Focus();
                fet.MyScint.Selection.Range = new Range(start, end, fet.MyScint);
            }
        }

        #endregion

        #region Find Replace Search

        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PerformSearch();
        }

        private void PerformSearch()
        {
            string searchString = txtSearch.Text;

            if (string.IsNullOrEmpty(searchString))
                return;

            bool matchCase = chkMatchCase.Checked;
            bool wholeWord = chkWholeWord.Checked;
            bool wordStart = chkWordStart.Checked;
            bool escape = chkEscape.Checked;

            SearchFlags flags = SearchFlags.Empty;

            if (matchCase)
                flags |= SearchFlags.MatchCase;

            if (wholeWord)
                flags |= SearchFlags.WholeWord;

            if (wordStart)
                flags |= SearchFlags.WordStart;

            if (escape)
            {
                searchString = searchString.Replace("\\\\", "\\");
                searchString = searchString.Replace("\\t", "\t");
                searchString = searchString.Replace("\\r", "\r");
                searchString = searchString.Replace("\\n", "\n");
            }

            ListView resListView = ((frmProjIDE)(this.ParentForm)).ListSearchResults;
            resListView.Items.Clear();

            foreach (KeyValuePair<string, FileEditorTab> tab in myListOfEditors)
            {
                string fileName = tab.Value.MyFile.FileName;

                Scintilla scint = tab.Value.MyScint;

                List<Range> results = scint.FindReplace.FindAll(searchString, flags);

                foreach (Range r in results)
                {
                    int lineNum = scint.Lines.FromPosition(r.Start).Number;
                    string resLine = scint.Lines[lineNum].Text.TrimEnd();

                    string hoverTxt = "";
                    if (lineNum >= 2)
                        hoverTxt += scint.Lines[lineNum - 2].Text.TrimEnd() + "\r\n";
                    if (lineNum >= 1)
                        hoverTxt += scint.Lines[lineNum - 1].Text.TrimEnd() + "\r\n";

                    hoverTxt += scint.Lines[lineNum].Text.TrimEnd() + "\r\n";

                    if (lineNum < scint.Lines.Count - 1)
                        hoverTxt += scint.Lines[lineNum + 1].Text.TrimEnd() + "\r\n";
                    if (lineNum < scint.Lines.Count - 2)
                        hoverTxt += scint.Lines[lineNum + 2].Text.TrimEnd() + "\r\n";

                    hoverTxt = hoverTxt.TrimEnd();

                    ListViewItem lvi = new ListViewItem(new string[] { resLine, fileName, (lineNum + 1).ToString(), r.Start.ToString(), r.End.ToString(), });
                    lvi.ToolTipText = hoverTxt;

                    resListView.Items.Add(lvi);
                }
            }

            ((frmProjIDE)(this.ParentForm)).TabCtrlMessages.SelectedIndex = 2;
        }

        #endregion
    }

    public class FileEditorTab
    {

        #region Fields and Properties

        private ProjectFile myFile;
        public ProjectFile MyFile
        {
            get { return myFile; }
            //set { myFile = value; }
        }

        private bool hasChanged;
        public bool HasChanged
        {
            get { return hasChanged | myScint.Modified; }
            set { hasChanged = value; myScint.Modified = value; }
        }

        private TabPage myTabPage;
        public TabPage MyTabPage
        {
            get { return myTabPage; }
            //set { myTabPage = value; }
        }

        private Scintilla myScint;
        public Scintilla MyScint
        {
            get { return myScint; }
            //set { myScint = value; }
        }

        private TreeNode myNode;
        public TreeNode MyNode
        {
            get { return myNode; }
            //set { myNode = value; }
        }

        private FileSystemWatcher externChangeWatcher = new FileSystemWatcher();
        public FileSystemWatcher ExternChangeWatcher
        {
            get { return externChangeWatcher; }
        }
        public bool WatchingForChange
        {
            get { return externChangeWatcher.EnableRaisingEvents; }
            set { externChangeWatcher.EnableRaisingEvents = value; }
        }

        #endregion

        public FileEditorTab(ProjectFile myFile, TreeNode treeNode)
        {
            this.myFile = myFile;
            this.myNode = treeNode;

            myTabPage = new TabPage(this.myFile.FileName);

            myScint = new Scintilla();
            myScint.Dock = DockStyle.Fill;
            myScint.Folding.IsEnabled = true;
            myScint.Folding.MarkerScheme = FoldMarkerScheme.BoxPlusMinus;
            myScint.Margins[0].Width = 20;
            myScint.Margins[1].Width = 10;
            myScint.Margins[2].Width = 10;
            myScint.IsBraceMatching = true;
            myScint.MatchBraces = true;
            myScint.ConfigurationManager.Language = "cs";
            //myScint.ConfigurationManager.CustomLocation
            myScint.Scrolling.ScrollBars = ScrollBars.Vertical;
            myScint.LineWrap.Mode = WrapMode.Word;
            myScint.Indentation.SmartIndentType = SmartIndent.CPP;
            myScint.Indentation.TabIndents = true;
            myScint.Indentation.UseTabs = true;
            myScint.Indentation.IndentWidth = 4;
            myScint.Indentation.ShowGuides = true;
            myScint.Modified = false;
            myScint.TextChanged += new EventHandler(MakeChange);

            Load();

            myTabPage.Controls.Add(myScint);

            externChangeWatcher.Filter = myFile.FileName;
            externChangeWatcher.Path = myFile.FileDir + "\\";
            externChangeWatcher.IncludeSubdirectories = false;
            externChangeWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;

            externChangeWatcher.Renamed += new RenamedEventHandler(externChangeWatcher_Renamed);
            externChangeWatcher.Deleted += new FileSystemEventHandler(externChangeWatcher_Deleted);
            externChangeWatcher.Changed += new FileSystemEventHandler(externChangeWatcher_Changed);
        }

        #region Saving and Loading

        private bool WriteToFile(string path)
        {
            path = Program.CleanFilePath(path);
            bool success = true;
            StreamWriter writer = null;
            if (Program.MakeSurePathExists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                return false;
            try
            {
                writer = new StreamWriter(path);
                foreach (Line l in myScint.Lines)
                {
                    writer.WriteLine(l.Text.TrimEnd());
                }
            }
            catch
            {
                success = false;
            }
            try
            {
                writer.Close();
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public void Backup()
        {
            WriteToFile(myFile.BackupPath);
        }

        public bool Save()
        {
            bool oldWatcherState = externChangeWatcher.EnableRaisingEvents;
            externChangeWatcher.EnableRaisingEvents = false;
            if (WriteToFile(myFile.FileAbsPath) == false)
            {
                MessageBox.Show("Could Not Save File " + myFile.FileName, "Save Error");
                externChangeWatcher.EnableRaisingEvents = oldWatcherState;
                return false;
            }
            else
            {
                myFile.DeleteBackup();
                HasChanged = false;
                myTabPage.Text = myFile.FileName;
                externChangeWatcher.EnableRaisingEvents = oldWatcherState;
                return true;
            }
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string ext = myFile.FileExt;
            if (string.IsNullOrEmpty(ext) == false)
                sfd.Filter = ext + " File|*." + ext;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (WriteToFile(sfd.FileName) == false)
                {
                    MessageBox.Show("Could Not Save File " + sfd.FileName, "Save Error");
                }
                else
                {
                    myFile.DeleteBackup();
                    myFile.FileAbsPath = sfd.FileName;
                    myNode.Text = myFile.FileName;
                    myTabPage.Text = myFile.FileName;
                    externChangeWatcher.Filter = myFile.FileName;
                    HasChanged = false;
                }
            }
        }

        public bool Load()
        {
            if (myFile.BackupExists)
            {
                if (MessageBox.Show("A backup of " + myFile.FileName + " still exists, load that instead?", "Backup Found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    return Load(myFile.BackupPath);
            }
            return Load(myFile.FileAbsPath);
        }

        public bool Load(string fileAbsPath)
        {
            bool success = true;
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileAbsPath);

                string line = reader.ReadLine();

                while (line != null)
                {
                    myScint.Text += line.TrimEnd() + "\r\n";
                    line = reader.ReadLine();
                }
            }
            catch
            {
                success = false;
            }
            try
            {
                reader.Close();
            }
            catch
            {
                success = false;
            }

            if (myFile.BackupPath != fileAbsPath)
                myFile.FileAbsPath = fileAbsPath;

            externChangeWatcher.Filter = myFile.FileName;
            HasChanged = false;
            myTabPage.Text = myFile.FileName;
            myScint.UndoRedo.EmptyUndoBuffer();
            return success;
        }

        #endregion

        #region Edit Detection

        public bool CheckIfChanged()
        {
            if (HasChanged)
            {
                HasChanged = HasChanged;
            }
            if (HasChanged)
            {
                myTabPage.Text = myFile.FileName + " *";
            }
            else
            {
                myTabPage.Text = myFile.FileName;
            }
            return HasChanged;
        }

        private void MakeChange(object sender, EventArgs e)
        {
            HasChanged = true;
            myTabPage.Text = myFile.FileName + " *";
        }

        #endregion

        #region External File Change Watcher Events

        public event FileChangedHandler FileChanged;
        public delegate void FileChangedHandler(FileChangeEventType typeOfChange, string oldName, string newName);

        private void externChangeWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (this.myTabPage.Parent.InvokeRequired)
            {
                this.myTabPage.Parent.Invoke(new RenamedEventHandler(externChangeWatcher_Renamed), new object[] { sender, e, });
            }
            else
            {
                if (e.OldName != externChangeWatcher.Filter)
                    return;

                string oldName = myFile.FileName;
                myFile.FileAbsPath = e.FullPath;
                string newName = myFile.FileName;
                myTabPage.Text = newName;
                myNode.Text = newName;
                externChangeWatcher.Filter = newName;
                FileChanged(FileChangeEventType.Renamed, oldName, newName);
            }
        }

        private void externChangeWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (this.myTabPage.Parent.InvokeRequired)
            {
                this.myTabPage.Parent.Invoke(new FileSystemEventHandler(externChangeWatcher_Deleted), new object[] { sender, e, });
            }
            else
            {
                if (e.Name != externChangeWatcher.Filter)
                    return;

                FileChanged(FileChangeEventType.Deleted, myFile.FileName, myFile.FileName);
            }
        }

        private void externChangeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (this.myTabPage.Parent.InvokeRequired)
            {
                this.myTabPage.Parent.Invoke(new FileSystemEventHandler(externChangeWatcher_Deleted), new object[] { sender, e, });
            }
            else
            {
                if (e.Name != externChangeWatcher.Filter)
                    return;

                FileChanged(FileChangeEventType.Changed, myFile.FileName, myFile.FileName);
            }
        }

        #endregion

        #region Block Editing Functions

        public void BlockComment()
        {
            int startLineNum = myScint.Lines.FromPosition(myScint.Caret.Position).Number;
            if (myScint.Selection.Length > 0)
                startLineNum = myScint.Lines.FromPosition(myScint.Selection.Start).Number;
            int endLineNum = startLineNum;
            if (myScint.Selection.Length > 0)
                endLineNum = myScint.Lines.FromPosition(myScint.Selection.End).Number;

            int numOfLines = endLineNum - startLineNum + 1;

            for (int i = startLineNum, j = 0; i <= endLineNum; i++, j++)
            {
                string lineTxt = myScint.Lines[i].Text.TrimEnd();
                string noSpaceTxt = lineTxt.TrimStart();
                if (noSpaceTxt.Length > 0 && noSpaceTxt.StartsWith("//") == false)
                {
                    int insertHere = lineTxt.IndexOf(noSpaceTxt);
                    insertHere += myScint.Lines[i].StartPosition;
                    myScint.Text = myScint.Text.Insert(insertHere, "//");
                }
            }

            myScint.Selection.Start = myScint.Lines[startLineNum].StartPosition;
            myScint.GoTo.Position(myScint.Selection.Start);
            myScint.Selection.End = myScint.Lines[endLineNum].EndPosition;
        }

        public void BlockUncomment()
        {
            int startLineNum = myScint.Lines.FromPosition(myScint.Caret.Position).Number;
            if (myScint.Selection.Length > 0)
                startLineNum = myScint.Lines.FromPosition(myScint.Selection.Start).Number;
            int endLineNum = startLineNum;
            if (myScint.Selection.Length > 0)
                endLineNum = myScint.Lines.FromPosition(myScint.Selection.End).Number;

            int numOfLines = endLineNum - startLineNum + 1;

            for (int i = startLineNum, j = 0; i <= endLineNum; i++, j++)
            {
                string lineTxt = myScint.Lines[i].Text.TrimEnd();
                string noSpaceTxt = lineTxt.TrimStart();
                if (noSpaceTxt.StartsWith("//"))
                {
                    int removeFrom = lineTxt.IndexOf(noSpaceTxt);
                    removeFrom += myScint.Lines[i].StartPosition;
                    myScint.Text = myScint.Text.Remove(removeFrom, 2);
                }
            }

            myScint.Selection.Start = myScint.Lines[startLineNum].StartPosition;
            myScint.GoTo.Position(myScint.Selection.Start);
            myScint.Selection.End = myScint.Lines[endLineNum].EndPosition;
        }

        public void BlockIndent()
        {
            int startLineNum = myScint.Lines.FromPosition(myScint.Caret.Position).Number;
            if (myScint.Selection.Length > 0)
                startLineNum = myScint.Lines.FromPosition(myScint.Selection.Start).Number;
            int endLineNum = startLineNum;
            if (myScint.Selection.Length > 0)
                endLineNum = myScint.Lines.FromPosition(myScint.Selection.End).Number;

            int numOfLines = endLineNum - startLineNum + 1;

            for (int i = startLineNum, j = 0; i <= endLineNum; i++, j++)
            {
                string lineTxt = myScint.Lines[i].Text.TrimEnd();
                string noSpaceTxt = lineTxt.TrimStart();
                if (noSpaceTxt.Length > 0)
                {
                    int insertHere = lineTxt.IndexOf(noSpaceTxt);
                    insertHere += myScint.Lines[i].StartPosition;
                    myScint.Text = myScint.Text.Insert(insertHere, "\t");
                }
            }

            myScint.Selection.Start = myScint.Lines[startLineNum].StartPosition;
            myScint.GoTo.Position(myScint.Selection.Start);
            myScint.Selection.End = myScint.Lines[endLineNum].EndPosition;
        }

        public void BlockUnindent()
        {
            int startLineNum = myScint.Lines.FromPosition(myScint.Caret.Position).Number;
            if (myScint.Selection.Length > 0)
                startLineNum = myScint.Lines.FromPosition(myScint.Selection.Start).Number;
            int endLineNum = startLineNum;
            if (myScint.Selection.Length > 0)
                endLineNum = myScint.Lines.FromPosition(myScint.Selection.End).Number;

            int numOfLines = endLineNum - startLineNum + 1;

            for (int i = startLineNum, j = 0; i <= endLineNum; i++, j++)
            {
                string lineTxt = myScint.Lines[i].Text.TrimEnd();
                string noSpaceTxt = lineTxt.TrimStart();
                int firstTab = lineTxt.IndexOf('\t');
                int first4Space = lineTxt.IndexOf("    ");
                int firstChar = lineTxt.IndexOf(noSpaceTxt);
                if (lineTxt.Contains('\t') && firstTab < firstChar)
                {
                    int removeFrom = firstTab;
                    removeFrom += myScint.Lines[i].StartPosition;
                    myScint.Text = myScint.Text.Remove(removeFrom, 1);
                }
                else if (lineTxt.Contains("    ") && first4Space < firstChar)
                {
                    int removeFrom = first4Space;
                    removeFrom += myScint.Lines[i].StartPosition;
                    myScint.Text = myScint.Text.Remove(removeFrom, 1);
                }
            }

            myScint.Selection.Start = myScint.Lines[startLineNum].StartPosition;
            myScint.GoTo.Position(myScint.Selection.Start);
            myScint.Selection.End = myScint.Lines[endLineNum].EndPosition;
        }

        #endregion
    }
}
