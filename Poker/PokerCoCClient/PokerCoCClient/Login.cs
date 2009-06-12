using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokerCoCClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        public string name = "";

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                name = textBox1.Text;
                //TO DO: SEND NAME TO SERVER!
                this.Close();
            }
        }
    }
}
