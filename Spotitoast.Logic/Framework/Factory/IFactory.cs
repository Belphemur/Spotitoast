using System.Collections.Generic;

namespace Spotitoast.Logic.Framework.Factory
{
    public interface IFactory<in TKey, out TImpl> where TImpl : IEnumImplementation<TKey>
    {
        /// <summary>
        /// Get the Implementation from the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TImpl Get(TKey key);

        /// <summary>
        /// All the values
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<TImpl> Values();
    }
}