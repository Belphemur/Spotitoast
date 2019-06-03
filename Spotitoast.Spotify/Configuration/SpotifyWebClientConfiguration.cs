using Spotitoast.Configuration;

namespace Spotitoast.Spotify.Configuration
{
    public class SpotifyWebClientConfiguration : BaseConfiguration
    {

        private int _checkCurrentlyPlayedSeconds = 15;
        /// <summary>
        /// Check for a new Currently played track in seconds.
        /// </summary>
        public int CheckCurrentlyPlayedSeconds
        {
            get => _checkCurrentlyPlayedSeconds;
            set
            {
                _checkCurrentlyPlayedSeconds = value;
                PropertyChanged();
            }
        }

        public override void Migrate()
        {
        }
    }
}