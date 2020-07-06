using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class SkipTrack : Action.BaseAction
    {
        public SkipTrack(SpotifyClient client) : base(client)
        {
        }

        public override Task<ActionResult> Execute()
        {
            return _client.SkipTrack();
        }

        public override string Label { get; } = "Skip track";
        public override ActionFactory.PlayerAction Enum { get; } = ActionFactory.PlayerAction.Skip;
    }
}