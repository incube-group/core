using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Represents a histogram type.
    /// </summary>
    /// <typeparam name="T">The type of objects in the histogram.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Companion object.")]
    [PublicAPI]
    public class Histogram<T>
    {
        internal Histogram(
            IReadOnlyList<T> binLowerEdges,
            IReadOnlyList<int> binCounts,
            IReadOnlyList<int> binIndices,
            int discardedCount,
            bool lowerEdgeEquals,
            Func<T, int> binSelector)
        {
            Preconditions.CheckArgument(binLowerEdges.Count == binCounts.Count, "number of bin lower edges {0} must match the number of bin count {1}", binLowerEdges.Count, binCounts.Count);
            Preconditions.CheckArgument(discardedCount <= binIndices.Count, "number of discarded elements {0} must be at most the total number of elements {1}", discardedCount, binIndices.Count);
            this.BinLowerEdges = binLowerEdges;
            this.BinCounts = binCounts;
            this.BinIndices = binIndices;
            this.DiscardedCount = discardedCount;
            this.LowerEdgeEquals = lowerEdgeEquals;
            this.BinSelector = binSelector;
        }

        /// <summary>
        /// Gets the lower edges of the bins.
        /// </summary>
        public IReadOnlyList<T> BinLowerEdges { get; }

        /// <summary>
        /// Gets the count of elements per bin.
        /// </summary>
        public IReadOnlyList<int> BinCounts { get; }

        /// <summary>
        /// Gets the indices of the bins.
        /// </summary>
        public IReadOnlyList<int> BinIndices { get; }

        /// <summary>
        /// Gets the count of discarded elements.
        /// </summary>
        public int DiscardedCount { get; }

        /// <summary>
        /// Gets the total count.
        /// </summary>
        public int TotalCount => this.BinIndices.Count;

        /// <summary>
        /// Gets a value indicating whether the lower edge are equals.
        /// </summary>
        public bool LowerEdgeEquals { get; }

        private Func<T, int> BinSelector { get; }

        /// <summary>
        /// Gets the bin in which the <paramref name="value"/> is located.
        /// </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>The index of the bin in which <paramref name="value"/> is located.</returns>
        public int FindBin(T value) => this.BinSelector(value);
    }
}