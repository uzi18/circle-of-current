using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace PokerCoCClient
{
    public partial class Client : Form
    {
        private string username; 

        public Client(string username_)
        {
            InitializeComponent();
            this.username = username_;

            //Connect to server
            string address = "129.97.209.42";

            TcpClient tcpclnt = new TcpClient();
            tcpclnt.Connect(address, 5000);

            Stream netstream = tcpclnt.GetStream();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome " + username + "!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
