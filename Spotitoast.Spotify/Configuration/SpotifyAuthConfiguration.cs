using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using Spotitoast.Configuration;

namespace Spotitoast.Spotify.Configuration
{
    public class SpotifyAuthConfiguration : BaseConfiguration
    {
        [method: JsonConstructor]
        public record Token(string AccessToken, DateTime ExpirationDate, string RefreshToken)
        {
            public Token(string accessToken, int expireInSeconds, string refreshToken) : this(accessToken, DateTime.UtcNow + TimeSpan.FromSeconds(expireInSeconds), refreshToken)
            {
            }


            [JsonIgnore]
            public TimeSpan Expire => ExpirationDate - DateTime.UtcNow;

            /// <summary>
            /// Is the token expired
            /// </summary>
            public bool IsExpired() => DateTime.UtcNow > ExpirationDate;
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

        public Uri ListenUri => new($"http://127.0.0.1:{ListenPort}/callback");
        public int ListenPort => 4002;

        public override void Migrate()
        {
        }
    }
}