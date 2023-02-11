using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using Spotitoast.Banner.Client;
using Spotitoast.Banner.Model;
using Spotitoast.Configuration;
using Spotitoast.HotKeys.Handler;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Context
{
    public class SpotitoastContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        private readonly HotkeysConfiguration _configuration;

        public SpotitoastContext(HotkeysConfiguration configuration, IActionFactory actionFactory,
                                 ISpotifyNotifier spotifyClient)
        {
            _trayIcon = BuildTrayIcon();
            InitBannerClient();
            _configuration = configuration;
            RegisterHotkeys(actionFactory);
            RegisterBanner(spotifyClient);
            RegisterBalloonTip(spotifyClient);
        }

        private void RegisterBalloonTip(ISpotifyNotifier spotifyClient)
        {
            spotifyClient.TrackLiked
                         .Subscribe(track =>
                         {
                             _trayIcon.BalloonTipTitle = @"Spotitoast liked 💖";
                             _trayIcon.BalloonTipText = $@"{track.Name} - {track.ArtistsDisplay}";
                             _trayIcon.ShowBalloonTip(1000);
                         });

            spotifyClient.TrackDisliked.Subscribe(track =>
            {
                _trayIcon.BalloonTipTitle = @"Spotitoast disliked 🖤";
                _trayIcon.BalloonTipText = $@"{track.Name} - {track.ArtistsDisplay}";
                _trayIcon.ShowBalloonTip(1000);
            });
        }

        private static void RegisterBanner(ISpotifyNotifier spotifyClient)
        {
            spotifyClient.TrackPlayed
                         .Select(track => Observable.FromAsync(async () =>
                         {
                             var bannerData = new BannerData()
                             {
                                 Title = $"{(track.IsLoved ? @"💖 " : null)}{track.Name}",
                                 Text = $"{track.Album.Name} ({track.Album.ReleaseDate.Year})",
                                 SubText = track.ArtistsDisplay,
                                 Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                             };
                             return bannerData;
                         }))
                         .Concat()
                         .Subscribe(BannerClient.ShowNotification);
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
                         .Select(action => Observable.FromAsync(action.Execute))
                         .Concat()
                         .Subscribe();
        }

        private void InitBannerClient()
        {
            new WindowsFormsSynchronizationContext().Post(state => BannerClient.Setup(), this);
        }


        private NotifyIcon BuildTrayIcon()
        {
// Initialize Tray Icon
            using var manifestResourceStream =
                Assembly.GetEntryAssembly()?.GetManifestResourceStream("Spotitoast.Resources.spotify.ico") ??
                throw new InvalidOperationException();
            var trayIcon = new NotifyIcon
            {
                Icon = new Icon(manifestResourceStream),

                ContextMenuStrip = new ContextMenuStrip
                {
                    Items = { { "Exit", null, Exit } }
                },
                BalloonTipTitle = @"Spotitoast",
                Text = @"Spotitoast",
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