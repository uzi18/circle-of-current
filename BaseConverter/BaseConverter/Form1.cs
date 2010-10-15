using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace BaseConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            NotiIcon.BalloonTipTitle = "Base Converter";

            // look through all processes to find one with the same name as the current process
            // if found, then kill self
            Process selfproc = Process.GetCurrentProcess();
            Process[] proclist = Process.GetProcesses();
            foreach (Process i in proclist)
            {
                if (i.Id != selfproc.Id)
                {
                    if (selfproc.ProcessName == i.ProcessName)
                    {
                        NotiIcon.Visible = false;
                        selfproc.Kill();
                    }
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        string old_clipboard = "";

        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            string s = ""; // input string
            string rh, rd, rb; // result string
            int r = 0; // input value
            bool show_balloon = false; // show only if needed

            if (WindowState != FormWindowState.Minimized) // window is open
            {
                s = InputBox.Text; // use user input box
            }
            else // window is hidden
            {
                if (Clipboard.ContainsText())
                {
                    s = Clipboard.GetText(); // use clipboard data as input
                    if (s != old_clipboard)
                    {
                        // text has changed, show balloon
                        old_clipboard = s;
                        show_balloon = true;
                    }
                }
            }

            s = s.Trim();

            /*
             * Below is the code trying to see what base the input is in.
             * First, it checks for standard prefixes, such as "0x" or "0b".
             * If those are not found, then it assumes the input is decimal.
             * If the input is not decimal, an exception is thrown and the catch
             * block is executed, which then assumes the input is hexadecimal, if
             * exception is thrown, then the input cannot be converted. If
             * the conversion is successful, then store the value as an integer.
             */

            if (s.IndexOf("0x") == 0 || s.IndexOf("0X") == 0)
            {
                try
                {
                    r = Convert.ToInt32(s.Substring(2), 16);
                    DetectedLabel.Text = "Hexadecimal";
                }
                catch
                {
                    DetectedLabel.Text = "Error";
                }
            }
            else if (s.IndexOf("0b") == 0)
            {
                try
                {
                    r = Convert.ToInt32(s.Substring(2), 2);
                    DetectedLabel.Text = "Binary";
                }
                catch
                {
                    try
                    {
                        r = Convert.ToInt32(s, 16);
                        DetectedLabel.Text = "Hexadecimal";
                    }
                    catch
                    {
                        DetectedLabel.Text = "Error";
                    }
                }
            }
            else
            {
                try
                {
                    r = Convert.ToInt32(s, 10);
                    DetectedLabel.Text = "Decimal";
                }
                catch
                {
                    try
                    {
                        r = Convert.ToInt32(s, 16);
                        DetectedLabel.Text = "Hexadecimal";
                    }
                    catch
                    {
                        DetectedLabel.Text = "Error";
                    }
                }
            }

            NotiIcon.BalloonTipText = "Detected: " + DetectedLabel.Text + "\r\n";

            if (DetectedLabel.Text != "Error") // conversion successful
            {
                // convert into hexadecimal, make sure length of resulting string
                // is a multiple of 2, so it looks neat
                rh = Convert.ToString(r, 16).ToUpper();
                if (rh.Length % 2 != 0)
                {
                    rh = "0" + rh;
                }

                // convert into binary, make sure length of resulting string
                // is a multiple of 4 and at least 8, so it looks neat
                rb = Convert.ToString(r, 2);
                while (rb.Length % 4 != 0 || rb.Length < 8)
                {
                    rb = "0" + rb;
                }

                // convert to decimal
                rd = Convert.ToString(r, 10);

                // prepare notification balloon
                NotiIcon.BalloonTipText += "Dec: " + rd + "\r\n";
                NotiIcon.BalloonTipText += "Hex: " + rh + "\r\n";
                NotiIcon.BalloonTipText += "Bin: " + rb + "\r\n";

                // prepare text in window
                HexLabel.Text = rh;
                BinLabel.Text = rb;
                DecLabel.Text = rd;

                if (show_balloon && EnableNoti.Checked) // show balloon if appropriate
                {
                    NotiIcon.ShowBalloonTip(2000);
                }
                InputBox.Text = s;
            }

            // enable/disable menu items
            if (EnableNoti.Checked)
            {
                onToolStripMenuItem.Enabled = false;
                offToolStripMenuItem.Enabled = true;
            }
            else
            {
                offToolStripMenuItem.Enabled = false;
                onToolStripMenuItem.Enabled = true;
            }
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void KillButton_Click(object sender, EventArgs e)
        {
            NotiIcon.Visible = false;
            Process.GetCurrentProcess().Kill();
        }

        private void NotiIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                NotiIcon.ShowBalloonTip(2000);
            }
        }

        private void decimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DecLabel.Text);
            old_clipboard = DecLabel.Text;
        }

        private void hexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(HexLabel.Text);
            old_clipboard = HexLabel.Text;
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(BinLabel.Text);
            old_clipboard = BinLabel.Text;
        }

        private void NotiIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableNoti.Checked = false;
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableNoti.Checked = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            e.Cancel = true;
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}