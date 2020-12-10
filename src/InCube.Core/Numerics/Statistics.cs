using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;
using JetBrains.Annotations;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Extension methods for computing rank statistics.
    /// </summary>
    [PublicAPI]
    public static class Statistics
    {
        /// <inheritdoc cref="Histograms.Create{T}"/>
        public static Histogram<T> MakeHistogram<T>(this IEnumerable<T> values, IReadOnlyList<T> edges, bool lowerBoundEquals = Histograms.DefaultLowerEdgeEquals)
            where T : IComparable<T> =>
            MakeHistogram(values, edges, Comparer<T>.Default, lowerBoundEquals);

        /// <inheritdoc cref="Histograms.Create{T}"/>
        public static Histogram<T> MakeHistogram<T>(this IEnumerable<T> values, IReadOnlyList<T> edges, IComparer<T> comparer, bool lowerEdgeEquals = Histograms.DefaultLowerEdgeEquals) =>
            Histograms.Create(values, edges, comparer, lowerEdgeEquals);

        /// <summary>
        /// Computes the rank of an <paramref name="element"/> in a collection of <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values to rank the element in.</param>
        /// <param name="element">The element to rank.</param>
        /// <typeparam name="T">The type of the collection and the element.</typeparam>
        /// <returns>The rank of the <paramref name="element"/> as an <see cref="int"/>.</returns>
        public static int Rank<T>(this IEnumerable<T> values, T element)
            where T : IComparable<T> =>
            values.Aggregate(0, (lesserCount, value) => value.CompareTo(element) <= 0 ? lesserCount + 1 : lesserCount);

        /// <summary>
        /// Computes the ranks of a collection of <paramref name="elements"/> in a collection of <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values to rank the elements in.</param>
        /// <param name="elements">The elements to rank.</param>
        /// <typeparam name="T">The type of the collection and the elements.</typeparam>
        /// <returns>The ranks of the <paramref name="elements"/>as an array of <see cref="int"/>s.</returns>
        public static int[] VectorRank<T>(this IEnumerable<T> values, IReadOnlyList<T> elements)
            where T : IComparable<T> =>
            values.VectorRank(elements, Comparer<T>.Default);

        /// <inheritdoc cref="VectorRank{T}(System.Collections.Generic.IEnumerable{T},System.Collections.Generic.IReadOnlyList{T})"/>
        public static int[] VectorRank<T>(this IEnumerable<T> values, IReadOnlyList<T> elements, IComparer<T> comparer)
        {
            var edgeCount = elements.Count;

            var (edges, optIndices) = Functions.If(
                elements.IsSorted(comparer),
                () => (elements, Option<int[]>.None),
                () =>
                {
                    var edgeArray = elements.ToArray();
                    var edgeIndices = Option.Some(Enumerable.Range(0, edgeCount).ToArray());
                    Array.Sort(edgeArray, edgeIndices.Value, comparer);
                    return (edgeArray, edgeIndices);
                });

            var ranks = new int[edgeCount];
            if (edgeCount == 0)
                return ranks;

            var histogram = values.MakeHistogram(edges, comparer);
            var binCounts = histogram.BinCounts;

            // reverse cumulative sum and inversion
            ranks[edgeCount - 1] = histogram.TotalCount - binCounts[edgeCount - 1];
            for (var i = edgeCount - 2; i >= 0; --i)
                ranks[i] = ranks[i + 1] - binCounts[i];

            return optIndices.Select(indices => ranks.Items(indices).ToArray()).GetValueOrDefault(ranks);
        }

        /// <summary>
        /// Computes the relative rank of an <paramref name="element"/> in a collection of <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values to rank the element in.</param>
        /// <param name="element">The element to rank.</param>
        /// <typeparam name="T">The type of the collection and the element.</typeparam>
        /// <returns>The relative rank of the <paramref name="element"/> as a <see cref="double"/>.</returns>
        public static double RelativeRank<T>(this IReadOnlyCollection<T> values, T element)
            where T : IComparable<T> =>
            values.Rank(element) / (double)values.Count;

        /// <summary>
        /// Computes the relative ranks of a collection of <paramref name="elements"/> in a collection of <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The values to rank the elements in.</param>
        /// <param name="elements">The elements to rank.</param>
        /// <typeparam name="T">The type of the collection and the elements.</typeparam>
        /// <returns>The relative ranks of the <paramref name="elements"/>as an array of <see cref="double"/>s.</returns>
        public static double[] VectorRelativeRank<T>(this IReadOnlyCollection<T> values, IReadOnlyList<T> elements)
            where T : IComparable<T> =>
            values.VectorRank(elements).Select(x => x / (double)values.Count).ToArray();
    }
}