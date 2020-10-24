using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Framework.Factory;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Business.Action
{
    public interface IAction : IEquatableImplementation<ActionKey>
    {
        /// <summary>
        /// Execute the action
        /// </summary>
        /// <returns></returns>
        Task<ActionResult> Execute();

        string Label { get; }
    }
}