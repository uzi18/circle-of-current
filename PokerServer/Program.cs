using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using LogUtilities;

namespace PokerServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            if (Database.EstablishDatabaseConnection() == false)
            {
                Logger.Log(LogType.Error, "MySQL could not connect");
                return;
            }

            Database.CleanInactiveUsers();

            Server server = new Server(Properties.Settings.Default.serverport);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ManagerWindow(server));
        }
    }
}
