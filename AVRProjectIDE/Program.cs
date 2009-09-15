using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AVRProjectIDE
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            EnviroSettings mySettings = new EnviroSettings();
            AVRProject myProject = new AVRProject();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmWelcome(mySettings, myProject));
            if (myProject.IsReady)
                Application.Run(new frmProjIDE(myProject, mySettings));
        }

        public static bool MakeSurePathExists(string dirPath)
        {
            dirPath = CleanFilePath(dirPath);
            string[] folders = dirPath.Split('\\');
            string curDir = "";
            foreach (string folder in folders)
            {
                curDir += folder + '\\';
                try
                {
                    if (Directory.Exists(curDir) == false)
                    {
                        Directory.CreateDirectory(curDir);
                    }
                }
                catch
                {
                }
            }
            return Directory.Exists(dirPath);
        }

        public static string CleanFilePath(string path)
        {
            return path.Trim().Replace('/', '\\').Trim('\\').Trim();
        }

        static public string RelativePath(string dirPath, string filePath)
        {
            string relPath = "";
            string[] curDirList = CleanFilePath(dirPath).Split(new char[] { '\\', '/', });
            string[] pathDirList = CleanFilePath(filePath).Split(new char[] { '\\', '/', });
            int i = 0;
            for (i = 0; i < curDirList.Length && i < pathDirList.Length; i++)
            {
                if (curDirList[i] != pathDirList[i])
                {
                    break;
                }
            }
            for (int j = curDirList.Length; j > i; j--)
            {
                relPath += @"..\";
            }
            for (int j = i; i < pathDirList.Length; i++)
            {
                relPath += pathDirList[i] + @"\";
            }
            return CleanFilePath(relPath);
        }
    }

    public enum SaveResult
    {
        Successful = 1,
        Failed = 0,
        Cancelled = 2
    }
}
