﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Syntax;

namespace Spotitoast.Logic.Framework.Factory
{
    public class BaseFactory<TKey, TImpl> : IFactory<TKey, TImpl> where TImpl : IEnumImplementation<TKey>
    {
        private readonly IDictionary<TKey, TImpl> _values;

        public BaseFactory(IResolutionRoot resolutionRoot)
        {
            _values = resolutionRoot.GetAll<TImpl>().ToDictionary(action => action.Enum, action => action);
        }

        /// <summary>
        /// All the available keys for the factory
        /// </summary>
        public TKey[] AvailableKeys { get; } = (TKey[]) Enum.GetValues(typeof(TKey));

        /// <summary>
        /// Get the action for the enum
        /// </summary>
        /// <param name="actionEnum"></param>
        /// <returns></returns>
        public TImpl Get(TKey actionEnum)
        {
            _values.TryGetValue(actionEnum, out var action);
            return action;
        }

        /// <summary>
        /// All the values
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<TImpl> Values()
        {
            return (IReadOnlyCollection<TImpl>) _values.Values;
        }
    }
}