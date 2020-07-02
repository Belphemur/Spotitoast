using System;
using System.Drawing;
using Notify.Linux.Client;
using Spotitoast.Linux.Command;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Linux.Notification
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly ISpotifyNotifier _spotifyNotifier;
        private readonly INotificationClient _notificationClient;
        private readonly ICommandExecutor _commandExecutor;

        public NotificationHandler(ISpotifyNotifier spotifyNotifier, INotificationClient notificationClient, ICommandExecutor commandExecutor)
        {
            _spotifyNotifier = spotifyNotifier;
            _notificationClient = notificationClient;
            _commandExecutor = commandExecutor;
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
            _spotifyNotifier.TrackLiked.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                await _notificationClient.NotifyAsync(new SpotitoastNotification
                {
                    Body =  "Spotitoast liked 💖",
                    Summary =  $@"{track.Name} - {track.ArtistsDisplay}",
                    Expiration = 1,
                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                });

            });
            
            _spotifyNotifier.TrackDisliked.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                await _notificationClient.NotifyAsync(new SpotitoastNotification
                {
                    Body =  "Spotitoast disliked💖 💔",
                    Summary =  $@"{track.Name} - {track.ArtistsDisplay}",
                    Expiration = 1,
                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                });
            });
        }

        private void RegisterTrackPlayed()
        {
            _spotifyNotifier.TrackPlayed.Subscribe(async trackTask =>
            {
                var track = await trackTask;
                var notificationData = new SpotitoastNotification
                {
                    Summary =  $"{(track.IsLoved ? @"💖 " : null)}{track.Name}",
                    Body = $"{track.Album.Name} ({track.Album.ReleaseDate.Year})\n{track.ArtistsDisplay}",
                    Expiration = 1,
                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                };
                if (!track.IsLoved)
                {
                    notificationData.Actions = new[]
                    {
                        new NotificationData.Action
                        {
                            Key = ActionFactory.PlayerAction.Like.ToString(),
                            Label = " 💖 Like",
                            OnActionCalled = () => _commandExecutor.Execute(ActionFactory.PlayerAction.Like)
                        },
                    };
                }
                else
                {
                    notificationData.Actions = new[]
                    {
                        new NotificationData.Action
                        {
                            Key = ActionFactory.PlayerAction.Dislike.ToString(),
                            Label = "💔 Dislike",
                            OnActionCalled = () => _commandExecutor.Execute(ActionFactory.PlayerAction.Dislike)
                        },
                    };
                }

                await _notificationClient.NotifyAsync(notificationData);
            });
        }
    }
}