using System.Collections.Generic;
using System.Linq;
using SpotifyAPI.Web.Models;

namespace Spotitoast.Logic.Model.Song.Adapter
{
    public class TrackAdapter : ITrack
    {
        public IReadOnlyCollection<string> Artists { get; }
        public string ArtistsDisplay => string.Join(", ", Artists);
        public string Name { get; }
        public IAlbum Album { get; }

        public TrackAdapter(FullTrack track)
        {
            Artists = track.Artists.Select(artist => artist.Name).ToList();
            Name = track.Name;
            Album = new AlbumAdapter(track.Album);
        }
    }
}