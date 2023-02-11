using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using Notify.Linux.Client;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;
using Spotitoast.Logic.Model.Song;

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
            _spotifyNotifier.TrackLiked.Select(track => Observable.FromAsync(async () =>
                            {
                                await _notificationClient.NotifyAsync(new SpotitoastNotification
                                {
                                    Summary = "You liked 💖",
                                    Body = $@"{track.Name} - {track.ArtistsDisplay}",
                                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                                });
                            }))
                            .Concat()
                            .Subscribe();

            _spotifyNotifier.TrackDisliked
                            .Select(track => Observable.FromAsync(async () =>
                            {
                                await _notificationClient.NotifyAsync(new SpotitoastNotification
                                {
                                    Summary = "You disliked 💔",
                                    Body = $@"{track.Name} - {track.ArtistsDisplay}",
                                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                                });
                            }))
                            .Concat()
                            .Subscribe();
        }

        private void RegisterTrackPlayed()
        {
            _spotifyNotifier.TrackPlayed
                            .Select(track => Observable.FromAsync(async () =>
                            {
                                var notificationData = new SpotitoastNotification
                                {
                                    Summary = $"{(track.IsLoved ? @"💖 " : null)}{track.Name}",
                                    Body = $"{track.Album.Name} ({track.Album.ReleaseDate.Year})\n{track.ArtistsDisplay}",
                                    Expiration = TimeSpan.FromSeconds(2),
                                    Image = (await track.Album.Art).ResizeImage(new Size(100, 100))
                                };
                                SetActions(track, notificationData);

                                await _notificationClient.NotifyAsync(notificationData);
                            }))
                            .Concat()
                            .Subscribe();
        }

        private void SetActions(ITrack track, SpotitoastNotification notificationData)
        {
            if (!track.IsLoved)
            {
                notificationData.Actions = new[]
                {
                    new NotificationData.Action
                    {
                        Key = ActionFactory.PlayerAction.Like.ToString(),
                        Label = " 💖 Like",
                        OnActionCalled = () => _commandExecutor.Execute(ActionFactory.PlayerAction.Like)
                    }
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

            notificationData.Actions = notificationData.Actions.Append(new NotificationData.Action
                                                       {
                                                           Key = ActionFactory.PlayerAction.Skip.ToString(),
                                                           Label = "⏭️Skip",
                                                           OnActionCalled = () => _commandExecutor.Execute(ActionFactory.PlayerAction.Skip)
                                                       })
                                                       .ToArray();
        }
    }
}