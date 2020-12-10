using System;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Invariance range interface.
    /// </summary>
    /// <typeparam name="T">Type of domain of the range.</typeparam>
    /// <typeparam name="TRange">Underlying range type.</typeparam>
    public interface IInvariantRange<T, TRange> : IRange<T>, IEquatable<TRange>
        where TRange : IInvariantRange<T, TRange>
    {
        /// <summary>
        /// Checks whether <paramref name="x"/> is in the range.
        /// </summary>
        /// <param name="x">The element to check for belonging to the range.</param>
        /// <returns>Whether or not <paramref name="x"/> belongs to the range.</returns>
        bool Contains(T x);

        /// <summary>
        /// Checks whether the other <paramref name="range"/> is contained in the range.
        /// </summary>
        /// <param name="range">The range to check for being contained in the range.</param>
        /// <returns>Whether or not <paramref name="range"/> is contained in the range.</returns>
        bool Contains(TRange range);

        /// <summary>
        /// Checks whether the other <paramref name="range"/> overlaps with the range.
        /// </summary>
        /// <param name="range">The other range to check for overlap.</param>
        /// <returns>Whether or not the <paramref name="range"/> overlaps with the range.</returns>
        bool OverlapsWith(TRange range);

        /// <summary>
        /// Creates the intersection with the other <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The other range.</param>
        /// <returns>The intersection of the two ranges.</returns>
        TRange IntersectWith(TRange range);
    }
}