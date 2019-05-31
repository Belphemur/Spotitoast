using System.Collections.Generic;
using Spotitoast.Logic.Framework;
using Spotitoast.Logic.Framework.Factory;

namespace Spotitoast.Logic.Business.Action
{
    public interface IActionFactory : IFactory<ActionFactory.PlayerAction, IAction>
    {
       
    }
}