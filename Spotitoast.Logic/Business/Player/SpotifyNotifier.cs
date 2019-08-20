﻿using System;
using System.Reactive.Linq;
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

        public SpotifyNotifier(SpotifyClient client)
        {
            TrackPlayed = client.PlayedTrack.Select(track => new TrackAdapter(track, client.IsLoved(track.Id).Result)).AsObservable();
            TrackLiked = client.TrackLiked.Select(track => new TrackAdapter(track, true)).AsObservable();
            TrackDisliked = client.TrackDisliked.Select(track => new TrackAdapter(track, false)).AsObservable();
        }
    }
}