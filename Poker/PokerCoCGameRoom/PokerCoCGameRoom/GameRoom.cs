using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace PokerCoCGameRoom
{
    public partial class GameRoom : Form
    {
        NetTunnel nt;
        string address = "poker.circleofcurrent.com";
        string[] username = new string[9];
        int[] chips = new int[9];
        int pot = 0;

        public GameRoom(string ticket)
        {
            InitializeComponent();

            this.txtGroup = new System.Windows.Forms.TextBox[9];
            for (int i = 0; i < this.txtGroup.Length; i++)
            {
                this.txtGroup[i] = new System.Windows.Forms.TextBox();

                this.txtGroup[i].BackColor = System.Drawing.Color.Khaki;
                this.txtGroup[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.txtGroup[i].Cursor = System.Windows.Forms.Cursors.Default;
                this.txtGroup[i].Location = new System.Drawing.Point(0, 0 + i * 25);
                this.txtGroup[i].Name = "txtGroup[" + i.ToString() + "]";
                this.txtGroup[i].ReadOnly = true;
                this.txtGroup[i].Size = new System.Drawing.Size(107, 20);
                this.txtGroup[i].TabIndex = 0;
                this.txtGroup[i].TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            }
            for (int i = 0; i < this.txtGroup.Length; i++)
            {
                this.Controls.Add(this.txtGroup[i]);
            }

            nt = new NetTunnel(100);
            nt.Connect(address, 5000);
            nt.StartAsyncReceive();
            nt.Send("useticket");
            nt.Send(ticket);

            string tmp;
            nt.Receive(out tmp,5000);
            getTableStatus();
            string success;
            bool temp = nt.Receive(out success, 10000);
            MessageBox.Show(success + temp.ToString());
        }

        private void GameRoom_FormClosed(object sender, FormClosedEventArgs e)
        {
            nt.Disconnect();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (nt.Available)
            {
                string cmd;
                nt.Receive(out cmd);
                if (cmd == "chatmsg")
                {
                    string strmsg;
                    nt.Receive(out strmsg, 5000);
                    chatHistoryBox.Text = strmsg.Trim(new char[]{'\r','\n'}) + "\r\n" + chatHistoryBox.Text;
                }
                else if (cmd == "tablestatus")
                {
                    getTableStatus();
                }
            }
        }

        private void getTableStatus()
        {
            string seatedCount;

            nt.Receive(out seatedCount, 5000);
            if (seatedCount == "0")
            {
                textBox1.Text = "<EMPTY SEAT>";
                textBox2.Text = "<EMPTY SEAT>";
                textBox3.Text = "<EMPTY SEAT>";
                textBox4.Text = "<EMPTY SEAT>";
                textBox5.Text = "<EMPTY SEAT>";
                textBox6.Text = "<EMPTY SEAT>";
                textBox7.Text = "<EMPTY SEAT>";
                textBox8.Text = "<EMPTY SEAT>";
                textBox9.Text = "<EMPTY SEAT>";
            }
            else
            {
                for (int i = 0; i < int.Parse(seatedCount); i++)
                {

                }
            }
        }

        private void chatTypeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && chatTypeBox.Text.Length > 0)
            {
                nt.Send ("chatmsg");
                nt.Send (chatTypeBox.Text);
            }
        }

        private void chatTypeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chatTypeBox.Text = "";
            }
        }
    }
}
