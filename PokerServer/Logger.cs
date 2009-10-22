using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogUtilities
{
    public enum LogType
    {
        Event = 1,
        Error = 0,
        Trace = 2,
        CommRx = 3,
        CommTx = 4,
        Debug = 5
    }

    public static class Logger
    {
        private const int RETRYWRITE = 5;
        private const string LOGFILENAME = "serverlog.txt";
        private const int LOGLEVEL = (int)LogType.Debug;

        private static System.Windows.Forms.TextBox logTextBox;

        public static System.Windows.Forms.TextBox LogTextBox
        {
            set { logTextBox = value; }
        }

        private delegate void TextBoxModify(string text);

        private static void TextBoxAppend(string text)
        {
            if (logTextBox == null)
                return;

            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new TextBoxModify(TextBoxAppend), new object[] { text, });
            }
            else
            {
                logTextBox.Text = text + "\r\n" + logTextBox.Text;
                if (logTextBox.Text.Length > logTextBox.MaxLength / 2)
                    logTextBox.Text = logTextBox.Text.Substring(0, logTextBox.Text.Length / 2);
            }
        }

        public static void Log(LogType type, string message, bool append)
        {
            if ((int)type > LOGLEVEL)
                return;

            string typeString;
            switch (type)
            {
                case LogType.CommRx:
                    typeString = "Comm Rx";
                    break;
                case LogType.CommTx:
                    typeString = "Comm Tx";
                    break;
                case LogType.Debug:
                    typeString = "Debug";
                    break;
                case LogType.Error:
                    typeString = "Error";
                    break;
                case LogType.Event:
                    typeString = "Event";
                    break;
                case LogType.Trace:
                    typeString = "Trace";
                    break;
                default:
                    typeString = "Unknown Log Type";
                    break;
            }

            string log = String.Format(
                "{0}: {1}: {2}",
                DateTime.Now.ToString("hh:mm:ss.ffff"),
                typeString,
                message
                );

            TextBoxAppend(log);

            StreamWriter writer = null;
            bool success;
            for (int i = 0; i < RETRYWRITE; i++)
            {
                try
                {
                    writer = new StreamWriter(LOGFILENAME, append);

                    writer.WriteLine(log);

                    writer.Close();

                    success = true;
                }
                catch
                {
                    success = false;
                }

                if (success)
                    break;
            }
        }

        public static void Log(LogType type, string message)
        {
            Log(type, message, true);
        }

        public static void Log(Exception ex)
        {
            Log(LogType.Error, String.Format(
                "Exception: {0}",
                ex.ToString()
                ),
                true
                );
        }

        public static void Log(string message, Exception ex)
        {
            Log(LogType.Error, String.Format(
                "{0}: Exception: {1}",
                message,
                ex.ToString()
                ),
                true
                );
        }

        public static void ClearLog()
        {
            Log(LogType.Event, "Log Cleared", false);
        }
    }
}