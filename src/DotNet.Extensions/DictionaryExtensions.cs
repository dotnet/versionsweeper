// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DotNet.Extensions
{
    /// <summary>
    /// Inspired by: https://stackoverflow.com/a/66193734/2410379
    /// </summary>
    public static class DictionaryExtensions
    {
        class ReadOnlyDictionaryWrapper<TKey, TValue>
            : IReadOnlyDictionary<TKey, TValue>
            where TKey : notnull
        {
            private readonly IDictionary<TKey, TValue> _dictionary;

            public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dictionary) =>
                _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

            public IEnumerable<TKey> Keys => _dictionary.Keys;

            public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                var result = _dictionary.TryGetValue(key, out var tempValue);
                value = tempValue!;
                return result;
            }

            public IEnumerable<TValue> Values => _dictionary.Values.Cast<TValue>();

            public TValue this[TKey key] => _dictionary[key];

            public int Count => _dictionary.Count;

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
                _dictionary.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Creates a wrapper on a dictionary that adapts the type of the values.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key.</typeparam>
        /// <typeparam name="TValue">The dictionary value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <returns>A dictionary where values are a base type of this dictionary.</returns>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            this IDictionary<TKey, TValue> source)
            where TKey : notnull =>
            new ReadOnlyDictionaryWrapper<TKey, TValue>(source);
    }
}
