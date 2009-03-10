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

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
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

            if (s.IndexOf("0x") == 0)
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
                        r = Convert.ToInt32(s.Substring(2), 16);
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

            NotiIcon.BalloonTipTitle = "Detected: " + DetectedLabel.Text;
            NotiIcon.BalloonTipText = "Detected: " + DetectedLabel.Text + "\r\n";

            if (DetectedLabel.Text != "Error")
            {
                rh = Convert.ToString(r, 16).ToUpper();
                if (rh.Length % 2 != 0)
                {
                    rh = "0" + rh;
                }

                rb = Convert.ToString(r, 2);
                while (rb.Length % 8 != 0)
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
        }

        private void NotiIcon_Click(object sender, EventArgs e)
        {
            NotiIcon.ShowBalloonTip(2000);
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void DecLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DecLabel.Text);
        }

        private void HexLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(HexLabel.Text);
        }

        private void BinLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(BinLabel.Text);
        }

        private void KillButton_Click(object sender, EventArgs e)
        {
            NotiIcon.Visible = false;
            Process.GetCurrentProcess().Kill();
        }
    }
}