using System.Threading.Tasks;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action
{
    public abstract class BaseAction : IAction
    {
        protected readonly SpotifyClient _client;

        protected BaseAction(SpotifyClient client)
        {
            _client = client;
        }

        public abstract Task<ActionResult> Execute();
        public abstract string Label { get; }
        public abstract ActionFactory.PlayerAction Enum { get; }
    }
}