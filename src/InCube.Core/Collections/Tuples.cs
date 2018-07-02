using System;
using System.Collections.Generic;
using System.Linq;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Extension methods for <see cref="ValueTuple{T1}"/>s, <see cref="Tuple{T1}"/>s, and <see cref="KeyValuePair{TKey,TValue}"/>s.
    /// </summary>
    public static class Tuples
    {
        public static (T2, T2) Select<T1, T2>(this (T1, T1) self, Func<T1, T2> functor) =>
            (functor(self.Item1), functor(self.Item2));

        public static (T2, T2, T2) Select<T1, T2>(this (T1, T1, T1) self, Func<T1, T2> functor) =>
            (functor(self.Item1), functor(self.Item2), functor(self.Item3));

        public static KeyValuePair<TK, TV> MakePair<TK, TV>(TK key, TV value) =>
            new KeyValuePair<TK, TV>(key, value);

        public static Tuple<T1, T2> MakeTuple<T1, T2>(T1 item1, T2 item2)
            => new Tuple<T1, T2>(item1, item2);

        public static (T1, T2) MakeValueTuple<T1, T2>(T1 item1, T2 item2)
            => (item1, item2);

        public static IEnumerable<(T1, T2)> ZipAsTuple<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
        {
            return left.Zip(right, MakeValueTuple);
        }

        public static IEnumerable<(T1, T2, T3)> ZipAsTuple<T1, T2, T3>(this IEnumerable<T1> e1, IEnumerable<T2> e2,
            IEnumerable<T3> e3)
        {
            return e1.ZipAsTuple(e2).Zip(e3, (x, y) => (x.Item1, x.Item2, y));
        }

        public static IEnumerable<(T1, T2, T3, T4)> ZipAsTuple<T1, T2, T3, T4>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4)
        {
            return e1.ZipAsTuple(e2, e3).Zip(e4, (x, y) => (x.Item1, x.Item2, x.Item3, y));
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> ZipAsTuple<T1, T2, T3, T4, T5>(this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4,
            IEnumerable<T5> e5)
        {
            return e1.ZipAsTuple(e2, e3, e4).Zip(e5, (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));
        }

        public static IEnumerable<U> TupleSelect<K, V, U>(this IEnumerable<(K Key, V Value)> enumerable,
            Func<K, V, U> mapper) =>
            enumerable.Select(kv => mapper(kv.Key, kv.Value));

        public static IEnumerable<U> TupleSelect<K, V, U>(this IEnumerable<KeyValuePair<K, V>> enumerable,
            Func<K, V, U> mapper) =>
            enumerable.Select(kv => mapper(kv.Key, kv.Value));

        public static IEnumerable<(T value, int index)> ZipWithIndex<T>(this IEnumerable<T> enumerable) =>
            enumerable.Select(MakeValueTuple);

        public static IEnumerable<KeyValuePair<T, V>> MapValues<T, U, V>(
            this IEnumerable<KeyValuePair<T, U>> enumerable, Func<U, V> mapper) =>
            enumerable.Select(keyValue => Tuples.MakePair(keyValue.Key, mapper(keyValue.Value)));

        public static IEnumerable<(T Key, V Value)> MapValues<T, U, V>(this IEnumerable<(T Key, U Value)> enumerable,
            Func<U, V> mapper) =>
            enumerable.Select(kv => (kv.Key, mapper(kv.Value)));

        public static IEnumerable<(T, V)> AsTuple<T, V>(this IEnumerable<KeyValuePair<T, V>> enumerable)
        {
            return enumerable.Select(kv => (kv.Key, kv.Value));
        }

    }
}
