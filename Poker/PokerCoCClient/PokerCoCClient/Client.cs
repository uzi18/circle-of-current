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
        //Connect to server
        string address = "5.163.58.64";

        TcpClient tcpclnt = new TcpClient();

        public Client(string username_)
        {
            InitializeComponent();
            this.username = username_;

            tcpclnt.Connect(address, 5000);
            Stream netstream = tcpclnt.GetStream();
            string a = "";

            byte[] bb = new byte[100];
            int k = netstream.Read(bb, 0, 4);
            for (int i = 0; i < k; i++)
                a += Convert.ToChar(bb[i]);

            MessageBox.Show(a);

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] bl = new byte[1];
            bl[0] = (byte)username.Length;
            byte[] ba = asen.GetBytes(this.username);
            netstream.Write(bl, 0, 1);
            netstream.Write(ba, 0, ba.Length);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome " + username + "!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            tcpclnt.Close();
        }
    }
}
