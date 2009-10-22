using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public partial class ManagerWindow : Form
    {
        private Server server;

        public ManagerWindow(Server server)
        {
            InitializeComponent();

            this.server = server;
            this.server.OnChatEvent += new Server.OnChatCall(server_OnChatEvent);

            Logger.LogTextBox = this.txtLog;

            string hostName = Dns.GetHostName();
            txtServerInfo.Text += "Host Name: " + hostName + "\r\n";

            foreach (IPAddress i in Dns.GetHostAddresses(hostName))
            {
                txtServerInfo.Text += "IP: " + i.ToString() + "\r\n";
            }

            txtServerInfo.Text += "LocalEndPoint: " + server.LocalEndPoint + "\r\n";
        }

        private void server_OnChatEvent(string name, string msg)
        {
            if (txtGlobalChat.InvokeRequired)
            {
                txtGlobalChat.Invoke(new Server.OnChatCall(server_OnChatEvent), new object[] { name, msg, });
            }
            else
            {
                txtGlobalChat.Text = String.Format("({0}) {1}: {2}\r\n{3}", DateTime.Now.ToString("h:mm:ss tt"), name, msg, txtGlobalChat.Text);
                if (txtGlobalChat.Text.Length > txtGlobalChat.MaxLength / 2)
                    txtGlobalChat.Text = txtGlobalChat.Text.Substring(0, txtGlobalChat.MaxLength / 4);
            }
        }

        private void ManagerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ManagerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Destroy();
            Process.GetCurrentProcess().Kill();
        }

        private void listPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listPlayers.SelectedIndex >= 0)
            {
                string name = (string)listPlayers.Items[listPlayers.SelectedIndex];
                server.GetLobbyPlayerInfo(name, txtPlayerInfo);
            }
        }

        private void listPlayers_Click(object sender, EventArgs e)
        {
            listPlayers_SelectedIndexChanged(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            server.FillPlayerListBox(listPlayers);
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (txtChatEntry.Text.Length > 0)
                server.OnGlobalChatEvent("Admin", txtChatEntry.Text);
            txtChatEntry.Text = "";
        }

        private void txtChatEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnChat_Click(sender, null);
        }
    }
}
