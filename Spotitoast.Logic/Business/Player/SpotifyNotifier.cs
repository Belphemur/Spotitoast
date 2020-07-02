using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Spotitoast.Logic.Model.Song;
using Spotitoast.Logic.Model.Song.Adapter;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Logic.Business.Player
{
    public class SpotifyNotifier : ISpotifyNotifier
    {
        public IObservable<Task<ITrack>> TrackPlayed { get; }
        public IObservable<Task<ITrack>> TrackLiked { get; }
        public IObservable<Task<ITrack>> TrackDisliked { get; }

        public SpotifyNotifier(SpotifyClient client)
        {
            TrackPlayed = client.PlayedTrack.Select(async track => (ITrack) new TrackAdapter(track, await client.IsLoved(track.Id))).AsObservable();
            TrackLiked = client.TrackLiked.Select(track => Task.FromResult<ITrack>(new TrackAdapter(track, true))).AsObservable();
            TrackDisliked = client.TrackDisliked.Select(track => Task.FromResult<ITrack>(new TrackAdapter(track, false))).AsObservable();
        }
    }
}