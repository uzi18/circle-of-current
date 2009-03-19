using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrigTableGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GenAtan(object sender, EventArgs e)
        {
            TableBox.Text = "#define atan_multiplier " + NumOfEntries.Value + "\r\n";
            TableBox.Text += "const double atan_tbl [atan_multiplier + 1] PROGMEM = {";
            string str = "";
            GenProgress.Maximum = Convert.ToInt32(NumOfEntries.Value);
            for (int i = 0, j = 0; i <= Convert.ToInt32(NumOfEntries.Value); i++, j++)
            {
                if (j % 5 == 0)
                {
                    str += "\r\n\t";
                    j = 0;
                }
                double val = Math.Atan((double)i / (double)NumOfEntries.Value);
                if (DegreeSwitch.Checked)
                {
                    val *= 57.2957795130823;
                }
                str += string.Format("{0,18:F15}, ", val);

                GenProgress.Value = i;
            }
            TableBox.Text += str + "\r\n};\r\n\r\n";

            TableBox.Focus();
            TableBox.SelectAll();
        }

        private void GenAsin(object sender, EventArgs e)
        {
            TableBox.Text = "#define asin_multiplier " + NumOfEntries.Value + "\r\n";
            TableBox.Text += "const double asin_tbl [asin_multiplier + 1] PROGMEM = {";
            string str = "";
            GenProgress.Maximum = Convert.ToInt32(NumOfEntries.Value);
            for (int i = 0, j = 0; i <= Convert.ToInt32(NumOfEntries.Value); i++, j++)
            {
                if (j % 5 == 0)
                {
                    str += "\r\n\t";
                    j = 0;
                }
                double val = Math.Asin((double)i / (double)NumOfEntries.Value);
                if (DegreeSwitch.Checked)
                {
                    val *= 57.2957795130823;
                }
                str += string.Format("{0,18:F15}, ", val);

                GenProgress.Value = i;
            }
            TableBox.Text += str + "\r\n};\r\n\r\n";

            TableBox.Focus();
            TableBox.SelectAll();
        }
    }
}
