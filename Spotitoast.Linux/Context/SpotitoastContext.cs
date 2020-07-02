using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Player;

namespace Spotitoast.Linux.Context
{
    public class SpotitoastContext
    {
        public SpotitoastContext(IActionFactory actionFactory, ISpotifyNotifier spotifyClient)
        {
            RegisterBanner(spotifyClient);
            RegisterBalloonTip(spotifyClient);
        }

        private void RegisterBalloonTip(ISpotifyNotifier spotifyClient)
        {
            // spotifyClient.TrackLiked.Subscribe(track =>
            // {
            //     _trayIcon.BalloonTipTitle = "Spotitoast liked 💖";
            //     _trayIcon.BalloonTipText = $@"{track.Name} - {track.ArtistsDisplay}";
            //     _trayIcon.ShowBalloonTip(1000);
            // });
            //
            // spotifyClient.TrackDisliked.Subscribe(track =>
            // {
            //     _trayIcon.BalloonTipTitle = "Spotitoast disliked 🖤";
            //     _trayIcon.BalloonTipText = $@"{track.Name} - {track.ArtistsDisplay}";
            //     _trayIcon.ShowBalloonTip(1000);
            // });
        }

        private static void RegisterBanner(ISpotifyNotifier spotifyClient)
        {
            // spotifyClient.TrackPlayed.Subscribe(async trackTask =>
            // {
            //     var track = await trackTask;
            //     var bannerData = new BannerData()
            //     {
            //         Title = $"{(track.IsLoved ? @"💖 " : null)}{track.Name}",
            //         Text = $"{track.Album.Name} ({track.Album.ReleaseDate.Year})",
            //         SubText = track.ArtistsDisplay,
            //         Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
            //     };
            //
            //     BannerClient.ShowNotification(bannerData);
            // });
        }
    }
}