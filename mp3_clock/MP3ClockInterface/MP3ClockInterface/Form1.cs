using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MP3ClockInterface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string str = "";

        private void Looper_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                while (SerPort.BytesToRead > 0)
                {
                    int cint = SerPort.ReadByte();
                    char c = Convert.ToChar(cint);
                    str += c;
                    if (c == '\n')
                    {
                        LogList.Items.Insert(0, str.Substring(0, str.Length - 2));
                        str = "";
                    }
                }
            }
            else
            {
                try
                {
                    SerPort.Open();
                    LogList.Items.Insert(0, "Port Open");
                }
                catch
                {
                    LogList.Items.Insert(0, "Cannot Open Port");
                }
            }
        }

        private void SendCmd(int cmd)
        {
            if (SerPort.IsOpen)
            {
                byte[] b = new byte[1] {Convert.ToByte(cmd)};
                SerPort.Write(b, 0, 1);
            }
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            SendCmd(0);
        }

        private void MenuBtn_Click(object sender, EventArgs e)
        {
            SendCmd(5);
        }

        private void LeftBtn_Click(object sender, EventArgs e)
        {
            SendCmd(2);
        }

        private void RightBtn_Click(object sender, EventArgs e)
        {
            SendCmd(1);
        }

        private void DownBtn_Click(object sender, EventArgs e)
        {
            SendCmd(4);
        }

        private void UpBtn_Click(object sender, EventArgs e)
        {
            SendCmd(3);
        }
    }
}
