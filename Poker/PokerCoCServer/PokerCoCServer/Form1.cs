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
    public partial class Form1 : Form
    {
        TcpListener tcpL;
        ArrayList lobby_list = new ArrayList();
        ArrayList game_rooms = new ArrayList();

        public Form1()
        {
            InitializeComponent();
        }

        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpL = new TcpListener(5000);

            tcpL.Start();
            tcpClientConnected.Reset();

            tcpL.BeginAcceptTcpClient(new AsyncCallback(NewClientEvent), tcpL);

            RequestListener.RunWorkerAsync();

            for (int p = 0; p < 5; p++)
            {
                GameRoom gr = new GameRoom("Empty Room", game_rooms.Count);
                game_rooms.Add(gr);
            }
        }

        delegate int LobbyListAddCallback(string ip, string n);
        delegate void RequestHandleCallback(int i);

        private void RequestHandle(int i)
        {
            string c;
            TcpClient tc = ((LobbyListEntry)lobby_list[i]).tc;
            int err = NetReceive(ref tc, out c, 100);
            if (err == 0)
            {
                if (c == "requestroomlist")
                {
                    for (int j = 0; j < game_rooms.Count; j++)
                    {
                        err = NetSend(ref tc, ((GameRoom)game_rooms[j]).id.ToString() + "," + ((GameRoom)game_rooms[j]).name + "," + ((GameRoom)game_rooms[j]).player_cnt + "," + ((GameRoom)game_rooms[j]).chips, 100);
                    }
                    err = NetSend(ref tc, "endaddgame", 100);
                    //MessageBox.Show("request received, " + Convert.ToString(err));
                }
                if (c == "joingame")
                {
                    string game_id;
                    err = NetReceive(ref tc, out game_id, 100);
                    err = NetSend(ref tc, "You have joined " + game_id, 100);
                }
                if (c == "requsers")
                {
                    string game_id;
                    err = NetReceive(ref tc, out game_id, 100);
                    err = NetSend(ref tc, "sdgsdf2435g," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdfgrty," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdfsdfgg," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdfgsdfg," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdf567g7," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdf567g6," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdf567g5," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdf567g3," + game_id, 100);
                    err = NetSend(ref tc, "sdgsdf567g1," + game_id, 100);
                }
            }
        }
        
        private int LobbyListAdd(string ip, string n)
        {
            int i = -1;
            if (this.LobbyListGrid.InvokeRequired)
            {
                // It's on a different thread, so use Invoke.
                LobbyListAddCallback d = new LobbyListAddCallback(LobbyListAdd);
                this.Invoke
                    (d, new object[] {ip, n});
            }
            else
            {
                // It's on the same thread, no need for Invoke
                this.LobbyListGrid.Rows.Add();
                i = lobby_list.Count;
                LobbyListGrid.Rows[i].Cells["Player1Index"].Value = i.ToString();
                LobbyListGrid.Rows[i].Cells["Player1IP"].Value = ip;
                LobbyListGrid.Rows[i].Cells["Player1Name"].Value = n;

                string[] str = new string[3];
                str[0] = i.ToString();
                str[1] = ip;
                str[2] = n;
                ListViewItem lvi = new ListViewItem(str);
                LobbyListView.Items.Add(lvi);
            }
            return i;
        }

        private void NewClientEvent(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            tcpClientConnected.Set();
            string c;
            int err = NetReceive(ref client, out c, 100);
            if (err == 0)
            {
                if (c == "joinlobby")
                {
                    string n;
                    err = NetReceive(ref client, out n, 100);
                    LobbyListEntry lle = new LobbyListEntry();
                    lle.name = n;
                    lle.tc = client;
                    err = NetSend(ref client, "Test String", 100);

                    string ip_str = client.Client.LocalEndPoint.ToString();

                    lle.i = LobbyListAdd(ip_str, n);

                    lobby_list.Add(lle);
                }
            }
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
                //sw.Start();

                for (int i = 0; i < len && sw.ElapsedMilliseconds < to * len; i++)
                {
                    st.WriteByte((byte)s[i]);
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
                //sw.Start();

                for (int i = 0; i < len && sw.ElapsedMilliseconds < to * len; i++)
                {
                    int b;
                    do
                    {
                        b = st.ReadByte();
                    }
                    while (b == -1 && sw.ElapsedMilliseconds < to * len);
                    
                    s += (char)b;
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

        struct LobbyListEntry
        {
            public TcpClient tc;
            public string name;
            public int i;
        }

        private void ConnectionAcceptingTimer_Tick(object sender, EventArgs e)
        {
            IPAddress[] local_IP = Dns.GetHostAddresses(Dns.GetHostName());
            string internal_ip_str = "";
            foreach (IPAddress i in local_IP)
            {
                internal_ip_str += i.ToString() + " ";
            }

            if (tcpClientConnected.WaitOne(10))
            {
                tcpClientConnected.Reset();
                tcpL.BeginAcceptTcpClient(new AsyncCallback(NewClientEvent), tcpL);
            }

            for (int i = 0; i < lobby_list.Count; i++)
            {
                if (((LobbyListEntry)lobby_list[i]).tc.Client.Connected == false)
                {
                    int to_remove = ((LobbyListEntry)lobby_list[i]).i;
                    lobby_list.RemoveAt(i);
                    for (int j = 0; j < LobbyListGrid.Rows.Count; j++)
                    {
                        try
                        {
                            if (int.Parse((string)(LobbyListGrid.Rows[j].Cells["Player1Index"].Value)) == to_remove)
                            {
                                LobbyListGrid.Rows.RemoveAt(j - 1);
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                    for (int j = 0; j < LobbyListView.Items.Count; j++)
                    {
                        try
                        {
                            if (int.Parse(LobbyListView.Items[j].SubItems[0].Text) == to_remove)
                            {
                                LobbyListView.Items.RemoveAt(j - 1);
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        ((LobbyListEntry)lobby_list[i]).tc.GetStream().WriteByte(0);
                    }
                    catch (Exception ex)
                    {
                        int to_remove = ((LobbyListEntry)lobby_list[i]).i;
                        lobby_list.RemoveAt(i);
                        for (int j = 0; j < LobbyListGrid.Rows.Count; j++)
                        {
                            try
                            {
                                if (int.Parse((string)(LobbyListGrid.Rows[j].Cells["Player1Index"].Value)) == to_remove)
                                {
                                    LobbyListGrid.Rows.RemoveAt(j - 1);
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                        for (int j = 0; j < LobbyListView.Items.Count; j++)
                        {
                            try
                            {
                                if (int.Parse(LobbyListView.Items[j].SubItems[0].Text) == to_remove)
                                {
                                    LobbyListView.Items.RemoveAt(j - 1);
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    i = 0;
                }
            }
        }

        
        private void RequestListener_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                for (int i = 0; i < lobby_list.Count; i++)
                {
                    if (((LobbyListEntry)lobby_list[i]).tc.Available > 0)
                    {
                        RequestHandleCallback d = new RequestHandleCallback(RequestHandle);
                        this.Invoke(d, new object[] { i });
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
