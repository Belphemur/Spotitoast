using System.Collections.Generic;
using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Command
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IActionFactory _actionFactory;

        public CommandExecutor(IActionFactory actionFactory)
        {
            _actionFactory = actionFactory;
        }

        /// <summary>
        /// What are the available commands
        /// </summary>
        public IReadOnlyCollection<ActionKey> AvailableCommands => _actionFactory.AvailableKeys;

        /// <summary>
        /// Parse the command
        /// </summary>
        public ActionKey? ParseCommand(string cmd)
        {
            ActionKey actionKey = cmd;
            return _actionFactory.ContainsKey(actionKey) ? (ActionKey?) null : actionKey;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public Task<ActionResult> Execute(ActionKey action) => _actionFactory.Get(action).Execute();
    }
}