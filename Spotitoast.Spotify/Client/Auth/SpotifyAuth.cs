using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using JetBrains.Annotations;
using Job.Scheduler.Scheduler;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using Spotitoast.Configuration;
using Spotitoast.Spotify.Client.Job;
using Spotitoast.Spotify.Configuration;

namespace Spotitoast.Spotify.Client.Auth
{
    internal class SpotifyAuth
    {
        internal class TokenUpdatedEventArg : EventArgs
        {
            public TokenUpdatedEventArg(Token newToken)
            {
                NewToken = newToken;
            }

            public Token NewToken { get; }
        }

        private readonly SpotifyAuthConfiguration _config;
        private readonly IJobScheduler            _jobScheduler;
        private readonly TokenSwapAuth            _tokenSwapAuth;
        private          bool                     _gettingToken = false;

        /// <summary>
        /// Currently used token
        /// </summary>
        [CanBeNull]
        public Token AuthToken => _config.LastToken;

        /// <summary>
        /// Triggered when the Access token is updated
        /// </summary>
        public event EventHandler<TokenUpdatedEventArg> TokenUpdated;

        public SpotifyAuth(SpotifyAuthConfiguration configuration, IJobScheduler jobScheduler)
        {
            _config            = configuration;
            _jobScheduler = jobScheduler;
            _tokenSwapAuth = new TokenSwapAuth(
                exchangeServerUri: _config.ExchangeUrl,
                serverUri: _config.InnerServerUrl,
                scope: _config.Scopes
            )
            {
                TimeAccessExpiry = false
            };
            ConfigureAuthClient();
        }

        private void ConfigureAuthClient()
        {
            _tokenSwapAuth.AuthReceived += async (sender, response) =>
            {
                _config.LastToken = await _tokenSwapAuth.ExchangeCodeAsync(response.Code);

                TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
                _tokenSwapAuth.Stop();
                _gettingToken = false;

                var timeToRefresh = _config.LastToken?.ExpiresIn ?? 3600;

                RestartTimer(timeToRefresh);
            };
        }

        private void RestartTimer(double timeToRefresh)
        {
            _jobScheduler.ScheduleJob(new RefreshTokenJob(this, TimeSpan.FromSeconds(timeToRefresh - 60)));
        }

        internal async Task RefreshToken()
        {
            var token = await _tokenSwapAuth.RefreshAuthAsync(_config.LastToken?.RefreshToken);
            if (token == null)
            {
                RequestNewToken();
                return;
            }

            _config.UpdateAccessToken(token);

            TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
            var timeToRefresh = _config.LastToken.ExpiresIn;
            RestartTimer(timeToRefresh);
            Trace.Write("Token Refreshed");
        }

        /// <summary>
        /// Refresh the Access Token
        /// </summary>
        /// <param name="forceRenewal"></param>
        public Task RefreshAccessToken(bool forceRenewal = false)
        {
            if (forceRenewal)
            {
                RequestNewToken();
                return Task.CompletedTask;
            }

            if (_config.LastToken == null)
            {
                RequestNewToken();
                return Task.CompletedTask;
            }

            if (_config.LastToken.HasError())
            {
                RequestNewToken();
                return Task.CompletedTask;
            }

            return RefreshToken();
        }

        private void RequestNewToken()
        {
            lock (this)
            {
                if (_gettingToken) return;

                _gettingToken = true;

                _tokenSwapAuth.Start();
                _tokenSwapAuth.OpenBrowser();
            }
        }
    }
}