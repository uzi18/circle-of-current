using System;
using System.Collections;
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
using System.Diagnostics;

namespace PokerCoCClient
{
    public partial class Client : Form
    {
        ArrayList rooms = new ArrayList();
        private string username;
        int roomCount;


        //Connect to server
        string address = "5.163.58.64";
        TcpClient tcpclnt = new TcpClient();

        struct GameListEntry
        {
            public string gameName;
            public string playerCount;
            public string totalChips;
        }

        public Client(string username_)
        {
            InitializeComponent();
            this.username = username_;
            label1.Text = label1.Text + " : " + username;
            tcpclnt.Connect(address, 5000);
            Stream netstream = tcpclnt.GetStream();
            NetSend(ref tcpclnt, username, 1000);
            string WelcomeMessage;
            NetReceive(ref tcpclnt, out WelcomeMessage, 1000);
            MessageBox.Show(WelcomeMessage);
        }

        private int NetSend(ref TcpClient tc, string s, int to)
        {
           try
           {
               if (tc.Connected == false) return 1;

               tc.SendTimeout = to;
               Stream st = tc.GetStream();

               st.WriteTimeout = to;
               byte len = Convert.ToByte(s.Length);

               st.WriteByte(len);

               Stopwatch sw = new Stopwatch();
               sw.Reset();
               sw.Start();

               for (int i = 0; i < len && sw.ElapsedMilliseconds < to * len; i++)
               {
                   st.WriteByte((byte)(s[i]));
               }

               sw.Stop();
               if (sw.ElapsedMilliseconds < to * len)
               {
                   return 0;
               }
               else
               {
                   return 1;
               }
           }
           catch (Exception e)
           {
               return 1;
           }
       }

       private int NetReceive(ref TcpClient tc, out string s, int to)
       {
           s = "";

           try
           {
               if (tc.Connected == false) return 1;

               tc.ReceiveTimeout = to;
               Stream st = tc.GetStream();

               st.ReadTimeout = to;

               retry_place:

               int len = st.ReadByte();

               if (len == 0) goto retry_place;

               Stopwatch sw = new Stopwatch();
               sw.Reset();
               sw.Start();

               for (int i = 0; i < len && sw.ElapsedMilliseconds < to * len; i++)
               {
                   int b;
                   do
                   {
                       b = st.ReadByte();
                   }
                   while (b == -1 && sw.ElapsedMilliseconds < to * len);

                   s += (char)(b);
               }

               sw.Stop();
               if (sw.ElapsedMilliseconds < to * len)
               {
                   return 0;
               }
               else
               {
                   return 1;
               }
           }
           catch (Exception e)
           {
               return 1;
           }
       }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            tcpclnt.Client.Disconnect(false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetSend(ref tcpclnt, "requestroomlist", 1000);
            rooms.Clear();
            dataGridView1.Rows.Clear();
            while(true)
            {
                string tmp;
                NetReceive(ref tcpclnt, out tmp, 1000);
                if (tmp == "endaddgame")
                {
                    return;
                }
                else 
                {
                    GameListEntry gle = new GameListEntry();
                    string[] game = tmp.Split(',');
                    gle.gameName = game[0];
                    gle.playerCount = game[1];
                    gle.totalChips = game[2];
                    rooms.Add(gle);
                    int i = dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = gle.gameName;
                    dataGridView1.Rows[i].Cells[1].Value = gle.playerCount;
                    dataGridView1.Rows[i].Cells[2].Value = gle.totalChips;
                }
            }
        }
    }
}