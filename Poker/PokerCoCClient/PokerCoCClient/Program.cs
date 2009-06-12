using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokerCoCClient
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
            Login user = new Login();
            Application.Run(user);
            Application.Run(new Client(user.name));
        }
    }
}
