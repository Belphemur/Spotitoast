using System.Collections.Generic;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Framework.Factory;

namespace Spotitoast.Logic.Business.Action
{
    public class ActionFactory(IEnumerable<IAction> actions)
        : EquatableFactory<ActionKey, IAction>(actions), IActionFactory
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
    }
}