using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InCube.Core.Functional;
using JetBrains.Annotations;
using static InCube.Core.Preconditions;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A variety of useful extension methods for lists
    /// </summary>
    [PublicAPI]
    public static class Lists
    {
        /// <summary>
        /// Returns the last element of a list
        /// </summary>
        /// <typeparam name="T">The type of the input list</typeparam>
        /// <param name="list">The input list</param>
        /// <returns>An object of the type of the input list</returns>
        public static T Last<T>(this IReadOnlyList<T> list) => list[list.Count - 1];

        /// <summary>
        /// Returns the last element of a list, if it exists, None otherwise
        /// </summary>
        /// <typeparam name="T">The type of the input list</typeparam>
        /// <param name="list">The input list</param>
        /// <returns>An object of the type of the input list</returns>
        public static Option<T> LastOption<T>(this IReadOnlyList<T> list) => list.Any() ? Option.Some(list[list.Count - 1]) : Option.None;

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)"/> and makes
        /// sure to return the greatest boundary in case of ties.
        /// Corresponds to the <code>upper_bound</code> function of the STL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">buckets must be sorted in ascending order</param>
        /// <param name="item">the item to place into the buckets</param>
        /// <seealso cref="List{T}.BinarySearch(T)"/>
        /// <seealso cref="UpperBound{T}(T[],T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int UpperBound<T>(this List<T> input, T item) where T : IComparable<T> => input.UpperBound(item, Comparer<T>.Default);

        /// <seealso cref="UpperBound{T}(List{T},T)"/>
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
        /// <seealso cref="UpperBound{T}(List{T},T)"/>
        /// <returns>the last bucket of the element</returns>
        public static int UpperBound<T>(this T[] input, T item) where T : IComparable<T> => input.UpperBound(item, Comparer<T>.Default);

        /// <seealso cref="UpperBound{T}(T[],T)"/>
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
                }
                while (idx < count && comparer.Compare(input[idx], item) == 0);
            }
            else
            {
                idx = ~idx;
            }

            Debug.Assert(idx == count || comparer.Compare(input[idx], item) > 0, "next item is less or equal");
            Debug.Assert(idx == 0 || comparer.Compare(input[idx - 1], item) <= 0, "previous item is greater");

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
        public static int LowerBound<T>(this List<T> input, T item) where T : IComparable<T> => input.LowerBound(item, Comparer<T>.Default);

        /// <see cref="LowerBound{T}(List{T},T)"/>
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
        public static int LowerBound<T>(this T[] input, T item) where T : IComparable<T> => input.LowerBound(item, Comparer<T>.Default);

        /// <see cref="LowerBound{T}(T[],T)"/>
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

            Debug.Assert(idx == count || comparer.Compare(input[idx], item) >= 0, "next item is less");
            Debug.Assert(idx == 0 || comparer.Compare(input[idx - 1], item) < 0, "previous item is greater or equal");

            return idx;
        }

        /// <summary>
        /// The purpose of this method is to issue a compiler warning if someone calls this by mistake.
        /// </summary>
        [Obsolete("unnecessary call")]
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IReadOnlyCollection<T> col) => col;

        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this List<T> col) => col;

        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this T[] col) => col;

        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list)
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
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IReadOnlyList<T> list) => list;

        public static IReadOnlyList<T> AsReadOnlyList<T>(this List<T> list) => list;

        public static IReadOnlyList<T> AsReadOnlyList<T>(this T[] list) => list;

        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> col)
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

        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, TU> map, TU[] result = null) => list.ParSelect(map, 0, list.Count, result);

        public static TU[] ParSelect<T, TU>(
            this IReadOnlyList<T> list,
            Func<T, TU> map,
            int fromInclusive,
            int toExclusive,
            TU[] result = null)
        {
            result = result ?? new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = map(list[i]));
            return result;
        }

        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, int, TU> map, TU[] result = null) => list.ParSelect(map, 0, list.Count, result);

        public static TU[] ParSelect<T, TU>(
            this IReadOnlyList<T> list,
            Func<T, int, TU> map,
            int fromInclusive,
            int toExclusive,
            TU[] result = null)
        {
            result = result ?? new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = map(list[i], i));
            return result;
        }

        public static T[] ParGenerate<T>(int count, Func<int, T> generator)
        {
            var result = new T[count];
            Parallel.For(0, count, i => result[i] = generator(i));
            return result;
        }

        public static IEnumerable<T> Items<T>(this IReadOnlyList<T> list, IEnumerable<int> indices) => indices.Select(i => list[i]);

        public static IEnumerable<IEnumerable<T>> Cols<T>(this IEnumerable<IReadOnlyList<T>> enumerable, IEnumerable<int> indices) => enumerable.Select(list => list.Items(indices));

        public static IEnumerable<T> Slice<T>(
            this IEnumerable<T> elems,
            int? startInclusive = default,
            int? stopExclusive = default)
        {
            switch (elems)
            {
                case IReadOnlyList<T> list:
                    return list.Slice(startInclusive, stopExclusive);
                default:
                    var skip = startInclusive ?? 0;
                    return elems.Skip(skip).ApplyOpt(e => stopExclusive.Select(stop => e.Take(stop - skip)));
            }
        }

        public static IEnumerable<T> Slice<T>(
            this IReadOnlyList<T> list,
            int startInclusive = default,
            int? stopExclusive = default) =>
            Enumerables.IntRange(startInclusive, stopExclusive ?? list.Count).Select(i => list[i]);

        public static ArraySegment<T> Slice<T>(
            this T[] elems,
            int startInclusive = default,
            int? stopExclusive = default) =>
            new ArraySegment<T>(elems, startInclusive, (stopExclusive ?? elems.Length) - startInclusive);

        public static ArraySegment<T> Slice<T>(
            this ArraySegment<T> elems,
            int startInclusive = default,
            int? stopExclusive = default) =>
            // ReSharper disable once AssignNullToNotNullAttribute
            new ArraySegment<T>(elems.Array, elems.Offset + startInclusive, (stopExclusive ?? elems.Count) - startInclusive);

        public static T[,] Slice<T>(
            this T[,] elems,
            int rowStartInclusive = default,
            int? rowStopExclusive = default,
            int colStartInclusive = default,
            int? colStopExclusive = default,
            T[,] result = null) where T : unmanaged
        {
            var srcRowCount = elems.GetLength(0);
            var rowStop = rowStopExclusive ?? srcRowCount;

#pragma warning disable SA1131 // Use readable conditions
            CheckArgument(0 <= rowStop && rowStop <= srcRowCount, "invalid row stop {0}", rowStop);
            var srcColCount = elems.GetLength(1);
            var colStop = colStopExclusive ?? srcColCount;
            CheckArgument(0 <= colStop && colStop <= srcColCount, "invalid col stop {0}", colStop);
            var rowCount = rowStop - rowStartInclusive;
            CheckArgument(0 <= rowCount && rowCount <= srcRowCount, "invalid row count {0}", rowCount);
            var colCount = colStop - colStartInclusive;
            CheckArgument(0 <= colCount && colCount <= srcRowCount, "invalid col count {0}", colCount);
#pragma warning restore SA1131 // Use readable conditions

            result = result ?? new T[rowCount, colCount];
            if (rowCount == 0 || colCount == 0) return result;

            var dstRowCount = result.GetLength(0);
            CheckArgument(rowCount <= dstRowCount, "insufficient space for {0} rows", rowCount);
            var dstColCount = result.GetLength(1);
            CheckArgument(colCount <= dstColCount, "insufficient space for {0} columns", colCount);

            unsafe
            {
                var rowSize = colCount * sizeof(T);
                fixed (T* src = &elems[rowStartInclusive, colStartInclusive])
                fixed (T* dst = &result[0, 0])
                {
                    for (var r = 0; r < rowCount; ++r)
                    {
                        var srcP = src + srcColCount * r;
                        var dstP = dst + dstColCount * r;
                        Buffer.MemoryCopy(srcP, dstP, rowSize, rowSize);
                    }
                }
            }

            return result;
        }

        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, IEqualityComparer<T> comparer) => x.Count == y.Count && x.SequenceEqual(y, comparer);

        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> x, IReadOnlyCollection<T> y) => x.Count == y.Count && x.SequenceEqual(y);

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
        public static int SeqRemoveAll<T>(this IList<T> elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0) stopIdx = elems.Count;

            CheckNotNull(match, nameof(match));

            var freeIndex = startIdx; // the first free slot in items array

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
        /// A parallel version of <see cref="SeqRemoveAll{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elems"></param>
        /// <param name="match"></param>
        /// <param name="startIdx"></param>
        /// <param name="stopIdx"></param>
        /// <returns></returns>
        public static int ParRemoveAll<T>(this T[] elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0) stopIdx = elems.Length;

            CheckNotNull(match, nameof(match));

            var partitioner = Partitioner.Create(startIdx, stopIdx);
            var sections = new ConcurrentDictionary<int, int>();
            Parallel.ForEach(
                partitioner,
                range =>
                {
                    var (start, stop) = range;
                    sections[start] = elems.SeqRemoveAll(match, start, stop);
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