using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Ninject;
using Ninject.Syntax;
using Spotitoast.Logic.Dependencies;
using Spotitoast.Logic.Framework;

namespace Spotitoast.Logic.Business.Action
{
    public class ActionFactory : BaseFactory<ActionFactory.PlayerAction, IAction>, IActionFactory
    {
       public enum PlayerAction
        {
            Like,
            Dislike,
            TogglePlayback
        }

        public ActionFactory(IResolutionRoot resolutionRoot) : base(resolutionRoot)
        {
        }
    }
}