using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A collection of extension methods for the <see cref="IReadOnlyDictionary{TKey,TValue}" />.
    /// </summary>
    [PublicAPI]
    public static class ReadOnlyDictionaries
    {
        /// <summary>
        /// Zips two dictionaries along the keys of the first dictionary.
        /// </summary>
        /// <param name="self">The first dictionary.</param>
        /// <param name="second">The second dictionary.</param>
        /// <typeparam name="TK">The key type of both dictionaries.</typeparam>
        /// <typeparam name="TV1">The value type of the first dictionary.</typeparam>
        /// <typeparam name="TV2">The value type of the second dictionary.</typeparam>
        /// <returns>An enumerable of tuples.</returns>
        public static IEnumerable<(TV1 Value1, TV2? Value2)> ZipAlongKeys<TK, TV1, TV2>(
            this IReadOnlyDictionary<TK, TV1> self,
            IReadOnlyDictionary<TK, TV2> second)
            where TV1 : class
            where TV2 : class
        {
            return self.Select(kvp => (kvp.Value, second.GetOrNull(kvp.Key)));
        }

        /// <summary>
        /// Zips two dictionaries along the <paramref name="keys" />.
        /// </summary>
        /// <param name="self">The first dictionary.</param>
        /// <param name="second">The second dictionary.</param>
        /// <param name="keys">The keys to enumerate along.</param>
        /// <typeparam name="TK">The key type of both dictionaries.</typeparam>
        /// <typeparam name="TV1">The value type of the first dictionary.</typeparam>
        /// <typeparam name="TV2">The value type of the second dictionary.</typeparam>
        /// <returns>An enumerable of tuples.</returns>
        public static IEnumerable<(TV1? Value1, TV2? Value2)> ZipAlongKeys<TK, TV1, TV2>(
            this IReadOnlyDictionary<TK, TV1> self,
            IReadOnlyDictionary<TK, TV2> second,
            IEnumerable<TK> keys)
            where TV1 : class
            where TV2 : class
        {
            return keys.Select(k => (self.GetOrNull(k), second.GetOrNull(k)));
        }

        /// <summary>
        /// Zips two dictionaries along the <paramref name="keys" />.
        /// </summary>
        /// <param name="self">The first dictionary.</param>
        /// <param name="second">The second dictionary.</param>
        /// <param name="keys">The keys to enumerate along.</param>
        /// <param name="selector">The selector to apply to the values.</param>
        /// <typeparam name="TK">The key type of both dictionaries.</typeparam>
        /// <typeparam name="TV1">The value type of the first dictionary.</typeparam>
        /// <typeparam name="TV2">The value type of the second dictionary.</typeparam>
        /// <typeparam name="TOut">The output type of the selector.</typeparam>
        /// <returns>An enumerable of tuples.</returns>
        public static IEnumerable<TOut> ZipAlongKeys<TK, TV1, TV2, TOut>(
            this IReadOnlyDictionary<TK, TV1> self,
            IReadOnlyDictionary<TK, TV2> second,
            IEnumerable<TK> keys,
            Func<TV1?, TV2?, TOut> selector)
            where TV1 : class
            where TV2 : class
        {
            return keys.Select(k => selector(self.GetOrNull(k), second.GetOrNull(k)));
        }
    }
}