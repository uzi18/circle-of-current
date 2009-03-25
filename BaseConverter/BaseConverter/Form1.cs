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
            string s = "";
            string rh, rd, rb;
            int r = 0;
            bool show_balloon = false;

            if (WindowState != FormWindowState.Minimized)
            {
                s = InputBox.Text;
            }
            else
            {
                if (Clipboard.ContainsText())
                {
                    s = Clipboard.GetText();
                    if (s != old_clipboard)
                    {
                        old_clipboard = s;
                        show_balloon = true;
                    }
                }
            }

            s = s.Trim();

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

            if (DetectedLabel.Text != "Error")
            {
                rh = Convert.ToString(r, 16).ToUpper();
                if (rh.Length % 2 != 0)
                {
                    rh = "0" + rh;
                }

                rb = Convert.ToString(r, 2);
                while (rb.Length % 4 != 0 || rb.Length < 8)
                {
                    rb = "0" + rb;
                }

                rd = Convert.ToString(r, 10);

                NotiIcon.BalloonTipText += "Dec: " + rd + "\r\n";
                NotiIcon.BalloonTipText += "Hex: " + rh + "\r\n";
                NotiIcon.BalloonTipText += "Bin: " + rb + "\r\n";

                HexLabel.Text = rh;
                BinLabel.Text = rb;
                DecLabel.Text = rd;

                if (show_balloon && EnableNoti.Checked)
                {
                    NotiIcon.ShowBalloonTip(2000);
                }
                InputBox.Text = s;
            }

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
            NotiIcon.Visible = false;
        }
    }
}