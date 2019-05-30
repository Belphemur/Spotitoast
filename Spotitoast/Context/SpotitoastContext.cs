using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Spotitoast.Banner.Client;
using Spotitoast.Configuration;

namespace Spotitoast.Context
{
    public class SpotitoastContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly SynchronizationContext _uiThreadContext = new WindowsFormsSynchronizationContext();

        private readonly ConfigurationManager _configurationManager;

        public SpotitoastContext(ConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            _trayIcon = BuildTrayIcon();
            InitBannerClient();
        }

        private void InitBannerClient()
        {
            _uiThreadContext.Post(state => BannerClient.Setup(), this);
        }


        private NotifyIcon BuildTrayIcon()
        {
// Initialize Tray Icon
            using var manifestResourceStream =
                Assembly.GetEntryAssembly()?.GetManifestResourceStream("Spotitoast.Resources.spotify.ico") ??
                throw new InvalidOperationException();
            var trayIcon = new NotifyIcon()
            {
                Icon = new Icon(manifestResourceStream),
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", Exit)
                }),
                BalloonTipTitle = "Spotitoast",
                Text = "Spotitoast",
                Visible = true
            };
            return trayIcon;
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            Application.Exit();
        }
    }
}