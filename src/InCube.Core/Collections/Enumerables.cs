using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) => 
            !enumerable.Any(predicate);

        public static IEnumerable<T> ToEnumerable<T>(T t)
        {
            yield return t;
        }

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

        public static IEnumerable<T> Repeat<T>(T value)
        {
            while (true)
            {
                yield return value;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static IEnumerable<T> Generate<T>(Func<T> generator)
        {
            while (true)
            {
                yield return generator();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static T[] Enumerate<T>(params T[] elems) => elems;

        public static IEnumerable<int> IntRange(int startInclusive, int stopExclusive) =>
            Enumerable.Range(startInclusive, stopExclusive - startInclusive);

        public static IEnumerable<int> IntRange(int stopExclusive) =>
            Enumerable.Range(0, stopExclusive);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", ").
        /// </summary>
        public static string MkString<T>(this IEnumerable<T> enumerable, string separator = ", ") =>
            String.Join(separator, enumerable);

        /// <summary>
        /// Joins the strings in the enumerable with the specified separator (default: ", "), wrapped by with a string in the beginning and the end.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="start">The string to place in front of the joined output.</param>
        /// <param name="separator">The separator to be placed between elements.</param>
        /// <param name="end">The string to place at the end of the joined output.</param>
        public static string MkString<T>(this IEnumerable<T> enumerable, string start, string separator, string end) =>
            $"{start}{enumerable.MkString(separator)}{end}";

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey> =>
            source.MaxBy(selector, Comparer<TKey>.Default);

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    throw new InvalidOperationException("empty source sequence");

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
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey> =>
            source.MinBy(selector, Comparer<TKey>.Default);

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            IComparer<TKey> comparer) =>
            source.MaxBy(selector, Comparer<TKey>.Create((x, y) => comparer.Compare(y, x)));

        public static int ArgMax<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
            source.ZipWithIndex().MaxBy(x => x.value, comparer).index;

        public static int ArgMax<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source.ArgMax(Comparer<T>.Default);

        public static int ArgMin<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
            source.ZipWithIndex().MinBy(x => x.value, comparer).index;

        public static int ArgMin<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source.ArgMax(Comparer<T>.Default);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(list => list);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) where T : class =>
            enumerable.SelectMany(opt => opt);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Option<T>> enumerable) =>
            enumerable.SelectMany(opt => opt);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T?> enumerable) where T : struct =>
            enumerable.SelectMany(nullable => nullable.ToOption());

        public static bool IsEmpty<T>(this IEnumerable<T> col) => !col.Any();

        public static IEnumerable<T> GenFilter<T, TU>(this IEnumerable<T> list, Func<TU, bool> predicate) where T : TU
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

        public static T SingleOrDefault<T>(this IEnumerable<T> self, Func<T> provider) => 
            self.SingleOption().GetValueOr(provider);

        public static Option<T> MaxOption<T>(this IEnumerable<T> self) =>
            self.AggregateOption(Enumerable.Max);

        public static Option<T> MinOption<T>(this IEnumerable<T> self) => 
            self.AggregateOption(Enumerable.Min);

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "prevent unnecessary exceptions")]
        public static Option<(T Min, T Max)> MinMaxOption<T>(this IEnumerable<T> self) =>
            self.MinOption().SelectMany(min => self.MaxOption().Select(max => (min, max)));

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "prevent unnecessary exceptions")]
        public static Option<T> AggregateOption<T>(this IEnumerable<T> self, Func<IEnumerable<T>, T> aggregator) =>
            self.IsEmpty() ? Option<T>.None : Try.Do(() => aggregator.Invoke(self)).AsOption;

        public static (IEnumerable<T> Left, IEnumerable<T> Right) Split<T>(this IEnumerable<T> self,
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
                :                    (x, y) => comparer.Compare(x, y) > 0;
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

        public static IEnumerable<bool> And(this IEnumerable<bool> xs, IEnumerable<bool> ys) =>
            xs.Zip(ys, (x, y) => x && y);

        public static IEnumerable<bool> Or(this IEnumerable<bool> xs, IEnumerable<bool> ys) =>
            xs.Zip(ys, (x, y) => x || y);
    }
}
