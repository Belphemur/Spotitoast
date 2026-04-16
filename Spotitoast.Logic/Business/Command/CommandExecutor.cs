using System.Collections.Generic;
using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Command
{
    public class CommandExecutor(IActionFactory actionFactory) : ICommandExecutor
    {
        /// <summary>
        /// What are the available commands
        /// </summary>
        public IReadOnlyCollection<ActionKey> AvailableCommands => actionFactory.AvailableKeys;

        /// <summary>
        /// Parse the command
        /// </summary>
        public ActionKey? ParseCommand(string cmd)
        {
            ActionKey actionKey = cmd;
            return actionFactory.ContainsKey(actionKey) ? actionKey : (ActionKey?) null;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public Task<ActionResult> Execute(ActionKey action) => actionFactory.Get(action).Execute();
    }
}