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
    /// A variety of useful extension methods for lists.
    /// </summary>
    [PublicAPI]
    public static class Lists
    {
        /// <summary>
        /// Returns the last element of a list.
        /// </summary>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <param name="list">The input list.</param>
        /// <returns>An object of the type of the input list.</returns>
        public static T Last<T>(this IReadOnlyList<T> list) => list[^1];

        /// <summary>
        /// Returns the last element of a list, if it exists, None otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <param name="list">The input list.</param>
        /// <returns>An object of the type of the input list.</returns>
        public static Option<T> LastOption<T>(this IReadOnlyList<T> list) => list.Any() ? Option.Some(list[^1]) : Option.None;

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)" /> and makes
        /// sure to return the greatest boundary in case of ties.
        /// Corresponds to the upper_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="input"/>.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <seealso cref="List{T}.BinarySearch(T)" />
        /// <seealso cref="UpperBound{T}(T[],T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int UpperBound<T>(this List<T> input, T item)
            where T : IComparable<T> =>
            input.UpperBound(item, Comparer<T>.Default);

        /// <summary>
        /// <see cref="UpperBound{T}(System.Collections.Generic.List{T},T)"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="input"/> array.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <param name="comparer">The comparer to use to find the upper bound.</param>
        /// <seealso cref="List{T}.BinarySearch(T)" />
        /// <seealso cref="UpperBound{T}(T[],T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int UpperBound<T>(this List<T> input, T item, IComparer<T> comparer)
        {
            var idx = input.BinarySearch(item, comparer);
            return UpperIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// and makes
        /// sure to return the greatest boundary in case of ties.
        /// Corresponds to the upper_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="input"/> array.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// <seealso cref="UpperBound{T}(List{T},T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int UpperBound<T>(this T[] input, T item)
            where T : IComparable<T> =>
            input.UpperBound(item, Comparer<T>.Default);

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// and makes sure to return the greatest boundary in case of ties. Corresponds to the upper_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="input"/> array.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use.</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// <seealso cref="UpperBound{T}(List{T},T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int UpperBound<T>(this T[] input, T item, IComparer<T> comparer)
        {
            var idx = Array.BinarySearch(input, item, comparer);
            return UpperIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)" /> and makes
        /// sure to return the greatest boundary in case of ties. Corresponds to the lower_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <seealso cref="List{T}.BinarySearch(T)" />
        /// <seealso cref="LowerBound{T}(T[],T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int LowerBound<T>(this List<T> input, T item)
            where T : IComparable<T> =>
            input.LowerBound(item, Comparer<T>.Default);

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)" /> and makes
        /// sure to return the greatest boundary in case of ties. Corresponds to the lower_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use.</param>
        /// <seealso cref="List{T}.BinarySearch(T)" />
        /// <seealso cref="LowerBound{T}(T[],T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int LowerBound<T>(this List<T> input, T item, IComparer<T> comparer)
        {
            var idx = input.BinarySearch(item, comparer);
            return LowerIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// and makes
        /// sure to return the greatest boundary in case of ties.
        /// Corresponds to the lower_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <seealso cref="Array.BinarySearch(System.Array,int,int,object)" />
        /// <seealso cref="LowerBound{T}(System.Collections.Generic.List{T},T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int LowerBound<T>(this T[] input, T item)
            where T : IComparable<T> =>
            input.LowerBound(item, Comparer<T>.Default);

        /// <summary>
        /// Sorts an item into a list of buckets. The method calls <see cref="List{T}.BinarySearch(T)" /> and makes
        /// sure to return the greatest boundary in case of ties. Corresponds to the lower_bound function of the STL.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <param name="input">buckets must be sorted in ascending order.</param>
        /// <param name="item">the item to place into the buckets.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use.</param>
        /// <seealso cref="List{T}.BinarySearch(T)" />
        /// <seealso cref="LowerBound{T}(T[],T)" />
        /// <returns>the last bucket of the element.</returns>
        public static int LowerBound<T>(this T[] input, T item, IComparer<T> comparer)
        {
            var idx = Array.BinarySearch(input, item, comparer);
            return LowerIdxToBin(input, item, idx, comparer);
        }

        /// <summary>
        /// Converts the <paramref name="list"/> to an <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="list">The input <see cref="List{T}"/> to convert.</param>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <returns>The <paramref name="list"/> as an <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this List<T> list) => list;

        /// <summary>
        /// Converts the <paramref name="array"/> to an <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="array">The input <see cref="Array"/> to convert.</param>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <returns>The <paramref name="array"/> as an <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this T[] array) => array;

        /// <summary>
        /// Converts any of the implementations of <see cref="IList{T}"/> to an <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}"/> to convert.</param>
        /// <typeparam name="T">The type of the input <see cref="IList{T}"/>.</typeparam>
        /// <returns>The <paramref name="list"/> as an <see cref="IReadOnlyList{T}"/>.</returns>
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
        /// Converts a <see cref="List{T}"/> to an <see cref="IReadOnlyList{T}"/>.
        /// Effectively does nothing, as <see cref="List{T}"/> implements <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="list">The <see cref="List{T}"/> to convert.</param>
        /// <typeparam name="T">The type of the <paramref name="list"/>.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this List<T> list) => list;

        /// <summary>
        /// Converts an <see cref="Array"/> to an <see cref="IReadOnlyList{T}"/>.
        /// Effectively does nothing, as <see cref="Array"/> implements <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to convert.</param>
        /// <typeparam name="T">The type of the <paramref name="array"/>.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this T[] array) => array;

        /// <summary>
        /// Converts a <see cref="ICollection{T}"/> to an <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="ICollection{T}"/> to convert.</param>
        /// <typeparam name="T">The type of the input <paramref name="collection"/>.</typeparam>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/>.</returns>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> collection)
        {
            switch (collection)
            {
                case List<T> l:
                    return l;
                case T[] a:
                    return a;
                default:
                    return new ReadOnlyCollection<T>(collection);
            }
        }

        /// <summary>
        /// Parallel implementation of select for <see cref="IReadOnlyList{T}"/>s.
        /// </summary>
        /// <param name="list">The input <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="selector">The function to apply to each element of <paramref name="list"/> in parallel.</param>
        /// <param name="result">The output container for the applied <paramref name="selector"/>. If null, result is returned only.</param>
        /// <typeparam name="T">The input type of the <paramref name="list"/>.</typeparam>
        /// <typeparam name="TU">The output type.</typeparam>
        /// <returns>An <see cref="Array"/>.</returns>
        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, TU> selector, TU[]? result = null) => list.ParSelect(selector, 0, list.Count, result);

        /// <summary>
        /// Parallel implementation of select for <see cref="IReadOnlyList{T}"/>s.
        /// </summary>
        /// <param name="list">The input <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="selector">The function to apply to each element of <paramref name="list"/> in parallel.</param>
        /// <param name="fromInclusive">The lowest index to consider.</param>
        /// <param name="toExclusive">The first index to NOT consider.</param>
        /// <param name="result">The output container for the applied <paramref name="selector"/>. If null, result is returned only.</param>
        /// <typeparam name="T">The input type of the <paramref name="list"/>.</typeparam>
        /// <typeparam name="TU">The output type.</typeparam>
        /// <returns>An <see cref="Array"/>.</returns>
        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, TU> selector, int fromInclusive, int toExclusive, TU[]? result = null)
        {
            result ??= new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = selector(list[i]));
            return result;
        }

        /// <summary>
        /// Parallel implementation of select for <see cref="IReadOnlyList{T}"/>s.
        /// </summary>
        /// <param name="list">The input <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="selector">The function to apply to each element of <paramref name="list"/> in parallel.</param>
        /// <param name="result">The output container for the applied <paramref name="selector"/>. If null, result is returned only.</param>
        /// <typeparam name="T">The input type of the <paramref name="list"/>.</typeparam>
        /// <typeparam name="TU">The output type.</typeparam>
        /// <returns>An <see cref="Array"/>.</returns>
        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, int, TU> selector, TU[]? result = null) => list.ParSelect(selector, 0, list.Count, result);

        /// <summary>
        /// Parallel implementation of select for <see cref="IReadOnlyList{T}"/>s.
        /// </summary>
        /// <param name="list">The input <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="selector">The function to apply to each element of <paramref name="list"/> in parallel.</param>
        /// <param name="fromInclusive">The lowest index to consider.</param>
        /// <param name="toExclusive">The first index to NOT consider.</param>
        /// <param name="result">The output container for the applied <paramref name="selector"/>. If null, result is returned only.</param>
        /// <typeparam name="T">The input type of the <paramref name="list"/>.</typeparam>
        /// <typeparam name="TU">The output type.</typeparam>
        /// <returns>An <see cref="Array"/>.</returns>
        public static TU[] ParSelect<T, TU>(this IReadOnlyList<T> list, Func<T, int, TU> selector, int fromInclusive, int toExclusive, TU[]? result = null)
        {
            result ??= new TU[toExclusive - fromInclusive];
            Parallel.For(fromInclusive, toExclusive, i => result[i - fromInclusive] = selector(list[i], i));
            return result;
        }

        /// <summary>
        /// Parallel implementation of <see cref="Enumerables.Generate{T}"/>.
        /// </summary>
        /// <param name="count">The number of items to generate.</param>
        /// <param name="generator">The generator to use.</param>
        /// <typeparam name="T">The output type of the generator.</typeparam>
        /// <returns>An <see cref="Array"/>.</returns>
        public static T[] ParGenerate<T>(int count, Func<int, T> generator)
        {
            var result = new T[count];
            Parallel.For(0, count, i => result[i] = generator(i));
            return result;
        }

        /// <summary>
        /// Gets the items from the <paramref name="list"/> by their <paramref name="indices"/>.
        /// </summary>
        /// <param name="list">The source of items.</param>
        /// <param name="indices">The indices of the items to get.</param>
        /// <typeparam name="T">The type of the <paramref name="list"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Items<T>(this IReadOnlyList<T> list, IEnumerable<int> indices) => indices.Select(i => list[i]);

        /// <summary>
        /// Assuming the <paramref name="indices"/> are in range of every element in the input <paramref name="enumerable"/>, fetches the <see cref="Items{T}"/> out of each element.
        /// Similar to getting the columns out of a row-major matrix.
        /// </summary>
        /// <param name="enumerable">The enumerable of input <see cref="IReadOnlyList{T}"/>s.</param>
        /// <param name="indices">The indices of the items to get out of each input <see cref="IReadOnlyList{T}"/>.</param>
        /// <typeparam name="T">The type of the input <see cref="IReadOnlyList{T}"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IEnumerable{T}"/>s.</returns>
        public static IEnumerable<IEnumerable<T>> Cols<T>(this IEnumerable<IReadOnlyList<T>> enumerable, IEnumerable<int> indices) => enumerable.Select(list => list.Items(indices));

        /// <summary>
        /// Fetches a slice of the <paramref name="list"/> from <paramref name="startInclusive"/> to <paramref name="stopExclusive"/>.
        /// </summary>
        /// <param name="list">The <see cref="IReadOnlyList{T}"/> to slice.</param>
        /// <param name="startInclusive">The start of the slice (included).</param>
        /// <param name="stopExclusive">The end of the slice (excluded). If omitted, runs to the end.</param>
        /// <typeparam name="T">The type of the <paramref name="list"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Slice<T>(this IReadOnlyList<T> list, int startInclusive = default, int? stopExclusive = default) => Enumerables.IntRange(startInclusive, stopExclusive ?? list.Count).Select(i => list[i]);

        /// <summary>
        /// Fetches a slice of the <paramref name="enumerable"/> from <paramref name="startInclusive"/> to the <paramref name="stopExclusive"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to slice.</param>
        /// <param name="startInclusive">The start of the slice (included).</param>
        /// <param name="stopExclusive">The end of the slice (excluded). If omitted, runs to the end.</param>
        /// <typeparam name="T">The type of the <paramref name="enumerable"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> enumerable, int? startInclusive = default, int? stopExclusive = default)
        {
            var skip = startInclusive ?? 0;
            return enumerable.Skip(skip).ApplyOpt(e => stopExclusive.Select(stop => e.Take(stop - skip)));
        }

        /// <summary>
        /// Fetches a slice of an input <see cref="Array"/> <paramref name="array"/> from <paramref name="startInclusive"/> to the <paramref name="stopExclusive"/>.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to slice.</param>
        /// <param name="startInclusive">The start of the slice (included).</param>
        /// <param name="stopExclusive">The end of the slice (excluded). If omitted, runs to the end.</param>
        /// <typeparam name="T">The type of the <paramref name="array"/>.</typeparam>
        /// <returns>An <see cref="ArraySegment{T}"/>.</returns>
        public static ArraySegment<T> Slice<T>(this T[] array, int startInclusive = default, int? stopExclusive = default) => new(array, startInclusive, (stopExclusive ?? array.Length) - startInclusive);

        /// <summary>
        /// Fetches a slice of an input <see cref="ArraySegment{T}"/> <paramref name="arraySegment"/> from <paramref name="startInclusive"/> to the <paramref name="stopExclusive"/>.
        /// </summary>
        /// <param name="arraySegment">The <see cref="ArraySegment{T}"/> to slice.</param>
        /// <param name="startInclusive">The start of the slice (included).</param>
        /// <param name="stopExclusive">The end of the slice (excluded). If omitted, runs to the end.</param>
        /// <typeparam name="T">The type of the <paramref name="arraySegment"/>.</typeparam>
        /// <returns>An <see cref="ArraySegment{T}"/>.</returns>
        public static ArraySegment<T> Slice<T>(this ArraySegment<T> arraySegment, int startInclusive = default, int? stopExclusive = default) =>
            new(arraySegment.Array!, arraySegment.Offset + startInclusive, (stopExclusive ?? arraySegment.Count) - startInclusive);

        /// <summary>
        /// Fetches a slice of a 2-dimensional array.
        /// </summary>
        /// <param name="elems">The 2-dimensional array to slice.</param>
        /// <param name="rowStartInclusive">The start row.</param>
        /// <param name="rowStopExclusive">The stop row.</param>
        /// <param name="colStartInclusive">The start column.</param>
        /// <param name="colStopExclusive">The stop column.</param>
        /// <param name="result">A container for the output. If empty, result is returned.</param>
        /// <typeparam name="T">The type of the input and output arrays.</typeparam>
        /// <returns>A 2-dimensional array.</returns>
        public static T[,] Slice<T>(
            this T[,] elems,
            int rowStartInclusive = default,
            int? rowStopExclusive = default,
            int colStartInclusive = default,
            int? colStopExclusive = default,
            T[,]? result = null)
            where T : unmanaged
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

            result ??= new T[rowCount, colCount];
            if (rowCount == 0 || colCount == 0)
                return result;

            var dstRowCount = result.GetLength(0);
            CheckArgument(rowCount <= dstRowCount, "insufficient space for {0} rows", rowCount);
            var dstColCount = result.GetLength(1);
            CheckArgument(colCount <= dstColCount, "insufficient space for {0} columns", colCount);

            unsafe
            {
                var rowSize = colCount * sizeof(T);
                fixed (T* src = &elems[rowStartInclusive, colStartInclusive])
                {
                    fixed (T* dst = &result[0, 0])
                    {
                        for (var r = 0; r < rowCount; ++r)
                        {
                            var srcP = src + (r * srcColCount);
                            var dstP = dst + (r * dstColCount);
                            Buffer.MemoryCopy(srcP, dstP, rowSize, rowSize);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checkes whether two <see cref="IReadOnlyCollection{T}"/>s contain the same elements.
        /// </summary>
        /// <param name="left">First collection.</param>
        /// <param name="right">Second collection.</param>
        /// <param name="comparer">The comparer to use to determine whether two elements are equal.</param>
        /// <typeparam name="T">The type of the input collections.</typeparam>
        /// <returns>A boolean indicating whether or not the two collections are equal.</returns>
        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> left, IReadOnlyCollection<T> right, IEqualityComparer<T> comparer) => left.Count == right.Count && left.SequenceEqual(right, comparer);

        /// <summary>
        /// Checks whether two <see cref="IReadOnlyCollection{T}"/>s contain the same elements using the default comparer.
        /// </summary>
        /// <param name="left">First collection.</param>
        /// <param name="right">Second collection.</param>
        /// <typeparam name="T">The type of the input collections.</typeparam>
        /// <returns>A boolean indicating whether or not the two collections are equal.</returns>
        public static bool CollectionEqual<T>(this IReadOnlyCollection<T> left, IReadOnlyCollection<T> right) => left.Count == right.Count && left.SequenceEqual(right);

        /// <summary>
        /// An adaptation of <see cref="List{T}.RemoveAll" /> by shifting all elements matching the predicate <paramref name="match" /> to the back of this list.
        /// </summary>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <param name="elems">The source to remove elements from.</param>
        /// <param name="match">Predicate to check whether or not to remove each element.</param>
        /// <param name="startIdx">The first index to consider.</param>
        /// <param name="stopIdx">The first index to NOT consider (exclusive).</param>
        /// <returns>The index of the first element in <paramref name="elems" /> that matches the predicate.</returns>
        public static int SeqRemoveAll<T>(this IList<T> elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0)
                stopIdx = elems.Count;

            CheckNotNull(match, nameof(match));

            var freeIndex = startIdx; // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < stopIdx && !match(elems[freeIndex]))
                freeIndex++;
            if (freeIndex >= stopIdx)
                return stopIdx;

            var current = freeIndex + 1;
            while (current < stopIdx)
            {
                // Find the first item which needs to be kept.
                while (current < stopIdx && match(elems[current]))
                    current++;

                if (current < stopIdx)
                    elems[freeIndex++] = elems[current++];
            }

            return freeIndex;
        }

        /// <summary>
        /// A parallel version of <see cref="SeqRemoveAll{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the input list.</typeparam>
        /// <param name="elems">The source to remove elements from.</param>
        /// <param name="match">Predicate to check whether or not to remove each element.</param>
        /// <param name="startIdx">The first index to consider.</param>
        /// <param name="stopIdx">The first index to NOT consider (exclusive).</param>
        /// <returns>The index of the first element in <paramref name="elems" /> that matches the predicate.</returns>
        public static int ParRemoveAll<T>(this T[] elems, Predicate<T> match, int startIdx = 0, int stopIdx = -1)
        {
            if (stopIdx < 0)
                stopIdx = elems.Length;

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
                if (length == 0)
                    continue;
                if (start > freeIndex)
                    Array.Copy(elems, start, elems, freeIndex, length);

                freeIndex += length;
            }

            return freeIndex;
        }

        private static int LowerIdxToBin<T>(IReadOnlyList<T> input, T item, int idx, IComparer<T> comparer)
        {
            var count = input.Count;
            if (idx >= 0)
            {
                while (idx > 0 && comparer.Compare(input[idx - 1], item) == 0)
                    --idx;
            }
            else
            {
                idx = ~idx;
            }

            Debug.Assert(idx == count || comparer.Compare(input[idx], item) >= 0, "next item is less");
            Debug.Assert(idx == 0 || comparer.Compare(input[idx - 1], item) < 0, "previous item is greater or equal");

            return idx;
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
    }
}