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
                    err = NetSend(ref tc, "foo,1,2", 100);
                    err = NetSend(ref tc, "go o,1," + Convert.ToString(DateTime.Now.Second), 100);
                    err = NetSend(ref tc, "endaddgame", 100);
                    MessageBox.Show("request received, " + Convert.ToString(err));
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
                i = this.LobbyListGrid.Rows.Add();
                LobbyListGrid.Rows[i].Cells["Index"].Value = i.ToString();
                LobbyListGrid.Rows[i].Cells["PlayerIP"].Value = ip;
                LobbyListGrid.Rows[i].Cells["PlayerName"].Value = n;
            }
            return i;
        }

        private void NewClientEvent(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            tcpClientConnected.Set();
            string n;
            int err = NetReceive(ref client, out n, 100);
            if (err == 0)
            {
                LobbyListEntry lle = new LobbyListEntry();
                lle.name = n;
                lle.tc = client;
                err = NetSend(ref client, "Test String", 100);

                string ip_str = client.Client.LocalEndPoint.ToString();

                lle.i = LobbyListAdd(ip_str, n);

                lobby_list.Add(lle);
                MessageBox.Show("New Client " + client.Client.LocalEndPoint.ToString() + " , NetReceive = " + n + " , NetSend = " + Convert.ToString(err));
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
                    lobby_list.RemoveAt(i);
                    for (int j = 0; j < LobbyListGrid.Rows.Count; j++)
                    {
                        if ((string)(LobbyListGrid.Rows[j].Cells["Index"].Value) == Convert.ToString(i))
                        {
                            LobbyListGrid.Rows.RemoveAt(j);
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
                        lobby_list.RemoveAt(i);
                        for (int j = 0; j < LobbyListGrid.Rows.Count; j++)
                        {
                            if (int.Parse((string)LobbyListGrid.Rows[j].Cells["Index"].Value) == i)
                            {
                                LobbyListGrid.Rows.RemoveAt(j);
                            }
                        }
                    }
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
