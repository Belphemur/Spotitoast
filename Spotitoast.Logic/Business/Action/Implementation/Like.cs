using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class Like(SpotifyClient client) : BaseAction(client)
    {
        public override async Task<ActionResult> Execute()
        {
            return await _client.LikePlayedTrack();
        }

        public override string Label => "Like Song";
        public override ActionKey Key => ActionFactory.PlayerAction.Like;
    }
}