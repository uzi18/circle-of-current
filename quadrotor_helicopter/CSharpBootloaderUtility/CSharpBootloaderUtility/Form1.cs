using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace CSharpBootloaderUtility
{
    public partial class Form1 : Form
    {
        delegate void SetTextCallback(string s);
        delegate void SetProgressCallback(int v);
        delegate void SetBtnCallback(bool a, bool b, bool c);

        short[] flash_data;
        short last_addr;
        short page_num;
        short previous_addr;
        short checksum;
        int retry_cnt;
        bool bootload_enabled;
        string workdir;
        Stopwatch stopwatch;

        public Form1()
        {
            workdir = Directory.GetCurrentDirectory();

            InitializeComponent();

            flash_data = new short[65536];
            bootload_enabled = false;
            CancelBtn.Enabled = false;

            stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 32; i++)
            {
                SerPort.PortName = "COM" + Convert.ToString(i);
                try
                {
                    SerPort.Open();
                    PortList.Items.Add(SerPort.PortName);
                }
                catch
                {
                }
                try
                {
                    SerPort.Close();
                }
                catch
                {
                }
            }

            try
            {
                PortList.SelectedIndex = 0;
            }
            catch
            {
            }

            if (File.Exists("savefile.txt"))
            {
                StreamReader sr = new StreamReader("savefile.txt");

                AddToList(ref PortList, sr.ReadLine());

                for (int i = 0; i < 10; i++)
                {
                    AddToList(ref FilePathTxt, sr.ReadLine());
                }

                sr.Close();
            }

            DataProcessor.RunWorkerAsync();
        }

        bool preventstackovf = false;

        private void AddToList(ref ComboBox cb, string str)
        {
            if (str != "#")
            {
                while (cb.Items.Count > 10)
                {
                    cb.Items.RemoveAt(cb.Items.Count - 1);
                }
                while (cb.Items.Contains(str))
                {
                    cb.Items.Remove(str);
                }
                cb.Items.Insert(0, str);

                preventstackovf = true;
                cb.SelectedIndex = 0;
                preventstackovf = false;
            }
        }



        private void ProgressSet(int v)
        {
            if (this.LoadProgress.InvokeRequired)
            {
                SetProgressCallback d = new SetProgressCallback(ProgressSet);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                LoadProgress.Value = v;
            }
        }

        private void ButtonsSet(bool a, bool b, bool c)
        {
            if (this.StartBtn.InvokeRequired || this.CancelBtn.InvokeRequired || this.BrowseBtn.InvokeRequired)
            {
                SetBtnCallback d = new SetBtnCallback(ButtonsSet);
                this.Invoke(d, new object[] { a, b, c });
            }
            else
            {
                StartBtn.Enabled = a;
                BrowseBtn.Enabled = b;
                CancelBtn.Enabled = c;
            }
        }

        private void Log(string s)
        {
            if (this.LogTxt.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Log);
                this.Invoke(d, new object[] {s});
            }
            else
            {
                LogTxt.Text = s + "\r\n" + LogTxt.Text;
            }
        }

        private void ProcessSend()
        {
            if (SerPort.IsOpen == false || bootload_enabled == false)
            {
                Thread.Sleep(250);
                return;
            }
            while (SerPort.BytesToRead >= 2)
            {
                while (SerPort.BytesToRead != 2)
                {
                    SerPort.ReadByte();
                }
                int c0 = SerPort.ReadByte();
                int c1 = SerPort.ReadByte();
                int addr = c0 + (c1 << 8);

                if (addr == 0xFFFF)
                {
                    stopwatch.Start();
                    byte[] b = new byte[2] { (byte)(page_num % 256), (byte)(page_num >> 8), };
                    SerPort.Write(b, 0, 2);
                    Log("MCU Sent Ready Signal");
                    Log("Replied Number of Pages: " + Convert.ToString(page_num, 10));
                }
                else if (addr == page_num)
                {
                    stopwatch.Stop();
                    long time_ms = stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    double time_s = (double)time_ms / 1000.0;

                    Log("Bootloader Finished in " + Convert.ToString(time_s) + " Seconds");

                    bootload_enabled = false;
                    ButtonsSet(true, true, false);
                }
                else
                {
                    if (previous_addr == addr)
                    {
                        Log("Page Re-requested: " + Convert.ToString(addr, 10));
                        retry_cnt++;
                    }
                    else
                    {
                        retry_cnt = 0;
                    }
                    if ((previous_addr == addr || previous_addr + 1 == addr) && retry_cnt < 3)
                    {
                        previous_addr = (short)addr;
                        checksum = 0;
                        byte[] b = new byte[256];
                        while ((SerPort.WriteBufferSize - 256) < SerPort.BytesToWrite) ;
                        if (page_num < 6)
                        {
                            for (int i = addr * 256, j = 0; j < 256; i++, j++)
                            {
                                b[0] = (byte)flash_data[i];
                                checksum += flash_data[i];
                                SerPort.Write(b, 0, 1);
                                ProgressSet(Convert.ToInt32(Math.Min(i, LoadProgress.Maximum)));
                            }
                        }
                        else
                        {
                            for (int i = addr * 256, j = 0; j < 256; i++, j++)
                            {
                                b[j] = (byte)flash_data[i];
                                checksum += flash_data[i];
                            }
                            SerPort.Write(b, 0, 256);
                            ProgressSet(Convert.ToInt32(Math.Min((addr * 256) + 256, LoadProgress.Maximum)));
                        }
                        b[0] = (byte)(checksum % 256);
                        b[1] = (byte)(checksum >> 8);
                        SerPort.Write(b, 0, 2);
                    }
                    else
                    {
                        Log("Fatal Error, Received: 0x" + Convert.ToString(addr, 16).ToUpper() + ", Checksum was: 0x" + Convert.ToString(checksum, 16).ToUpper() + ", Retried: " + Convert.ToString(retry_cnt));

                        bootload_enabled = false;
                        ButtonsSet(true, true, false);
                    }
                }
            }
        }

        private void ProcTimer_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                PortStatus.Text = "Opened";

                //ProcessSend();
            }
            else
            {
                try
                {
                    SerPort.PortName = PortList.Items[PortList.SelectedIndex].ToString();
                    SerPort.Open();
                    Log("Port Opened");
                }
                catch
                {
                    PortStatus.Text = "Error Opening";

                    bootload_enabled = false;
                    ButtonsSet(false, true, false);
                }
            }
        }

        private void PortList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log("Port Changed");
            try
            {
                SerPort.Close();
                SerPort.PortName = PortList.Items[PortList.SelectedIndex].ToString();
                SerPort.Open();
                Log("Port Opened");
            }
            catch
            {
                Log("Port Cannot be Opened");

                bootload_enabled = false;
                ButtonsSet(true, true, false);
            }
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Hex Files (*.hex)|*.hex";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddToList(ref FilePathTxt, ofd.FileName);
            }            
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            LoadProgress.Value = 0;

            if (!File.Exists(FilePathTxt.Text))
            {
                Log("File Does Not Exist");

                bootload_enabled = false;
                ButtonsSet(true, true, false);

                return;
            }

            AddToList(ref FilePathTxt, FilePathTxt.Text);

            for (int i = 0; i < 65536; i++)
            {
                flash_data[i] = 0xFF;
            }

            StreamReader sr;
            sr = new StreamReader(FilePathTxt.Text);

            string str = "";
            bool fail_flag = false;

            do
            {
                try
                {
                    str = sr.ReadLine();
                    if (str == null)
                    {
                        Log("End of File Reached");
                    }
                    int record_type = Convert.ToInt32(str.Substring(7, 2), 16);
                    if (record_type == 0)
                    {
                        int byte_cnt = Convert.ToInt32(str.Substring(1, 2), 16);
                        int addr = Convert.ToInt32(str.Substring(3, 4), 16);
                        for (int i = 0, j = 9; i < byte_cnt; i++, j += 2)
                        {
                            flash_data[addr + i] = Convert.ToInt16(str.Substring(j, 2), 16);
                            if ((addr + i) > last_addr)
                            {
                                last_addr = (short)(addr + i);
                            }
                        }
                    }
                    else if (record_type == 1)
                    {
                        Log("End of Flash Reached");
                        break;
                    }
                }
                catch
                {
                    fail_flag = true;
                }
            }
            while (str != null);

            if (fail_flag)
            {
                Log("File Not Read");

                bootload_enabled = false;
                ButtonsSet(true, true, false);
            }
            else
            {
                Log("File Successfully Read");
                Log("Waiting for MCU Ready Signal");

                bootload_enabled = true;
                ButtonsSet(false, false, true);

                previous_addr = -1;
                retry_cnt = 0;
                page_num = (short)((last_addr + 256) / 256);
                LoadProgress.Maximum = page_num * 256;
                page_num++;

                if (SerPort.IsOpen)
                {
                    while (SerPort.BytesToRead > 0)
                    {
                        SerPort.ReadByte();
                    }
                }
            }

            sr.Close();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            bootload_enabled = false;
            ButtonsSet(true, true, false);
        }

        private void FilePathTxt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (preventstackovf == false)
            {
                AddToList(ref FilePathTxt, FilePathTxt.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Directory.SetCurrentDirectory(workdir);

            StreamWriter sw = new StreamWriter("savefile.txt");

            sw.WriteLine(PortList.Text);

            for (int i = 9; i >= 0; i--)
            {
                if (i < FilePathTxt.Items.Count)
                {
                    sw.WriteLine(FilePathTxt.Items[i]);
                }
                else
                {
                    sw.WriteLine("#");
                }
            }

            sw.Close();
        }

        private void SerPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //ProcessSend();
        }

        private void DataProcessor_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                ProcessSend();
            }
        }   
    }
}