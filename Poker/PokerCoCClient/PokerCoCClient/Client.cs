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
using System.Threading;

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
        TcpClient temp = new TcpClient();

        struct GameListEntry
        {
            public string gameID;
            public string gameName;
            public string playerCount;
            public string totalChips;
        }

        public Client(string username_)
        {
            InitializeComponent();
            this.username = username_;
            label1.Text = label1.Text + "\n" + username;

            tcpclnt.Connect(address, 5000);
            NetSend(ref tcpclnt, "joinlobby", 1000);
            NetSend(ref tcpclnt, username, 1000);                            

            string WelcomeMessage;
            NetReceive(ref tcpclnt, out WelcomeMessage, 1000);
            receiveGameList();
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

        private void receiveGameList()
        {
            NetSend(ref tcpclnt, "requestroomlist", 1000);
            rooms.Clear();
            listView1.Items.Clear();
            while (true)
            {
                string tmp;
                NetReceive(ref tcpclnt, out tmp, 1000);
                if (tmp == "endaddgame")
                {
                    listView1.Columns[0].Width = listView1.Width * 5 / 100;
                    listView1.Columns[1].Width = listView1.Width * 59 / 100;
                    listView1.Columns[2].Width = listView1.Width * 15 / 100;
                    listView1.Columns[3].Width = listView1.Width * 20 / 100;
                    return;
                }
                else
                {
                    GameListEntry gle = new GameListEntry();
                    string[] game = tmp.Split(',');

                    if (game.Length == 4)
                    {
                        gle.gameID = game[0];
                        gle.gameName = game[1];
                        gle.playerCount = game[2];
                        gle.totalChips = game[3];
                        rooms.Add(gle);
                        ListViewItem lvi = new ListViewItem(game);
                        listView1.Items.Add(lvi);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            receiveGameList();
        }

        private void launchNewRoom()
        {
            /*
             * temp.Connect(address, 5000);
            NetSend(ref temp, username, 1000);
            Application.Run(new GameRoom());
            temp.Client.Disconnect(true);
            temp.Close();
            temp = new TcpClient();
            //MessageBox.Show(temp.Connected.ToString());
             */
            TcpClient tc = new TcpClient();
            tc.Connect(address, 5000);
            NetSend(ref tc, username, 1000);
            Application.Run(new GameRoom());
            tc.Client.Disconnect(true);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string msg;
            NetSend(ref tcpclnt, "joingame", 1000);
            NetSend(ref tcpclnt, listView1.SelectedItems[0].SubItems[0].Text, 1000);
            NetReceive(ref tcpclnt, out msg, 1000);
            MessageBox.Show(msg);
            Thread t = new Thread(new ThreadStart(launchNewRoom));
            t.Start();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            listBox2.Items.Clear();
            string[] users = new string[9];
            NetSend(ref tcpclnt, "requsers", 1000);
            NetSend(ref tcpclnt, listView1.SelectedItems[0].SubItems[0].Text, 1000);
            for (int i = 0; i < 9; i++)
            {
                NetReceive(ref tcpclnt, out users[i], 1000);
                listBox2.Items.Add(users[i]);
            }
        }
    }
}