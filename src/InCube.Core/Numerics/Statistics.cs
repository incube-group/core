using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;

namespace InCube.Core.Numerics
{
    public static class Statistics
    {
        public static Histogram<T> MakeHistogram<T>(this IEnumerable<T> values,
            IReadOnlyList<T> edges, bool lowerBoundEquals = Histogram<T>.DefaultLowerEdgeEquals)
            where T : IComparable<T> =>
            MakeHistogram(values, edges, Collections.Collections.DefaultComparer<T>(), lowerBoundEquals);

        public static Histogram<T> MakeHistogram<T>(this IEnumerable<T> values,
            IReadOnlyList<T> edges, IComparer<T> comparer,
            bool lowerEdgeEquals = Histogram<T>.DefaultLowerEdgeEquals) =>
            Histogram<T>.Create(values, edges, comparer, lowerEdgeEquals);

        public static int Rank<T>(this IEnumerable<T> values, T element) where T : IComparable<T> =>
            values.Aggregate(0, (lesserCount, value) => value.CompareTo(element) <= 0 ? lesserCount + 1 : lesserCount);

        public static int[] VectorRank<T>(this IEnumerable<T> values, IReadOnlyList<T> elements)
            where T : IComparable<T> =>
            values.VectorRank(elements, Collections.Collections.DefaultComparer<T>());

        public static int[] VectorRank<T>(this IEnumerable<T> values, IReadOnlyList<T> elements, IComparer<T> comparer)
        {
            var edgeCount = elements.Count;

            var (edges, optIndices) = Functions.If(elements.IsSorted(comparer: comparer),
                ifBranch:   () => (elements, Option.Empty<int[]>()),
                elseBranch: () =>
                {
                    var edgeArray = elements.ToArray();
                    var edgeIndices = Option.Some(Enumerable.Range(0, edgeCount).ToArray());
                    Array.Sort(edgeArray, edgeIndices.Value, comparer);
                    return (edgeArray, edgeIndices);
                }
            );

            var ranks = new int[edgeCount];
            if (edgeCount == 0) return ranks;

            var histogram = values.MakeHistogram(edges, comparer, lowerEdgeEquals: true);
            var binCounts = histogram.BinCounts;

            // reverse cumulative sum and inversion
            ranks[edgeCount - 1] = histogram.TotalCount - binCounts[edgeCount - 1];
            for (var i = edgeCount - 2; i >= 0; --i)
            {
                ranks[i] = ranks[i + 1] - binCounts[i];
            }

            return optIndices.Select(indices => ranks.Get(indices).ToArray()).GetValueOrDefault(ranks);
        }

        public static double RelativeRank<T>(this IReadOnlyCollection<T> values, T element) where T : IComparable<T> =>
            values.Rank(element) / (double)values.Count;

        public static double[] VectorRelativeRank<T>(this IReadOnlyCollection<T> values, IReadOnlyList<T> elements) where T : IComparable<T> =>
            values.VectorRank(elements).Select(x => x / (double)values.Count).ToArray();
    }
}
