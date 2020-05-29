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
    /// A variety of useful extension methods for dictionaries.
    /// </summary>
    public static class Dictionaries
    {
        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key, TV @default) => dict.TryGetValue(key, out var value) ? value : @default;

        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key, Func<TV> supplier) => dict.TryGetValue(key, out var value) ? value : supplier();

        public static Option<TV> GetOption<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) => dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;

        public static Maybe<TV> GetMaybe<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) where TV : class => dict.TryGetValue(key, out var value) ? Maybe.Some(value) : Maybe.None;

        public static TV? GetNullable<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) where TV : struct => dict.TryGetValue(key, out var value) ? value : default(TV?);

        public static bool IsEmpty<TK, TV>(this IReadOnlyDictionary<TK, TV> dict) => dict.Count == 0;

        [StringFormatMethod("format")]
        public static TV GetOrThrow<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, string format, TK key) => dict.GetOrDefault(key, () => throw new KeyNotFoundException(string.Format(format, key)));

        public static TV GetOrThrow<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key) => dict.GetOrThrow("missing key: {0}", key);

        public static Dictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<(TK key, TV value)> enumerable) => enumerable.ToDictionary(kv => kv.key, kv => kv.value);

        public static Dictionary<TK, IEnumerable<TV>> ToDictionary<TK, TV>(this IEnumerable<IGrouping<TK, TV>> enumerable) => enumerable.ToDictionary(kv => kv.Key, kv => kv as IEnumerable<TV>);

        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this IDictionary<TK, TV> dict)
        {
            switch (dict)
            {
                case Dictionary<TK, TV> d:
                    return d;
                case ConcurrentDictionary<TK, TV> d:
                    return d;
                case SortedDictionary<TK, TV> d:
                    return d;
                case SortedList<TK, TV> d:
                    return d;
                default:
                    return new ReadOnlyDictionary<TK, TV>(dict);
            }
        }

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this IReadOnlyDictionary<TK, TV> dict) => dict;

        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this Dictionary<TK, TV> dict) => dict;

        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this ConcurrentDictionary<TK, TV> dict) => dict;

        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this SortedDictionary<TK, TV> dict) => dict;

        public static IReadOnlyDictionary<TK, TV> AsReadOnlyDictionary<TK, TV>(this SortedList<TK, TV> dict) => dict;

        public static SortedDictionary<TK, TV> AsSorted<TK, TV>(this IDictionary<TK, TV> dict, IComparer<TK> comparer = null)
        {
            comparer = comparer ?? Comparer<TK>.Default;
            return dict is SortedDictionary<TK, TV> sorted && sorted.Comparer == comparer ? sorted : new SortedDictionary<TK, TV>(dict.ToDictionary(), default);
        }

        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() => Dictionaries<TKey, TValue>.Empty;
    }

#pragma warning disable SA1402 // File may only contain a single type
    public sealed class Dictionaries<TKey, TValue>
#pragma warning restore SA1402 // File may only contain a single type
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