using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Spotitoast.Spotify.Client.Job
{
    public class CheckCurrentlyPlayingJob : IRecurringJob
    {
        private readonly SpotifyClient _client;

        public CheckCurrentlyPlayingJob(SpotifyClient client, TimeSpan delay)
        {
            _client    = client;
            Delay = delay;
        }

        public IRetryAction FailRule { get; } = new AlwaysRetry();
        public TimeSpan? MaxRuntime { get; } = null;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            return _client.CheckCurrentPlayedTrackWithAutoRefresh();
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }

        public TimeSpan Delay { get; }
    }
}