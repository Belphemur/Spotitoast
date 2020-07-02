using System;
using System.Drawing;
using Notify.Linux.Client;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Linux.Context
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly ISpotifyNotifier _spotifyClient;
        private readonly INotificationClient _notificationClient;

        public NotificationHandler(ISpotifyNotifier spotifyClient, INotificationClient notificationClient)
        {
            _spotifyClient = spotifyClient;
            _notificationClient = notificationClient;
        }

        /// <summary>
        /// Link different event (Like, track played etc ...) to a notification
        /// </summary>
        public void RegisterNotifications()
        {
            RegisterLikedDisliked();
            RegisterTrackPlayed();
        }

        private void RegisterLikedDisliked()
        {
            _spotifyClient.TrackLiked.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                await _notificationClient.NotifyAsync(new NotificationData
                {
                    ApplicationName = "Spotitoast",
                    Summary =  "Spotitoast liked💖",
                    Body =  $@"{track.Name} - {track.ArtistsDisplay}",
                    Expiration = 1
                });

            });
            
            _spotifyClient.TrackDisliked.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                await _notificationClient.NotifyAsync(new NotificationData
                {
                    ApplicationName = "Spotitoast",
                    Summary =  "Spotitoast disliked💖",
                    Body =  $@"{track.Name} - {track.ArtistsDisplay}",
                    Expiration = 1
                });
            });
        }

        private void RegisterTrackPlayed()
        {
            _spotifyClient.TrackPlayed.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                var notificationData = new NotificationData
                {
                    ApplicationName = "Spotitoast",
                    Summary =  $"{(track.IsLoved ? @"💖 " : null)}{track.Name}",
                    Body = $"{track.Album.Name} ({track.Album.ReleaseDate.Year})\n{track.ArtistsDisplay}",
                    Expiration = 1,
                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                };
                await _notificationClient.NotifyAsync(notificationData);
            });
        }
    }
}