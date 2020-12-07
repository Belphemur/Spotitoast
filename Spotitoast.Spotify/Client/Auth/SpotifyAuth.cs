using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Job.Scheduler.Scheduler;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Spotitoast.Spotify.Client.Job;
using Spotitoast.Spotify.Configuration;

namespace Spotitoast.Spotify.Client.Auth
{
    internal class SpotifyAuth
    {
        internal class TokenUpdatedEventArg : EventArgs
        {
            public TokenUpdatedEventArg(SpotifyAuthConfiguration.Token newToken)
            {
                NewToken = newToken;
            }

            public SpotifyAuthConfiguration.Token NewToken { get; }
        }

        private readonly SpotifyAuthConfiguration _config;
        private readonly IOAuthClient _tokenSwapAuth;
        private bool _gettingToken;
        private readonly EmbedIOAuthServer _server;

        /// <summary>
        /// Triggered when the Access token is updated
        /// </summary>
        public event EventHandler<TokenUpdatedEventArg> TokenUpdated;

        public SpotifyAuth(SpotifyAuthConfiguration configuration, IJobScheduler jobScheduler)
        {
            _config = configuration;
            _tokenSwapAuth = new OAuthClient();
            _server = new EmbedIOAuthServer(_config.ListenUri, _config.ListenPort);
            ConfigureAuthServer(jobScheduler);
        }

        private void ConfigureAuthServer(IJobScheduler jobScheduler)
        {
            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            jobScheduler.ScheduleJob(new RefreshTokenRecurringJob(this, _config));
        }

        internal async Task RefreshToken()
        {
            if (_config.LastToken == null)
            {
                await RequestNewToken();
                return;
            }

            var token = await _tokenSwapAuth.RequestToken(new TokenSwapRefreshRequest(_config.ExchangeUrl, _config.LastToken.RefreshToken));

            _config.LastToken = _config.LastToken with {AccessToken = token.AccessToken, Expire = TimeSpan.FromSeconds(token.ExpiresIn)};

            TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
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
                return RequestNewToken();
            }

            if (_config.LastToken == null)
            {
                return RequestNewToken();
            }

            return RefreshToken();
        }

        private async Task RequestNewToken()
        {
            if (_gettingToken) return;

            _gettingToken = true;

            await _server.Start();


            var request = new LoginRequest(_server.BaseUri, _config.ClientId, LoginRequest.ResponseType.Code)
            {
                Scope = _config.AuthScopes
            };

            var uri = request.ToUri();
            try
            {
                BrowserUtil.Open(uri);
            }
            catch (Exception)
            {
                Trace.WriteLine($"Unable to open URL, manually open: {uri}");
            }
        }

        private async Task OnAuthorizationCodeReceived(object arg1, AuthorizationCodeResponse response)
        {
            var oauth = new OAuthClient();

            var tokenRequest = new TokenSwapTokenRequest(_config.ExchangeUrl, response.Code);
            var tokenResponse = await oauth.RequestToken(tokenRequest);
            _config.LastToken = new SpotifyAuthConfiguration.Token(tokenResponse.AccessToken, tokenResponse.ExpiresIn, tokenResponse.RefreshToken);

            _gettingToken = false;
            await _server.Stop();
            TokenUpdated?.Invoke(this, new TokenUpdatedEventArg(_config.LastToken));
        }
    }
}