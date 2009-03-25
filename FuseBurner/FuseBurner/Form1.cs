using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace FuseBurner
{
    public partial class Form1 : Form
    {
        string save_fname = "savedstate.txt";
        string base_url = "http://www.engbedded.com/cgi-bin/fc.cgi";
        string last_url = "http://www.engbedded.com/cgi-bin/fc.cgi";
        string last_opt = "-c usbtiny";

        public Form1()
        {
            InitializeComponent();

            if (File.Exists(save_fname))
            {
                try
                {
                    StreamReader sr = new StreamReader(save_fname);
                    last_url = sr.ReadLine();
                    last_opt = sr.ReadLine();
                    int h = Convert.ToInt32(sr.ReadLine());
                    this.Height = h;
                    int w = Convert.ToInt32(sr.ReadLine());

                    this.Width = w;
                    int split = Convert.ToInt32(sr.ReadLine());
                    WindowSplitter.SplitterDistance = split;
                    
                    sr.Close();
                }
                catch
                {
                }
            }

            OptTxt.Text = last_opt;
            if (last_url.Contains(base_url))
            {
                FuseCalcWeb.Navigate(last_url);
            }
            else if (last_url.Contains("temppage.html") && File.Exists("temppage.html"))
            {
                FileInfo fi = new FileInfo("temppage.html");
                FuseCalcWeb.Navigate(fi.FullName);
            }
            else
            {
                FuseCalcWeb.Navigate(base_url);
            }
        }

        private int ExecuteAVRDUDE(string args)
        {
            Process avrdude = new Process();
            avrdude.StartInfo.UseShellExecute = false;
            avrdude.StartInfo.RedirectStandardOutput = true;
            avrdude.StartInfo.RedirectStandardError = true;
            avrdude.StartInfo.FileName = "avrdude";
            avrdude.StartInfo.Arguments = args + " -q";
            OutputTxt.Text = "Executing: avrdude " + args + "\r\n" + OutputTxt.Text;
            avrdude = Process.Start(avrdude.StartInfo);
            StreamReader stdout = avrdude.StandardOutput;
            StreamReader stderr = avrdude.StandardError;
            string str;
            str = stderr.ReadToEnd();
            if (str != null)
            {
                OutputTxt.Text = "Stderr:\r\n" + str + "\r\n" + OutputTxt.Text;
            }
            OutputTxt.Text = "Exit Code: " + Convert.ToString(avrdude.ExitCode) + "\r\n" + OutputTxt.Text;

            return avrdude.ExitCode;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter sw = new StreamWriter(save_fname);
            last_url = FuseCalcWeb.Url.ToString();
            last_opt = OptTxt.Text;
            sw.WriteLine(last_url);
            sw.WriteLine(last_opt);
            int h = Math.Max(500, this.Size.Height);
            int w = Math.Max(500, this.Size.Width);
            int s = WindowSplitter.SplitterDistance;
            if (this.Size.Height < 250)
            {
                s = 250;
            }
            sw.WriteLine(Convert.ToString(h));
            sw.WriteLine(Convert.ToString(w));
            sw.WriteLine(Convert.ToString(s));
            sw.Close();
        }

        private void BurnBtn_Click(object sender, EventArgs e)
        {
            bool lfuse_f = false;
            bool hfuse_f = false;
            bool efuse_f = false;
            bool fuse_f = false;
            string lfuse_s = "";
            string hfuse_s = "";
            string efuse_s = "";
            string fuse_s = "";
            string dev = "";
            string page = "";
            int indexof = -1;

            try
            {
                page = FuseCalcWeb.DocumentText;

                indexof = page.IndexOf("<select size=\"1\" name=\"P\" onChange=\"this.form.submit()\" style=\"font-weight: bold;\">", 0);
                indexof = page.IndexOf("<option selected value=\"", indexof);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("\"", indexof);
                    indexof += 1;
                    int indexofnext = page.IndexOf("\"", indexof);
                    dev = " -p " + page.Substring(indexof, indexofnext - indexof);
                }

                indexof = page.IndexOf("<input type=\"text\" name=\"V_LOW\"", 0);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("value=\"", indexof);
                    indexof += 7;
                    lfuse_s = page.Substring(indexof, 2);
                    lfuse_f = true;
                }

                indexof = page.IndexOf("<input type=\"text\" name=\"V_HIGH\"", 0);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("value=\"", indexof);
                    indexof += 7;
                    hfuse_s = page.Substring(indexof, 2);
                    hfuse_f = true;
                }

                indexof = page.IndexOf("<input type=\"text\" name=\"V_EXTENDED\"", 0);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("value=\"", indexof);
                    indexof += 7;
                    efuse_s = page.Substring(indexof, 2);
                    efuse_f = true;
                }

                indexof = page.IndexOf("<input type=\"text\" name=\"V_BYTE0\"", 0);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("value=\"", indexof);
                    indexof += 7;
                    fuse_s = page.Substring(indexof, 2);
                    fuse_f = true;
                }

                string args = "";

                args += dev;

                if (lfuse_f)
                {
                    args += " -U lfuse:w:0x" + lfuse_s.ToUpper() + ":m";
                }

                if (fuse_f)
                {
                    args += " -U fuse:w:0x" + fuse_s.ToUpper() + ":m";
                }

                if (hfuse_f)
                {
                    args += " -U hfuse:w:0x" + hfuse_s.ToUpper() + ":m";
                }

                if (efuse_f)
                {
                    args += " -U efuse:w:0x" + efuse_s.ToUpper() + ":m";
                }

                if (Forbid.Checked)
                {
                    args += " -n";
                }

                args += " -u";

                args = OptTxt.Text + args;

                ExecuteAVRDUDE(args);
            }
            catch
            {

            }
        }

        private void ReadBtn_Click(object sender, EventArgs e)
        {
            string dev = "";
            int indexof = -1;

            try
            {
                string page = FuseCalcWeb.DocumentText;

                indexof = page.IndexOf("<select size=\"1\" name=\"P\" onChange=\"this.form.submit()\" style=\"font-weight: bold;\">", 0);
                indexof = page.IndexOf("<option selected value=\"", indexof);
                if (indexof != -1)
                {
                    indexof = page.IndexOf("\"", indexof);
                    indexof += 1;
                    int indexofnext = page.IndexOf("\"", indexof);
                    dev = " -p " + page.Substring(indexof, indexofnext - indexof);
                }
            }
            catch
            {
            }

            if (dev.Length > 0)
            {
                string urlstr = base_url + "?P=" + dev.Substring(4);

                bool success_f = false;

                if (ExecuteAVRDUDE(OptTxt.Text + dev + " -U lfuse:r:lfusebin.bin:r") == 0)
                {
                    StreamReader sr = new StreamReader("lfusebin.bin");
                    BinaryReader br = new BinaryReader(sr.BaseStream);
                    string f = Convert.ToString(br.ReadByte(), 16).ToUpper();
                    if (f.Length == 1)
                    {
                        f = "0" + f;
                    }
                    urlstr += "&V_LOW=" + f;
                    success_f = true;
                    br.Close();
                    sr.Close();
                }

                if (ExecuteAVRDUDE(OptTxt.Text + dev + " -U hfuse:r:hfusebin.bin:r") == 0)
                {
                    StreamReader sr = new StreamReader("hfusebin.bin");
                    BinaryReader br = new BinaryReader(sr.BaseStream);
                    string f = Convert.ToString(br.ReadByte(), 16).ToUpper();
                    if (f.Length == 1)
                    {
                        f = "0" + f;
                    }
                    urlstr += "&V_HIGH=" + f;
                    success_f = true;
                    br.Close();
                    sr.Close();
                }

                if (ExecuteAVRDUDE(OptTxt.Text + dev + " -U efuse:r:efusebin.bin:r") == 0)
                {
                    StreamReader sr = new StreamReader("efusebin.bin");
                    BinaryReader br = new BinaryReader(sr.BaseStream);
                    string f = Convert.ToString(br.ReadByte(), 16).ToUpper();
                    if (f.Length == 1)
                    {
                        f = "0" + f;
                    }
                    urlstr += "&V_EXTENDED=" + f;
                    success_f = true;
                    br.Close();
                    sr.Close();
                }

                if (ExecuteAVRDUDE(OptTxt.Text + dev + " -U fuse:r:fusebin.bin:r") == 0)
                {
                    StreamReader sr = new StreamReader("fusebin.bin");
                    BinaryReader br = new BinaryReader(sr.BaseStream);
                    string f = Convert.ToString(br.ReadByte(), 16).ToUpper();
                    if (f.Length == 1)
                    {
                        f = "0" + f;
                    }
                    urlstr += "&V_BYTE0=" + f;
                    br.Close();
                    sr.Close();
                }

                urlstr += "&O_HEX=Apply+user+values";

                if (success_f)
                {
                    FuseCalcWeb.Navigate(urlstr);
                }
            }
        }

        private void Checker_Tick(object sender, EventArgs e)
        {
            if (FuseCalcWeb.IsBusy)
            {
                BurnBtn.Enabled = false;
                ReadBtn.Enabled = false;
            }
            else
            {
                if (FuseCalcWeb.DocumentText.Contains("<title>Engbedded Atmel AVR&reg; Fuse Calculator</title>") == false)
                {
                    FuseCalcWeb.Navigate(base_url);
                }
                else
                {
                    BurnBtn.Enabled = true;
                    ReadBtn.Enabled = true;

                    if (FuseCalcWeb.Url.ToString().Contains(base_url) && FuseCalcWeb.DocumentText.Contains("</html>"))
                    {
                        StreamReader sr = new StreamReader(FuseCalcWeb.DocumentStream);
                        StreamWriter sw = new StreamWriter("temppage.html");
                        string line;
                        do
                        {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                line = line.Replace("<div style=\"text-align: center; margin: 0 0 1em 0;\"", "<div>");
                                line = line.Replace("<form method=\"GET\">", "<form action=\"" + base_url + "\" method=\"GET\">");
                                line = line.Replace("http://pagead2.googlesyndication.com/pagead/show_ads.js", "");
                                sw.WriteLine(line);
                            }
                        }
                        while (line != null);
                        sr.Close();
                        sw.Close();
                        FileInfo fi = new FileInfo("temppage.html");
                        FuseCalcWeb.Navigate(fi.FullName);
                    }
                }
            }
        }

        private void FuseCalcWeb_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            
        }
    }
}