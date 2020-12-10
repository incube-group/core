using System;
using JetBrains.Annotations;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Extension methods for <see cref="IComparable{T}"/>s.
    /// </summary>
    [PublicAPI]
    public static class Comparables
    {
        /// <summary>
        /// Returns the minimum of the two inputs.
        /// </summary>
        /// <param name="x">First input.</param>
        /// <param name="y">Second input.</param>
        /// <typeparam name="T">Type of the inputs.</typeparam>
        /// <returns>The minimum of the two inputs.</returns>
        public static T Min<T>(T x, T y)
            where T : IComparable<T> => x.CompareTo(y) <= 0 ? x : y;

        /// <summary>
        /// Returns the maximum of the two inputs.
        /// </summary>
        /// <param name="x">First input.</param>
        /// <param name="y">Second input.</param>
        /// <typeparam name="T">Type of the inputs.</typeparam>
        /// <returns>The maximum of the two inputs.</returns>
        public static T Max<T>(T x, T y)
            where T : IComparable<T> => x.CompareTo(y) >= 0 ? x : y;
    }
}