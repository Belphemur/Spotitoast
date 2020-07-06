using System;
using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action;
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
        public ActionFactory.PlayerAction[] AvailableCommands => _actionFactory.AvailableKeys;

        /// <summary>
        /// Parse the command
        /// </summary>
        public ActionFactory.PlayerAction? ParseCommand(string cmd)
        {
            if (!Enum.TryParse<ActionFactory.PlayerAction>(cmd, true, out var @enum))
            {
                return null;
            }

            return @enum!;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public Task<ActionResult> Execute(ActionFactory.PlayerAction action) => _actionFactory.Get(action).Execute();
    }
}