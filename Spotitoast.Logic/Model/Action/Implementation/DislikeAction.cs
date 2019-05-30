using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Model.Action.Implementation
{
    public class DislikeAction : BaseAction
    {
        public DislikeAction(SpotifyClient client) : base(client)
        {
        }

        public override async Task<ActionResult> Execute()
        {
            return await _client.DislikePlayedTrack();
        }

        public override string Label => "Dislike Song";
        public override ActionFactory.Action Enum => ActionFactory.Action.Dislike;
    }
}