using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class ResumeAction : BaseAction
    {
        public ResumeAction(SpotifyClient client) : base(client)
        {
        }

        public override async Task<ActionResult> Execute()
        {
            return await _client.Resume();
        }

        public override string Label => "Resume Playback";
        public override ActionFactory.PlayerAction Enum => ActionFactory.PlayerAction.Play;
    }
}