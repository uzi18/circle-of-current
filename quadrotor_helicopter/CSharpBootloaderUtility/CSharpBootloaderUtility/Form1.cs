using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CSharpBootloaderUtility
{
    public partial class Form1 : Form
    {
        Int16[] flash_data;
        Int16 last_addr;
        Int16 page_num;
        bool bootload_enabled;

        public Form1()
        {
            InitializeComponent();

            flash_data = new Int16[65536];
            bootload_enabled = false;
            CancelBtn.Enabled = false;

            for (int i = 0; i < 32; i++)
            {
                SerPort.PortName = "COM" + Convert.ToString(i);
                try
                {
                    SerPort.Open();
                    PortList.Items.Add(SerPort.PortName);
                    PortList.SelectedIndex = 0;
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
        }

        private void Log(string s)
        {
            LogTxt.Text = s + "\r\n" + LogTxt.Text;
        }

        private void ProcTimer_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                PortStatus.Text = "Opened";

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
                        Log("MCU Sent Ready Signal");
                    }

                    if (bootload_enabled)
                    {
                        if (addr == 0xFFFF)
                        {
                            byte[] b = new byte[2] { (byte)(page_num >> 8), (byte)(page_num % 256), };
                            SerPort.Write(b, 0, 2);
                            Log("Replied Number of Pages: " + Convert.ToString(page_num, 10));
                        }
                        else
                        {
                            Log("MCU Requested Page # " + Convert.ToString(addr, 10));
                            if (addr == page_num)
                            {
                                Log("Bootloader Finished");

                                bootload_enabled = false;
                                StartBtn.Enabled = true;
                                BrowseBtn.Enabled = true;
                                CancelBtn.Enabled = true;
                            }
                            else
                            {
                                Int16 checksum = 0;
                                byte[] b = new byte[2];
                                for (int i = addr * 256, j = 0; j < 256; i++, j++)
                                {
                                    b[0] = (byte)flash_data[i];
                                    checksum += flash_data[i];
                                    SerPort.Write(b, 0, 1);
                                    LoadProgress.Value = Convert.ToInt32(Math.Min(i, LoadProgress.Maximum));
                                }
                                b[0] = (byte)(checksum >> 8);
                                b[1] = (byte)(checksum % 256);
                                SerPort.Write(b, 0, 2);
                            }
                        }
                    }
                }
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
                    StartBtn.Enabled = true;
                    BrowseBtn.Enabled = true;
                    CancelBtn.Enabled = true;
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
                StartBtn.Enabled = true;
                BrowseBtn.Enabled = true;
                CancelBtn.Enabled = true;
            }
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Hex Files (*.hex)|*.hex";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FilePathTxt.Text = ofd.FileName;
            }            
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(FilePathTxt.Text))
            {
                Log("File Does Not Exist");

                bootload_enabled = false;
                StartBtn.Enabled = true;
                BrowseBtn.Enabled = true;
                CancelBtn.Enabled = true;

                return;
            }

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
                        for (int i = 0, j = 9, k = 0; i < byte_cnt - 11; i += 2, j += 2, k++)
                        {
                            flash_data[addr + k] = Convert.ToInt16(str.Substring(j, 2), 16);
                            if ((addr + k) > last_addr)
                            {
                                last_addr = (Int16)(addr + k);
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
                StartBtn.Enabled = true;
                BrowseBtn.Enabled = true;
                CancelBtn.Enabled = true;
            }
            else
            {
                Log("File Successfully Read");
                Log("Waiting for MCU Ready Signal");

                bootload_enabled = true;
                StartBtn.Enabled = false;
                BrowseBtn.Enabled = false;
                CancelBtn.Enabled = true;

                page_num = (Int16)((last_addr + 256) / 256);
                page_num += 1;
                LoadProgress.Maximum = page_num * 256;

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
            StartBtn.Enabled = true;
            BrowseBtn.Enabled = true;
            CancelBtn.Enabled = false;
        }   
    }
}