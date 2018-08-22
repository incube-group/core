using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Functional;

namespace InCube.Core.Collections
{
    public static class Enumerables
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
        /// 
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T, T1, T2>(this IEnumerable<T> zipped, Func<T, T1> selector1, Func<T, T2> selector2)
        {
            if (zipped is IReadOnlyCollection<T> col)
            {
                return col.Unzip(selector1, selector2);
            }

            return zipped.ToList().Unzip(selector1, selector2);
        }

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T, T1, T2>(this IReadOnlyCollection<T> zipped,
            Func<T, T1> selector1, Func<T, T2> selector2) => (zipped.Select(selector1), zipped.Select(selector2));

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
        /// 
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T, T1, T2, T3>(
            this IEnumerable<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2,
            Func<T, T3> selector3)
        {
            if (zipped is IReadOnlyCollection<T> col)
            {
                return col.Unzip(selector1, selector2, selector3);
            }

            return zipped.ToList().Unzip(selector1, selector2, selector3);
        }

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T, T1, T2, T3>(this IReadOnlyCollection<T> zipped,
            Func<T, T1> selector1, Func<T, T2> selector2, Func<T, T3> selector3) => 
            (zipped.Select(selector1), zipped.Select(selector2), zipped.Select(selector3));

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T1, T2>(this IEnumerable<(T1, T2)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2);

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T1, T2>(this IReadOnlyCollection<(T1, T2)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2);

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2, t => t.Item3);

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T1, T2, T3>(this IReadOnlyCollection<(T1, T2, T3)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2, t => t.Item3);

        public static IEnumerable<T> ToEnumerable<T>(T t)
        {
            yield return t;
        }

        public static EnumerableCollection<T> ToCollection<T>(this IEnumerable<T> enumerable, int count) =>
            new EnumerableCollection<T>(enumerable, count);

        public static IEnumerable<T> Iterate<T>(T start, Func<T, T> f)
        {
            T next = start;
            yield return next;
            while (true)
            {
                next = f(next);
                yield return next;
            }
        }

        public static IEnumerable<T> Repeat<T>(T value)
        {
            while (true)
            {
                yield return value;
            }
        }

        public static IEnumerable<T> Generate<T>(Func<T> generator)
        {
            while (true)
            {
                yield return generator();
            }
        }

        public static T[] Enumerate<T>(params T[] elems) => elems;

        public static IEnumerable<int> IntRange(int startInclusive, int stopExclusive) =>
            Enumerable.Range(startInclusive, stopExclusive - startInclusive);

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> elems, int startInclusive, int stopExclusive)
        {
            switch (elems)
            {
                case IReadOnlyList<T> list:
                    return list.Slice(startInclusive, stopExclusive);
                default:
                    return elems.Skip(startInclusive).Take(stopExclusive - startInclusive);
            }
        }

        public static IEnumerable<T> Slice<T>(this IReadOnlyList<T> list, int startInclusive, int stopExclusive) =>
            IntRange(startInclusive, stopExclusive).Select(i => list[i]);

        public static ArraySegment<T> Slice<T>(this T[] elems, int startInclusive, int stopExclusive) =>
            new ArraySegment<T>(elems, startInclusive, stopExclusive - startInclusive);

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> elems, int startInclusive, int stopExclusive) =>
            new ArraySegment<T>(elems.Array, elems.Offset + startInclusive, stopExclusive - startInclusive);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(list => list);

        public static bool IsEmpty<T>(this IEnumerable<T> col) => !col.Any();

        public static IEnumerable<T> GenFilter<T, U>(this IEnumerable<T> list, Func<U, bool> predicate) where T : U
        {
            foreach (var l in list)
            {
                if (predicate.Invoke(l)) yield return l;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var l in list)
            {
                action.Invoke(l);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int index = 0;
            foreach (var l in list)
            {
                action.Invoke(l, index);
                index++;
            }
        }

        public static Option<T> FirstOption<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                return enumerator.MoveNext() ? Options.Some(enumerator.Current) : Options.None;
            }
        }

        public static bool IsSorted<T>(this IEnumerable<T> self, Comparer<T> comparer = null, bool strict = false)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var outOfOrder = strict ? (Func<T, T, bool>)
                ((x, y) => comparer.Compare(x, y) >= 0) : 
                ((x, y) => comparer.Compare(x, y) >  0);
            using (var enumerator = self.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return true;
                var current = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    var next = enumerator.Current;
                    if (outOfOrder(current, next)) return false;
                    current = next;
                }

                return true;
            }
        }

        /// <summary>
        /// Produces the union (duplicates removed) of the provided enumerables.
        /// Items are considered equal when their keySelector return equal values (or objects which are equal).
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <typeparam name="TKey">The type of the comparison value.</typeparam>
        /// <param name="self">This enumerable.</param>
        /// <param name="other">The enumerable to create the union with.</param>
        /// <param name="keySelector">A function projecting each item to a value used for determining whether an item is present in both sets.</param>
        public static IEnumerable<T> Union<T, TKey>(this IEnumerable<T> self, IEnumerable<T> other, Func<T, TKey> keySelector)
        {
            return self.Concat(other).GroupBy(keySelector, x => x, (key, group) => group.First());
        }
    }
}
