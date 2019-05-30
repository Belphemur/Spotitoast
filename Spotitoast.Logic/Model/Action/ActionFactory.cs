using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Ninject;
using Ninject.Syntax;
using Spotitoast.Logic.Dependencies;

namespace Spotitoast.Logic.Model.Action
{
    public class ActionFactory : IActionFactory
    {
        private readonly IDictionary<Action, IAction> _values;

        public ActionFactory(IResolutionRoot resolutionRoot)
        {
            _values = resolutionRoot.GetAll<IAction>().ToDictionary(action => action.Enum, action => action);
        }

        public enum Action
        {
            Like,
            Dislike,
            Play,
            Pause
        }


        /// <summary>
        /// Get the action for the enum
        /// </summary>
        /// <param name="actionEnum"></param>
        /// <returns></returns>
        public IAction Get(Action actionEnum)
        {
            _values.TryGetValue(actionEnum, out var action);
            return action;
        }

        /// <summary>
        /// All the values
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IAction> Values()
        {
            return (IReadOnlyCollection<IAction>) _values.Values;
        }
    }
}