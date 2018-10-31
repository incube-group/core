using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using static InCube.Core.Preconditions;

namespace InCube.Core.Collections
{

    public static class Collections
    {
        /// <returns>the last element of a list</returns>
        public static T Last<T>(this IReadOnlyList<T> list) => list[list.Count - 1];

        [PublicAPI]
        public static IComparer<T> DefaultComparer<T>() where T : IComparable<T> =>
            Comparer<T>.Create((x, y) => x.CompareTo(y));

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        ///
        /// Corresponds to the <code>upper_bound</code> function of the STL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="List{T}.BinarySearch(T)"/>
        /// <seealso cref="UpperBound{T}(T[],T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int UpperBound<T>(this List<T> input, T item) where T : IComparable<T> =>
            input.UpperBound(item, DefaultComparer<T>());

        public static int UpperBound<T>(this List<T> input, T item, IComparer<T> comparer)
        {
            var idx = input.BinarySearch(item, comparer);
            return UpperIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        /// 
        /// Corresponds to the <code>upper_bound</code> function of the STL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)"/>
        /// <seealso cref="UpperBound{T}(System.Collections.Generic.List{T},T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int UpperBound<T>(this T[] input, T item) where T : IComparable<T> =>
            input.UpperBound(item, DefaultComparer<T>());

        public static int UpperBound<T>(this T[] input, T item, IComparer<T> comparer)
        {
            var idx = Array.BinarySearch(input, item, comparer);
            return UpperIdxToBin(input, item, idx, comparer);
        }

        private static int UpperIdxToBin<T>(IReadOnlyList<T> input, T item, int idx, IComparer<T> comparer)
        {
            var count = input.Count;
            if (idx >= 0)
            {
                do
                {
                    ++idx;
                } while (idx < count && comparer.Compare(input[idx], item) == 0);
            }
            else
            {
                idx = ~idx;
            }

            Debug.Assert(idx == count || comparer.Compare(input[idx], item) > 0);
            Debug.Assert(idx == 0 || comparer.Compare(input[idx - 1], item) <= 0);

            return idx;
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        ///
        /// Corresponds to the <code>lower_bound</code> function of the STL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="List{T}.BinarySearch(T)"/>
        /// <seealso cref="LowerBound{T}(T[],T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int LowerBound<T>(this List<T> input, T item) where T : IComparable<T> =>
            input.LowerBound(item, DefaultComparer<T>());

        public static int LowerBound<T>(this List<T> input, T item, IComparer<T> comparer)
        {
            var idx = input.BinarySearch(item, comparer);
            return LowerIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        /// 
        /// Corresponds to the <code>lower_bound</code> function of the STL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)"/>
        /// <seealso cref="LowerBound{T}(System.Collections.Generic.List{T},T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int LowerBound<T>(this T[] input, T item) where T : IComparable<T> =>
            input.LowerBound(item, DefaultComparer<T>());

        public static int LowerBound<T>(this T[] input, T item, IComparer<T> comparer)
        {
            var idx = Array.BinarySearch(input, item, comparer);
            return LowerIdxToBin(input, item, idx, comparer);
        }

        private static int LowerIdxToBin<T>(IReadOnlyList<T> input, T item, int idx, IComparer<T> comparer)
        {
            var count = input.Count;
            if (idx >= 0)
            {
                while (idx > 0 && comparer.Compare(input[idx - 1], item) == 0)
                {
                    --idx;
                }
            }
            else
            {
                idx = ~idx;
            }

            Debug.Assert(idx == count || comparer.Compare(input[idx], item) >= 0);
            Debug.Assert(idx == 0 || comparer.Compare(input[idx - 1], item) < 0);

            return idx;
        }

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", ").
        /// </summary>
        public static String MkString<T>(this IEnumerable<T> enumerable, string separator = ", ") =>
            string.Join(separator, enumerable);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", "), wrapped by with a string in the beginning and the end.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="start">The string to place in front of the joined output.</param>
        /// <param name="separator">The separator to be placed between elements.</param>
        /// <param name="end">The string to place at the end of the joined output.</param>
        public static String MkString<T>(this IEnumerable<T> enumerable, string start, string separator, string end) =>
            $"{start}{enumerable.MkString(separator)}{end}";

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

        public static TU[] ParallelMap<T, TU>(this IReadOnlyList<T> list, Func<T, TU> map, TU[] result = null) =>
            list.ParallelMap(map, 0, list.Count, result);

        public static TU[] ParallelMap<T, TU>(this IReadOnlyList<T> list, Func<T, TU> map,
            int fromInclusive, int toExclusive, TU[] result = null)
        {
            result = result ?? new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = map(list[i]));
            return result;
        }

        public static TU[] ParallelMapI<T, TU>(this IReadOnlyList<T> list, Func<T, int, TU> map, TU[] result = null) =>
            list.ParallelMapI(map, 0, list.Count, result);

        public static TU[] ParallelMapI<T, TU>(this IReadOnlyList<T> list, Func<T, int, TU> map, 
            int fromInclusive, int toExclusive, TU[] result = null)
        {
            result = result ?? new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = map(list[i], i));
            return result;
        }

        public static T[] ParallelGenerate<T>(int count, Func<int, T> generator)
        {
            var result = new T[count];
            Parallel.For(0, count, i => result[i] = generator(i));
            return result;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey> =>
            source.MaxBy(selector, DefaultComparer<TKey>());

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    throw new InvalidOperationException("empty source sequence");

                var opt = iterator.Current;
                var optValue = selector(opt);

                while (iterator.MoveNext())
                {
                    var current = iterator.Current;
                    var currentValue = selector(current);

                    if (comparer.Compare(currentValue, optValue) > 0)
                    {
                        opt = current;
                        optValue = currentValue;
                    }
                }

                return opt;
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey> =>
            source.MinBy(selector, DefaultComparer<TKey>());

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer) => source.MaxBy(selector, Comparer<TKey>.Create((x, y) => comparer.Compare(y, x)));

        public static int ArgMax<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
            source.ZipWithIndex().MaxBy(x => x.value, comparer).index;

        public static int ArgMax<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source.ArgMax(DefaultComparer<T>());

        public static int ArgMin<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
            source.ZipWithIndex().MinBy(x => x.value, comparer).index;

        public static int ArgMin<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source.ArgMax(DefaultComparer<T>());

        public static IEnumerable<T> Get<T>(this IReadOnlyList<T> list, IEnumerable<int> indices) =>
            indices.Select(i => list[i]);

        public static IEnumerable<IEnumerable<T>> GetCols<T>(this IEnumerable<IReadOnlyList<T>> enumerable, IEnumerable<int> indices) =>
            enumerable.Select(list => indices.Select(i => list[i]));

        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, IEqualityComparer<T> comparer) =>
            x.Count == y.Count && x.SequenceEqual(y, comparer);

        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> x, IReadOnlyCollection<T> y) =>
            x.Count == y.Count && x.SequenceEqual(y);

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

        public static IReadOnlyList<T> Empty<T>() => new EmptyList<T>();

        private class EmptyList<T> : IReadOnlyList<T>
        {
            public IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public int Count => 0;

            public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index), index, "invalid index");
        }
    }

}