using System;
using SpotifyAPI.Web.Models;

namespace Spotitoast.Spotify.Client.Event
{
    public class CurrentlyPlayedTrackChangedEvent : EventArgs
    {
        public FullTrack Track { get; }

        public CurrentlyPlayedTrackChangedEvent(FullTrack track)
        {
            Track = track;
        }
    }
}