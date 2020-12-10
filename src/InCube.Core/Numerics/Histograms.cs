using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;
using static InCube.Core.Preconditions;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Companion class of the <see cref="Histogram{T}"/>.
    /// </summary>
    public static class Histograms
    {
        /// <summary>
        /// Gets the default value for the equality of the lower edges.
        /// </summary>
        public const bool DefaultLowerEdgeEquals = true;

        /// <summary>
        /// Initializes a <see cref="Histogram{T}"/>.
        /// </summary>
        /// <param name="values">The values to histogramize.</param>
        /// <param name="edges">The edges of the histogram.</param>
        /// <param name="comparer">The comparer to use to compare the <paramref name="values"/>.</param>
        /// <param name="lowerEdgeEquals">Whether the lower edges are equal.</param>
        /// <typeparam name="T">The type of values contained in the histogram.</typeparam>
        /// <returns>A <see cref="Histogram{T}"/>.</returns>
        public static Histogram<T> Create<T>(IEnumerable<T> values, IReadOnlyList<T> edges, IComparer<T> comparer, bool lowerEdgeEquals = DefaultLowerEdgeEquals)
        {
            CheckArgument(edges.IsSorted(comparer), "edges must be sorted");

            var binCount = edges.Count;
            var binSelector = Functions.Invoke(() =>
            {
                if (edges is T[] edgeArray)
                    return lowerEdgeEquals ? (Func<T, int>)(v => edgeArray.UpperBound(v, comparer) - 1) : (Func<T, int>)(v => edgeArray.LowerBound(v, comparer) - 1);

                var listEdges = edges as List<T> ?? edges.ToList();
                return lowerEdgeEquals ? (Func<T, int>)(v => listEdges.UpperBound(v, comparer) - 1) : (Func<T, int>)(v => listEdges.LowerBound(v, comparer) - 1);
            });

            var (counts, indices, discarded) = GenericHistogram(values, binCount, binSelector);
            return new Histogram<T>(
                edges,
                counts,
                indices,
                discarded,
                lowerEdgeEquals,
                binSelector);
        }

        private static (int[] Counts, List<int> Indices, int Discarded) GenericHistogram<T>(IEnumerable<T> values, int binCount, Func<T, int> binSelector)
        {
            var indices = values.Select(binSelector).ToList();
            var counts = new int[binCount];
            var discarded = 0;
            foreach (var index in indices)
            {
                if (index >= 0)
                    ++counts[index];
                else
                    ++discarded;
            }

            return (counts, indices, discarded);
        }
    }
}