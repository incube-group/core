using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InCube.Core.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Implements an option data type. This in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Maybe<>))]
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    public readonly struct Maybe<T> : IInvariantOption<T, Maybe<T>> where T : class
    {
        private readonly T? value;

        private Maybe(T? value)
        {
            this.value = value;
        }

        /// <summary>
        /// Maps value of <see cref="Maybe{T}"/> to output type and None to None
        /// </summary>
        /// <typeparam name="TOut">Output type</typeparam>
        /// <param name="f">Mapping</param>
        /// <returns>A <see cref="Maybe{T}"/> of output type</returns>
        public Maybe<TOut> Select<TOut>(Func<T, TOut> f) where TOut : class => this.value?.Apply(f);

        /// <summary>
        /// Maps value of <see cref="Maybe{T}"/> to output type and None to None, when mapping itself returns a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="TOut">Output type</typeparam>
        /// <param name="f">Mapping which returns a <see cref="Maybe{T}"/> of the output type</param>
        /// <returns>A <see cref="Maybe{T}"/> of the output type</returns>
        public Maybe<TOut> SelectMany<TOut>(Func<T, Maybe<TOut>> f) where TOut : class => this.value?.Apply(f) ?? default;

        /// <summary>
        /// Tries to downcast value of <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="TD">Subtype of <see cref="Maybe{T}"/>'s type</typeparam>
        /// <returns>A <see cref="Maybe{T}"/> of the subtype</returns>
        public Maybe<TD> Cast<TD>() where TD : class, T => this.SelectMany(x => x is TD d ? new Maybe<TD>(d) : default);

        /// <summary>
        /// Reference to default <see cref="Maybe{T}"/>, i.e. a <see cref="Maybe{T}"/> with no value
        /// </summary>
        public static readonly Maybe<T> None = default;

        #region Built In

        public int Count => this.HasValue ? 1 : 0;

        /// <see cref="Nullable{T}.Value"/>
        /// <exception cref="InvalidOperationException">If this <see cref="Maybe"/> is undefined or the <paramref name="index"/> != 0.</exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T this[int index] => index == 0 ? this.Value : throw new InvalidOperationException();

        public IEnumerator<T> GetEnumerator()
        {
            if (this.HasValue) yield return this.value!;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(this.value);

        public override string ToString() => this.Match(some: x => x.ToString(), none: () => null);

        #endregion

        #region Equality

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(Maybe<T> that) =>
            !this.HasValue && !that.HasValue ||
            this.HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(this.value!, that.value!);

        /// <inheritdoc cref="ValueType"/>
        public override bool Equals(object obj)
        {
            if (obj == null) return !this.HasValue;
            return obj is Maybe<T> option && this.Equals(option);
        }

        public static bool operator ==(Maybe<T> c1, Maybe<T> c2) => c1.Equals(c2);

        public static bool operator !=(Maybe<T> c1, Maybe<T> c2) => !c1.Equals(c2);

        #endregion

        #region Implicit Conversion

        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        public static implicit operator Maybe<T>(T? value) => new Maybe<T>(value);

        public static implicit operator Maybe<T>(Option<T> option) => option.GetValueOrDefault();

        public static implicit operator Option<T>(Maybe<T> maybe) => maybe.Select(x => x.ToAny()).ToOption();

        [SuppressMessage("Usage", "CA1801: Review unused parameters", Justification = "Need parameter for complying with implicit conversion operator.")]
        public static implicit operator Maybe<T>(Maybe<Nothing> x) => default;

        #endregion

        #region Option

        /// <inheritdoc cref="IOption{T}"/>
        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => this.value?.Apply(x => some(x).ToAny()) ?? none();

        /// <inheritdoc cref="IOption{T}"/>
        public T Value => this.HasValue ? this.value! : throw new InvalidOperationException("Trying to access value of None.");

        /// <inheritdoc cref="IOption{T}"/>
        public bool HasValue => this.value != null;

        /// <inheritdoc cref="IOption{T}"/>
        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => (this.value?.Apply(x => f(x).ToAny())).ToOption();

        /// <inheritdoc cref="IOption{T}"/>
        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => this.value?.Apply(f) ?? default(Option<TOut>);

        /// <inheritdoc cref="IOption{T}"/>
        public bool Any() => this.HasValue;

        /// <inheritdoc cref="IOption{T}"/>
        public bool Any(Func<T, bool> p) => this.value?.Apply(p) ?? false;

        /// <inheritdoc cref="IOption{T}"/>
        public bool All(Func<T, bool> p) => this.value?.Apply(p) ?? true;

        /// <inheritdoc cref="IOption{T}"/>
        public void ForEach(Action<T> action) => this.value?.Apply(action);

        /// <inheritdoc cref="IOption{T}"/>
        public void ForEach(Action none, Action<T> some)
        {
            this.ForEach(some);
            if (!this.HasValue)
            {
                none();
            }
        }

        #endregion

        #region InvariantOption

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public T GetValueOr(Func<T> @default) => this.value ?? CheckNotNull(@default, nameof(@default)).Invoke();

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public Maybe<T> OrElse(Func<Maybe<T>> @default) => this.HasValue ? this : @default();

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public Maybe<T> OrElse(Maybe<T> @default) => this.HasValue ? this : @default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public T? GetValueOrDefault() => this.value;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public T GetValueOrDefault(T @default) => this.value ?? @default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public bool Contains(T elem) => this.Contains(elem, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public bool Contains(T elem, IEqualityComparer<T> comparer) => this.Select(x => comparer.Equals(x, elem)) ?? false;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        public Maybe<T> Where(Func<T, bool> p) => !this.HasValue || p(this.value!) ? this : default;

        /// <inheritdoc cref="IInvariantOption{T,TOpt}"/>
        IOption<T> IOption<T>.Where(Func<T, bool> p) => this.Where(p);

        #endregion
    }

    /// <summary>
    /// Extension methods for <see cref="Maybe{T}"/>
    /// </summary>
    [PublicAPI]
    public static class Maybe
    {
        #region Construction

        /// <summary>
        /// Instantiates an empty <see cref="Maybe{T}"/>
        /// </summary>
        public static readonly Maybe<Nothing> None = Maybe<Nothing>.None;

        /// <summary>
        /// Instantiates a non-empty <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Maybe{T}"/></typeparam>
        /// <param name="value">Value of the <see cref="Maybe{T}"/></param>
        /// <returns>A new <see cref="Maybe{T}"/></returns>
        public static Maybe<T> Some<T>(T value) where T : class => CheckNotNull(value, nameof(value));

        #endregion

        #region Conversion

        /// <summary>
        /// Converts reference type to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Maybe{T}"/></typeparam>
        /// <param name="value">The value of the <see cref="Maybe{T}"/></param>
        /// <returns>A <see cref="Maybe{T}"/></returns>
        public static Maybe<T> ToMaybe<T>(this T? value) where T : class => value;

        #endregion

        #region Flattening

        /// <summary>
        /// Converts <see cref="Option{T}"/> of <see cref="Maybe{T}"/> of a reference type to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The reference type</typeparam>
        /// <param name="self">The <see cref="Option{T}"/> nested <see cref="Maybe{T}"/></param>
        /// <returns>A <see cref="Maybe{T}"/></returns>
        public static Maybe<T> Flatten<T>(this in Option<Maybe<T>> self) where T : class => self.AsAny?.Apply(x => x.Value) ?? Maybe<T>.None;

        /// <summary>
        /// Converts nullable nested <see cref="Maybe{T}"/> to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">The input nullable nested <see cref="Maybe{T}"/></param>
        /// <returns>A <see cref="Maybe{T}"/></returns>
        public static Maybe<T> Flatten<T>(this in Maybe<T>? self) where T : class => self ?? Maybe<T>.None;

        #endregion

        #region Projection

        /// <summary>
        /// Maps a <see cref="Maybe{T}"/> to a nullable value type
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping</typeparam>
        /// <typeparam name="TOut">Output type of mapping</typeparam>
        /// <param name="this">Input <see cref="Maybe{T}"/></param>
        /// <param name="f">Mapping</param>
        /// <returns>A nullable output value type</returns>
        public static TOut? Select<TIn, TOut>(this Maybe<TIn> @this, Func<TIn, TOut> f) where TIn : class where TOut : struct => @this.GetValueOrDefault()?.Apply(f);

        /// <summary>
        /// Maps <see cref="Maybe{T}"/> to a nullable output type, when mapping can output null
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping</typeparam>
        /// <typeparam name="TOut">Output type of mapping</typeparam>
        /// <param name="this">Input <see cref="Maybe{T}"/></param>
        /// <param name="f">Mapping</param>
        /// <returns>A nullable output value type</returns>
        public static TOut? SelectMany<TIn, TOut>(this Maybe<TIn> @this, Func<TIn, TOut?> f) where TIn : class where TOut : struct => @this.GetValueOrDefault()?.Apply(f);

        /// <summary>
        /// Maps nullable value type to a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping</typeparam>
        /// <typeparam name="TOut">Output type of mapping</typeparam>
        /// <param name="self">Input nullable value type</param>
        /// <param name="f">Mapping</param>
        /// <returns>A <see cref="Maybe{T}"/> of the output type</returns>
        public static Maybe<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) where TIn : struct where TOut : class => self?.Apply(f);

        /// <summary>
        /// Maps nullable value type to a <see cref="Maybe{T}"/> when mapping can output None
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping</typeparam>
        /// <typeparam name="TOut">Output type of mapping</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="f">Mapping</param>
        /// <returns>A <see cref="Maybe{T}"/> of the output type</returns>
        public static Maybe<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Maybe<TOut>> f) where TIn : struct where TOut : class => self?.Apply(f) ?? None;

        #endregion
    }
}