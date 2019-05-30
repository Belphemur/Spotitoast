using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Model.Action.Implementation
{
    public class PauseAction : BaseAction
    {
        public PauseAction(SpotifyClient client) : base(client)
        {
        }

        public override async Task<ActionResult> Execute()
        {
            return await _client.Pause();
        }

        public override string Label => "Pause Playback";
        public override ActionFactory.Action Enum => ActionFactory.Action.Pause;
    }
}