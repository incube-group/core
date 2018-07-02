using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InCube.Core.Functional;

namespace InCube.Core.Collections
{
    public static class Dictionaries
    {
        public static SortedDictionary<T, V> AsSorted<T, V>(this Dictionary<T, V> dict, IComparer<T> comparer = null) =>
            new SortedDictionary<T, V>(dict, comparer);

        public static SortedDictionary<T, V> AsSorted<T, V>(this IReadOnlyDictionary<T, V> dict, IComparer<T> comparer = null)
        {
            return !(dict is SortedDictionary<T, V> sorted) || comparer != null
                ? new SortedDictionary<T, V>(dict.ToDictionary())
                : sorted;
        }

        public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key, V @default) =>
            dict.TryGetValue(key, out var value) ? value : @default;

        public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key, Func<V> supplier) =>
            dict.TryGetValue(key, out var value) ? value : supplier();

        public static Option<V> GetOption<K, V>(this IReadOnlyDictionary<K, V> dict, K key) =>
            dict.TryGetValue(key, out var value) ? Options.Some(value) : Options.None;

        public static Dictionary<T, V> ToDictionary<T, V>(this IEnumerable<KeyValuePair<T, V>> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<T, V> ToDictionary<T, V>(this IEnumerable<(T key, V value)> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.key, kv => kv.value);
        }

        public static IReadOnlyDictionary<T, V> AsReadOnly<T, V>(this IDictionary<T, V> dict)
        {
            switch (dict)
            {
                case Dictionary<T, V> d:
                    return d;
                case ConcurrentDictionary<T, V> d:
                    return d;
                case SortedDictionary<T, V> d:
                    return d;
                case SortedList<T, V> d:
                    return d;
                default:
                    return new ReadOnlyDictionary<T, V>(dict);
            }
        }

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyDictionary<T, V> AsReadOnly<T, V>(this IReadOnlyDictionary<T, V> dict) => dict;

    }
}
