using Ninject.Syntax;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Framework.Factory;

namespace Spotitoast.Logic.Business.Action
{
    public class ActionFactory : EquatableFactory<ActionKey, IAction>, IActionFactory
    {
       public enum PlayerAction
        {
            Like,
            Dislike,
            TogglePlayback,
            CurrentlyPlaying,
            Exit,
            Skip
        }

        public ActionFactory(IResolutionRoot resolutionRoot) : base(resolutionRoot)
        {
        }
    }
}