using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;

namespace AVRProjectIDE
{
    public class ProjectFile : ICloneable
    {
        #region Fields and Properties

        private string projDirPath;
        public string ProjDirPath
        {
            get { return Program.CleanFilePath(projDirPath); }
            set { projDirPath = Program.CleanFilePath(value); }
        }

        private string fileAbsPath;
        public string FileAbsPath
        {
            get { return Program.CleanFilePath(fileAbsPath); }
            set
            {
                fileAbsPath = Program.CleanFilePath(value);
            }
        }

        public string FileName
        {
            get { return FileAbsPath.Substring(fileAbsPath.LastIndexOf('\\') + 1); }
        }

        public string FileNameNoExt
        {
            get
            {
                string res = FileAbsPath.Substring(fileAbsPath.LastIndexOf('\\') + 1);
                if (res.Contains('.'))
                {
                    res = res.Substring(0, res.LastIndexOf('.'));
                }
                return res;
            }
        }

        public string FileRelPath
        {
            get { return Program.RelativePath(projDirPath, fileAbsPath); }
        }

        public string FileDir
        {
            get { return FileAbsPath.Substring(0, fileAbsPath.LastIndexOf('\\')); }
        }

        public string FileExt
        {
            get
            {
                if (fileAbsPath.Contains('.'))
                {
                    if (fileAbsPath.LastIndexOf('.') > fileAbsPath.LastIndexOf('\\'))
                    {
                        return FileAbsPath.Substring(fileAbsPath.LastIndexOf('.') + 1).ToLower();
                    }
                }
                return "";
            }
        }

        public string BackupPath
        {
            get { return FileAbsPath + ".backup." + FileExt; }
        }

        public bool BackupExists
        {
            get { return File.Exists(BackupPath); }
        }

        private string options;
        public string Options
        {
            get { return options.Trim(); }
            set { options = value.Trim(); }
        }

        private bool toCompile;
        public bool ToCompile
        {
            get { return toCompile; }
            set { toCompile = value; }
        }

        public bool Exists
        {
            get
            {
                return File.Exists(FileAbsPath);
            }
        }

        #endregion

        //public ProjectFile()
        //{
        //}

        public ProjectFile(string fileAbsPath, string projDirPath)
        {
            FileAbsPath = fileAbsPath;
            ProjDirPath = projDirPath;
            if (FileExt == "c" || FileExt == "s")
                ToCompile = true;
            options = "";
        }

        public void DeleteBackup()
        {
            if (BackupExists)
            {
                try
                {
                    File.Delete(BackupPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Deleting Backup\r\n" + ex.ToString());
                }
            }
        }

        #region ICloneable Members

        /// <summary>
        /// Makes a copy of ProjectFile, used by the background worker of the project builder
        /// </summary>
        /// <returns>Reference to the Cloned ProjectFile</returns>
        public object Clone()
        {
            return new ProjectFile(fileAbsPath, projDirPath);
        }

        #endregion
    }

    public class AVRProject : ICloneable
    {
        #region Project File Fields and Properties
        
        private string filePath;
        public string FilePath
        {
            get { return Program.CleanFilePath(filePath); }
            set { filePath = Program.CleanFilePath(value); }
        }

        private string dirPath;
        public string DirPath
        {
            get
            {
                string path = Program.CleanFilePath(FilePath);
                return path.Substring(0, path.LastIndexOf('\\'));
            }
            //get { return Program.CleanFilePath(dirPath); }
            //set { dirPath = Program.CleanFilePath(value); }
        }

        public string FileName
        {
            get { return FilePath.Substring(FilePath.LastIndexOf('\\') + 1); }
        }

        public string FileNameNoExt
        {
            get
            {
                string res = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                if (res.Contains('.'))
                {
                    res = res.Substring(0, res.LastIndexOf('.'));
                }
                return res;
            }
        }

        private bool isReady;
        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; }
        }

        #endregion

        #region Compilation Settings Fields and Properties

        private bool unsignedChars;
        public bool UnsignedChars
        {
            get { return unsignedChars; }
            set { unsignedChars = value; }
        }

        private bool unsignedBitfields;
        public bool UnsignedBitfields
        {
            get { return unsignedBitfields; }
            set { unsignedBitfields = value; }
        }

        private bool packStructs;
        public bool PackStructs
        {
            get { return packStructs; }
            set { packStructs = value; }
        }

        private bool shortEnums;
        public bool ShortEnums
        {
            get { return shortEnums; }
            set { shortEnums = value; }
        }

        private bool useInitStack;
        public bool UseInitStack
        {
            get { return useInitStack; }
            set { useInitStack = value; }
        }

        private uint initStackAddr;
        public uint InitStackAddr
        {
            get { return initStackAddr; }
            set { initStackAddr = value; }
        }

        private string otherOpt;
        public string OtherOptions
        {
            get { return otherOpt.Trim(); }
            set { otherOpt = value.Trim(); }
        }

        private string linkerOpt;
        public string LinkerOptions
        {
            get { return linkerOpt.Trim(); }
            set { linkerOpt = value.Trim(); }
        }

        private string optimization;
        public string Optimization
        {
            get { return optimization; }
            set
            {
                char c = value[value.IndexOf('O') + 1];
                if (c != '0' || c != '1' || c != '2' || c != '3')
                {
                    c = 's';
                }
                optimization = "-O" + c;
            }
        }

        private decimal clkFreq;
        public decimal ClockFreq
        {
            get { return clkFreq; }
            set { clkFreq = value; }
        }

        private string device;
        public string Device
        {
            get { return device; }
            set { device = value; }
        }

        private string outputDir;
        public string OutputDir
        {
            get { return outputDir; }
            set
            {
                string str = Program.CleanFilePath(value);
                if (string.IsNullOrEmpty(str))
                {
                    str = ".";
                }
                outputDir = str.Replace(' ', '_');
            }
        }

        #endregion

        #region AVRDUDE Fields and Properties

        private string burnPart;
        public string BurnPart
        {
            get { return burnPart.Trim(); }
            set { burnPart = value.Trim(); }
        }

        private string burnProg;
        public string BurnProgrammer
        {
            get { return burnProg.Trim(); }
            set { burnProg = value.Trim(); }
        }

        private string burnOpt;
        public string BurnOptions
        {
            get { return burnOpt.Trim(); }
            set { burnOpt = value.Trim(); }
        }

        #endregion

        #region Lists

        private Dictionary<string, ProjectFile> fileList = new Dictionary<string, ProjectFile>();
        public Dictionary<string, ProjectFile> FileList
        {
            get { return fileList; }
            set { fileList = value; }
        }

        private List<string> includeDirList = new List<string>();
        public List<string> IncludeDirList
        {
            get { return includeDirList; }
            set { includeDirList = value; }
        }

        private List<string> libraryDirList = new List<string>();
        public List<string> LibraryDirList
        {
            get { return libraryDirList; }
            set { libraryDirList = value; }
        }

        private List<string> linkObjList = new List<string>();
        public List<string> LinkObjList
        {
            get { return linkObjList; }
            set { linkObjList = value; }
        }

        private List<string> linkLibList = new List<string>();
        public List<string> LinkLibList
        {
            get { return linkLibList; }
            set { linkLibList = value; }
        }

        private List<MemorySegment> memorySegList = new List<MemorySegment>();
        public List<MemorySegment> MemorySegList
        {
            get { return memorySegList; }
            set { memorySegList = value; }
        }

        #endregion

        public AVRProject()
        {
            Reset();
        }

        #region XML Saving and Loading

        public SaveResult Save()
        {
            if (string.IsNullOrEmpty(filePath) == false)
            {
                if (Save(filePath))
                {
                    return SaveResult.Successful;
                }
                else
                {
                    return SaveResult.Failed;
                }
            }
            else
            {
                SaveResult res = CreateNew();
                if (res == SaveResult.Successful)
                {
                    return Save();
                }
                else
                {
                    return res;
                }
            }
        }

        public bool Save(string path)
        {
            bool success = true;
            XmlTextWriter xml = null;
            try
            {
                xml = new XmlTextWriter(path, null);

                xml.Indentation = 4;
                xml.Formatting = Formatting.Indented;
                xml.WriteStartDocument();

                xml.WriteStartElement("Project");

                xml.WriteElementString("DirPath", DirPath);

                xml.WriteElementString("Device", device);
                xml.WriteElementString("ClockFreq", clkFreq.ToString());
                xml.WriteElementString("LinkerOpt", linkerOpt);
                xml.WriteElementString("OtherOpt", otherOpt);
                xml.WriteElementString("OutputDir", outputDir);
                xml.WriteElementString("Optimization", optimization);

                xml.WriteElementString("UseInitStack", useInitStack.ToString().ToLower());
                xml.WriteElementString("InitStackAddr", Convert.ToString(initStackAddr, 16).ToUpper());

                xml.WriteElementString("PackStructs", packStructs.ToString().ToLower());
                xml.WriteElementString("ShortEnums", shortEnums.ToString().ToLower());
                xml.WriteElementString("UnsignedBitfields", unsignedBitfields.ToString().ToLower());
                xml.WriteElementString("UnsignedChars", unsignedChars.ToString().ToLower());

                xml.WriteStartElement("IncludeDirList");
                foreach (string i in IncludeDirList)
                {
                    xml.WriteElementString("DirPath", i);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("LibraryDirList");
                foreach (string i in LibraryDirList)
                {
                    xml.WriteElementString("DirPath", i);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("LinkObjList");
                foreach (string i in LinkObjList)
                {
                    xml.WriteElementString("Obj", i);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("LinkLibList");
                foreach (string i in LinkLibList)
                {
                    xml.WriteElementString("Lib", i);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("MemorySegList");
                foreach (MemorySegment i in MemorySegList)
                {
                    xml.WriteStartElement("Segment");
                    xml.WriteElementString("Name", i.Name);
                    xml.WriteElementString("Type", i.Type);
                    xml.WriteElementString("Addr", Convert.ToString(i.Addr, 16).ToUpper());
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteElementString("BurnPart", BurnPart);
                xml.WriteElementString("BurnProgrammer", BurnProgrammer);
                xml.WriteElementString("BurnOptions", BurnOptions);

                xml.WriteStartElement("FileList");
                foreach (KeyValuePair<string, ProjectFile> file in fileList)
                {
                    xml.WriteStartElement("File");
                    xml.WriteElementString("RelPath", file.Value.FileRelPath);
                    xml.WriteElementString("ToCompile", file.Value.ToCompile.ToString().ToLower());
                    xml.WriteElementString("Options", file.Value.Options);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteEndElement();

                xml.WriteEndDocument();
            }
            catch
            {
                success = false;
            }
            try
            {
                xml.Close();
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool Open(string path)
        {
            bool success = true;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlElement docx = doc.DocumentElement;

                XmlElement param;
                param = (XmlElement)docx.GetElementsByTagName("DirPath")[0];
                string xDirPath = Program.CleanFilePath(param.InnerText);
                param = (XmlElement)docx.GetElementsByTagName("ClockFreq")[0];
                ClockFreq = decimal.Parse(param.InnerText);
                param = (XmlElement)docx.GetElementsByTagName("Device")[0];
                Device = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("LinkerOpt")[0];
                LinkerOptions = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("OtherOpt")[0];
                OtherOptions = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("OutputDir")[0];
                OutputDir = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("Optimization")[0];
                Optimization = param.InnerText;

                param = (XmlElement)docx.GetElementsByTagName("UseInitStack")[0];
                UseInitStack = param.InnerText.ToLower().Trim() == "true";
                param = (XmlElement)docx.GetElementsByTagName("InitStackAddr")[0];
                InitStackAddr = Convert.ToUInt32("0x" + param.InnerText, 16);

                param = (XmlElement)docx.GetElementsByTagName("PackStructs")[0];
                PackStructs = param.InnerText.ToLower().Trim() == "true";
                param = (XmlElement)docx.GetElementsByTagName("ShortEnums")[0];
                ShortEnums = param.InnerText.ToLower().Trim() == "true";
                param = (XmlElement)docx.GetElementsByTagName("UnsignedBitfields")[0];
                UnsignedBitfields = param.InnerText.ToLower().Trim() == "true";
                param = (XmlElement)docx.GetElementsByTagName("UnsignedChars")[0];
                UnsignedChars = param.InnerText.ToLower().Trim() == "true";

                param = (XmlElement)docx.GetElementsByTagName("BurnPart")[0];
                BurnPart = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("BurnProgrammer")[0];
                BurnProgrammer = param.InnerText;
                param = (XmlElement)docx.GetElementsByTagName("BurnOptions")[0];
                BurnOptions = param.InnerText;

                IncludeDirList.Clear();
                XmlElement container = (XmlElement)docx.GetElementsByTagName("IncludeDirList")[0];
                XmlNodeList list = container.GetElementsByTagName("DirPath");
                foreach (XmlElement i in list)
                {
                    IncludeDirList.Add(i.InnerText);
                }

                LibraryDirList.Clear();
                container = (XmlElement)docx.GetElementsByTagName("IncludeDirList")[0];
                list = container.GetElementsByTagName("DirPath");
                foreach (XmlElement i in list)
                {
                    LibraryDirList.Add(i.InnerText);
                }

                LinkObjList.Clear();
                container = (XmlElement)docx.GetElementsByTagName("LinkObjList")[0];
                list = container.GetElementsByTagName("Obj");
                foreach (XmlElement i in list)
                {
                    LinkObjList.Add(i.InnerText);
                }

                LinkLibList.Clear();
                container = (XmlElement)docx.GetElementsByTagName("LinkLibList")[0];
                list = container.GetElementsByTagName("Lib");
                foreach (XmlElement i in list)
                {
                    LinkLibList.Add(i.InnerText);
                }

                MemorySegList.Clear();
                container = (XmlElement)docx.GetElementsByTagName("MemorySegList")[0];
                list = container.GetElementsByTagName("Segment");
                foreach (XmlElement i in list)
                {
                    XmlElement type = (XmlElement)i.GetElementsByTagName("Type")[0];
                    XmlElement name = (XmlElement)i.GetElementsByTagName("Name")[0];
                    XmlElement addr = (XmlElement)i.GetElementsByTagName("Addr")[0];
                    MemorySegList.Add(new MemorySegment(type.InnerText, name.InnerText, Convert.ToUInt32(addr.InnerText, 16)));
                }

                FileList.Clear();
                container = (XmlElement)docx.GetElementsByTagName("FileList")[0];
                list = container.GetElementsByTagName("File");

                string dirPath = Program.CleanFilePath(path);
                dirPath = dirPath.Substring(0, dirPath.LastIndexOf('\\'));

                List<ProjectFile> flistOld = new List<ProjectFile>();
                List<ProjectFile> flistNew = new List<ProjectFile>();

                foreach (XmlElement i in list)
                {
                    XmlElement relPath = (XmlElement)i.GetElementsByTagName("RelPath")[0];
                    XmlElement toComp = (XmlElement)i.GetElementsByTagName("ToCompile")[0];
                    XmlElement opt = (XmlElement)i.GetElementsByTagName("Options")[0];

                    string newPath = dirPath + "\\" + Program.CleanFilePath(relPath.InnerText);
                    string oldPath = xDirPath + "\\" + Program.CleanFilePath(relPath.InnerText);

                    ProjectFile newFile = new ProjectFile(newPath, dirPath.Substring(0, path.LastIndexOf('\\')));
                    flistNew.Add(newFile);

                    if (xDirPath != dirPath)
                    {
                        ProjectFile oldFile = new ProjectFile(oldPath, xDirPath);
                        flistOld.Add(oldFile);
                    }
                }

                int newCnt = 0;
                int oldCnt = 0;
                int total = flistNew.Count;

                if (flistOld.Count > 0)
                {
                    for (int i = 0; i < total && newCnt < (total + 1) / 2 && oldCnt < (total + 1) / 2; i++)
                    {
                        if (flistNew[i].Exists)
                            newCnt++;
                        if (flistOld[i].Exists)
                            oldCnt++;
                    }
                }
                else
                {
                    newCnt = total;
                }

                if (newCnt >= oldCnt)
                {
                    foreach (ProjectFile file in flistNew)
                    {
                        fileList.Add(file.FileName, file);
                    }
                    xDirPath = dirPath;
                }
                else
                {
                    foreach (ProjectFile file in flistOld)
                    {
                        fileList.Add(file.FileName, file);
                    }
                    dirPath = xDirPath;
                }
            }
            catch
            {
                success = false;
            }

            if (success)
            {
                filePath = path;
                isReady = true;
            }

            return success;
        }

        #endregion

        public void Reset()
        {
            clkFreq = 8000000;
            device = "atmega168";
            packStructs = true;
            shortEnums = true;
            unsignedBitfields = true;
            unsignedChars = true;
            useInitStack = false;
            initStackAddr = ushort.MaxValue;
            otherOpt = "";
            linkerOpt = "";
            outputDir = "output";
            optimization = "-Os";

            burnPart = device;
            burnProg = "avrisp";
            burnOpt = "";

            fileList.Clear();
            IncludeDirList.Clear();
            LibraryDirList.Clear();
            LinkObjList.Clear();
            LinkLibList.Clear();
            MemorySegList.Clear();

            isReady = false;
        }

        public SaveResult CreateNew()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "AVR Project (*.avrproj)|*.avrproj";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filePath = Program.CleanFilePath(sfd.FileName);
                dirPath = filePath.Substring(0, filePath.LastIndexOf('\\'));
                isReady = true;
                return SaveResult.Successful;
            }
            return SaveResult.Cancelled;
        }

        public bool GenerateMakefile()
        {
            return false;
        }

        #region ICloneable Members
        
        /// <summary>
        /// Makes a copy of AVRProject, used by the background worker of the project builder
        /// </summary>
        /// <returns>Reference to the Cloned AVRProject</returns>
        public object Clone()
        {
            object newObject = new AVRProject();
            ((AVRProject)newObject).BurnOptions = this.BurnOptions;
            ((AVRProject)newObject).BurnPart = this.BurnPart;
            ((AVRProject)newObject).BurnProgrammer = this.BurnProgrammer;
            ((AVRProject)newObject).ClockFreq = this.ClockFreq;
            ((AVRProject)newObject).Device = this.Device;
            ((AVRProject)newObject).dirPath = this.DirPath;
            ((AVRProject)newObject).FilePath = this.FilePath;
            ((AVRProject)newObject).InitStackAddr = this.InitStackAddr;
            ((AVRProject)newObject).LinkerOptions = this.LinkerOptions;
            ((AVRProject)newObject).Optimization = this.Optimization;
            ((AVRProject)newObject).OtherOptions = this.OtherOptions;
            ((AVRProject)newObject).OutputDir = this.OutputDir;
            ((AVRProject)newObject).PackStructs = this.PackStructs;
            ((AVRProject)newObject).ShortEnums = this.ShortEnums;
            ((AVRProject)newObject).UnsignedBitfields = this.UnsignedBitfields;
            ((AVRProject)newObject).UnsignedChars = this.UnsignedChars;
            ((AVRProject)newObject).UseInitStack = this.UseInitStack;

            ((AVRProject)newObject).fileList = new Dictionary<string, ProjectFile>();
            ((AVRProject)newObject).fileList.Clear();
            foreach (KeyValuePair<string, ProjectFile> file in this.FileList)
            {
                ProjectFile newFile = (ProjectFile)file.Value.Clone();
                ((AVRProject)newObject).fileList.Add(file.Key, newFile);
            }

            ((AVRProject)newObject).includeDirList = new List<string>();
            ((AVRProject)newObject).includeDirList.Clear();
            foreach (string dir in this.IncludeDirList)
            {
                ((AVRProject)newObject).includeDirList.Add((string)dir.Clone());
            }

            ((AVRProject)newObject).libraryDirList = new List<string>();
            ((AVRProject)newObject).libraryDirList.Clear();
            foreach (string dir in this.LibraryDirList)
            {
                ((AVRProject)newObject).libraryDirList.Add((string)dir.Clone());
            }

            ((AVRProject)newObject).linkLibList = new List<string>();
            ((AVRProject)newObject).linkLibList.Clear();
            foreach (string obj in this.LinkLibList)
            {
                ((AVRProject)newObject).linkLibList.Add((string)obj.Clone());
            }

            ((AVRProject)newObject).linkObjList = new List<string>();
            ((AVRProject)newObject).linkObjList.Clear();
            foreach (string obj in this.LinkObjList)
            {
                ((AVRProject)newObject).linkObjList.Add((string)obj.Clone());
            }

            ((AVRProject)newObject).memorySegList = new List<MemorySegment>();
            ((AVRProject)newObject).memorySegList.Clear();
            foreach (MemorySegment obj in this.MemorySegList)
            {
                ((AVRProject)newObject).memorySegList.Add(new MemorySegment(obj.Type, obj.Name, obj.Addr));
            }

            return newObject;
        }

        #endregion
    }
}