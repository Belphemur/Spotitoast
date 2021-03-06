﻿using System;
using System.Threading.Tasks;
using Spotitoast.Logic.Model.Song;

namespace Spotitoast.Logic.Business.Player
{
    public interface ISpotifyNotifier
    {
        /// <summary>
        /// When a new track is played
        /// </summary>
        IObservable<Task<ITrack>> TrackPlayed { get; }
        /// <summary>
        /// When a track is successfully liked
        /// </summary>
        IObservable<Task<ITrack>> TrackLiked { get; }
        /// <summary>
        /// When a track is successfully disliked
        /// </summary>
        IObservable<Task<ITrack>> TrackDisliked { get; }
    }
}