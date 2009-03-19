using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SettingsGeneratorTool
{
    public partial class Form1 : Form
    {
        string[] val_name;
        string[] val_data;
        string output_str;

        public Form1()
        {
            InitializeComponent();

            val_name = new string[128];
            val_data = new string[128];
        }

        private void SaveListBut_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("savedlist");
            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                sw.WriteLine((string)ListGrid.Rows[i].Cells[0].Value + "#" + (string)ListGrid.Rows[i].Cells[1].Value);
            }
            sw.Close();
        }

        private void LoadBut_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("savedlist");
            ListGrid.Rows.Clear();
            int cnt = 0;
            while(true)
            {
                string line = sr.ReadLine();

                if (line == null) break;

                string[] parts = line.Split('#');

                try
                {
                    ListGrid.Rows.Add();
                    ListGrid.Rows[cnt].Cells[0].Value = parts[0];
                    ListGrid.Rows[cnt].Cells[1].Value = parts[1];
                    cnt++;
                }
                catch { }
            }
            sr.Close();
        }

        private void GenBut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                val_name[i] = (string)ListGrid.Rows[i].Cells[0].Value;
                if (val_name[i] == null)
                {
                    val_name[i] = "";
                }
                val_name[i] = val_name[i].Trim();
                if (val_name[i].Length == 0)
                {
                    val_name[i] = "_blank_name_" + i;
                }
                val_name[i] = val_name[i].Replace(' ', '_');
                val_data[i] = (string)ListGrid.Rows[i].Cells[1].Value;
                if (val_data[i] == null)
                {
                    val_data[i] = "";
                }
                val_data[i] = val_data[i].Trim();
                if (val_data[i].Length == 0)
                {
                    val_data[i] = "0";
                }
            }

            output_str = "";

            output_str += "// define default values\r\n\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "#define " + val_name[i] + "_def_val " + val_data[i] + "\r\n";
            }

            output_str += "\r\n// define addresses\r\n\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "#define " + val_name[i] + "_addr " + i + "\r\n";
            }

            output_str += "\r\n// make union\r\n\r\n";

            output_str += "typedef union saved_params_s_ {\r\n";


            output_str += "\tstruct {\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "\t\tdouble " + val_name[i] + ";\r\n";
            }

            output_str += "\t} d_s;\r\n\r\n";

            output_str += "\tstruct {\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "\t\tsigned long " + val_name[i] + ";\r\n";
            }

            output_str += "\t} sl_s;\r\n\r\n";

            output_str += "\tstruct {\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "\t\tunsigned long " + val_name[i] + ";\r\n";
            }

            output_str += "\t} ul_s;\r\n\r\n";


            output_str += "\tdouble d_val[" + Convert.ToString(ListGrid.RowCount - 1) + "];\r\n";
            output_str += "\tsigned long sl_val[" + Convert.ToString(ListGrid.RowCount - 1) + "];\r\n";
            output_str += "\tunsigned long ul_val[" + Convert.ToString(ListGrid.RowCount - 1) + "];\r\n";
            output_str += "\tunsigned char c[" + Convert.ToString((ListGrid.RowCount - 1) * 4) + "];\r\n";

            output_str += "} saved_params_s;\r\n\r\n";

            output_str += "void params_set_default();\r\n\r\n";

            output_str += "#ifdef COMING_FROM_SAVE_C\r\n\r\n";
            output_str += "static saved_params_s saved_params;\r\n\r\n";
            output_str += "// set default\r\n\r\n";

            output_str += "void params_set_default()\r\n{\r\n";

            for (int i = 0; i < ListGrid.RowCount - 1; i++)
            {
                output_str += "\tsaved_params.d_s." + val_name[i] + " = " + val_name[i] + "_def_val;\r\n";
            }

            output_str += "}\r\n\r\n#endif\r\n\r\n";

            OutputText.Text = "#ifndef settings_autogen_h_inc\r\n#define settings_autogen_h_inc\r\n\r\n" + output_str + "\r\n\r\n#endif\r\n\r\n";

            OutputText.Focus();
            OutputText.SelectAll();
        }
    }
}