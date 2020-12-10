using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InCube.Core.Format;
using Newtonsoft.Json;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Implements an option data type. This in is essentially an extension of <see cref="Nullable{T}" /> that works for
    /// class types.
    /// </summary>
    /// <typeparam name="T">Wrapped reference type.</typeparam>
    [Serializable]
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Maybe<>))]
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    public readonly struct Maybe<T> : IInvariantOption<T, Maybe<T>>
        where T : class
    {
        /// <summary>
        /// Reference to default <see cref="Maybe{T}" />, i.e. a <see cref="Maybe{T}" /> with no value.
        /// </summary>
        public static readonly Maybe<T> None = default;

        private readonly T? value;

        private Maybe(T? value) => this.value = value;

        /// <inheritdoc/>
        public int Count => this.HasValue ? 1 : 0;

        /// <inheritdoc cref="IOption{T}" />
        public T Value => this.HasValue ? this.value! : throw new InvalidOperationException("Trying to access value of None.");

        /// <inheritdoc cref="IOption{T}" />
        public bool HasValue => this.value != null;

        /// <summary>
        /// <see cref="IReadOnlyList{T}.this"/>.
        /// </summary>
        /// <see cref="Nullable{T}.Value" />
        /// <param name="index">The index to look at.</param>
        /// <exception cref="InvalidOperationException">
        /// If this <see cref="Maybes" /> is undefined or the <paramref name="index" />
        /// != 0.
        /// </exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T this[int index] => index == 0 ? this.Value : throw new InvalidOperationException();

        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        public static implicit operator Maybe<T>(T? value) => new(value);

        public static implicit operator Maybe<T>(Option<T> option) => option.GetValueOrDefault();

        public static implicit operator Option<T>(Maybe<T> maybe) => maybe.Select(x => x.ToAny()).ToOption();

        [SuppressMessage("Usage", "CA1801: Review unused parameters", Justification = "Need parameter for complying with implicit conversion operator.")]
        public static implicit operator Maybe<T>(Maybe<Nothing> x) => default;

        public static bool operator ==(Maybe<T> c1, Maybe<T> c2) => c1.Equals(c2);

        public static bool operator !=(Maybe<T> c1, Maybe<T> c2) => !c1.Equals(c2);

        /// <summary>
        /// Maps value of <see cref="Maybe{T}" /> to output type and None to None.
        /// </summary>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="f">Mapping.</param>
        /// <returns>A <see cref="Maybe{T}" /> of output type.</returns>
        public Maybe<TOut> Select<TOut>(Func<T, TOut> f)
            where TOut : class =>
            this.value?.Apply(f);

        /// <summary>
        /// Maps value of <see cref="Maybe{T}" /> to output type and None to None, when mapping itself returns a
        /// <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="f">Mapping which returns a <see cref="Maybe{T}" /> of the output type.</param>
        /// <returns>A <see cref="Maybe{T}" /> of the output type.</returns>
        public Maybe<TOut> SelectMany<TOut>(Func<T, Maybe<TOut>> f)
            where TOut : class =>
            this.value?.Apply(f) ?? default;

        /// <summary>
        /// Tries to downcast value of <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="TD">Subtype of <see cref="Maybe{T}" />'s type.</typeparam>
        /// <returns>A <see cref="Maybe{T}" /> of the subtype.</returns>
        public Maybe<TD> Cast<TD>()
            where TD : class, T =>
            this.SelectMany(x => x is TD d ? new Maybe<TD>(d) : default);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.HasValue)
                yield return this.value!;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public override int GetHashCode() => this.HasValue ? EqualityComparer<T>.Default.GetHashCode(this.value!) : 0;

        /// <inheritdoc/>
        public override string? ToString() => this.Match(some: x => x.ToString(), none: () => null);

        /// <inheritdoc cref="IEquatable{T}" />
        public bool Equals(Maybe<T> that) => (!this.HasValue && !that.HasValue) || (this.HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(this.value!, that.value!));

        /// <inheritdoc cref="ValueType" />
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return !this.HasValue;
            return obj is Maybe<T> option && this.Equals(option);
        }

        /// <inheritdoc cref="IOption{T}" />
        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => this.value?.Apply(x => some(x).ToAny()) ?? none();

        /// <inheritdoc cref="IOption{T}" />
        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> selector) => (this.value?.Apply(x => selector(x).ToAny())).ToOption();

        /// <inheritdoc cref="IOption{T}" />
        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> selector) => this.value?.Apply(selector) ?? default(Option<TOut>);

        /// <inheritdoc cref="IOption{T}" />
        public bool Any() => this.HasValue;

        /// <inheritdoc cref="IOption{T}" />
        public bool Any(Func<T, bool> predicate) => this.value?.Apply(predicate) ?? false;

        /// <inheritdoc cref="IOption{T}" />
        public bool All(Func<T, bool> predicate) => this.value?.Apply(predicate) ?? true;

        /// <inheritdoc cref="IOption{T}" />
        public void ForEach(Action<T> action) => this.value?.Apply(action);

        /// <inheritdoc cref="IOption{T}" />
        public void ForEach(Action none, Action<T> some)
        {
            this.ForEach(some);
            if (!this.HasValue)
                none();
        }

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public T GetValueOr(Func<T> @default) => this.value ?? Preconditions.CheckNotNull(@default, nameof(@default)).Invoke();

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public Maybe<T> OrElse(Func<Maybe<T>> @default) => this.HasValue ? this : @default();

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public Maybe<T> OrElse(Maybe<T> @default) => this.HasValue ? this : @default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public T? GetValueOrDefault() => this.value ?? default(T);

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public T GetValueOrDefault(T @default) => this.value ?? @default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public bool Contains(T elem) => this.Contains(elem, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public bool Contains(T elem, IEqualityComparer<T> comparer) => this.Select(x => comparer.Equals(x, elem)) ?? false;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        public Maybe<T> Where(Func<T, bool> predicate) => !this.HasValue || predicate(this.value!) ? this : default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}" />
        IOption<T> IOption<T>.Where(Func<T, bool> predicate) => this.Where(predicate);
    }
}