using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;
using static InCube.Core.Preconditions;

namespace InCube.Core.Numerics
{

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleType",
        Justification = "Companion object.")]
    public class Histogram<T>
    {
        internal Histogram(IReadOnlyList<T> binLowerEdges,
            IReadOnlyList<int> binCounts,
            IReadOnlyList<int> binIndices,
            int discardedCount,
            bool lowerEdgeEquals,
            Func<T, int> binSelector)
        {
            CheckArgument(binLowerEdges.Count == binCounts.Count,
                "number of bin lower edges {} must match the number of bin count {}",
                binLowerEdges.Count,
                binCounts.Count);
            CheckArgument(discardedCount <= binIndices.Count,
                "number of discarded elements {} must be at most the total number of elements {}",
                discardedCount,
                binIndices.Count);
            BinLowerEdges = binLowerEdges;
            BinCounts = binCounts;
            BinIndices = binIndices;
            DiscardedCount = discardedCount;
            LowerEdgeEquals = lowerEdgeEquals;
            BinSelector = binSelector;
        }

        public IReadOnlyList<T> BinLowerEdges { get; }
        
        public IReadOnlyList<int> BinCounts { get; }
        
        public IReadOnlyList<int> BinIndices { get; }
        
        public int DiscardedCount { get; }
        
        public int TotalCount => BinIndices.Count;
        
        public bool LowerEdgeEquals { get; }

        private Func<T, int> BinSelector { get; }

        public int FindBin(T value) => BinSelector(value);
    }

    public static class Histogram
    {
        public const bool DefaultLowerEdgeEquals = true;

        public static Histogram<T> Create<T>(IEnumerable<T> values,
            IReadOnlyList<T> edges,
            IComparer<T> comparer,
            bool lowerEdgeEquals = DefaultLowerEdgeEquals)
        {
            CheckArgument(edges.IsSorted(comparer: comparer), "edges must be sorted");

            var binCount = edges.Count;
            var binSelector = Functions.Invoke(() =>
            {
                if (edges is T[] edgeArray)
                {
                    return lowerEdgeEquals
                        ? (Func<T, int>)(v => edgeArray.UpperBound(v, comparer) - 1)
                        : (Func<T, int>)(v => edgeArray.LowerBound(v, comparer) - 1);
                }

                var listEdges = edges as List<T> ?? edges.ToList();
                return lowerEdgeEquals
                    ? (Func<T, int>)(v => listEdges.UpperBound(v, comparer) - 1)
                    : (Func<T, int>)(v => listEdges.LowerBound(v, comparer) - 1);
            });

            var (counts, indices, discarded) = GenericHistogram(values, binCount, binSelector);
            return new Histogram<T>(edges, counts, indices, discarded, lowerEdgeEquals, binSelector);
        }

        private static (int[] Counts, List<int> Indices, int Discarded) GenericHistogram<T>(
            IEnumerable<T> values,
            int binCount,
            Func<T, int> binSelector)
        {
            var indices = values.Select(binSelector).ToList();
            var counts = new int[binCount];
            var discarded = 0;
            foreach (var index in indices)
            {
                if (index >= 0)
                {
                    ++counts[index];
                }
                else
                {
                    ++discarded;
                }
            }
            return (counts, indices, discarded);
        }
    }
}