using System.Threading.Tasks;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class Exit : IAction
    {

        public Task<ActionResult> Execute()
        {
            return Task.FromResult(ActionResult.ExitApplication);
        }

        public string Label { get; } = "Quit the app";
        public ActionKey Key => ActionFactory.PlayerAction.Exit;
    }
}