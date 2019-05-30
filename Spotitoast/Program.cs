using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spotitoast.Context;
using Spotitoast.HotKeys.Handler;

namespace Spotitoast
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            HotKeyHandler.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SpotitoastContext());

            HotKeyHandler.Stop();
        }
    }
}
