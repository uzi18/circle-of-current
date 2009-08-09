using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlainTextClipboard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // make icon visible
            SystrayIcon.Visible = true;

            // start minimized and hidden
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // make icon invisible
            SystrayIcon.Visible = false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                // hide if minimized
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            // convert text and show notification
            Clipboard.SetText(Clipboard.GetText());
            SystrayIcon.ShowBalloonTip(1000, "Plain Text Converted", "Text has been converted", ToolTipIcon.Info);
        }

        private void ShowWinBtn_Click(object sender, EventArgs e)
        {
            // show the window
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            Show();

            // load preview of converted text
            ConvertedTxt.Text = Clipboard.GetText();
        }

        private void SystrayIcon_DoubleClick(object sender, EventArgs e)
        {
            // convert text and show notification
            Clipboard.SetText(Clipboard.GetText());
            SystrayIcon.ShowBalloonTip(1000, "Plain Text Converted", "Text has been converted", ToolTipIcon.Info);
        }
    }
}
