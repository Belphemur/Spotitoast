using System;

namespace Spotitoast.Logic.Framework.Factory
{
    /// <summary>
    /// Represent the implementation of the Key
    /// </summary>
    /// <typeparam name="TKey">Equatable type</typeparam>
    public interface IEquatableImplementation<out TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Key of the implementation
        /// </summary>
        public TKey Key { get; }
    }
}