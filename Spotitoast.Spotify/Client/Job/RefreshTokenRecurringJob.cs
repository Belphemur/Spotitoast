using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Spotitoast.Spotify.Client.Auth;
using Spotitoast.Spotify.Configuration;

namespace Spotitoast.Spotify.Client.Job
{
    internal class RefreshTokenRecurringJob : IRecurringJob
    {
        private readonly SpotifyAuth _auth;

        public RefreshTokenRecurringJob(SpotifyAuth auth, SpotifyAuthConfiguration authConfiguration)
        {
            _auth = auth;
            SetDelayFromToken(authConfiguration.LastToken);
            authConfiguration.PropertyUpdated.Subscribe(property =>
            {
                if (property != nameof(SpotifyAuthConfiguration.LastToken))
                {
                    return;
                }

                SetDelayFromToken(authConfiguration.LastToken);
            });
        }

        private void SetDelayFromToken([CanBeNull] SpotifyAuthConfiguration.Token token)
        {
            Delay = (token?.ExpirationDate != null ? token!.Expire : TimeSpan.FromSeconds(70)) - TimeSpan.FromSeconds(60);
            Trace.WriteLine($"Update delay of Refresh job to {Delay}");
        }

        public IRetryAction FailRule { get; } = new AlwaysRetry();
        public TimeSpan? MaxRuntime { get; } = null;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _auth.RefreshToken();
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }
        

        public TimeSpan Delay { get; private set; }
    }
}