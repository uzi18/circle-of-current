using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokerCoCGameRoom
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new GameRoom(args[0]));
            }
            catch
            { 
            }
        }
    }
}
