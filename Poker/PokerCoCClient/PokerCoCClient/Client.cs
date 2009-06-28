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
        string address = "poker.circleofcurrent.com";
        NetTunnel lobbyTunnel;
        

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
            
            lobbyTunnel = new NetTunnel(500);
            lobbyTunnel.Connect(address, 5000);
            lobbyTunnel.Send ("joinlobby");
            lobbyTunnel.Send (username);                            

            string WelcomeMessage;
            lobbyTunnel.Receive(out WelcomeMessage);
            receiveGameList();
            MessageBox.Show(WelcomeMessage);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://mark.circleofcurrent.com");
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            //
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void receiveGameList()
        {
            lobbyTunnel.Send("requestroomlist");
            rooms.Clear();
            listView1.Items.Clear();
            while (true)
            {
                string tmp;
                lobbyTunnel.Receive(out tmp);
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

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lobbyTunnel.Send("joingame");
            lobbyTunnel.Send(listView1.SelectedItems[0].SubItems[0].Text);
            lobbyTunnel.Send(username);

            string key;

            lobbyTunnel.Receive(out key);
            ProcessStartInfo psi = new ProcessStartInfo("C:/Users/Mark/Desktop/CoCPoker/PokerCoCGameRoom/PokerCoCGameRoom/bin/Debug/PokerCoCGameRoom.exe", key);
            Process.Start(psi);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            listBox2.Items.Clear();
            string[] users = new string[9];
            lobbyTunnel.Send("requsers");
            lobbyTunnel.Send(listView1.SelectedItems[0].SubItems[0].Text);
            while (true)
            {
                string tmp;
                lobbyTunnel.Receive(out tmp);
                if (tmp == "endplayerlist")
                {
                    return;
                }
                else
                {
                    string[] s = tmp.Split(',');
                    if (s.Length == 2)
                    {
                        listBox2.Items.Add(s[0] + "  /  Chips: " + s[1]);
                    }
                }
            }
        }
    }
}