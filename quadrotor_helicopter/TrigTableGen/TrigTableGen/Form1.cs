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

        private void GenTable(object sender, EventArgs e)
        {
            TableBox.Text = "#ifndef trig_tbl_inc\r\n#define trig_tbl_inc\r\n\r\n#include <avr/pgmspace.h>\r\n#include \"config.h\"\r\n\r\n";
            TableBox.Text += "#ifdef use_atan\r\n\r\n#define atan_multiplier " + NumOfAtanEntries.Value + "\r\n";
            TableBox.Text += "const signed long atan_tbl [atan_multiplier + 1] PROGMEM = {";
            string str = "";
            for (int i = 0, j = 0; i <= Convert.ToInt32(NumOfAtanEntries.Value); i++, j++)
            {
                if (j % 5 == 0)
                {
                    str += "\r\n\t";
                    j = 0;
                }
                double val = Math.Atan((double)i / (double)NumOfAtanEntries.Value);
                val *= 57.2957795130823;
                val *= (double)MATH_MULTIPLIER.Value;
                str += string.Format("{0,6}, ", Math.Round(val));

            }
            TableBox.Text += str + "\r\n};\r\n\r\n#endif\r\n\r\n";

            str = "";

            TableBox.Text += "#ifdef use_asin\r\n\r\n#define asin_multiplier " + NumOfAsinEntries.Value + "\r\n";
            TableBox.Text += "const signed long asin_tbl [asin_multiplier + 1] PROGMEM = {";

            for (int i = 0, j = 0; i <= Convert.ToInt32(NumOfAsinEntries.Value); i++, j++)
            {
                if (j % 5 == 0)
                {
                    str += "\r\n\t";
                    j = 0;
                }
                double val = Math.Asin((double)i / (double)NumOfAsinEntries.Value);
                val *= 57.2957795130823;
                val *= (double)MATH_MULTIPLIER.Value;
                str += string.Format("{0,6}, ", Math.Round(val));
            }
            TableBox.Text += str + "\r\n};\r\n\r\n#endif\r\n\r\n#endif\r\n";

            TableBox.Focus();
            TableBox.SelectAll();
        }
    }
}
