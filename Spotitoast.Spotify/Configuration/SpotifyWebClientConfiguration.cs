using Spotitoast.Configuration;

namespace Spotitoast.Spotify.Configuration
{
    public class SpotifyWebClientConfiguration : IConfiguration
    {
        public string FileLocation { get; set; }

        /// <summary>
        /// Check for a new Currently played track in seconds.
        /// </summary>
        public int CheckCurrentlyPlayedSeconds { get; set; } = 15;
        public void Migrate()
        {
        }
    }
}