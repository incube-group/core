using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InCube.Core.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Implements an option data type. This in is essentially an extension of <see cref="Nullable{T}" /> that works for
    /// both struct and class types.
    /// </summary>
    /// <typeparam name="T">The type wrapped in the option.</typeparam>
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Option<>))]
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    [PublicAPI]
    public readonly struct Option<T> : IInvariantOption<T, Option<T>>
    {
        /// <summary>
        /// The 'empty' option.
        /// </summary>
        public static readonly Option<T> None;

        internal Option(T value) => this.AsAny = value;

        internal Option(in Any<T>? any) => this.AsAny = any;

        /// <inheritdoc cref="Nullable{T}.HasValue" />
        public bool HasValue => this.AsAny.HasValue;

        /// <inheritdoc cref="Nullable{T}.Value" />
        /// <exception cref="InvalidOperationException">If this option is undefined.</exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T Value => this.AsAny.Value;

        /// <inheritdoc />
        public int Count => this.HasValue ? 1 : 0;

        internal Any<T>? AsAny { get; }

        /// <summary>
        /// <see cref="IReadOnlyList{T}.this"/>.
        /// </summary>
        /// <see cref="Nullable{T}.Value" />
        /// <param name="index">Has to be zero.</param>
        /// <exception cref="InvalidOperationException">
        /// If this <see cref="Option" /> is undefined or the <paramref name="index" />
        /// != 0.
        /// </exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T this[int index] => index == 0 ? this.Value : throw new InvalidOperationException();

        public static explicit operator T(Option<T> value) => value.Value;

        [SuppressMessage("Usage", "CA1801: Review unused parameters", Justification = "Need parameter for complying with implicit conversion operator.")]
        public static implicit operator Option<T>(Option<Nothing> x) => default;

        public static implicit operator Option<T>(T value) => typeof(T).IsValueType || !ReferenceEquals(value, null) ? new Option<T>(value) : default;

        public static bool operator ==(Option<T> c1, Option<T> c2) => c1.Equals(c2);

        public static bool operator !=(Option<T> c1, Option<T> c2) => !(c1 == c2);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            if (this.AsAny.HasValue)
                yield return this.AsAny.Value;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool Equals(Option<T> that) => Nullable.Equals(this.AsAny, that.AsAny);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, null))
                return !this.HasValue;
            return obj is Option<T> option && this.Equals(option);
        }

        /// <inheritdoc />
        public override int GetHashCode() => this.AsAny.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.Match(some: x => x.ToString(), none: () => null);

        /// <inheritdoc />
        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => this.AsAny.Select(x => some(x).ToAny()) ?? none();

        /// <inheritdoc />
        public T GetValueOrDefault() => this.AsAny.GetValueOrDefault();

        /// <inheritdoc />
        public T GetValueOrDefault(T @default) => this.AsAny ?? @default;

        /// <inheritdoc />
        public T GetValueOr(Func<T> @default) => this.AsAny ?? CheckNotNull(@default, nameof(@default)).Invoke();

        /// <inheritdoc />
        public bool Any() => this.HasValue;

        /// <inheritdoc />
        public bool Any(Func<T, bool> predicate) => this.AsAny.Any(x => predicate(x));

        /// <inheritdoc />
        public bool All(Func<T, bool> predicate) => this.AsAny.All(x => predicate(x));

        /// <inheritdoc />
        public void ForEach(Action<T> action) => this.AsAny.ForEach(x => action(x));

        /// <inheritdoc />
        public void ForEach(Action none, Action<T> some) => this.AsAny.ForEach(none, x => some(x));

        /// <inheritdoc />
        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> selector) => this.Select(selector);

        /// <inheritdoc />
        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> selector) => this.SelectMany(x => selector(x).ToOption());

        /// <inheritdoc cref="IOption{T}.Select{TOut}" />
        public Option<TOut> Select<TOut>(Func<T, TOut> f) => new(this.AsAny.Select(x => f(x).ToAny()));

        /// <inheritdoc cref="IOption{T}.SelectMany{TOut}" />
        public Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> f) => new(this.AsAny.SelectMany(x => f(x).AsAny));

        /// <inheritdoc />
        IOption<T> IOption<T>.Where(Func<T, bool> predicate) => this.Where(predicate);

        /// <inheritdoc />
        public Option<T> Where(Func<T, bool> predicate) => this.Any(predicate) ? this : default;

        /// <inheritdoc cref="Enumerable.Cast{TResult}" />
        public Option<TD> Cast<TD>()
            where TD : T =>
            this.SelectMany(x => x is TD d ? Option.Some(d) : default);

        /// <summary>
        /// Provides an alternative to the option's value in case it is empty.
        /// </summary>
        /// <param name="default">Alternative generator.</param>
        /// <returns>An <see cref="Option{T}"/>.</returns>
        public Option<T> OrElse(Func<Option<T>> @default) => this.HasValue ? this : @default();

        /// <summary>
        /// Provides an alternative to the option's value in case it is empty.
        /// </summary>
        /// <param name="default">Alternative option.</param>
        /// <returns>An <see cref="Option{T}"/>.</returns>
        public Option<T> OrElse(Option<T> @default) => this.HasValue ? this : @default;

        /// <inheritdoc />
        public bool Contains(T elem) => this.Contains(elem, EqualityComparer<T>.Default);

        /// <inheritdoc />
        public bool Contains(T elem, IEqualityComparer<T> comparer) => this.AsAny.Select(x => comparer.Equals(x.Value, elem)) ?? false;
    }

    /// <summary>
    /// Defines utility operations on instances of <see cref="Option{T}" />.
    /// </summary>
    [PublicAPI]
    public static class Option
    {
        #region Construction

        /// <summary>
        /// Reference to empty <see cref="Option{T}" />.
        /// </summary>
        public static readonly Option<Nothing> None = Option<Nothing>.None;

        /// <summary>
        /// Convenience method to create non-empty <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">Underlying type.</typeparam>
        /// <param name="value">Non-null value for <see cref="Option{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> Some<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Option<T>(value);
        }

        /// <summary>
        /// Convenience method to create empty or non-empty <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">Underlying type.</typeparam>
        /// <param name="value">Null or non-null value.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T?> Some<T>(in T? value)
            where T : struct =>
            new(value);

        #endregion

        #region Conversion

        /// <summary>
        /// Converts nullable reference type to an <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">Underlying reference type of the option.</typeparam>
        /// <param name="value">Reference type object to embed in an <see cref="Option{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> ToOption<T>(this T? value)
            where T : class =>
            value?.Apply(Some) ?? None;

        /// <summary>
        /// Converts nullable value type fo an <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">Underlying value type of the option.</typeparam>
        /// <param name="value">Value type object to embed in an <see cref="Option{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> ToOption<T>(this in T? value)
            where T : struct =>
            value?.Apply(Some) ?? None;

        internal static Option<T> ToOption<T>(this in Any<T>? any) => new(any);

        /// <summary>
        /// Converts a <see cref="Maybe{T}" /> to an <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">
        /// Underlying type of both the input <see cref="Maybe{T}" /> and of the output.
        /// <see cref="Option{T}" />
        /// </typeparam>
        /// <param name="maybe">The input <see cref="Maybe{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> ToOption<T>(this Maybe<T> maybe)
            where T : class =>
            maybe;

        /// <summary>
        /// Casts <see cref="IOption{T}" /> to concrete <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="value">Input <see cref="IOption{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> ToOption<T>(this IOption<T> value) => value is Option<T> opt ? opt : value.HasValue ? Some(value.Value) : None;

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">Doesn't matter.</param>
        /// <typeparam name="T">Type doesn't matter.</typeparam>
        /// <returns>Doesn't matter what it returns.</returns>
        [Obsolete("remove unnecessary call to " + nameof(ToOption))]
        public static Option<T> ToOption<T>(this in Option<T> value) => value;

        /// <summary>
        /// Converts <see cref="Option{T}" /> of value type to a nullable value type.
        /// </summary>
        /// <typeparam name="T">Underlying value type.</typeparam>
        /// <param name="self">Input <see cref="Option{T}" />.</param>
        /// <returns>Nullable value type.</returns>
        public static T? ToNullable<T>(this in Option<T> self)
            where T : struct =>
            self.AsAny?.Apply(x => x.Value);

        /// <summary>
        /// Converts <see cref="Option{T}" /> of reference type to a nullable reference type.
        /// </summary>
        /// <typeparam name="T">Underlying reference type.</typeparam>
        /// <param name="self">Input <see cref="Option{T}" />.</param>
        /// <returns>Nullable reference type.</returns>
        public static T? ToNullable<T>(this Option<T> self)
            where T : class =>
            self.AsAny?.Apply(x => x.Value);

        /// <summary>
        /// Maps <see cref="Option{T}" /> of value type to nullable value type.
        /// </summary>
        /// <typeparam name="TIn">Underlying value type of input <see cref="Option{T}" />.</typeparam>
        /// <typeparam name="TOut">Output type of mapping.</typeparam>
        /// <param name="self">Input <see cref="Option{T}" />.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A nullable value type.</returns>
        public static TOut? ToNullable<TIn, TOut>(this in Option<TIn> self, Func<TIn, TOut> f)
            where TOut : struct =>
            self.AsAny?.Apply(x => f(x.Value));

        #endregion

        #region Flattening

        /// <summary>
        /// Converts nested <see cref="Option{T}" />'s to an <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="self">The <see cref="Option{T}" /> nested <see cref="Option{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> Flatten<T>(this in Option<Option<T>> self) => self.AsAny?.Apply(x => x.Value) ?? None;

        /// <summary>
        /// Converts <see cref="Option{T}" /> nested nullable value type to a nullable value type.
        /// </summary>
        /// <typeparam name="T">The underlying value type.</typeparam>
        /// <param name="self">The <see cref="Option{T}" /> nested value type.</param>
        /// <returns>A nullable value type.</returns>
        public static T? Flatten<T>(this in Option<T?> self)
            where T : struct =>
            self.AsAny?.Apply(x => x.Value);

        /// <summary>
        /// Converts a nullable nested <see cref="Option{T}" /> to an <see cref="Option{T}" />.
        /// </summary>
        /// <typeparam name="T">The underlying value type.</typeparam>
        /// <param name="self">The nullable nested <see cref="Option{T}" />.</param>
        /// <returns>An <see cref="Option{T}" />.</returns>
        public static Option<T> Flatten<T>(this in Option<T>? self) => self ?? None;

        #endregion
    }
}