using System;
using System.Threading.Tasks;
using Spotitoast.Logic.Model.Song;

namespace Spotitoast.Logic.Business.Player
{
    public interface ISpotifyNotifier
    {
        /// <summary>
        /// When a new track is played
        /// </summary>
        IObservable<ITrack> TrackPlayed { get; }
        /// <summary>
        /// When a track is successfully liked
        /// </summary>
        IObservable<ITrack> TrackLiked { get; }
        /// <summary>
        /// When a track is successfully disliked
        /// </summary>
        IObservable<ITrack> TrackDisliked { get; }
    }
}