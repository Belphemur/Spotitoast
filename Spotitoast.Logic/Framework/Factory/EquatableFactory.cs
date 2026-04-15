using System;
using System.Collections.Generic;
using System.Linq;

namespace Spotitoast.Logic.Framework.Factory
{
    public abstract class EquatableFactory<TKey, TImplementation> : IFactory<TKey, TImplementation>
        where TImplementation : IEquatableImplementation<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, TImplementation> _dictionary;

        protected EquatableFactory(IEnumerable<TImplementation> implementations)
        {
            _dictionary = implementations.ToDictionary(action => action.Key);
        }

        public IReadOnlyCollection<TKey> AvailableKeys => _dictionary.Keys;

        /// <inheritdoc />
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException"></exception>
        public TImplementation Get(TKey key) => _dictionary[key];

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public IReadOnlyCollection<TImplementation> Values() => _dictionary.Values;
    }
}