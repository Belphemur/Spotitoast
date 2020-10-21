using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class TogglePlayback : BaseAction
    {
        public TogglePlayback(SpotifyClient client) : base(client)
        {
        }

        public override async Task<ActionResult> Execute()
        {
            return _client.IsPlaying ? await _client.Pause() : await _client.Resume();
        }

        public override string Label => "Toggle Playback";
        public override ActionKey Key => ActionFactory.PlayerAction.TogglePlayback;
    }
}