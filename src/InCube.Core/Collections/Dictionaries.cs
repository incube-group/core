using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InCube.Core.Functional;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A collection of convenience extension methods for dictionaries
    /// </summary>
    [PublicAPI]
    public static class Dictionaries
    {
        /// <summary>
        /// Tries to get a value out of a dictionary by its key, returns the provided default value if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="self">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <param name="default">The default value to return if the key doesn't exist</param>
        /// <returns>An object of the dictionary's value type</returns>
        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> self, TK key, TV @default) => self.TryGetValue(key, out var value) ? value : @default;

        /// <summary>
        /// Tries to get a value out of a dictionary by its key, evaluates the provider to get default value if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <param name="supplier">The function to evaluate to get the default value</param>
        /// <returns>An object of the dictionary's value type</returns>
        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key, Func<TV> supplier) => dict.TryGetValue(key, out var value) ? value : supplier();

        /// <summary>
        /// Tries to get a value out of a dictionary by its key, returns None option if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <returns>An option of the dictionary's value type</returns>
        public static Option<TV> GetOption<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) => dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;

        /// <summary>
        /// Tries to get a value out of a dictionary of reference types by its key, returns None maybe if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <returns>A maybe of the dictionary's value type</returns>
        public static Maybe<TV> GetMaybe<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) where TV : class => dict.TryGetValue(key, out var value) ? Maybe.Some(value) : Maybe.None;

        /// <summary>
        /// Tries to get a value out of a dictionary of value types by its key, returns null if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <returns>A nullable value type object</returns>
        public static TV? GetNullable<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) where TV : struct => dict.TryGetValue(key, out var value) ? value : default(TV?);

        /// <summary>
        /// Tries to get a value out of a dictionary of value types by its key, returns null if it fails
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <returns>A nullable value type object</returns>
        public static TV? GetOrNull<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) where TV : class => dict.TryGetValue(key, out var value) ? value : default;

        /// <summary>
        /// Checks whether or not a dictionary contains any element
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to check for emptiness</param>
        /// <returns>True if the dictionary is empty, false otherwise</returns>
        public static bool IsEmpty<TK, TV>(this IReadOnlyDictionary<TK, TV> dict) => dict.Count == 0;

        /// <summary>
        /// Tries to get a value ouf of a dictionary, throws if key doesn't exist
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="format">Format string around the key to look for</param>
        /// <param name="key">The key to look for</param>
        /// <returns>An object of the dictionary's value type</returns>
        [StringFormatMethod("format")]
        public static TV GetOrThrow<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, string format, TK key) => dict.GetOrDefault(key, () => throw new KeyNotFoundException(string.Format(format, key)));

        /// <summary>
        /// Convenience method to get or throw and print generic message mentioning missing key
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to look through</param>
        /// <param name="key">The key to look for</param>
        /// <returns>An object of the dictionary's value type</returns>
        public static TV GetOrThrow<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) => dict.GetOrThrow("missing key: {0}", key);

        /// <summary>
        /// Convenience method to create a dictionary from an enumerable of key-value pairs.
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="enumerable">The enumerable to source the key-value pairs from</param>
        /// <returns>A dictionary</returns>
        public static Dictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> enumerable) => enumerable.ToDictionary(kv => kv.Key, kv => kv.Value);

        /// <summary>
        /// Convenience method to create a dictionary from an enumerable of pairs (tuples)
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="enumerable">The enumerable to source the pairs from</param>
        /// <returns>A dictionary</returns>
        public static Dictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<(TK Key, TV Value)> enumerable) => enumerable.ToDictionary(kv => kv.Key, kv => kv.Value);

        /// <summary>
        /// Convenience method to create a dictionary from a grouping (result of Enumerable.GroupBy())
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="enumerable">The groups to create the dictionary from</param>
        /// <returns>A dictionary of groups (as <see cref="IEnumerable"/>)</returns>
        public static Dictionary<TK, IEnumerable<TV>> ToDictionary<TK, TV>(this IEnumerable<IGrouping<TK, TV>> enumerable) => enumerable.ToDictionary(kv => kv.Key, kv => kv as IEnumerable<TV>);

        /// <summary>
        /// Convenience method to cast various <see cref="IDictionary"/> implementations as an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary implementation to 'cast'</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/></returns>
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this IDictionary<TK, TV> dict) =>
            dict switch
            {
                Dictionary<TK, TV> d => d,
                ConcurrentDictionary<TK, TV> d => d,
                SortedDictionary<TK, TV> d => d,
                SortedList<TK, TV> d => d,
                _ => new ReadOnlyDictionary<TK, TV>(dict)
            };

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this IReadOnlyDictionary<TK, TV> dict) => dict;

        /// <summary>
        /// Convenience method to cast a <see cref="Dictionary{TKey,TValue}"/> to an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to 'cast'</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/></returns>
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this Dictionary<TK, TV> dict) => dict;

        /// <summary>
        /// Convenience method to cast a <see cref="ConcurrentDictionary{TKey,TValue}"/> to an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to 'cast'</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/></returns>
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this ConcurrentDictionary<TK, TV> dict) => dict;

        /// <summary>
        /// Convenience method to cast a <see cref="SortedDictionary{TKey,TValue}"/> to an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to 'cast'</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/></returns>
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this SortedDictionary<TK, TV> dict) => dict;

        /// <summary>
        /// Convenience method to cast a <see cref="SortedList{TKey,TValue}"/> to an <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to 'cast'</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/></returns>
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this SortedList<TK, TV> dict) => dict;

        /// <summary>
        /// Sorts the input dictionary
        /// </summary>
        /// <typeparam name="TK">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TV">Type of the value of the dictionary</typeparam>
        /// <param name="dict">The dictionary to sort</param>
        /// <param name="comparer">The comparer to use to sort</param>
        /// <returns>A sorted dictionary</returns>
        public static SortedDictionary<TK, TV> AsSorted<TK, TV>(this IDictionary<TK, TV> dict, IComparer<TK>? comparer = null)
        {
            comparer ??= Comparer<TK>.Default;
            return dict is SortedDictionary<TK, TV> sorted && sorted.Comparer == comparer ? sorted : new SortedDictionary<TK, TV>(dict.ToDictionary(), default);
        }

        /// <summary>
        /// Create an empty <see cref="IReadOnlyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">Type of the key of the dictionary</typeparam>
        /// <typeparam name="TValue">Type of the value of the dictionary</typeparam>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() => Dictionaries<TKey, TValue>.Empty;
    }

    public sealed class Dictionaries<TKey, TValue>
    {
        private class EmptyDictionary : IReadOnlyDictionary<TKey, TValue>
        {
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public int Count => 0;

            public bool ContainsKey(TKey key) => false;

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = default;
                return false;
            }

            public TValue this[TKey key] => throw new KeyNotFoundException();

            public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();

            public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();
        }

        public static readonly IReadOnlyDictionary<TKey, TValue> Empty = new EmptyDictionary();
    }
}