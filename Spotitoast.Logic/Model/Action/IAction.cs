using System.Threading.Tasks;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Model.Action
{
    public interface IAction
    {
        /// <summary>
        /// Execute the action
        /// </summary>
        /// <returns></returns>
        Task<ActionResult> Execute();

        string Label { get; }

        ActionFactory.Action Enum { get; }
    }
}