using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Spotitoast.Logic.Framework.Extensions;
using Spotitoast.Logic.Model.Song;
using Spotitoast.Logic.Model.Song.Adapter;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Logic.Business.Player
{
    public class SpotifyNotifier : ISpotifyNotifier
    {
        public IObservable<ITrack> TrackPlayed { get; }
        public IObservable<ITrack> TrackLiked { get; }
        public IObservable<ITrack> TrackDisliked { get; }

        public SpotifyNotifier(SpotifyClient client, ImageDownloader imageDownloader)
        {
            TrackPlayed = client.PlayedTrack.Select(track => Observable.FromAsync(async _ => (ITrack) new TrackAdapter(track, await client.IsLoved(track.Id), imageDownloader))).Concat();
            TrackLiked = client.TrackLiked.Select(track => new TrackAdapter(track, true, imageDownloader)).AsObservable();
            TrackDisliked = client.TrackDisliked.Select(track => new TrackAdapter(track, false, imageDownloader)).AsObservable();
        }
    }
}