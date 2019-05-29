using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Spotitoast.Context
{
    public class SpotitoastContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        public SpotitoastContext()
        {
            // Initialize Tray Icon
            using var manifestResourceStream = Assembly.GetEntryAssembly()?.GetManifestResourceStream("Spotitoast.Resources.spotify.ico") ?? throw new InvalidOperationException();
            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon(manifestResourceStream),
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Exit", Exit)
                }),
                BalloonTipTitle = "Spotitoast",
                Text = "Spotitoast",
                Visible = true
            };
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            Application.Exit();
        }
    }
}