using System;
using JetBrains.Annotations;
using SpotifyAPI.Web;
using Spotitoast.Configuration;

namespace Spotitoast.Spotify.Configuration
{
    public class SpotifyAuthConfiguration : BaseConfiguration
    {
        public record Token
        {
            public Token(string accessToken, int expireInSeconds, string refreshToken) => (AccessToken, Expire, RefreshToken) = (accessToken, TimeSpan.FromSeconds(expireInSeconds), refreshToken);

            public string AccessToken { get; init; }
            public TimeSpan Expire { get; init; }
            public string RefreshToken { get; init; }
        }

        private Token _lastToken = null;

        /// <summary>
        /// OAuth Token
        /// </summary>
        [CanBeNull]
        public Token LastToken
        {
            get => _lastToken;
            set
            {
                _lastToken = value;
                PropertyChanged();
            }
        }

        public Uri ExchangeUrl => new("https://www.aaflalo.me/spotitoast/");
        public string[] AuthScopes => new[] {Scopes.UserReadEmail, Scopes.UserLibraryModify, Scopes.UserLibraryRead, Scopes.UserModifyPlaybackState, Scopes.UserReadPlaybackState};
        public string ClientId => "fc945fa1296945d09628cff8e2159941";

        public Uri ListenUri => new($"http://localhost:{ListenPort}/callback");
        public int ListenPort => 4002;

        public override void Migrate()
        {
        }
    }
}