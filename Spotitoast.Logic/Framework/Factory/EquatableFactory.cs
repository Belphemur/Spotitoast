using System;
using System.Collections.Generic;
using System.Linq;

namespace Spotitoast.Logic.Framework.Factory
{
    public abstract class EquatableFactory<TImplementation, TKey> : IFactory<TKey, TImplementation>
        where TImplementation : IEquatableImplementation<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, TImplementation> _dictionary;

        protected EquatableFactory(IEnumerable<TImplementation> enumerableOfImplementations)
        {
            _dictionary = enumerableOfImplementations.ToDictionary(impl => impl.Key);
        }

        public IReadOnlyCollection<TKey> AvailableKeys => _dictionary.Keys;

        /// <inheritdoc />
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException"></exception>
        public TImplementation Get(TKey key) => _dictionary[key];

        public IReadOnlyCollection<TImplementation> Values() => _dictionary.Values;
    }
}