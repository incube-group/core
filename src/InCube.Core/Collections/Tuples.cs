using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Extension methods for <see cref="ValueTuple{T1}" />s, <see cref="Tuple{T1}" />s, and
    /// <see cref="KeyValuePair{TKey,TValue}" />s.
    /// </summary>
    [PublicAPI]
    public static class Tuples
    {
        /// <summary>
        /// Applies selector to each element in the tuple.
        /// </summary>
        /// <param name="self">Source tuple.</param>
        /// <param name="functor">Function to apply.</param>
        /// <typeparam name="T1">Input type.</typeparam>
        /// <typeparam name="T2">Output type.</typeparam>
        /// <returns>The transformed tuple.</returns>
        public static (T2 OutputItem1, T2 OutputItem2) Select<T1, T2>(this (T1 InputItem1, T1 InputItem2) self, Func<T1, T2> functor) => (functor(self.InputItem1), functor(self.InputItem2));

        /// <summary>
        /// Applies selector to each element in the tuple.
        /// </summary>
        /// <param name="self">Source tuple.</param>
        /// <param name="functor">Function to apply.</param>
        /// <typeparam name="T1">Input type.</typeparam>
        /// <typeparam name="T2">Output type.</typeparam>
        /// <returns>The transformed tuple.</returns>
        public static (T2 OutputItem1, T2 OutputItem2, T2 OutputItem3) Select<T1, T2>(this (T1 InputItem1, T1 InputItem2, T1 InputItem3) self, Func<T1, T2> functor) =>
            (functor(self.InputItem1), functor(self.InputItem2), functor(self.InputItem3));

        /// <summary>
        /// Creates a <see cref="KeyValuePair{TKey,TValue}" /> out of a <paramref name="key" /> and a <paramref name="value" />.
        /// </summary>
        /// <param name="key">The key of the <see cref="KeyValuePair{TKey,TValue}" />.</param>
        /// <param name="value">The value of the <see cref="KeyValuePair{TKey,TValue}" />.</param>
        /// <typeparam name="TK">The type of the <paramref name="key" />.</typeparam>
        /// <typeparam name="TV">The type of the <paramref name="value" />.</typeparam>
        /// <returns>A <see cref="KeyValuePair{TKey,TValue}" />.</returns>
        public static KeyValuePair<TK, TV> MakePair<TK, TV>(TK key, TV value) => new(key, value);

        /// <summary>
        /// Creates a <see cref="Tuple" /> ouf of two items.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <typeparam name="T1">The type of the <paramref name="item1" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="item2" />.</typeparam>
        /// <returns>A <see cref="Tuple" />.</returns>
        public static Tuple<T1, T2> MakeTuple<T1, T2>(T1 item1, T2 item2) => new(item1, item2);

        /// <summary>
        /// Creats a <see cref="ValueTuple" /> out of two items.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <typeparam name="T1">The type of the <paramref name="item1" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="item2" />.</typeparam>
        /// <returns>A <see cref="ValueTuple" />.</returns>
        public static (T1 OutputItem1, T2 OutputItem2) MakeValueTuple<T1, T2>(T1 item1, T2 item2) => (item1, item2);

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s out of two <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="left">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="right">The second <see cref="IEnumerable{T}" />.</param>
        /// <typeparam name="T1">The type of the <paramref name="left" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="right" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s.</returns>
        public static IEnumerable<(T1 OutputItem1, T2 OutputItem2)> ZipAsTuple<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) => left.Zip(right, MakeValueTuple);

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s out of three <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" />.</param>
        /// <param name="e3">The third <see cref="IEnumerable{T}" />.</param>
        /// <typeparam name="T1">The type of the <paramref name="e1" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="e2" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T3">The type of the <paramref name="e3" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s.</returns>
        public static IEnumerable<(T1 OutputItem1, T2 OutputItem2, T3 OutputItem3)> ZipAsTuple<T1, T2, T3>(this IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3) =>
            e1.ZipAsTuple(e2).Zip(e3, (x, y) => (x.OutputItem1, x.OutputItem2, y));

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s out of four <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" />.</param>
        /// <param name="e3">The third <see cref="IEnumerable{T}" />.</param>
        /// <param name="e4">The fourth <see cref="IEnumerable{T}" />.</param>
        /// <typeparam name="T1">The type of the <paramref name="e1" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="e2" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T3">The type of the <paramref name="e3" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T4">The type of the <paramref name="e4" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s.</returns>
        public static IEnumerable<(T1 OutputItem1, T2 OutputItem2, T3 OutputItem3, T4 OutputItem4)> ZipAsTuple<T1, T2, T3, T4>(this IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4) =>
            e1.ZipAsTuple(e2, e3).Zip(e4, (x, y) => (x.OutputItem1, x.OutputItem2, x.OutputItem3, y));

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s out of five <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" />.</param>
        /// <param name="e3">The third <see cref="IEnumerable{T}" />.</param>
        /// <param name="e4">The fourth <see cref="IEnumerable{T}" />.</param>
        /// <param name="e5">The fifth <see cref="IEnumerable{T}" />.</param>
        /// <typeparam name="T1">The type of the <paramref name="e1" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">The type of the <paramref name="e2" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T3">The type of the <paramref name="e3" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T4">The type of the <paramref name="e4" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T5">The type of the <paramref name="e5" /> <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s.</returns>
        public static IEnumerable<(T1 OutputItem1, T2 OutputItem2, T3 OutputItem3, T4 OutputItem4, T5 OutputItem5)> ZipAsTuple<T1, T2, T3, T4, T5>(
            this IEnumerable<T1> e1,
            IEnumerable<T2> e2,
            IEnumerable<T3> e3,
            IEnumerable<T4> e4,
            IEnumerable<T5> e5) =>
            e1.ZipAsTuple(e2, e3, e4).Zip(e5, (x, y) => (x.OutputItem1, x.OutputItem2, x.OutputItem3, x.OutputItem4, y));

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s out of an <see cref="IEnumerable{T}" /> by
        /// <see cref="Enumerable.Zip{TFirst,TSecond}" />ing each element with its index.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}" />.</param>
        /// <typeparam name="T">The type of the <paramref name="enumerable" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ValueTuple" />s.</returns>
        public static IEnumerable<(T Value, int Index)> ZipWithIndex<T>(this IEnumerable<T> enumerable) => enumerable.Select(MakeValueTuple);

        /// <summary>
        /// Version of
        /// <see
        ///     cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,int,TResult})" />
        /// allowing automatic deconstruction of input <see cref="ValueTuple" />s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}" />.</param>
        /// <param name="selector">The function to apply to each tuple in <paramref name="enumerable" />.</param>
        /// <typeparam name="TK">The type of the first element of each input <see cref="ValueTuple" />.</typeparam>
        /// <typeparam name="TV">The type of the second element of each input <see cref="ValueTuple{T1}" />.</typeparam>
        /// <typeparam name="TU">The output type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TU" />s.</returns>
        public static IEnumerable<TU> TupleSelect<TK, TV, TU>(this IEnumerable<(TK Key, TV Value)> enumerable, Func<TK, TV, TU> selector) => enumerable.Select(kv => selector(kv.Key, kv.Value));

        /// <summary>
        /// Version of
        /// <see
        ///     cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,int,TResult})" />
        /// allowing automatic deconstruction of input <see cref="KeyValuePair{TKey,TValue}" />s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}" />.</param>
        /// <param name="selector">
        /// The function to apply to each <see cref="KeyValuePair{TKey,TValue}" /> in
        /// <paramref name="enumerable" />.
        /// </param>
        /// <typeparam name="TK">The type of the first element of each input <see cref="KeyValuePair{TKey,TValue}" />.</typeparam>
        /// <typeparam name="TV">The type of the second element of each input <see cref="KeyValuePair{TKey,TValue}" />.</typeparam>
        /// <typeparam name="TU">The output type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TU" />s.</returns>
        public static IEnumerable<TU> TupleSelect<TK, TV, TU>(this IEnumerable<KeyValuePair<TK, TV>> enumerable, Func<TK, TV, TU> selector) => enumerable.Select(kv => selector(kv.Key, kv.Value));

        /// <summary>
        /// Version of <see cref="Enumerable.Zip{TFirst,TSecond}" /> with index.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" /> to zip.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" /> to zip.</param>
        /// <param name="selector">The function to apply to each pair.</param>
        /// <typeparam name="T1">Type of the first <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">Type of the second <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="TOut">Output type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TOut" />s.</returns>
        public static IEnumerable<TOut> ZipI<T1, T2, TOut>(this IEnumerable<T1> e1, IEnumerable<T2> e2, Func<T1, T2, int, TOut> selector) => e1.ZipAsTuple(e2).Select((x, i) => selector(x.OutputItem1, x.OutputItem2, i));

        /// <summary>
        /// Version of <see cref="Enumerable.Zip{TFirst,TSecond}" /> allowing three input <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" />.</param>
        /// <param name="e3">The third <see cref="IEnumerable{T}" />.</param>
        /// <param name="selector">The function to apply to each triple.</param>
        /// <typeparam name="T1">Type of the first <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">Type of the second <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T3">Type of the third <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="TOut">The outpyt type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TOut" />.</returns>
        public static IEnumerable<TOut> Zip3<T1, T2, T3, TOut>(this IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, Func<T1, T2, T3, TOut> selector) =>
            e1.ZipAsTuple(e2).Zip(e3, (x, y) => selector(x.OutputItem1, x.OutputItem2, y));

        /// <summary>
        /// Version of <see cref="Enumerable.Zip{TFirst,TSecond}" /> allowing four input <see cref="IEnumerable{T}" />s.
        /// </summary>
        /// <param name="e1">The first <see cref="IEnumerable{T}" />.</param>
        /// <param name="e2">The second <see cref="IEnumerable{T}" />.</param>
        /// <param name="e3">The third <see cref="IEnumerable{T}" />.</param>
        /// <param name="e4">The fourth <see cref="IEnumerable{T}" />.</param>
        /// <param name="selector">The function to apply to each triple.</param>
        /// <typeparam name="T1">Type of the first <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T2">Type of the second <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T3">Type of the third <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="T4">Type of the fourth <see cref="IEnumerable{T}" />.</typeparam>
        /// <typeparam name="TOut">The outpyt type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TOut" />.</returns>
        public static IEnumerable<TOut> Zip4<T1, T2, T3, T4, TOut>(this IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4, Func<T1, T2, T3, T4, TOut> selector) =>
            e1.ZipAsTuple(e2).Zip3(e3, e4, (x, y, z) => selector(x.OutputItem1, x.OutputItem2, y, z));

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
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2) Unzip<T, T1, T2>(this IEnumerable<T> zipped, Func<T, T1> selector1, Func<T, T2> selector2) =>
            zipped is IReadOnlyCollection<T> col ? col.Unzip(selector1, selector2) : zipped.ToList().Unzip(selector1, selector2);

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <param name="zipped">The input enumerable.</param>
        /// <param name="selector1">The selector for the first enumerable to return (Item1 in the tuple).</param>
        /// <param name="selector2">The selector for the second enumerable to return (Item2 in the tuple).</param>
        /// <returns> A tuple containing the two enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2) Unzip<T, T1, T2>(this IReadOnlyCollection<T> zipped, Func<T, T1> selector1, Func<T, T2> selector2) =>
            (zipped.Select(selector1), zipped.Select(selector2));

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
        /// <returns> A tuple containing the three enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2, IEnumerable<T3> EnumerableItem3)
            Unzip<T, T1, T2, T3>(this IEnumerable<T> zipped, Func<T, T1> selector1, Func<T, T2> selector2, Func<T, T3> selector3) =>
            zipped is IReadOnlyCollection<T> col ? col.Unzip(selector1, selector2, selector3) : zipped.ToList().Unzip(selector1, selector2, selector3);

        /// <summary>
        /// Selects three enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <typeparam name="T3">The type of the 3rd output enumerable.</typeparam>
        /// <param name="zipped">The input <see cref="IReadOnlyCollection{T}" />.</param>
        /// <param name="selector1">The selector for the first enumerable to return (Item1 in the tuple).</param>
        /// <param name="selector2">The selector for the second enumerable to return (Item2 in the tuple).</param>
        /// <param name="selector3">The selector for the third enumerable to return (Item3 in the tuple).</param>
        /// <returns> A tuple containing the three enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2, IEnumerable<T3> EnumerableItem3) Unzip<T, T1, T2, T3>(
            this IReadOnlyCollection<T> zipped,
            Func<T, T1> selector1,
            Func<T, T2> selector2,
            Func<T, T3> selector3) =>
            (zipped.Select(selector1), zipped.Select(selector2), zipped.Select(selector3));

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <param name="zipped">The input enumerable.</param>
        /// <returns> A tuple containing the two enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2) Unzip<T1, T2>(this IEnumerable<(T1 InputItem1, T2 InputItem2)> zipped) => zipped.Unzip(t => t.InputItem1, t => t.InputItem2);

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <param name="zipped">The input enumerable.</param>
        /// <returns> A tuple containing the two enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2) Unzip<T1, T2>(this IReadOnlyCollection<(T1 InputItem1, T2 InputItem2)> zipped) => zipped.Unzip(t => t.InputItem1, t => t.InputItem2);

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <typeparam name="T3">The type of the 3rd output enumerable.</typeparam>
        /// <param name="zipped">The input <see cref="IEnumerable{T}"/>.</param>
        /// <returns> A tuple containing the three enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2, IEnumerable<T3> EnumerableItem3) Unzip<T1, T2, T3>(this IEnumerable<(T1 InputItem1, T2 InputItem2, T3 InputItem3)> zipped) =>
            zipped.Unzip(t => t.InputItem1, t => t.InputItem2, t => t.InputItem3);

        /// <summary>
        /// Selects two enumerables while iterating over <paramref name="zipped" /> only once.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st output enumerable.</typeparam>
        /// <typeparam name="T2">The type of the 2nd output enumerable.</typeparam>
        /// <typeparam name="T3">The type of the 3rd output enumerable.</typeparam>
        /// <param name="zipped">The input <see cref="IReadOnlyCollection{T}"/>.</param>
        /// <returns> A tuple containing the three enumerables extracted.</returns>
        /// <remarks>This method is very expensive since all elements need to be cached in temporary lists.</remarks>
        public static (IEnumerable<T1> EnumerableItem1, IEnumerable<T2> EnumerableItem2, IEnumerable<T3> EnumerableItem3) Unzip<T1, T2, T3>(this IReadOnlyCollection<(T1 InputItem1, T2 InputItem2, T3 InputItem3)> zipped) =>
            zipped.Unzip(t => t.InputItem1, t => t.InputItem2, t => t.InputItem3);

        /// <summary>
        /// Applies <paramref name="selector"/> on the values of the input <see cref="KeyValuePair{TKey,TValue}"/>s.
        /// </summary>
        /// <param name="enumerable">The input <see cref="IEnumerable{T}"/> to select from.</param>
        /// <param name="selector">The function to apply to each value.</param>
        /// <typeparam name="TK">Key type of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TU">Value type of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TV">Output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.</returns>
        public static IEnumerable<KeyValuePair<TK, TV>> MapValues<TK, TU, TV>(this IEnumerable<KeyValuePair<TK, TU>> enumerable, Func<TU, TV> selector) => enumerable.Select(keyValue => MakePair(keyValue.Key, selector(keyValue.Value)));

        /// <summary>
        /// Applies <paramref name="selector"/> on the values of the input <see cref="KeyValuePair{TKey,TValue}"/>s, but can depend on the key.
        /// </summary>
        /// <param name="enumerable">The input <see cref="IEnumerable{T}"/> to select from.</param>
        /// <param name="selector">The function to apply to each value.</param>
        /// <typeparam name="TK">Key type of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TU">Value type of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TV">Output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.</returns>
        public static IEnumerable<KeyValuePair<TK, TV>> MapValues<TK, TU, TV>(this IEnumerable<KeyValuePair<TK, TU>> enumerable, Func<TK, TU, TV> selector) =>
            enumerable.Select(keyValue => MakePair(keyValue.Key, selector(keyValue.Key, keyValue.Value)));

        /// <summary>
        /// Applies <paramref name="selector"/> on the second elements of the input <see cref="ValueTuple{TKey,TValue}"/>s, but can depend on the first element.
        /// </summary>
        /// <param name="enumerable">The input <see cref="IEnumerable{T}"/> to select from.</param>
        /// <param name="selector">The function to apply to each value.</param>
        /// <typeparam name="T1">First type of the <see cref="ValueTuple{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TU">Second type of the <see cref="ValueTuple{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="T2">Output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValueTuple{TKey,TValue}"/>s.</returns>
        public static IEnumerable<(T1 Key, T2 Value)> MapValues<T1, TU, T2>(this IEnumerable<(T1 Key, TU Value)> enumerable, Func<TU, T2> selector) => enumerable.Select(kv => (kv.Key, selector(kv.Value)));

        /// <summary>
        /// Applies <paramref name="selector"/> on the values of the input <see cref="ValueTuple{TKey,TValue}"/>s, but can depend on the key.
        /// </summary>
        /// <param name="enumerable">The input <see cref="IEnumerable{T}"/> to select from.</param>
        /// <param name="selector">The function to apply to each value.</param>
        /// <typeparam name="T1">First type of the <see cref="ValueTuple{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TU">Second type of the <see cref="ValueTuple{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="T2">Output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValueTuple{TKey,TValue}"/>s.</returns>
        public static IEnumerable<(T1 Key, T2 Value)> MapValues<T1, TU, T2>(this IEnumerable<(T1 Key, TU Value)> enumerable, Func<T1, TU, T2> selector) => enumerable.Select(kv => (kv.Key, selector(kv.Key, kv.Value)));

        /// <summary>
        /// Applies <paramref name="selector"/> on the values of the input <see cref="IGrouping{TKey,TElement}"/>s.
        /// </summary>
        /// <param name="enumerable">The input <see cref="IEnumerable{T}"/> to select from.</param>
        /// <param name="selector">The function to apply to each value.</param>
        /// <typeparam name="TK">Key type of the <see cref="IGrouping{TKey,TElement}"/>s.</typeparam>
        /// <typeparam name="TU">Output type of the <paramref name="selector"/>.</typeparam>
        /// <typeparam name="TV">Value type of the <see cref="IGrouping{TKey,TElement}"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValueTuple{TKey,TValue}"/>s.</returns>
        public static IEnumerable<(TK Key, TV Value)> MapValues<TK, TU, TV>(this IEnumerable<IGrouping<TK, TU>> enumerable, Func<IEnumerable<TU>, TV> selector) => enumerable.Select(keyValue => (keyValue.Key, selector(keyValue)));

        /// <summary>
        /// Selects the values ouf of an <see cref="IEnumerable{T}"/> of <see cref="ValueTuple"/>s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/>.</param>
        /// <typeparam name="T1">The type of the first element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <typeparam name="T2">The type of the second element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="T2"/>.</returns>
        public static IEnumerable<T2> Values<T1, T2>(this IEnumerable<(T1 Key, T2 Value)> enumerable) => enumerable.TupleSelect((_, value) => value);

        /// <summary>
        /// Selects the values ouf of an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/>.</param>
        /// <typeparam name="TK">The type of the key element of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <typeparam name="TV">The type of the value element of the <see cref="KeyValuePair{TKey,TValue}"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TV"/>.</returns>
        public static IEnumerable<TV> Values<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> enumerable) => enumerable.TupleSelect((_, value) => value);

        /// <summary>
        /// Selects the keys out of an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/>.</param>
        /// <typeparam name="TK">The type of the key element of the <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TV">The type of the value element of the <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TK"/>.</returns>
        public static IEnumerable<TK> Keys<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> enumerable) => enumerable.TupleSelect((key, _) => key);

        /// <summary>
        /// Selects the first elements out of an <see cref="IEnumerable{T}"/> of <see cref="ValueTuple"/>s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/>.</param>
        /// <typeparam name="T1">The type of the first element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <typeparam name="T2">The type of the second element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="T1"/>.</returns>
        public static IEnumerable<T1> Keys<T1, T2>(this IEnumerable<(T1 Key, T2 Value)> enumerable) => enumerable.TupleSelect((key, _) => key);

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s to an <see cref="IEnumerable{T}"/> of <see cref="ValueTuple"/>.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.</param>
        /// <typeparam name="TK">The type of the key element of the <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TV">The type of the value element of the <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValueTuple"/>s.</returns>
        public static IEnumerable<(TK KeyItem, TV ValueItem)> AsTuple<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> enumerable) => enumerable.TupleSelect((key, value) => (key, value));

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="ValueTuple"/>s to an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.
        /// </summary>
        /// <param name="enumerable">The source <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.</param>
        /// <typeparam name="T1">The type of the first element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <typeparam name="T2">The type of the second element of the <see cref="ValueTuple"/>s.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s.</returns>
        public static IEnumerable<KeyValuePair<T1, T2>> AsKeyValuePair<T1, T2>(this IEnumerable<(T1 Key, T2 Value)> enumerable) => enumerable.TupleSelect(MakePair);

        /// <summary>
        /// Convenience method to filter some <paramref name="pairs"/> on the nullability of the first element.
        /// </summary>
        /// <param name="pairs">The pairs to filter.</param>
        /// <typeparam name="T1">Type of the first element.</typeparam>
        /// <typeparam name="T2">Type of the second element.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of pairs.</returns>
        public static IEnumerable<(T1 First, T2 Second)> WhereFirstNotNull<T1, T2>(this IEnumerable<(T1? First, T2 Second)> pairs) => pairs.Where(pair => pair.First != null)!;

        /// <summary>
        /// Convenience method to filter some <paramref name="pairs"/> on the nullability of the second element.
        /// </summary>
        /// <param name="pairs">The pairs to filter.</param>
        /// <typeparam name="T1">Type of the first element.</typeparam>
        /// <typeparam name="T2">Type of the second element.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of pairs.</returns>
        public static IEnumerable<(T1 First, T2 Second)> WhereSecondNotNull<T1, T2>(this IEnumerable<(T1 First, T2? Second)> pairs) => pairs.Where(pair => pair.Second != null)!;
    }
}