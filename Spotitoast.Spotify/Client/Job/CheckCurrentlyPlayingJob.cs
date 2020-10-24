using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Exception;

namespace Spotitoast.Spotify.Client.Job
{
    public class CheckCurrentlyPlayingJob : IRecurringJob
    {
        private readonly SpotifyClient _client;

        public CheckCurrentlyPlayingJob(SpotifyClient client)
        {
            _client = client;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            return _client.CheckCurrentPlayedTrackWithAutoRefresh();
        }
        public Task<bool> OnFailure(JobException exception)
        {
            return Task.FromResult(true);
        }

        public TimeSpan Delay { get; } = TimeSpan.FromSeconds(15);
    }
}