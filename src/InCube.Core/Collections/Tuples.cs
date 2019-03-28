using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Extension methods for <see cref="ValueTuple{T1}"/>s, <see cref="Tuple{T1}"/>s, and <see cref="KeyValuePair{TKey,TValue}"/>s.
    /// </summary>
    public static class Tuples
    {
        [PublicAPI]
        public static (T2, T2) Select<T1, T2>(this (T1, T1) self, Func<T1, T2> functor) =>
            (functor(self.Item1), functor(self.Item2));

        [PublicAPI]
        public static (T2, T2, T2) Select<T1, T2>(this (T1, T1, T1) self, Func<T1, T2> functor) =>
            (functor(self.Item1), functor(self.Item2), functor(self.Item3));

        [PublicAPI]
        public static KeyValuePair<TK, TV> MakePair<TK, TV>(TK key, TV value) =>
            new KeyValuePair<TK, TV>(key, value);

        [PublicAPI]
        public static Tuple<T1, T2> MakeTuple<T1, T2>(T1 item1, T2 item2)
            => new Tuple<T1, T2>(item1, item2);

        [PublicAPI]
        public static (T1, T2) MakeValueTuple<T1, T2>(T1 item1, T2 item2)
            => (item1, item2);

        [PublicAPI]
        public static IEnumerable<(T1, T2)> ZipAsTuple<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
        {
            return left.Zip(right, MakeValueTuple);
        }

        [PublicAPI]
        public static IEnumerable<(T1, T2, T3)> ZipAsTuple<T1, T2, T3>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3)
        {
            return e1.ZipAsTuple(e2).Zip(e3, (x, y) => (x.Item1, x.Item2, y));
        }

        [PublicAPI]
        public static IEnumerable<(T1, T2, T3, T4)> ZipAsTuple<T1, T2, T3, T4>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4)
        {
            return e1.ZipAsTuple(e2, e3).Zip(e4, (x, y) => (x.Item1, x.Item2, x.Item3, y));
        }

        [PublicAPI]
        public static IEnumerable<(T1, T2, T3, T4, T5)> ZipAsTuple<T1, T2, T3, T4, T5>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4,
            IEnumerable<T5> e5)
        {
            return e1.ZipAsTuple(e2, e3, e4).Zip(e5, (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));
        }

        [PublicAPI]
        public static IEnumerable<(T value, int index)> ZipWithIndex<T>(this IEnumerable<T> enumerable) =>
            enumerable.Select(MakeValueTuple);

        [PublicAPI]
        public static IEnumerable<TU> TupleSelect<TK, TV, TU>(this IEnumerable<(TK Key, TV Value)> enumerable,
            Func<TK, TV, TU> mapper) =>
            enumerable.Select(kv => mapper(kv.Key, kv.Value));

        [PublicAPI]
        public static IEnumerable<TU> TupleSelect<TK, TV, TU>(this IEnumerable<KeyValuePair<TK, TV>> enumerable,
            Func<TK, TV, TU> mapper) =>
            enumerable.Select(kv => mapper(kv.Key, kv.Value));

        [PublicAPI]
        public static IEnumerable<TOut> ZipI<T1, T2, TOut>(
            this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            Func<T1, T2, int, TOut> mapper) =>
            e1.ZipAsTuple(e2).Select((x, i) => mapper(x.Item1, x.Item2, i));

        [PublicAPI]
        public static IEnumerable<TOut> Zip3<T1, T2, T3, TOut>(
            this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            Func<T1, T2, T3, TOut> mapper) =>
            e1.ZipAsTuple(e2).Zip(e3, (x, y) => mapper(x.Item1, x.Item2, y));

        [PublicAPI]
        public static IEnumerable<TOut> Zip4<T1, T2, T3, T4, TOut>(
            this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4,
            Func<T1, T2, T3, T4, TOut> mapper) =>
            e1.ZipAsTuple(e2).Zip3(e3, e4, (x, y, z) => mapper(x.Item1, x.Item2, y, z));

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
        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T, T1, T2>(this IEnumerable<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2) =>
            zipped is IReadOnlyCollection<T> col ? col.Unzip(selector1, selector2) : zipped.ToList().Unzip(selector1, selector2);

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T, T1, T2>(this IReadOnlyCollection<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2) => (zipped.Select(selector1), zipped.Select(selector2));

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
            Func<T, T3> selector3) =>
            zipped is IReadOnlyCollection<T> col
                ? col.Unzip(selector1, selector2, selector3)
                : zipped.ToList().Unzip(selector1, selector2, selector3);

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T, T1, T2, T3>(
            this IReadOnlyCollection<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2,
            Func<T, T3> selector3) =>
            (zipped.Select(selector1), zipped.Select(selector2), zipped.Select(selector3));

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T1, T2>(this IEnumerable<(T1, T2)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2);

        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T1, T2>(this IReadOnlyCollection<(T1, T2)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2);

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2, t => t.Item3);

        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip<T1, T2, T3>(this IReadOnlyCollection<(T1, T2, T3)> zipped) =>
            zipped.Unzip(t => t.Item1, t => t.Item2, t => t.Item3);

        [PublicAPI]
        public static IEnumerable<KeyValuePair<T, TV>> MapValues<T, TU, TV>(
            this IEnumerable<KeyValuePair<T, TU>> enumerable, Func<TU, TV> mapper) =>
            enumerable.Select(keyValue => MakePair(keyValue.Key, mapper(keyValue.Value)));

        [PublicAPI]
        public static IEnumerable<KeyValuePair<T, TV>> MapValues<T, TU, TV>(
            this IEnumerable<KeyValuePair<T, TU>> enumerable, Func<T, TU, TV> mapper) =>
            enumerable.Select(keyValue => MakePair(keyValue.Key, mapper(keyValue.Key, keyValue.Value)));

        [PublicAPI]
        public static IEnumerable<(T Key, TV Value)> MapValues<T, TU, TV>(this IEnumerable<(T Key, TU Value)> enumerable,
            Func<TU, TV> mapper) =>
            enumerable.Select(kv => (kv.Key, mapper(kv.Value)));

        [PublicAPI]
        public static IEnumerable<(T Key, TV Value)> MapValues<T, TU, TV>(this IEnumerable<(T Key, TU Value)> enumerable,
            Func<T, TU, TV> mapper) =>
            enumerable.Select(kv => (kv.Key, mapper(kv.Key, kv.Value)));

        [PublicAPI]
        public static IEnumerable<(T Key, TV Value)> MapValues<T, TU, TV>(
            this IEnumerable<IGrouping<T, TU>> enumerable, Func<IEnumerable<TU>, TV> mapper) =>
            enumerable.Select(keyValue => (keyValue.Key, mapper(keyValue)));

        [PublicAPI]
        public static IEnumerable<TV> Values<T, TV>(
            this IEnumerable<(T Key, TV Value)> enumerable) =>
            enumerable.TupleSelect((_, value) => value);

        [PublicAPI]
        public static IEnumerable<TV> Values<T, TV>(
            this IEnumerable<KeyValuePair<T, TV>> enumerable) =>
            enumerable.TupleSelect((_, value) => value);

        [PublicAPI]
        public static IEnumerable<T> Keys<T, TV>(
            this IEnumerable<KeyValuePair<T, TV>> enumerable) =>
            enumerable.TupleSelect((key, _) => key);

        [PublicAPI]
        public static IEnumerable<T> Keys<T, TV>(
            this IEnumerable<(T Key, TV Value)> enumerable) =>
            enumerable.TupleSelect((key, _) => key);

        [PublicAPI]
        public static IEnumerable<(T, TV)> AsTuple<T, TV>(this IEnumerable<KeyValuePair<T, TV>> enumerable)
        {
            return enumerable.TupleSelect((key, value) => (key, value));
        }

        [PublicAPI]
        public static IEnumerable<KeyValuePair<T, TV>> AsKeyValuePair<T, TV>(this IEnumerable<(T Key, TV Value)> enumerable)
        {
            return enumerable.TupleSelect(MakePair);
        }
    }
}
