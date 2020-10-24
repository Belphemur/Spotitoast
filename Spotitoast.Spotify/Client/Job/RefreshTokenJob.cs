using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Exception;
using Spotitoast.Spotify.Client.Auth;

namespace Spotitoast.Spotify.Client.Job
{
    internal class RefreshTokenJob : IDelayedJob
    {
        private readonly SpotifyAuth _auth;

        public RefreshTokenJob(SpotifyAuth auth, TimeSpan delay)
        {
            Delay = delay;
            _auth = auth;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _auth.RefreshToken();
            return Task.CompletedTask;
        }

        public Task<bool> OnFailure(JobException exception)
        {
            return Task.FromResult(true);
        }

        public TimeSpan Delay { get; }
    }
}