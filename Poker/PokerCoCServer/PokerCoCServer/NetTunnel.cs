using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

class NetTunnel
{
    TcpClient tc = new TcpClient();
    int to;

    public NetTunnel(TcpClient tc_, int to_)
    {
        tc = tc_;
        to = to_;
    }

    public int Send(string s)
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

    public int Receive(out string s)
    {
        s = "";

        try
        {
            if (tc.Connected == false) return 1;

            tc.ReceiveTimeout = to;
            Stream st = tc.GetStream();

            st.ReadTimeout = to;

            int len;

            do
            {
                len = st.ReadByte();
            } while (len == 0);

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
}