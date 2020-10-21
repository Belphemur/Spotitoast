using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Framework.Factory;

namespace Spotitoast.Logic.Business.Action
{
    public interface IActionFactory : IFactory<ActionKey, IAction>
    {
       
    }
}