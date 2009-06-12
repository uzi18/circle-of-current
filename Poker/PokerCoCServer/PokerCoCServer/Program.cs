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

    class GamePlayerNode
    {
        GamePlayerNode next;
        GamePlayerNode prev;
        GamePlayer cur;

        public GamePlayerNode Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        public GamePlayerNode Prev
        {
            get
            {
                return prev;
            }
            set
            {
                prev = value;
            }
        }

        public GamePlayer Current
        {
            get
            {
                return cur;
            }
            set
            {
                cur = value;
            }
        }

        public GamePlayerNode(GamePlayerNode p, GamePlayerNode n, GamePlayer gp)
        {
            prev = p;
            next = n;
            cur = gp;
        }
    }

    class GamePlayer
    {
        string name;
        TcpClient tcpC;

        public TcpClient Client
        {
            get
            {
                return tcpC;
            }
            set
            {
                tcpC = value;
            }
        }
        

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public GamePlayer(TcpClient tcpC_, string n)
        {
            tcpC = tcpC_;
            name = n;
        }
    }

    class PlayerList
    {
        int cnt;
        GamePlayerNode player;

        public int Count
        {
            get
            {
                return cnt;
            }
            set
            {
                cnt = value;
            }
        }

        public GamePlayerNode Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }

        public PlayerList()
        {
            player = new GamePlayerNode(null, null, null);
            player.Next = null;
            player.Prev = null;
            cnt = 0;
        }

        public void AddPlayer(GamePlayer p)
        {
            if (player.Next == null)
            {
                player = player.Prev = player.Next = new GamePlayerNode(player, player, p);
            }
            else
            {
                player = player.Next = new GamePlayerNode(player, player.Next, p);
            }
            cnt++;
        }

        public void ToNext()
        {
            if (player.Next != null)
            {
                player = player.Next;
            }
        }

        public void Remove(string n)
        {
            if (player.Next != null)
            {
                int iter = 0;
                do
                {
                    if (player.Current.Name == n)
                    {
                        player.Next = player.Next.Next;
                        cnt--;
                        break;
                    }
                    else
                    {
                        player = player.Next;
                        iter++;
                    }
                }
                while (iter < cnt);
            }
        }

        public void Clean()
        {
            if (player.Next != null)
            {
                int iter = 0;
                do
                {
                    if (false)
                    {
                        cnt--;
                    }
                    iter++;
                }
                while (iter < cnt);
            }
        }
    }

    class GameRoom
    {
        string name;
        GamePlayer[] player;
        
        public GameRoom(string n)
        {
            name = n;
            player = new GamePlayer[10];
        }
    }
}
