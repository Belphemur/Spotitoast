using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Linux.Command
{
    public interface ICommandExecutor
    {
        /// <summary>
        /// Parse the command
        /// </summary>
        ActionFactory.PlayerAction? ParseCommand(string cmd);

        /// <summary>
        /// Execute the command
        /// </summary>
        Task<ActionResult> Execute(ActionFactory.PlayerAction action);

        /// <summary>
        /// What are the available commands
        /// </summary>
        ActionFactory.PlayerAction[] AvailableCommands { get; }
    }
}