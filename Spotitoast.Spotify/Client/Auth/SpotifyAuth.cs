using System;
using System.Diagnostics;
using System.Timers;
using JetBrains.Annotations;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using Spotitoast.Configuration;
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

        private readonly ConfigurationManager _configurationManager;
        private readonly SpotifyAuthConfiguration _config;
        private readonly TokenSwapAuth _tokenSwapAuth;
        private readonly Timer _refreshTimer = new Timer();

        /// <summary>
        /// Currently used token
        /// </summary>
        [CanBeNull]
        public Token AuthToken => _config.LastToken;

        /// <summary>
        /// Triggered when the Access token is updated
        /// </summary>
        public event EventHandler<TokenUpdatedEventArg> TokenUpdated;

        public SpotifyAuth(ConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            _config = _configurationManager.LoadConfiguration<SpotifyAuthConfiguration>();
            _tokenSwapAuth = new TokenSwapAuth(
                exchangeServerUri: _config.ExchangeUrl,
                serverUri: _config.InnerServerUrl,
                scope: _config.Scopes
            )
            {
                TimeAccessExpiry = false
            };

            _refreshTimer.Elapsed += (sender, args) => RefreshToken();
            ConfigureAuthClient();
        }

        private void ConfigureAuthClient()
        {
            _tokenSwapAuth.AuthReceived += async (sender, response) =>
            {
                _config.LastToken = await _tokenSwapAuth.ExchangeCodeAsync(response.Code);


                _configurationManager.SaveConfiguration(_config);
                TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
                _tokenSwapAuth.Stop();

                Debug.Assert(_config.LastToken != null, "_config.LastToken != null");
                var timeToRefresh = _config.LastToken.ExpiresIn;

                RestartTimer(timeToRefresh);
            };

            _tokenSwapAuth.OnAccessTokenExpired += (sender, args) => RefreshToken();
        }

        private void RestartTimer(double timeToRefresh)
        {
            _refreshTimer.Stop();
            _refreshTimer.Interval = (timeToRefresh - 60) * 1000L;
            _refreshTimer.Start();
        }

        private async void RefreshToken()
        {
            var token = await _tokenSwapAuth.RefreshAuthAsync(_config.LastToken?.RefreshToken);
            if (token == null)
            {
                RequestNewToken();
                return;
            }

            Debug.Assert(_config.LastToken != null, "_config.LastToken != null");

            _config.LastToken.AccessToken = token.AccessToken;
            _config.LastToken.ExpiresIn = token.ExpiresIn;
            _config.LastToken.CreateDate = token.CreateDate;


            _configurationManager.SaveConfiguration(_config);

            TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
            var timeToRefresh = _config.LastToken.ExpiresIn;
            RestartTimer(timeToRefresh);
        }

        /// <summary>
        /// Refresh the Access Token
        /// </summary>
        /// <param name="forceRenewal"></param>
        public void RefreshAccessToken(bool forceRenewal = false)
        {
            if (forceRenewal)
            {
                RequestNewToken();
                return;
            }

            if (_config.LastToken == null)
            {
                RequestNewToken();
                return;
            }

            if (_config.LastToken.HasError())
            {
                RequestNewToken();
                return;
            }

            RefreshToken();
        }

        private void RequestNewToken()
        {
            _tokenSwapAuth.Start();
            _tokenSwapAuth.OpenBrowser();
        }
    }
}