using System.Threading.Tasks;
using Spotitoast.Logic.Framework;
using Spotitoast.Logic.Framework.Factory;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action
{
    public interface IAction : IEnumImplementation<ActionFactory.PlayerAction>
    {
        /// <summary>
        /// Execute the action
        /// </summary>
        /// <returns></returns>
        Task<ActionResult> Execute();

        string Label { get; }
    }
}