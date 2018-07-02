using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using InCube.Core.Functional;
using static InCube.Core.Preconditions;

namespace InCube.Core.Collections
{

    public static class Collections
    {
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, U state, Func<U, T, U> next)
        {
            yield return state;
            foreach (var item in input)
            {
                state = next(state, item);
                yield return state;
            }
        }

        /// <returns>the last element of a list</returns>
        public static T Last<T>(this IReadOnlyList<T> list) => list[list.Count - 1];

        public static IComparer<T> DefaultComparer<T>() where T : IComparable<T> =>
            Comparer<T>.Create((x, y) => x.CompareTo(y));

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="List{T}.BinarySearch(T)"/>
        /// <seealso cref="Digitize{T}(T[],T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int Digitize<T>(this List<T> input, T item) where T : IComparable<T>
        {
            var idx = input.BinarySearch(item, DefaultComparer<T>());
            return IdxToBin(input, item, idx);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)"/>
        /// <seealso cref="Digitize{T}(System.Collections.Generic.List{T},T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int Digitize<T>(this T[] input, T item) where T : IComparable<T>
        {
            var idx = Array.BinarySearch(input, item, DefaultComparer<T>());
            return IdxToBin(input, item, idx);
        }

        private static int IdxToBin<T>(IReadOnlyList<T> input, T item, int idx) where T : IComparable<T>
        {
            if (idx >= 0)
            {
                var count = input.Count;
                while (idx < count && input[idx].CompareTo(item) <= 0)
                {
                    ++idx;
                }
            }
            return idx > 0 ? idx : ~idx;
        }


        public static int Rank<T>(this IEnumerable<T> values, T element) where T : IComparable<T> => 
            values.Aggregate(0, (lesserCount, value) => value.CompareTo(element) <= 0 ? lesserCount + 1 : lesserCount);

        public static int[] VectorRank<T>(this IEnumerable<T> values, IReadOnlyList<T> elements)
            where T : IComparable<T>
        {
            var elementCount = elements.Count;
            var result = new int[elementCount];
            foreach (var value in values)
            {
                for (var i = 0; i < elementCount; ++i)
                {
                    if (value.CompareTo(elements[i]) <= 0)
                    {
                        ++result[i];
                    }
                }
            }

            return result;
        }

        public static double RelativeRank<T>(this IReadOnlyCollection<T> values, T element) where T : IComparable<T> =>
            values.Rank(element) / (double) values.Count;

        public static double[] VectorRelativeRank<T>(this IReadOnlyCollection<T> values, IReadOnlyList<T> elements) where T : IComparable<T> =>
            values.VectorRank(elements).Select(x => x / (double)values.Count).ToArray();

        public static IEnumerable<(T value, int index)> ZipWithIndex<T>(this IEnumerable<T> enumerable) =>
            enumerable.Select(MakeValueTuple);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", ").
        /// </summary>
        public static String MkString<T>(this IEnumerable<T> enumerable, string separator = ", ") => 
            String.Join(separator, enumerable);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", "), wrapped by with a string in the beginning and the end.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="start">The string to place in front of the joined output.</param>
        /// <param name="separator">The separator to be placed between elements.</param>
        /// <param name="end">The string to place at the end of the joined output.</param>
        public static String MkString<T>(this IEnumerable<T> enumerable, string start, string separator, string end) =>
            $"{start}{enumerable.MkString(separator)}{end}";

        public static IEnumerable<KeyValuePair<T, V>> MapValues<T, U, V>(
            this IEnumerable<KeyValuePair<T, U>> enumerable, Func<U, V> mapper) =>
            enumerable.Select(keyValue => MakePair(keyValue.Key, mapper(keyValue.Value)));

        public static IEnumerable<(T Key, V Value)> MapValues<T, U, V>(this IEnumerable<(T Key, U Value)> enumerable,
            Func<U, V> mapper) =>
            enumerable.Select(kv => (kv.Key, mapper(kv.Value)));

        public static Dictionary<T, V> ToDictionary<T, V>(this IEnumerable<KeyValuePair<T, V>> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<T, V> ToDictionary<T, V>(this IEnumerable<(T key, V value)> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.key, kv => kv.value);
        }

        public static IReadOnlyDictionary<T, V> ToReadOnlyDictionary<T, V>(this IEnumerable<(T key, V value)> enumerable)
        {
            return enumerable.ToDictionary(kv => kv.key, kv => kv.value);
        }

        public static IEnumerable<(T, V)> AsTuple<T, V>(this IEnumerable<KeyValuePair<T, V>> enumerable)
        {
            return enumerable.Select(kv => (kv.Key, kv.Value));
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

        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> col)
        {
            switch (col)
            {
                case List<T> l:
                    return l;
                case T[] a:
                    return a;
                default:
                    return new ReadOnlyCollection<T>(col);
            }
        }

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyCollection<T> AsReadOnly<T>(this IReadOnlyCollection<T> col) => col;

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            switch (list)
            {
                case List<T> l:
                    return l;
                case T[] a:
                    return a;
                default:
                    return new ReadOnlyList<T>(list);
            }
        }

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyList<T> AsReadOnly<T>(this IReadOnlyList<T> list) => list;

        public static SortedDictionary<T, V> AsSorted<T, V>(this Dictionary<T, V> dict, IComparer<T> comparer = null) => 
            new SortedDictionary<T, V>(dict, comparer);

        public static SortedDictionary<T, V> AsSorted<T, V>(this IReadOnlyDictionary<T, V> dict, IComparer<T> comparer = null)
        {
            return dict is SortedDictionary<T, V> sorted && comparer == null
                ? sorted
                : new SortedDictionary<T, V>(dict.ToDictionary());
        }

        public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key, V @default) => 
            dict.TryGetValue(key, out var value) ? value : @default;

        public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key, Func<V> supplier) =>
            dict.TryGetValue(key, out var value) ? value : supplier();

        public static Option<V> GetOption<K, V>(this IReadOnlyDictionary<K, V> dict, K key) =>
            dict.TryGetValue(key, out var value) ? Options.Some(value) : Options.None;

        public static KeyValuePair<T, V> MakePair<T, V>(T key, V value) => 
            new KeyValuePair<T, V>(key, value);

        public static (T Key, V Value) Unpack<T, V>(this KeyValuePair<T, V> keyValue) => (keyValue.Key, keyValue.Value);

        public static Tuple<T1, T2> MakeTuple<T1, T2>(T1 item1, T2 item2) 
            => new Tuple<T1, T2>(item1, item2);

        public static (T1 Item1, T2 Item2) Unpack<T1, T2>(this Tuple<T1, T2> tuple) => (tuple.Item1, tuple.Item2);


        public static (T1, T2) MakeValueTuple<T1, T2>(T1 item1, T2 item2) 
            => (item1, item2);

        public static IEnumerable<(T1, T2)> ZipAsTuple<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
        {
            return left.Zip(right, MakeValueTuple);
        }

        public static IEnumerable<(T1, T2, T3)> ZipAsTuple<T1, T2, T3>(this IEnumerable<T1> e1, IEnumerable<T2> e2,
            IEnumerable<T3> e3)
        {
            return e1.ZipAsTuple(e2).Zip(e3, (x, y) => (x.Item1, x.Item2, y));
        }

        public static IEnumerable<(T1, T2, T3, T4)> ZipAsTuple<T1, T2, T3, T4>(this IEnumerable<T1> e1, 
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4)
        {
            return e1.ZipAsTuple(e2, e3).Zip(e4, (x, y) => (x.Item1, x.Item2, x.Item3, y));
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> ZipAsTuple<T1, T2, T3, T4, T5>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4,
            IEnumerable<T5> e5)
        {
            return e1.ZipAsTuple(e2, e3, e4).Zip(e5, (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));
        }

        public static System.Collections.Generic.HashSet<T> ToHashSet<T>(this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new System.Collections.Generic.HashSet<T>(source, comparer);
        }

        public static U[] ParallelMap<T, U>(this IReadOnlyList<T> list, Func<T, U> map) =>
            list.ParallelMap(map, list.Count);

        public static U[] ParallelMap<T, U>(this IReadOnlyList<T> list, Func<T, U> map, int count)
        {
            var result = new U[count];
            Parallel.For(0, count, i => result[i] = map(list[i]));
            return result;
        }

        public static U[] ParallelMapI<T, U>(this IReadOnlyList<T> list, Func<T, int, U> map) =>
            list.ParallelMapI(map, list.Count);

        public static U[] ParallelMapI<T, U>(this IReadOnlyList<T> list, Func<T, int, U> map, int count)
        {
            var result = new U[count];
            Parallel.For(0, count, i => result[i] = map(list[i], i));
            return result;
        }

        public static T[] ParallelGenerate<T>(int count, Func<int, T> generator)
        {
            var result = new T[count];
            Parallel.For(0, count, i => result[i] = generator(i));
            return result;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) =>
            source.MaxBy(selector, Comparer<TKey>.Default);

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    throw new InvalidOperationException();

                var max = iterator.Current;
                var maxValue = selector(max);

                while (iterator.MoveNext())
                {
                    var current = iterator.Current;
                    var currentValue = selector(current);

                    if (comparer.Compare(currentValue, maxValue) > 0)
                    {
                        max = current;
                        maxValue = currentValue;
                    }
                }

                return max;
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, 
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        public sealed class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> _inner;

            public static ReverseComparer<T> Default => new ReverseComparer<T>();

            public ReverseComparer() : this(null)
            {
            }

            public ReverseComparer(IComparer<T> inner)
            {
                this._inner = inner ?? Comparer<T>.Default;
            }

            int IComparer<T>.Compare(T x, T y)
            {
                return _inner.Compare(y, x);
            }
        }

        public static ReverseComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            return new ReverseComparer<T>(comparer);
        }

        public static IEnumerable<T> Get<T>(this IReadOnlyList<T> list, IEnumerable<int> indices) =>
            indices.Select(i => list[i]);

        public static IEnumerable<IEnumerable<T>> GetCols<T>(this IEnumerable<IReadOnlyList<T>> enumerable, IEnumerable<int> indices) =>
            enumerable.Select(list => indices.Select(i => list[i]));

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(list => list);

        public static bool IsEmpty<T>(this IEnumerable<T> col) => !col.Any();

        public static IEnumerable<T> GenFilter<T, U>(this IEnumerable<T> list, Func<U, bool> predicate) where T : U
        {
            foreach (var l in list)
            {
                if (predicate.Invoke(l)) yield return l;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var l in list)
            {
                action.Invoke(l);
            }
        }

        public delegate IEnumerator<T> EnumeratorGenerator<out T>();

        private class WrappedEnumerable<T>: IEnumerable<T>
        {
            private readonly EnumeratorGenerator<T> _generator;

            public WrappedEnumerable(EnumeratorGenerator<T> generator)
            {
                this._generator = generator;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _generator.Invoke();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IEnumerable<T> MakeEnumerable<T>(EnumeratorGenerator<T> generator)
        {
            return new WrappedEnumerable<T>(generator);
        }

        public static ArraySegment<T> Slice<T>(this T[] elems, int startInclusive, int stopExclusive) => 
            new ArraySegment<T>(elems, startInclusive, stopExclusive - startInclusive);

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> elems, int startInclusive, int stopExclusive) => 
            new ArraySegment<T>(elems.Array, elems.Offset + startInclusive, stopExclusive - startInclusive);

        public static Option<T> FirstOption<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                return enumerator.MoveNext() ? Options.Some(enumerator.Current) : Options.None;
            }
        }

        /// <summary>
        /// An adaptation of <see cref="List{T}.RemoveAll"/> by shifting all elements matching the 
        /// predicate <paramref name="match"/> to the back of this list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elems"></param>
        /// <param name="match"></param>
        /// <param name="startIdx"></param>
        /// <param name="stopIdx"></param>
        /// <returns>The index of the first element in <paramref name="elems"/> that matches the predicate.</returns>
        public static int RemoveSeq<T>(this IList<T> elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0) stopIdx = elems.Count;
            
            CheckNotNull(match, nameof(match));

            var freeIndex = startIdx;   // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < stopIdx && !match(elems[freeIndex])) freeIndex++;
            if (freeIndex >= stopIdx) return stopIdx;

            var current = freeIndex + 1;
            while (current < stopIdx)
            {
                // Find the first item which needs to be kept.
                while (current < stopIdx && match(elems[current])) current++;

                if (current < stopIdx)
                {
                    // copy item to the free slot.
                    elems[freeIndex++] = elems[current++];
                }
            }

            return freeIndex;
        }

        /// <summary>
        /// A parallel version of <see cref="RemoveSeq{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elems"></param>
        /// <param name="match"></param>
        /// <param name="startIdx"></param>
        /// <param name="stopIdx"></param>
        /// <returns></returns>
        public static int RemovePar<T>(this T[] elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0) stopIdx = elems.Length;

            CheckNotNull(match, nameof(match));

            var partitioner = Partitioner.Create(startIdx, stopIdx);
            var sections = new ConcurrentDictionary<int, int>();
            Parallel.ForEach(partitioner, range =>
            {
                var (start, stop) = range;
                sections[start] = elems.RemoveSeq(match, start, stop);
            });

            var freeIndex = 0;
            foreach (var knv in sections.OrderBy(pair => pair.Key))
            {
                var (start, stop) = (knv.Key, knv.Value);
                var length = stop - start;
                if (length == 0) continue;
                if (start > freeIndex)
                {
                    Array.Copy(elems, start, elems, freeIndex, length);
                }

                freeIndex += length;
            }

            return freeIndex;
        }


    }

}