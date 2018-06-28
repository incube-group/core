using System;
using System.Collections.Generic;
using System.Linq;

namespace InCube.Core
{
    public static class EnumerableExtensionMethods
    {
        /// <summary>
        /// Returns a value indicating whether the enumerable contains any element for which the predicate is false, or is empty.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <param name="enumerable">The source enumerable.</param>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <returns>True, if none of the elements fulfills the predicate, false otherwise.</returns>
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return !enumerable.Any(predicate);
        }

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <param name="zipped">The input enumerable.</param>
        /// <param name="selector1">The selector for the first enumerable to return.</param>
        /// <param name="selector2">The selector for the second enumerable to return.</param>
        /// <returns>A tuple containing the two enumerables extracted.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> Unzip<T, T1, T2>(this IEnumerable<T> zipped, Func<T, T1> selector1, Func<T, T2> selector2)
        {
            if (!zipped.Any())
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(Enumerable.Empty<T1>(), Enumerable.Empty<T2>());
            }
            else
            {
                var l1 = new List<T1>();
                var l2 = new List<T2>();
                foreach (var t in zipped)
                {
                    l1.Add(selector1(t));
                    l2.Add(selector2(t));
                }

                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(l1, l2);
            }
        }

        /// <summary>
        /// Selects three enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <typeparam name="T3">The type of the 3rd output enumerable.</typeparam>
        /// <param name="zipped">The input enumerable.</param>
        /// <param name="selector1">The selector for the first enumerable to return (Item1 in the tuple).</param>
        /// <param name="selector2">The selector for the second enumerable to return (Item2 in the tuple).</param>
        /// <param name="selector3">The selector for the third enumerable to return (Item3 in the tuple).</param>
        /// <returns> A tuple containing the three enumerables extracted. </returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> Unzip<T, T1, T2, T3>(
            this IEnumerable<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2,
            Func<T, T3> selector3)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();
            var l3 = new List<T3>();
            foreach (var t in zipped)
            {
                l1.Add(selector1(t));
                l2.Add(selector2(t));
                l3.Add(selector3(t));
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(l1, l2, l3);
        }

        public static IEnumerable<T> ToEnumerable<T>(T t)
        {
            yield return t;
        }
    }
}
