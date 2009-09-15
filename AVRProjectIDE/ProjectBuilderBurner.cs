using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace AVRProjectIDE
{
    public class ProjectBuilder
    {
        #region Fields and Properties

        private Dictionary<string, ProjectFile> myFileList;
        private AVRProject myProject;
        private Dictionary<string, ProjectFile> workingFileList;
        private AVRProject workingProject;
        private TextBox myOutput;
        private ListView myErrorList;
        private BackgroundWorker worker;

        #endregion

        #region Event Handler and Delegate

        public event EventHandler DoneWork;
        public delegate void EventHandler(bool success);

        #endregion

        public ProjectBuilder(AVRProject myProject, TextBox myOutput, ListView myErrorList)
        {
            this.myProject = myProject;
            this.myFileList = myProject.FileList;
            this.myOutput = myOutput;
            this.myErrorList = myErrorList;

            this.worker = new BackgroundWorker();
            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        #region Background Worker

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DoneWork((bool)e.Result);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Build();
        }

        #endregion

        #region Form Control Modification

        private delegate void TextBoxCallback(TextBox box, string text, TextBoxChangeMode mode);

        private void TextBoxModify(TextBox box, string text, TextBoxChangeMode mode)
        {
            if (box.InvokeRequired)
            {
                box.Invoke(new TextBoxCallback(TextBoxModify), new object[] { box, text, mode, });
            }
            else
            {
                if (mode == TextBoxChangeMode.Append)
                    box.Text += text;
                else if (mode == TextBoxChangeMode.AppendNewLine)
                    box.Text += "\r\n" + text;
                else if (mode == TextBoxChangeMode.Prepend)
                    box.Text = text + box.Text;
                else if (mode == TextBoxChangeMode.PrependNewLine)
                    box.Text = text + "\r\n" + box.Text;
                else if (mode == TextBoxChangeMode.Set)
                    box.Text = text;
                else if (mode == TextBoxChangeMode.SetNewLine)
                    box.Text = text + "\r\n";
            }
        }

        private delegate void ListViewCallback(ListView box, ListViewItem item, ListViewChangeMode mode);

        private void ListViewModify(ListView box, ListViewItem item, ListViewChangeMode mode)
        {
            if (box.InvokeRequired)
            {
                box.Invoke(new ListViewCallback(ListViewModify), new object[] { box, item, mode, });
            }
            else
            {
                if (mode == ListViewChangeMode.Add)
                    box.Items.Add(item);
                else if (mode == ListViewChangeMode.Clear)
                    box.Items.Clear();
            }
        }

        #endregion

        #region Public Methods

        public void StartBuild()
        {
            workingFileList = new Dictionary<string, ProjectFile>();
            workingFileList.Clear();
            foreach (KeyValuePair<string, ProjectFile> file in myFileList)
            {
                ProjectFile newFile = (ProjectFile)file.Value.Clone();
                workingFileList.Add(file.Key, newFile);
            }

            workingProject = (AVRProject)myProject.Clone();

            TextBoxModify(myOutput, "", TextBoxChangeMode.Set);
            ListViewModify(myErrorList, null, ListViewChangeMode.Clear);

            worker.RunWorkerAsync();
        }

        #endregion

        #region Private Build Methods

        private bool Build()
        {
            bool result = true;

            string objFiles = "";
            foreach (KeyValuePair<string, ProjectFile> file in workingFileList)
            {
                if (file.Value.Exists && file.Value.ToCompile && file.Value.FileExt != "h")
                {
                    bool objRes = Compile(file.Value);
                    result &= objRes;
                    if (objRes)
                    {
                        objFiles += file.Value.FileNameNoExt + ".o ";
                    }
                }
            }

            bool elfRes = CreateELF(objFiles);

            result &= elfRes;

            CleanOD(objFiles);

            if (elfRes)
            {
                result &= CreateHex();
                CreateEEP();
                
                //CreateLSS();

                ReadSize();
            }

            return result;
        }

        private bool Compile(ProjectFile file)
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            if (Program.MakeSurePathExists(outputAbsPath) == false)
                return false;

            string objectFileAbsPath = outputAbsPath + "\\" + file.FileNameNoExt + ".o";

            if (File.Exists(objectFileAbsPath))
            {
                try
                {
                    File.Delete(objectFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: object file could not be deleted at " + objectFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string args = "";
            foreach (string path in workingProject.IncludeDirList)
            {
                if(string.IsNullOrEmpty(path) == false)
                    args += "-I\"" + path + "\" ";
            }
            //args += "-I\"" + file.FileDir + "\" ";

            string checklist = "";
            
            if (myProject.PackStructs)
                checklist += "-fpack-struct ";

            if (myProject.ShortEnums)
                checklist += "-fshort-enums ";

            if (myProject.UnsignedChars)
                checklist += "-funsigned-char ";

            if (myProject.UnsignedBitfields)
                checklist += "-funsigned-bitfields ";

            string asmflags = "";

            if (file.FileExt == "s")
                asmflags += "-x assembler-with-cpp -Wa,-gdwarf2";

            args += String.Format(" -mmcu={0} -Wall -gdwarf-2 -std=gnu99 -DF_CPU={1}UL {2} {3} {4} -MD -MP -MT {5}.o {6} -c {7} \"{8}\"",
                workingProject.Device,
                Math.Round(myProject.ClockFreq),
                workingProject.Optimization,
                checklist,
                workingProject.OtherOptions,
                file.FileNameNoExt,
                asmflags,
                file.Options,
                file.FileAbsPath.Replace('\\', '/')
            );

            TextBoxModify(myOutput, "Execute: avr-gcc " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-gcc", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process avrgcc = new Process();
            avrgcc.StartInfo = psi;
            try
            {
                if (avrgcc.Start())
                {
                    StreamReader stderr = avrgcc.StandardError;
                    ReadErrAndWarnings(stderr);
                    StreamReader stdout = avrgcc.StandardOutput;
                    ReadErrAndWarnings(stdout);
                    avrgcc.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to start avr-gcc", TextBoxChangeMode.AppendNewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to start avr-gcc\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                return false;
            }

            if (File.Exists(objectFileAbsPath))
            {
                return true;
            }
            else
            {
                TextBoxModify(myOutput, "Error: object file not created at " + objectFileAbsPath, TextBoxChangeMode.AppendNewLine);
                return false;
            }
        }

        private bool CreateELF(string OBJECTS)
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            if (Program.MakeSurePathExists(outputAbsPath) == false)
                return false;

            string elfFileAbsPath = outputAbsPath + "\\" + workingProject.FileNameNoExt + ".elf";

            if (File.Exists(elfFileAbsPath))
            {
                try
                {
                    File.Delete(elfFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: ELF file could not be deleted at " + elfFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string mapFileAbsPath = outputAbsPath + "\\" + workingProject.FileNameNoExt + ".map";

            if (File.Exists(mapFileAbsPath))
            {
                try
                {
                    File.Delete(mapFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: MAP file could not be deleted at " + mapFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string LDSFLAGS = "-mmcu=" + workingProject.Device + " ";
            foreach (MemorySegment seg in workingProject.MemorySegList)
            {
                int addr = (int)seg.Addr;
                if (seg.Type.ToLower() == "sram")
                {
                    addr += 0x800000;
                }
                else if (seg.Type.ToLower() == "eeprom")
                {
                    addr += 0x810000;
                }
                LDSFLAGS += "-Wl,-section-start=" + seg.Name + "=0x" + Convert.ToString(addr, 16) + " ";
            }

            if (workingProject.UseInitStack)
            {
                LDSFLAGS += "-Wl,--defsym=__stack=0x" + Convert.ToString(workingProject.InitStackAddr, 16) + " ";
            }

            LDSFLAGS += "-Wl,-Map=" + workingProject.FileNameNoExt + ".map";

            string LINKONLYOBJECTS = "";
            foreach (string obj in workingProject.LinkObjList)
            {
                if (string.IsNullOrEmpty(obj) == false)
                {
                    LINKONLYOBJECTS += "\"" + obj + "\" ";
                }
            }
            
            string LIBS = "";
            foreach (string obj in workingProject.LinkLibList)
            {
                if (string.IsNullOrEmpty(obj) == false)
                {
                    if (obj.StartsWith("lib"))
                    {
                        LIBS += "-l" + obj.Substring(3).TrimEnd('a').TrimEnd('.') + " ";
                    }
                    else
                    {
                        LIBS += "-l\"" + obj.TrimEnd('a').TrimEnd('.') + "\" ";
                    }
                }
            }

            string LIBDIRS = "";
            foreach (string obj in workingProject.LinkObjList)
            {
                if (string.IsNullOrEmpty(obj) == false)
                {
                    LIBDIRS += "-L\"" + obj + "\" ";
                }
            }

            string args = String.Format("{0} {1} {2} {3} {4} -o {5}.elf",
                LDSFLAGS.Trim(),
                OBJECTS.Trim(),
                LINKONLYOBJECTS.Trim(),
                LIBDIRS.Trim(),
                LIBS.Trim(),
                workingProject.FileNameNoExt
            );

            TextBoxModify(myOutput, "Execute: avr-gcc " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-gcc", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process avrgcc = new Process();
            avrgcc.StartInfo = psi;
            try
            {
                if (avrgcc.Start())
                {
                    StreamReader stderr = avrgcc.StandardError;
                    ReadErrAndWarnings(stderr);
                    StreamReader stdout = avrgcc.StandardOutput;
                    ReadErrAndWarnings(stdout);
                    avrgcc.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to start avr-gcc", TextBoxChangeMode.AppendNewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to start avr-gcc\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                return false;
            }

            if (File.Exists(elfFileAbsPath))
            {
                if (File.Exists(mapFileAbsPath) == false)
                {
                    TextBoxModify(myOutput, "Error: MAP file not created at " + elfFileAbsPath, TextBoxChangeMode.AppendNewLine);
                }

                return true;
            }
            else
            {
                TextBoxModify(myOutput, "Error: ELF file not created at " + elfFileAbsPath, TextBoxChangeMode.AppendNewLine);
                return false;
            }
        }

        private void CleanOD(string OBJECTS)
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            string args = "-rf " + OBJECTS + " " + (OBJECTS + " ").Replace(".o ", ".d ");

            TextBoxModify(myOutput, "Execute: rm " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("rm", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process rm = new Process();
            rm.StartInfo = psi;
            try
            {
                if (rm.Start())
                {
                    rm.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to clean .o and .d files", TextBoxChangeMode.AppendNewLine);
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to clean .o and .d files\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
            }
        }

        private bool CreateHex()
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            if (Program.MakeSurePathExists(outputAbsPath) == false)
                return false;

            string hexFileAbsPath = outputAbsPath + "\\" + workingProject.FileNameNoExt + ".hex";

            if (File.Exists(hexFileAbsPath))
            {
                try
                {
                    File.Delete(hexFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: HEX file could not be deleted at " + hexFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string HEX_FLASH_FLAGS = "-R .eeprom -R .fuse -R .lock -R .signature ";
            string args = "-O ihex " + HEX_FLASH_FLAGS + workingProject.FileNameNoExt + ".elf " + workingProject.FileNameNoExt + ".hex";

            TextBoxModify(myOutput, "Execute: avr-objcopy " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-objcopy", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process avrobjcopy = new Process();
            avrobjcopy.StartInfo = psi;
            try
            {
                if (avrobjcopy.Start())
                {
                    StreamReader stderr = avrobjcopy.StandardError;
                    ReadErrAndWarnings(stderr);
                    StreamReader stdout = avrobjcopy.StandardOutput;
                    ReadErrAndWarnings(stdout);
                    avrobjcopy.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to start avr-objcopy", TextBoxChangeMode.AppendNewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to start avr-objcopy\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                return false;
            }

            if (File.Exists(hexFileAbsPath))
            {
                return true;
            }
            else
            {
                TextBoxModify(myOutput, "Error: HEX file not created at " + hexFileAbsPath, TextBoxChangeMode.AppendNewLine);
                return false;
            }
        }

        private bool CreateEEP()
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            if (Program.MakeSurePathExists(outputAbsPath) == false)
                return false;

            string eepFileAbsPath = outputAbsPath + "\\" + workingProject.FileNameNoExt + ".eep";

            if (File.Exists(eepFileAbsPath))
            {
                try
                {
                    File.Delete(eepFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: EEP file could not be deleted at " + eepFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string HEX_EEPROM_FLAGS = "-j .eeprom --set-section-flags=.eeprom=\"alloc,load\" --change-section-lma .eeprom=0 --no-change-warnings ";

            foreach (MemorySegment seg in workingProject.MemorySegList)
            {
                if (seg.Type == "eeprom")
                    HEX_EEPROM_FLAGS += "--change-section-lma " + seg.Name + "=0x" + Convert.ToString(seg.Addr, 16) + " ";
            }

            string args = HEX_EEPROM_FLAGS + " -O ihex " + workingProject.FileNameNoExt + ".elf " + workingProject.FileNameNoExt + ".hex";

            TextBoxModify(myOutput, "Execute: avr-objcopy " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-objcopy", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process avrobjcopy = new Process();
            avrobjcopy.StartInfo = psi;
            try
            {
                if (avrobjcopy.Start())
                {
                    StreamReader stderr = avrobjcopy.StandardError;
                    ReadErrAndWarnings(stderr);
                    StreamReader stdout = avrobjcopy.StandardOutput;
                    ReadErrAndWarnings(stdout);
                    avrobjcopy.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to start avr-objcopy", TextBoxChangeMode.AppendNewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to start avr-objcopy\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                return false;
            }

            if (File.Exists(eepFileAbsPath))
            {
                return true;
            }
            else
            {
                TextBoxModify(myOutput, "Error: EEP file not created at " + eepFileAbsPath, TextBoxChangeMode.AppendNewLine);
                return false;
            }
        }

        private bool CreateLSS()
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            if (Program.MakeSurePathExists(outputAbsPath) == false)
                return false;

            string lssFileAbsPath = outputAbsPath + "\\" + workingProject.FileNameNoExt + ".lss";

            if (File.Exists(lssFileAbsPath))
            {
                try
                {
                    File.Delete(lssFileAbsPath);
                }
                catch (Exception ex)
                {
                    TextBoxModify(myOutput, "Error: LSS file could not be deleted at " + lssFileAbsPath + "\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                }
            }

            string args = "-h -S " + workingProject.FileNameNoExt + ".elf >> " + workingProject.FileNameNoExt + ".lss";

            TextBoxModify(myOutput, "Execute: avr-objdump " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-objdump", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = false;
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardInput = false;
            Process avrobjcopy = new Process();
            avrobjcopy.StartInfo = psi;
            try
            {
                if (avrobjcopy.Start())
                {
                    avrobjcopy.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to start avr-objdump", TextBoxChangeMode.AppendNewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to start avr-objdump\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
                return false;
            }

            if (File.Exists(lssFileAbsPath))
            {
                return true;
            }
            else
            {
                TextBoxModify(myOutput, "Error: LSS file not created at " + lssFileAbsPath, TextBoxChangeMode.AppendNewLine);
                return false;
            }
        }

        private void ReadSize()
        {
            string outputAbsPath = workingProject.DirPath + "\\" + workingProject.OutputDir;

            string args = "-C --mcu=" + workingProject.Device + " " + workingProject.FileNameNoExt + ".elf";

            TextBoxModify(myOutput, "Execute: avr-size " + args, TextBoxChangeMode.AppendNewLine);

            ProcessStartInfo psi = new ProcessStartInfo("avr-size", args);
            psi.WorkingDirectory = outputAbsPath + "\\";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            Process avrsize = new Process();
            avrsize.StartInfo = psi;
            try
            {
                if (avrsize.Start())
                {
                    StreamReader stderr = avrsize.StandardError;
                    ReadErrAndWarnings(stderr);
                    StreamReader stdout = avrsize.StandardOutput;
                    ReadErrAndWarnings(stdout);
                    avrsize.WaitForExit(10000);
                }
                else
                {
                    TextBoxModify(myOutput, "Error: unable to get the memory usage info", TextBoxChangeMode.AppendNewLine);
                }
            }
            catch (Exception ex)
            {
                TextBoxModify(myOutput, "Error: unable to get the memory usage info\r\n" + ex.ToString(), TextBoxChangeMode.AppendNewLine);
            }
        }

        private void ReadErrAndWarnings(StreamReader reader)
        {
            string loc = "";

            string line = reader.ReadLine();

            while (line != null)
            {
                string re1 = "((?:[a-z][a-z0-9_]*))";   // Variable Name 1
                string re2 = "(\\.)";	// Any Single Character 1
                string re3 = "((?:[a-z][a-z0-9_]*))";	// Variable Name 2
                string re4 = "(:)";	// Any Single Character 2
                string re5 = "(\\d+)";	// Integer Number 1
                string re6 = "(:)";	// Any Single Character 3
                string re7 = "( )";	// White Space 1
                string re8 = "((?:[a-z][a-z]+))";	// Word 2
                string re9 = "(:)";	// Any Single Character 4
                string re10 = "( )";	// White Space 2
                string re11 = "(.*)";	// The Rest

                Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8 + re9 + re10 + re11, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(line);
                if (m.Success)
                {
                    string fileName = m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value;
                    string type = m.Groups[8].Value;
                    string lineNum = m.Groups[5].Value;
                    string msg = m.Groups[11].Value;

                    ListViewItem lvi = new ListViewItem(new string[] {fileName, lineNum, type, loc, msg });
                    ListViewModify(myErrorList, lvi, ListViewChangeMode.Add);
                }
                else
                {
                    re1 = "((?:[a-z][a-z0-9_]*))";	// Variable Name 1
                    re2 = "(\\.)";	// Any Single Character 1
                    re3 = "((?:[a-z][a-z0-9_]*))";	// Variable Name 2
                    re4 = "(:)";	// Any Single Character 2
                    re5 = "( )";	// Any Single Character 3
                    re6 = "(.*)";	// The Rest

                    r = new Regex(re1 + re2 + re3 + re4 + re5 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    m = r.Match(line);
                    if (m.Success)
                    {
                        loc = m.Groups[6].Value.Trim().Trim(':').Trim();
                        if (loc.ToLower().Contains("in function"))
                        {
                            loc = loc.Substring(loc.ToLower().IndexOf("in function") + "in function".Length).Trim();
                        }
                    }
                    else
                    {
                        TextBoxModify(myOutput, line, TextBoxChangeMode.AppendNewLine);
                    }
                }

                line = reader.ReadLine();
            }
        }

        #endregion
    }

    public class ProjectBurner
    {
        private AVRProject myProject;
        private Process avrdude;

        public ProjectBurner(AVRProject myProject)
        {
            this.myProject = myProject;
            avrdude = new Process();
        }

        public void Burn(bool onlyOptions)
        {
            try
            {
                avrdude.Kill();
            }
            catch
            {
            }

            string fileStr = "";
            if (onlyOptions == false)
                fileStr = String.Format("-U flash:w:\"{0}\\{1}\\{2}.hex\":a", myProject.DirPath, myProject.OutputDir, myProject.FileNameNoExt);

            string args = String.Format("avrdude -p {0} -c {1} {2} {3}", myProject.BurnPart, myProject.BurnProgrammer, fileStr, myProject.BurnOptions);
            avrdude.StartInfo = new ProcessStartInfo("cmd", "/k " + args);
            try
            {
                if (avrdude.Start() == false)
                {
                    MessageBox.Show("Error, Unable to Start AVRDUDE");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, Unable to Start AVRDUDE\r\n" + ex.ToString());
            }
        }

        public string[] GetAvailParts()
        {
            List<string> res = new List<string>();

            Process avrdude = new Process();
            avrdude.StartInfo = new ProcessStartInfo("avrdude", "-c usbisp -p blarg");
            avrdude.StartInfo.UseShellExecute = false;
            avrdude.StartInfo.RedirectStandardError = true;
            avrdude.StartInfo.RedirectStandardOutput = true;
            avrdude.StartInfo.RedirectStandardInput = true;
            try
            {
                if (avrdude.Start())
                {
                    StreamReader stdout = avrdude.StandardOutput;
                    StreamReader stderr = avrdude.StandardError;

                    res = GetFirstWords(stdout, res);
                    res = GetFirstWords(stderr, res);
                }
                else
                {
                    MessageBox.Show("Error, Unable to Start AVRDUDE");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, Unable to Start AVRDUDE\r\n" + ex.ToString());
            }

            return res.ToArray();
        }

        public string[] GetAvailProgrammers()
        {
            List<string> res = new List<string>();

            Process avrdude = new Process();
            avrdude.StartInfo = new ProcessStartInfo("avrdude", "-c blarg");
            avrdude.StartInfo.UseShellExecute = false;
            avrdude.StartInfo.RedirectStandardError = true;
            avrdude.StartInfo.RedirectStandardOutput = true;
            avrdude.StartInfo.RedirectStandardInput = true;
            try
            {
                if (avrdude.Start())
                {
                    StreamReader stdout = avrdude.StandardOutput;
                    StreamReader stderr = avrdude.StandardError;

                    res = GetFirstWords(stdout, res);
                    res = GetFirstWords(stderr, res);
                }
                else
                {
                    MessageBox.Show("Error, Unable to Start AVRDUDE");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, Unable to Start AVRDUDE\r\n" + ex.ToString());
            }

            return res.ToArray();
        }

        private List<string> GetFirstWords(StreamReader reader, List<string> res)
        {
            string line = reader.ReadLine();

            while (line != null)
            {
                if (line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length >= 2)
                    {
                        res.Add(parts[0].Trim());
                    }
                }

                line = reader.ReadLine();
            }

            return res;
        }
    }

    public class Makefile
    {
        public static bool Generate(AVRProject proj)
        {
            bool success = true;

            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(proj.DirPath + "\\Makefile");

                writer.WriteLine("##################################");
                writer.WriteLine("## Makefile for project: {0}", proj.FileNameNoExt);
                writer.WriteLine("##################################");
                writer.WriteLine();
                writer.WriteLine("## General Flags");
                writer.WriteLine("PROJECT = {0}", proj.FileNameNoExt);
                writer.WriteLine("MCU = {0}", proj.Device);
                writer.WriteLine("TARGET = {0}/$(PROJECT).elf", proj.OutputDir.Replace('\\', '/'));
                writer.WriteLine("CC = avr-gcc");
                writer.WriteLine();
                writer.WriteLine("## Flags common to C, ASM, and Linker");
                writer.WriteLine("COMMON = -mmcu=$(MCU)");
                writer.WriteLine();
                writer.WriteLine("## Flags common to C only");
                writer.WriteLine("CFLAGS = $(COMMON)");

                string cflags = "";
                cflags += "-Wall ";
                cflags += "-gdwarf-2 ";
                cflags += "-std=gnu99 ";

                if (proj.ClockFreq != 0)
                    cflags += "-DF_CPU=" + Convert.ToString(Math.Round(proj.ClockFreq)) + "UL ";

                cflags += proj.Optimization + " ";

                if (proj.UnsignedChars)
                    cflags += "-funsigned-char ";

                if (proj.UnsignedBitfields)
                    cflags += "-funsigned-bitfields ";

                if (proj.PackStructs)
                    cflags += "-fpack-struct ";

                if (proj.ShortEnums)
                    cflags += "-fshort-enums ";

                writer.WriteLine("CFLAGS += {0} {1}", cflags.Trim(), proj.OtherOptions.Trim());
                writer.WriteLine("CFLAGS += -MD -MP -MT $(*F).o");

                writer.WriteLine();
                writer.WriteLine("## Flags common to ASM only");
                writer.WriteLine("ASMFLAGS = $(COMMON)");
                writer.WriteLine("ASMFLAGS += $(CFLAGS)");
                writer.WriteLine("ASMFLAGS += -x assembler-with-cpp -Wa,-gdwarf2");

                writer.WriteLine();
                writer.WriteLine("## Flags common to Linker only");
                writer.WriteLine("LDFLAGS = $(COMMON)");
                writer.WriteLine("LDFLAGS += {0}", proj.LinkerOptions);
                writer.WriteLine("LDFLAGS += -Wl,-Map={0}/$(PROJECT).map", proj.OutputDir.Replace('\\', '/'));

                if (proj.UseInitStack)
                {
                    writer.WriteLine("LDFLAGS += -Wl,--defsym=__stack=0x{0:X}", proj.InitStackAddr);
                }

                foreach (MemorySegment seg in proj.MemorySegList)
                {
                    int addr = (int)seg.Addr;
                    if (seg.Type.ToLower() == "sram")
                    {
                        addr += 0x800000;
                    }
                    else if (seg.Type.ToLower() == "eeprom")
                    {
                        addr += 0x810000;
                    }
                    writer.WriteLine("LDFLAGS += -Wl,-section-start={0}=0x{1:X}", seg.Name, addr);
                }
                
                writer.WriteLine();
                writer.WriteLine("## Flags for Intel HEX file production");
                writer.WriteLine("HEX_FLASH_FLAGS = -R .eeprom -R .fuse -R .lock -R .signature");
                writer.WriteLine();
                writer.WriteLine("HEX_EEPROM_FLAGS = -j .eeprom");
                writer.WriteLine("HEX_EEPROM_FLAGS += --set-section-flags=.eeprom=\"alloc,load\"");
                writer.WriteLine("HEX_EEPROM_FLAGS += --change-section-lma .eeprom=0 --no-change-warnings");

                foreach (MemorySegment seg in proj.MemorySegList)
                {
                    if (seg.Type.ToLower() == "eeprom")
                    {
                        writer.WriteLine("HEX_EEPROM_FLAGS += --change-section-lma {0}=0x{1}", seg.Name, seg.Addr);
                    }
                }

                string incdirs = "";
                foreach (string s in proj.IncludeDirList)
                {
                    if (string.IsNullOrEmpty(s) == false)
                        incdirs += "-I\"" + s + "\" ";
                }
                incdirs = incdirs.Trim();
                if (string.IsNullOrEmpty(incdirs) == false)
                {
                    writer.WriteLine();
                    writer.WriteLine("## Include Directories");
                    writer.WriteLine("INCLUDES = {0}", incdirs);
                }

                string libdirs = "";
                foreach (string s in proj.LibraryDirList)
                {
                    if (string.IsNullOrEmpty(s) == false)
                        libdirs += "-L\"" + s + "\" ";
                }
                libdirs = libdirs.Trim();
                if (string.IsNullOrEmpty(libdirs) == false)
                {
                    writer.WriteLine();
                    writer.WriteLine("## Library Directories");
                    writer.WriteLine("LIBDIRS = {0}", libdirs);
                }

                string linklibstr = "";
                foreach (string s in proj.LinkLibList)
                {
                    if (string.IsNullOrEmpty(s) == false)
                    {
                        if (s.StartsWith("lib"))
                        {
                            linklibstr += "-l" + s.Substring(3).TrimEnd('a').TrimEnd('.') + " ";
                        }
                        else
                        {
                            linklibstr += "-l\"" + s.TrimEnd('a').TrimEnd('.') + "\" ";
                        }
                    }
                }
                linklibstr = linklibstr.Trim();
                if (string.IsNullOrEmpty(linklibstr) == false)
                {
                    writer.WriteLine();
                    writer.WriteLine("## Libraries");
                    writer.WriteLine("LIBS = {0}", linklibstr);
                }

                string ofiles = "";
                string compileStr = "";

                foreach (KeyValuePair<string, ProjectFile> file in proj.FileList)
                {
                    if (file.Value.ToCompile && file.Value.FileExt != "h")
                    {
                        ofiles += file.Value.FileNameNoExt + ".o ";

                        compileStr += file.Value.FileNameNoExt + ".o: ./" + file.Value.FileRelPath.Replace('\\', '/');
                        compileStr += "\r\n";
                        compileStr += "\t $(CC) $(INCLUDES) ";
                        if (file.Value.FileExt == "s")
                        {
                            compileStr += "$(ASMFLAGS)";
                        }
                        else if (file.Value.FileExt == "c")
                        {
                            compileStr += "$(CFLAGS)";
                        }
                        compileStr += " -c ";
                        compileStr += file.Value.Options.Trim();
                        compileStr += " $<\r\n\r\n";
                    }
                }

                ofiles = ofiles.Trim();

                writer.WriteLine();
                writer.WriteLine("## Link these object files to be made");
                writer.WriteLine("OBJECTS = {0}", ofiles);

                string linkobjstr = "";
                foreach (string s in proj.LinkObjList)
                {
                    if (string.IsNullOrEmpty(s) == false)
                        linkobjstr += "\"" + s + "\" ";
                }

                writer.WriteLine();
                writer.WriteLine("## Link objects specified by users");
                writer.WriteLine("LINKONLYOBJECTS = {0}", linkobjstr.Trim());

                writer.WriteLine();
                writer.WriteLine("## Compile");
                writer.WriteLine();
                writer.WriteLine("all: $(TARGET)");
                writer.WriteLine();
                writer.WriteLine(compileStr);

                writer.WriteLine();
                writer.WriteLine("## Link");
                writer.WriteLine("$(TARGET): $(OBJECTS)");

                writer.WriteLine("\t-rm -rf $(TARGET) {0}/$(PROJECT).map", proj.OutputDir.Replace('\\', '/'));

                writer.WriteLine("\t $(CC) $(LDFLAGS) $(OBJECTS) $(LINKONLYOBJECTS) $(LIBDIRS) $(LIBS) -o $(TARGET)");

                writer.WriteLine("\t-rm -rf $(OBJECTS) {0}", (ofiles + " ").Replace(".o ", ".d "));
                writer.WriteLine("\t-rm -rf {0}/$(PROJECT).hex {0}/$(PROJECT).eep {0}/$(PROJECT).lss", proj.OutputDir.Replace('\\', '/'));

                writer.WriteLine("\tavr-objcopy -O ihex $(HEX_FLASH_FLAGS) $(TARGET) {0}/$(PROJECT).hex", proj.OutputDir.Replace('\\', '/'));
                writer.WriteLine("\tavr-objcopy $(HEX_FLASH_FLAGS) -O ihex $(TARGET) {0}/$(PROJECT).eep || exit 0", proj.OutputDir.Replace('\\', '/'));
                writer.WriteLine("\tavr-objdump -h -S $(TARGET) >> {0}/$(PROJECT).lss", proj.OutputDir.Replace('\\', '/'));
                writer.WriteLine("\t@avr-size -C --mcu=${MCU} ${TARGET}");

                writer.WriteLine();
                writer.WriteLine("## Clean target");
                writer.WriteLine(".PHONY: clean");
                writer.WriteLine("clean:");
                writer.WriteLine("\t-rm -rf $(OBJECTS) {1} {0}/$(PROJECT).elf {0}/$(PROJECT).map {0}/$(PROJECT).lss {0}/$(PROJECT).hex {0}/$(PROJECT).eep", proj.OutputDir.Replace('\\', '/'), (ofiles + " ").Replace(".o ", ".d "));
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
    }
}
