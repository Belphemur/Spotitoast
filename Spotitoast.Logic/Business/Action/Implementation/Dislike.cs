using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class Dislike : BaseAction
    {
        public Dislike(SpotifyClient client) : base(client)
        {
        }

        public override async Task<ActionResult> Execute()
        {
            return await _client.DislikePlayedTrack();
        }

        public override string Label => "Dislike Song";
        public override ActionFactory.PlayerAction Enum => ActionFactory.PlayerAction.Dislike;
    }
}