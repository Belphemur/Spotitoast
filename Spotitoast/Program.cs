using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ninject;
using Spotitoast.Banner.Client;
using Spotitoast.Configuration;
using Spotitoast.Context;
using Spotitoast.HotKeys.Handler;
using Spotitoast.Logic.Dependencies;

namespace Spotitoast
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {

            HotKeyHandler.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Bootstrap.Kernel.Get<SpotitoastContext>());

            HotKeyHandler.Stop();
        }
    }
}
