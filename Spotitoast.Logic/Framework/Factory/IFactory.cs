using System.Collections.Generic;

namespace Spotitoast.Logic.Framework.Factory
{
    public interface IFactory<TKey, out TImpl> where TImpl : IEnumImplementation<TKey>
    {
        
        /// <summary>
        /// All the available keys for the factory
        /// </summary>
        public TKey[] AvailableKeys { get; }
        
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