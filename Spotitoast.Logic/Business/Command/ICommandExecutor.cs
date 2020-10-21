using System.Collections.Generic;
using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Command
{
    public interface ICommandExecutor
    {
        /// <summary>
        /// Parse the command
        /// </summary>
        ActionKey? ParseCommand(string cmd);

        /// <summary>
        /// Execute the command
        /// </summary>
        Task<ActionResult> Execute(ActionKey action);

        /// <summary>
        /// What are the available commands
        /// </summary>
        IReadOnlyCollection<ActionKey> AvailableCommands { get; }
    }
}