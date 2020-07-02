using System;
using System.Threading.Tasks;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public class Exit : IAction
    {
        public ActionFactory.PlayerAction Enum { get; } = ActionFactory.PlayerAction.Exit;

        public Task<ActionResult> Execute()
        {
            Environment.Exit(0);
            return Task.FromResult(ActionResult.Success);
        }

        public string Label { get; } = "Quit the app";
    }
}