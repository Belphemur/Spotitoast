using System;
using Spotitoast.Logic.Model.Song;

namespace Spotitoast.Logic.Business.Player
{
    public interface ISpotifyPlayer
    {
        IObservable<ITrack> TrackPlayed { get; }
        IObservable<ITrack> TrackLiked { get; }
        IObservable<ITrack> TrackDisliked { get; }
    }
}