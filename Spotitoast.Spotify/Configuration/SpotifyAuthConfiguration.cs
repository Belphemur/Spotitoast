using System;
using JetBrains.Annotations;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Spotitoast.Configuration;

namespace Spotitoast.Spotify.Configuration
{
    public class SpotifyAuthConfiguration : BaseConfiguration
    {

        private Token _lastToken = null;
        /// <summary>
        /// OAuth Token
        /// </summary>
        [CanBeNull]
        public Token LastToken {
            get => _lastToken;
            set
            {
                _lastToken = value;
                PropertyChanged();
            }
        }

        public string ExchangeUrl => "https://www.aaflalo.me/spotitoast/";
        public Scope Scopes => Scope.UserReadEmail | Scope.UserLibraryModify | Scope.UserLibraryRead | Scope.UserModifyPlaybackState | Scope.UserReadPlaybackState;
        public string InnerServerUrl => "http://localhost:4002";

        public override void Migrate()
        {
           
        }
    }
}