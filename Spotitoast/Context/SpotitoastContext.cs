using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Spotitoast.Banner.Client;
using Spotitoast.Banner.Model;
using Spotitoast.Configuration;
using Spotitoast.HotKeys.Handler;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Context
{
    public class SpotitoastContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly SynchronizationContext _uiThreadContext = new WindowsFormsSynchronizationContext();

        private readonly ConfigurationManager _configurationManager;
        private readonly HotkeysConfiguration _configuration;

        public SpotitoastContext(ConfigurationManager configurationManager, IActionFactory actionFactory,
            ISpotifyPlayer spotifyClient)
        {
            _configurationManager = configurationManager;
            _trayIcon = BuildTrayIcon();
            InitBannerClient();
            _configuration = _configurationManager.LoadConfiguration<HotkeysConfiguration>();
            RegisterHotkeys(actionFactory);
            RegisterBanner(spotifyClient);
            RegisterBalloonTip(spotifyClient);
        }

        private void RegisterBalloonTip(ISpotifyPlayer spotifyClient)
        {
            spotifyClient.TrackLiked.Subscribe(track =>
            {
                _trayIcon.BalloonTipTitle = "Spotitoast liked 💖";
                _trayIcon.BalloonTipText = $@"{track.Name} - {track.Album.Name}";
                _trayIcon.ShowBalloonTip(1000);
            });

            spotifyClient.TrackDisliked.Subscribe(track =>
            {
                _trayIcon.BalloonTipTitle = "Spotitoast disliked 🖤";
                _trayIcon.BalloonTipText = $@"{track.Name} - {track.Album.Name}";
                _trayIcon.ShowBalloonTip(1000);
            });
        }

        private static void RegisterBanner(ISpotifyPlayer spotifyClient)
        {
            spotifyClient.TrackPlayed.Subscribe(track =>
            {
                var bannerData = new BannerData()
                {
                    Title = track.Name,
                    Text = track.Album.Name,
                    SubText = string.Join(", ", track.Artists),
                    Image = track.Album.Art.ResizeImage(new Size(100, 100))
                };

                BannerClient.ShowNotification(bannerData);
            });
        }

        private void RegisterHotkeys(IActionFactory actionFactory)
        {
            foreach (var key in _configuration.HotKeys.Keys)
            {
                HotKeyHandler.RegisterHotKey(key);
            }

            HotKeyHandler.HotKeyPressed
                .Select(keys => actionFactory.Get(_configuration.GetAction(keys)))
                .Where(action => action != null)
                .Subscribe(action => action.Execute());
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