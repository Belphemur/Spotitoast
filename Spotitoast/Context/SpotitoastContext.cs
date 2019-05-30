﻿using System;
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
            SpotifyClient spotifyClient)
        {
            _configurationManager = configurationManager;
            _trayIcon = BuildTrayIcon();
            InitBannerClient();
            _configuration = _configurationManager.LoadConfiguration<HotkeysConfiguration>();
            RegisterHotkeys(actionFactory);
            RegisterBanner(spotifyClient);
            RegisterBalloonTip(spotifyClient);
        }

        private void RegisterBalloonTip(SpotifyClient spotifyClient)
        {
            spotifyClient.TrackLiked.Subscribe(track =>
            {
                _trayIcon.BalloonTipText = $@"💖 {track.Name}";
                _trayIcon.ShowBalloonTip(1000);
            });

            spotifyClient.TrackDisliked.Subscribe(track =>
            {
                _trayIcon.BalloonTipText = $@"🖤 {track.Name}";
                _trayIcon.ShowBalloonTip(1000);
            });
        }

        private static void RegisterBanner(SpotifyClient spotifyClient)
        {
            spotifyClient.PlayedTrack.Subscribe(track =>
            {
                var trackName = track.Name;
                var artists = string.Join(", ", track.Artists.Select(artist => artist.Name));
                var imageUrl = track.Album.Images.First().Url;
                var bannerData = new BannerData(imageUrl, new Size(100, 100))
                {
                    Title = trackName,
                    Text = track.Album.Name,
                    SubText = artists
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