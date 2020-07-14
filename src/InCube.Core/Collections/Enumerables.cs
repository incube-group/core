using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InCube.Core.Functional;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A collection of convenience extension methods for enumerables
    /// </summary>
    [PublicAPI]
    public static class Enumerables
    {
        /// <summary>
        /// Returns a value indicating whether the enumerable contains any element for which the predicate is false, or is empty.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <param name="enumerable">The source enumerable.</param>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <returns>True, if none of the elements fulfills the predicate, false otherwise.</returns>
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) => !enumerable.Any(predicate);

        /// <summary>
        /// Returns an enumerable containing the input as its only element
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="element">The single element to create the enumerable from</param>
        /// <returns>An enumerable</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            yield return element;
        }

        /// <summary>
        /// Creates an enumerable from an initial element and a generating function for the next element
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="start">The initial element</param>
        /// <param name="f">The generating function for the next element</param>
        /// <returns>An enumerable</returns>
        public static IEnumerable<T> Iterate<T>(T start, Func<T, T> f)
        {
            T next = start;
            yield return next;
            while (true)
            {
                next = f(next);
                yield return next;
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Creates an enumerable from a generator function
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="generator">The generator function</param>
        /// <returns>An enumerable</returns>
        public static IEnumerable<T> Generate<T>(Func<T> generator)
        {
            while (true)
            {
                yield return generator();
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Creates an array out of a collection of elements
        /// </summary>
        /// <typeparam name="T">The type of the array to create</typeparam>
        /// <param name="elems">The elements to create the array from</param>
        /// <returns>An array</returns>
        public static T[] Enumerate<T>(params T[] elems) => elems;

        /// <summary>
        /// Convenience method to create a range of integers
        /// </summary>
        /// <param name="startInclusive">First integer of the range, included</param>
        /// <param name="stopExclusive">Last integer of the range, excluded</param>
        /// <returns>An enumerable of integers</returns>
        public static IEnumerable<int> IntRange(int startInclusive, int stopExclusive) => Enumerable.Range(startInclusive, stopExclusive - startInclusive);

        /// <summary>
        /// Convenience method to create a range of integers starting with 0
        /// </summary>
        /// <param name="stopExclusive">Last integer of the range, excluded</param>
        /// <returns>An enumerable of integers</returns>
        public static IEnumerable<int> IntRange(int stopExclusive) => Enumerable.Range(0, stopExclusive);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", ").
        /// </summary>
        /// <typeparam name="T">The type of the enumerable to join as a string</typeparam>
        /// <param name="enumerable">The input enumerable to join as a string</param>
        /// <param name="separator">The separator so join the elements with</param>
        /// <returns>A string</returns>
        public static string MkString<T>(this IEnumerable<T> enumerable, string separator = ", ") => string.Join(separator, enumerable);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", "), wrapped by with a string in the beginning and the end.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="start">The string to place in front of the joined output.</param>
        /// <param name="separator">The separator to be placed between elements.</param>
        /// <param name="end">The string to place at the end of the joined output.</param>
        /// <returns>A string</returns>
        public static string MkString<T>(this IEnumerable<T> enumerable, string start, string separator, string end) => $"{start}{enumerable.MkString(separator)}{end}";

        /// <summary>
        /// Gets the maximum element of an enumerable by comparing elements based on a selector's comparable results
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable</typeparam>
        /// <typeparam name="TComparable">Comparable type to project the source type to</typeparam>
        /// <param name="source">The source enumerable to get the max out of</param>
        /// <param name="selector">The selector used to obtain a comparable type</param>
        /// <returns>An object of the value type of the enumerable</returns>
        public static TSource MaxBy<TSource, TComparable>(this IEnumerable<TSource> source, Func<TSource, TComparable> selector)
            where TComparable : IComparable<TComparable> =>
            source.MaxBy(selector, Comparer<TComparable>.Default);

        /// <summary>
        /// Gets the maximum element of an enumerable by its elements using a projection to comparable elements and provided <see cref="IComparer{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable</typeparam>
        /// <typeparam name="TComparable">The projection producing a comparable result</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">The selector to use to project to a comparable type</param>
        /// <param name="comparer">The comparer to use to compare the projected elements</param>
        /// <returns>An object of the enumerable's type</returns>
        public static TSource MaxBy<TSource, TComparable>(
            this IEnumerable<TSource> source,
            Func<TSource, TComparable> selector,
            IComparer<TComparable> comparer)
        {
            using var iterator = source.GetEnumerator();
            if (!iterator.MoveNext())
                throw new InvalidOperationException("Source sequence was empty. No maximum could be identified.");

            var opt = iterator.Current;
            var optValue = selector(opt);

            while (iterator.MoveNext())
            {
                var current = iterator.Current;
                var currentValue = selector(current);

                if (comparer.Compare(currentValue, optValue) > 0)
                {
                    opt = current;
                    optValue = currentValue;
                }
            }

            return opt;
        }

        /// <summary>
        /// Gets the minimum element of an enumerable by comparing elements based on a selector's comparable results
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable</typeparam>
        /// <typeparam name="TComparable">Comparable type to project the source type to</typeparam>
        /// <param name="source">The source enumerable to get the max out of</param>
        /// <param name="selector">The selector used to obtain a comparable type</param>
        public static TSource MinBy<TSource, TComparable>(this IEnumerable<TSource> source, Func<TSource, TComparable> selector)
            where TComparable : IComparable<TComparable> =>
            source.MinBy(selector, Comparer<TComparable>.Default);

        /// <summary>
        /// Gets the minimum element of an enumerable by its elements using a projection to comparable elements and provided <see cref="IComparer{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the source enumerable</typeparam>
        /// <typeparam name="TComparable">The projection producing a comparable result</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">The selector to use to project to a comparable type</param>
        /// <param name="comparer">The comparer to use to compare the projected elements</param>
        /// <returns>An object of the enumerable's type</returns>
        public static TSource MinBy<TSource, TComparable>(
            this IEnumerable<TSource> source,
            Func<TSource, TComparable> selector,
            IComparer<TComparable> comparer) =>
            source.MaxBy(selector, Comparer<TComparable>.Create((x, y) => comparer.Compare(y, x)));

        /// <summary>
        /// Gets the index of the maximum element of an enumerable using the provided <see cref="IComparer{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="source">The source enumerable to get the maximum out of</param>
        /// <param name="comparer">The comparer to use to compare the elements</param>
        /// <returns>The index of the maximum element</returns>
        public static int ArgMax<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.ZipWithIndex().MaxBy(x => x.value, comparer).index;

        /// <summary>
        /// Gets the index of the maximum element of an enumerable of comparable elements
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="source">The source enumerable to get the maximum out of</param>
        /// <returns>The index of the maximum element</returns>
        public static int ArgMax<T>(this IEnumerable<T> source) where T : IComparable<T> => source.ArgMax(Comparer<T>.Default);

        /// <summary>
        /// Gets the index of the minimum element of an enumerable using the provided <see cref="IComparer{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="source">The source enumerable to get the minimum out of</param>
        /// <param name="comparer">The comparer to use to compare the elements</param>
        /// <returns>The index of the minimum element</returns>
        public static int ArgMin<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.ZipWithIndex().MinBy(x => x.value, comparer).index;

        /// <summary>
        /// Gets the index of the minimum element of an enumerable of comparable elements
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="source">The source enumerable to get the maximum out of</param>
        /// <returns>The index of the minimum element</returns>
        public static int ArgMin<T>(this IEnumerable<T> source) where T : IComparable<T> => source.ArgMax(Comparer<T>.Default);

        /// <summary>
        /// Flattens a collection of enumerables into a single enumerable
        /// </summary>
        /// <typeparam name="T">The type of the enumerables</typeparam>
        /// <param name="enumerable">The collection of enumerables to flatten</param>
        /// <returns>A single enumerable</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) => enumerable.SelectMany(list => list);

        /// <summary>
        /// Flattens a collection of <see cref="Maybe"/>'s into an enumerable, effectively removing the None's
        /// </summary>
        /// <typeparam name="T">The type of the input <see cref="Maybe"/>'s</typeparam>
        /// <param name="enumerable">The collection of <see cref="Maybe"/>'s</param>
        /// <returns>An enumerable</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) where T : class => enumerable.SelectMany(opt => opt);

        /// <summary>
        /// Flattens a collection of <see cref="Option"/>'s into an enumerable, effectively removing the None's
        /// </summary>
        /// <typeparam name="T">The type of the input <see cref="Option"/>'s</typeparam>
        /// <param name="enumerable">The collection of <see cref="Option"/>'s</param>
        /// <returns>An enumerable</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<Option<T>> enumerable) => enumerable.SelectMany(opt => opt);

        /// <summary>
        /// Flattens a collection of nullable value types into an enumerable, effectively removing the null values
        /// </summary>
        /// <typeparam name="T">The type of the nullable's</typeparam>
        /// <param name="enumerable">The enumerable of nullable's</param>
        /// <returns>A collection of (non-nullable) elements</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T?> enumerable) where T : struct
        {
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                    yield break;
                yield return enumerator.Current.Value;
            }
        }

        /// <summary>
        /// Flattens a collection of nullable value types into an enumerable, effectively removing the null values
        /// </summary>
        /// <typeparam name="T">The type of the nullable's</typeparam>
        /// <param name="enumerable">The enumerable of nullable's</param>
        /// <returns>A collection of (non-nullable) elements</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T?> enumerable) where T : class
        {
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                    yield break;
                yield return enumerator.Current!;
            }
        }

        /// <summary>
        /// Checks whether an enumerable is empty or not
        /// </summary>
        /// <typeparam name="T">The type of the enumerable to check for emptiness</typeparam>
        /// <param name="col">The enumerable to check for emptiness</param>
        /// <returns>True if the enumerable is empty, false otherwise</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> col) => !col.Any();

        /// <summary>
        /// Convenience method to apply a 'where' clause when the predicate applies to a subtype of the enumerable's type
        /// </summary>
        /// <typeparam name="T">Type of the input enumerable</typeparam>
        /// <typeparam name="TU">Type of the predicate's input</typeparam>
        /// <param name="enumerable">The enumerable to filter</param>
        /// <param name="predicate">The predicate to use as a filter</param>
        /// <returns>A filtered enumerable</returns>
        public static IEnumerable<T> GenFilter<T, TU>(this IEnumerable<T> enumerable, Func<TU, bool> predicate) where T : TU => enumerable.Where(l => predicate(l));

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

        public static IEnumerable<TU> Scan<T, TU>(this IEnumerable<T> input, TU state, Func<TU, T, TU> next)
        {
            yield return state;
            foreach (var item in input)
            {
                state = next(state, item);
                yield return state;
            }
        }

        public static IEnumerable<TOut> Scan<T, TState, TOut>(this IEnumerable<T> input, TState state, Func<TState, T, (TState, TOut)> next)
        {
            foreach (var item in input)
            {
                var (newState, result) = next(state, item);
                state = newState;
                yield return result;
            }
        }

        public static IEnumerable<T> Scan<T>(this IEnumerable<T> input, Func<T, T, T> next)
        {
            using (var it = input.GetEnumerator())
            {
                if (it.MoveNext())
                {
                    var state = it.Current;
                    yield return state;
                    while (it.MoveNext())
                    {
                        state = next(state, it.Current);
                        yield return state;
                    }
                }
            }
        }

        public static Option<T> FirstOption<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return enumerator.MoveNext() ? Option.Some(enumerator.Current) : Option.None;
            }
        }

        public static Option<T> SingleOption<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                return enumerator.MoveNext()
                    // ReSharper disable once AccessToDisposedClosure
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ? Option.Some(enumerator.Current).Where(_ => !enumerator.MoveNext())
                    : Option.None;
            }
        }

        public static T SingleOrDefault<T>(this IEnumerable<T> self, Func<T> provider) => self.SingleOption().GetValueOr(provider);

        public static Option<T> MaxOption<T>(this IEnumerable<T> self) => self.AggregateOption(Enumerable.Max);

        public static Option<T> MinOption<T>(this IEnumerable<T> self) => self.AggregateOption(Enumerable.Min);

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "prevent unnecessary exceptions")]
        public static Option<(T Min, T Max)> MinMaxOption<T>(this IEnumerable<T> self) => self.MinOption().SelectMany(min => self.MaxOption().Select(max => (min, max)));

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "prevent unnecessary exceptions")]
        public static Option<T> AggregateOption<T>(this IEnumerable<T> self, Func<IEnumerable<T>, T> aggregator) => self.IsEmpty() ? Option<T>.None : Try.Do(() => aggregator.Invoke(self)).AsOption;

        public static (IEnumerable<T> Left, IEnumerable<T> Right) Split<T>(
            this IEnumerable<T> self,
            Func<T, bool> isLeft)
        {
            var groups = self.GroupBy(isLeft).ToDictionary();
            return (groups.GetOption(true).GetValueOrDefault(Enumerable.Empty<T>()),
                groups.GetOption(false).GetValueOrDefault(Enumerable.Empty<T>()));
        }

        public static bool IsSorted<T>(this IEnumerable<T> self, IComparer<T> comparer = null, bool strict = false)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var outOfOrder = strict
                ? (Func<T, T, bool>)((x, y) => comparer.Compare(x, y) >= 0)
                : (x, y) => comparer.Compare(x, y) > 0;
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
            return self.Concat(other).GroupBy(keySelector, x => x, (key, group) => @group.First());
        }

        public static IEnumerable<bool> And(this IEnumerable<bool> xs, IEnumerable<bool> ys) => xs.Zip(ys, (x, y) => x && y);

        public static IEnumerable<bool> Or(this IEnumerable<bool> xs, IEnumerable<bool> ys) => xs.Zip(ys, (x, y) => x || y);
    }
}