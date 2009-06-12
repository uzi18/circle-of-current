using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace PokerCoCServer
{
    public partial class Form1 : Form
    {
        TcpListener tcpL;
        PlayerList pl;

        public Form1()
        {
            InitializeComponent();
        }

        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        private void Form1_Load(object sender, EventArgs e)
        {
            pl = new PlayerList();
            tcpL = new TcpListener(5000);

            tcpClientConnected.Reset();

            tcpL.BeginAcceptTcpClient(new AsyncCallback(NewClientEvent), tcpL);
        }

        private void NewClientEvent(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            GamePlayer gp = new GamePlayer(client, "new player");
            pl.AddPlayer(gp);
            tcpClientConnected.Set();
        }

        private void ConnectionAcceptingTimer_Tick(object sender, EventArgs e)
        {
            IPAddress[] local_IP = Dns.GetHostAddresses(Dns.GetHostName());
            string internal_ip_str = "";
            foreach (IPAddress i in local_IP)
            {
                internal_ip_str += i.ToString() + " ";
            }
            label1.Text = internal_ip_str;
            if (tcpClientConnected.WaitOne(100))
            {
                MessageBox.Show("New Client " + pl.Player.Current.Client.Client.LocalEndPoint.ToString());
            }
        }
    }
}
