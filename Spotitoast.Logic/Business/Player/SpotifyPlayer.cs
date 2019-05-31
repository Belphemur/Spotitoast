using System;
using System.Reactive.Linq;
using Spotitoast.Logic.Model.Song;
using Spotitoast.Logic.Model.Song.Adapter;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Logic.Business.Player
{
    public class SpotifyPlayer : ISpotifyPlayer
    {
        public IObservable<ITrack> TrackPlayed { get; }
        public IObservable<ITrack> TrackLiked { get; }
        public IObservable<ITrack> TrackDisliked { get; }

        public SpotifyPlayer(SpotifyClient client)
        {
            TrackPlayed = client.PlayedTrack.Select(track => new TrackAdapter(track)).AsObservable();
            TrackLiked = client.TrackLiked.Select(track => new TrackAdapter(track)).AsObservable();
            TrackDisliked = client.TrackDisliked.Select(track => new TrackAdapter(track)).AsObservable();
        }
    }
}