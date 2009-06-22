using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokerCoCServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class GamePlayer
    {
        public string name;
        public TcpClient tcpC;

        public GamePlayer(TcpClient tcpC_, string n)
        {
            tcpC = tcpC_;
            name = n;
        }
    }

    class GameRoom
    {
        public string name;
        public int id;

        GamePlayer[] player;

        public int chips;
        public int player_cnt;
        
        public GameRoom(string n, int i)
        {
            name = n;
            id = i;
            player = new GamePlayer[10];
        }
    }
}
