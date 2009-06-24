using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
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

namespace PokerCoCServer
{
    public partial class MainMonitor : Form
    {
        Lobby lobby;
        public MainMonitor()
        {
            InitializeComponent();
            lobby = new Lobby();
        }

        private void MainMonitor_Load(object sender, EventArgs e)
        {
            
        }

        private void RefreshLobbyBtn_Click(object sender, EventArgs e)
        {
            LobbyListView.Items.Clear();
            for (int i = 0; i < lobby.lobby_list.Count; i++)
            {
                string[] str = new string[3];
                str[0] = ((LobbyListEntry)lobby.lobby_list[i]).i.ToString();
                str[1] = ((LobbyListEntry)lobby.lobby_list[i]).tc.Client.LocalEndPoint.ToString();
                str[2] = ((LobbyListEntry)lobby.lobby_list[i]).name;
                ListViewItem lvi = new ListViewItem(str);
                LobbyListView.Items.Add(lvi);
            }
            GameListView.Items.Clear();
            for (int i = 0; i < lobby.game_rooms.Count; i++)
            {
                string[] str = new string[4];
                str[0] = ((GameRoom)lobby.game_rooms[i]).id.ToString();
                str[1] = ((GameRoom)lobby.game_rooms[i]).name;
                str[2] = ((GameRoom)lobby.game_rooms[i]).player_cnt.ToString();
                str[3] = ((GameRoom)lobby.game_rooms[i]).chips.ToString();
                ListViewItem lvi = new ListViewItem(str);
                GameListView.Items.Add(lvi);
            }
        }

        private void GameListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = int.Parse(GameListView.SelectedItems[0].SubItems[0].Text);
            GamePlayerList.Items.Clear();
            for (int j = 0; j < 9; j++)
            {

            }
        }

        private void Performance_ValueChanged(object sender, EventArgs e)
        {
            lobby.timeout = Convert.ToInt32(TimeoutTxt.Value);
            lobby.connection_check_speed = Convert.ToInt32(ConnCheckerSleepTxt.Value);
            lobby.room_management_speed = Convert.ToInt32(RoomManSleepTxt.Value);
            lobby.request_check_speed = Convert.ToInt32(ReqHandlerSleepTxt.Value);
        }

        private void MainMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            lobby.room_man_thread.Abort();
            lobby.connection_check_thread.Abort();
            lobby.request_handle_thread.Abort();
        }
    }
}
