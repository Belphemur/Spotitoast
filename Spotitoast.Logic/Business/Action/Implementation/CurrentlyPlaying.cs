﻿using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class CurrentlyPlaying : BaseAction
    {
        public CurrentlyPlaying(SpotifyClient client) : base(client)
        {
        }

        public override Task<ActionResult> Execute()
        {
            return _client.ForceCheckCurrentlyPlaying();
        }

        public override string Label { get; } = "Currently Playing Track";
        public override ActionKey Key => ActionFactory.PlayerAction.CurrentlyPlaying;
    }
}