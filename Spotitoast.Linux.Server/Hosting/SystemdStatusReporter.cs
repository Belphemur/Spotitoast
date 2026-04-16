using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Model.Song;

#nullable enable

namespace Spotitoast.Linux.Server.Hosting
{
    /// <summary>
    /// Reports the currently playing track (and like/dislike events) to systemd
    /// via the <c>STATUS=</c> notification so that
    /// <c>systemctl --user status spotitoast</c> shows live information.
    /// When the application is not running under systemd the reporter is a no-op.
    /// </summary>
    public class SystemdStatusReporter(ISpotifyNotifier spotifyNotifier, ISystemdNotifier? systemdNotifier = null)
        : IHostedService, IDisposable
    {
        private IDisposable? _trackPlayedSubscription;
        private IDisposable? _trackLikedSubscription;
        private IDisposable? _trackDislikedSubscription;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (systemdNotifier is null || !SystemdHelpers.IsSystemdService())
            {
                return Task.CompletedTask;
            }

            _trackPlayedSubscription = spotifyNotifier.TrackPlayed
                .Subscribe(track => NotifyStatus("Playing", track));

            _trackLikedSubscription = spotifyNotifier.TrackLiked
                .Subscribe(track => NotifyStatus("Liked", track));

            _trackDislikedSubscription = spotifyNotifier.TrackDisliked
                .Subscribe(track => NotifyStatus("Disliked", track));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _trackPlayedSubscription?.Dispose();
            _trackLikedSubscription?.Dispose();
            _trackDislikedSubscription?.Dispose();
            _trackPlayedSubscription = null;
            _trackLikedSubscription = null;
            _trackDislikedSubscription = null;
        }

        private void NotifyStatus(string prefix, ITrack track)
        {
            systemdNotifier?.Notify(
                new ServiceState($"STATUS={prefix}: {track.Name} \u2014 {track.ArtistsDisplay}"));
        }
    }
}
