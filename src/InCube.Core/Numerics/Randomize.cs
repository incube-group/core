using System;
using System.Collections.Generic;
using InCube.Core.Functional;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Extension methods for randomization.
    /// </summary>
    public static class Randomize
    {
        /// <summary>
        /// Shuffles a <see cref="IList{T}"/> in place.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle.</param>
        /// <param name="rngOpt">The random source.</param>
        /// <typeparam name="T">The type of the <see cref="IList{T}"/>.</typeparam>
        /// <returns>The shuffled list.</returns>
        public static IList<T> Shuffle<T>(this IList<T> list, Option<Random> rngOpt = default)
        {
            var rng = rngOpt.GetValueOr(() => new Random());
            for (var n = list.Count - 1; n > 0; --n)
            {
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
