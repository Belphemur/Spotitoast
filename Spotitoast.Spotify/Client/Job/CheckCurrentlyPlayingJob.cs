using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Spotitoast.Spotify.Client.Job
{
    public class CheckCurrentlyPlayingJob(SpotifyClient client, TimeSpan delay) : IRecurringJob
    {
        public IRetryAction FailRule { get; } = new AlwaysRetry();
        public TimeSpan? MaxRuntime { get; } = null;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            return client.CheckCurrentPlayedTrackWithAutoRefresh();
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }

        public TimeSpan Delay { get; } = delay;
    }
}