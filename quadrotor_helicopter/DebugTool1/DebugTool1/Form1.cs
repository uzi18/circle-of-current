using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DebugTool1
{
    public partial class Form1 : Form
    {
        int selection;

        public Form1()
        {
            InitializeComponent();

            string temp = "";
            for(int y = -2; y <= 2; y++)
	        {                
                for (int x = -2; x <= 2; x++)
		        {
                    temp += Convert.ToString(Math.Atan2(y, x)) + " " + y + " " + x +"\r\n";
		        }
	        }
            MessageBox.Show(temp);

            for (int i = 0; i < 64; i++)
            {
                string[] sub_items = new string[2];
                sub_items[0] = Convert.ToString(i);
                sub_items[1] = Convert.ToString(0);
                ListViewItem lvi = new ListViewItem(sub_items);
                DataList.Items.Add(lvi);
            }

            WatchMin.Minimum = int.MinValue / 2;
            WatchMin.Maximum = int.MaxValue / 2;

            WatchMax.Minimum = int.MinValue / 2;
            WatchMax.Maximum = int.MaxValue / 2;

            WatchMin.Value = -12000;
            WatchMax.Value = 12000;
        }

        private void FormChecker_Tick(object sender, EventArgs e)
        {
            WatchBar.Minimum = (int)Math.Min(WatchMin.Value, WatchMax.Value);
            WatchBar.Maximum = (int)Math.Max(WatchMin.Value, WatchMax.Value);

            try
            {
                WatchBar.Value = (int)Math.Min(Math.Max(WatchBar.Minimum, Convert.ToDecimal(DataList.Items[selection].SubItems[1].Text)), WatchBar.Maximum);
            }
            catch
            {
                WatchBar.Value = (int)Math.Min(Math.Max(WatchBar.Minimum, 0), WatchBar.Maximum);
            }
        }

        private void DataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selection = DataList.SelectedItems[0].Index;
                AddrLabel.Text = "Addr: " + Convert.ToString(selection);
            }
            catch
            {
            }
        }

        int addr, cnt, data, sign_flag;

        private void PortProcessor_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                int c;
                while (SerPort.BytesToRead != 0)
                {
                    c = SerPort.ReadByte();
                    if (c >= 0x80)
                    {
                        addr = (c & 0x7F) >> 1;
                        data = 0;
                        cnt = 0;
                        sign_flag = c % 2;
                    }
                    else
                    {
                        data += (c << (7 * cnt));
                        cnt++;
                        if (cnt == 5)
                        {
							double data_;
							
							if ((c & 0x40) == 0x40)
							{
								data -= (c << (7 * (cnt - 1)));								
								
								if ((c & 0x20) == 0x20)
								{
									data_ = (double)data * Math.Pow(10, -1 * (double)(c & 0x1F));
								}
								else
								{
									data_ = (double)data * Math.Pow(10, (double)(c & 0x1F));
								}
							}
							else
							{
								data_ = (double)data;
							}
							
                            if (sign_flag != 0)
                            {
                                data_ *= -1;
                            }
                            try
                            {
                                DataList.Items[addr].SubItems[1].Text = Convert.ToString(data_);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                SerPort.BaudRate = 19200;
                try
                {
                    SerPort.PortName = "COM6";
                    SerPort.Open();
                }
                catch
                {
                    try
                    {
                        SerPort.PortName = "COM3";
                        SerPort.Open();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
