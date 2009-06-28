using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Collections;

public class NetTunnel
{
    TcpClient tc = new TcpClient();
    Stream st;
    int to;
    bool mode = false;
    ArrayList data = new ArrayList();

    public NetTunnel(int to_)
    {
        to = to_;
        AsyncRxThread = new Thread(new ThreadStart(AsyncReceiveThread));
    }

    public void Connect(string addr, int port)
    {
        tc = new TcpClient(addr, port);

        tc.ReceiveTimeout = to;
        tc.SendTimeout = to;
        st = tc.GetStream();
        st.WriteTimeout = to;
        st.ReadTimeout = to;
    }

    public void Connect(TcpClient tc_)
    {
        tc = tc_;

        tc.ReceiveTimeout = to;
        tc.SendTimeout = to;
        st = tc.GetStream();
        st.WriteTimeout = to;
        st.ReadTimeout = to;
    }

    public void Disconnect()
    {
        mode = false;
        StopAsyncReceive();
        tc.Client.Disconnect(false);
    }

    public string Address
    {
        get
        {
            return tc.Client.RemoteEndPoint.ToString();
        }
    }

    public bool Connected
    {
        get
        {
            if (tc.Connected == false)
            {
                return false;
            }
            else
            {
                try
                {
                    st.WriteByte(0);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }

    private Thread AsyncRxThread;

    public void StartAsyncReceive()
    {
        if (AsyncRxThread.IsAlive == false)
        {
            mode = true;
            exit_thread = false;
            AsyncRxThread.Start();
        }
    }

    public void StopAsyncReceive()
    {
        mode = false;
        exit_thread = true;
        DateTime dt = DateTime.Now;
        while (AsyncRxThread.IsAlive && DateTime.Now.Subtract(dt).TotalMilliseconds > to * 256)
        {
            Thread.Sleep(0);
        }
        if (AsyncRxThread.IsAlive)
        {
            AsyncRxThread.Abort();
        }
    }

    public bool Available
    {
        get
        {
            if (AsyncRxThread.IsAlive && data.Count > 0)
            {
                return true;
            }
            else if (tc.Available > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool Send(string s)
    {
        try
        {
            byte len = Convert.ToByte(s.Length);

            st.WriteByte(len);

            for (int i = 0; i < len; i++)
            {
                st.WriteByte((byte)s[i]);
            }

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public bool Receive(out string s)
    {
        s = "";

        if (AsyncRxThread.IsAlive)
        {
            if (data.Count > 0)
            {
                s = (string)data[0];
                data.RemoveAt(0);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            try
            {
                DateTime dt = DateTime.Now;

                while (DateTime.Now.Subtract(dt).TotalMilliseconds < to * 5 && data.Count == 0)
                {
                    AsyncReceive();
                    Thread.Sleep(0);
                }

                if (data.Count > 0)
                {
                    s = (string)data[0];
                    data.RemoveAt(0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public bool Receive(out string s, int wait)
    {
        s = "";
        DateTime dt = DateTime.Now;

        while (DateTime.Now.Subtract(dt).TotalMilliseconds < wait)
        {
            bool res = Receive(out s);
            if (res)
            {
                return true;
            }
        }

        return false;
    }

    delegate void AsyncReceiveCallback();

    private void AsyncReceive()
    {
        DateTime dt = DateTime.Now;

        while (tc.Available > 0 && DateTime.Now.Subtract(dt).TotalMilliseconds < 1000)
        {
            try
            {
                int len = st.ReadByte();
                dt = DateTime.Now;
                string s = "";
                for (int i = 0; i < len; i++)
                {
                    int b = st.ReadByte();
                    if (b > 0)
                    {
                        s += (char)b;
                    }
                    else
                    {
                        i--;
                        if (DateTime.Now.Subtract(dt).TotalMilliseconds > to * len)
                        {
                            break;
                        }
                    }
                }

                if (s.Length == len && len != 0)
                {
                    data.Insert(data.Count, s);
                }
            }
            catch
            {
                while (tc.Available > 0)
                {
                    int i = st.ReadByte();
                    if (i == 0)
                    {
                        break;
                    }

                    Thread.Sleep(to);
                }
            }
        }
    }

    private volatile bool exit_thread = true;

    private void AsyncReceiveThread()
    {
        while (exit_thread == false)
        {
            AsyncReceiveCallback d = new AsyncReceiveCallback(AsyncReceive);
            d.Invoke();

            Thread.Sleep(to);
        }
    }
}