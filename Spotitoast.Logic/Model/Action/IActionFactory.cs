using System.Collections.Generic;

namespace Spotitoast.Logic.Model.Action
{
    public interface IActionFactory
    {
        /// <summary>
        /// Get the action for the enum
        /// </summary>
        /// <param name="actionEnum"></param>
        /// <returns></returns>
        IAction Get(ActionFactory.Action actionEnum);

        /// <summary>
        /// All the values
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IAction> Values();
    }
}