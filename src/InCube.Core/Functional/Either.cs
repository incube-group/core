using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <inheritdoc cref="IEither{TL,TR}" />
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    [PublicAPI]
    public readonly struct Either<TL, TR> : IEither<TL, TR>, IEquatable<Either<TL, TR>>
    {
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TL, TR}" /> struct from a <see cref="Left" /> value.
        /// </summary>
        /// <param name="value">The <see cref="Left" /> value.</param>
        public Either(TL value)
            : this(value, true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TL, TR}" /> struct from a <see cref="Right" /> value.
        /// </summary>
        /// <param name="value">The <see cref="Right" /> value.</param>
        public Either(TR value)
            : this(value, false, true)
        {
        }

        private Either(object value, bool left, bool right)
        {
            this.value = value;
            this.IsLeft = left;
            this.IsRight = right;
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="Either" /> is of kind <see cref="Left" />.
        /// </summary>
        public bool IsLeft { get; }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="Either" /> is of kind <see cref="Right" />.
        /// </summary>
        public bool IsRight { get; }

        /// <summary>
        /// Gets the left value of the <see cref="Either{TL,TR}"/>.
        /// </summary>
        public TL Left => this.IsLeft ? (TL)this.value : throw new NotSupportedException($"Either is not Left<{typeof(TL)}>, but Right<{typeof(TR)}>");

        /// <summary>
        /// Gets the right value of the <see cref="Either{TL,TR}"/>.
        /// </summary>
        public TR Right => this.IsRight ? (TR)this.value : throw new NotSupportedException($"Either is not Right<{typeof(TR)}>, but Left<{typeof(TL)}>");

        /// <summary>
        /// Gets the <see cref="Left"/> value of the <see cref="Either{TL,TR}"/> as an <see cref="Option{T}"/>.
        /// </summary>
        public Option<TL> LeftOption => this.IsLeft ? Option.Some((TL)this.value) : Option.None;

        /// <summary>
        /// Gets the <see cref="Right"/> value of the <see cref="Either{TL,TR}"/> as an <see cref="Option{T}"/>.
        /// </summary>
        public Option<TR> RightOption => this.IsRight ? Option.Some((TR)this.value) : Option.None;

        /// <summary>
        /// Gets the type of the <see cref="Either{TL,TR}"/> depending on whether it is <see cref="Left"/> or <see cref="Right"/>.
        /// </summary>
        public Type Type => this.IsLeft ? typeof(TL) : typeof(TR);

        /// <inheritdoc/>
        IOption<TL> IEither<TL, TR>.LeftOption => this.LeftOption;

        /// <inheritdoc/>
        IOption<TR> IEither<TL, TR>.RightOption => this.RightOption;

        public static implicit operator Either<TL, TR>(TL left) => new(left);

        public static implicit operator Either<TL, TR>(TR right) => new(right);

        public static bool operator ==(Either<TL, TR> left, Either<TL, TR> right) => left.Equals(right);

        public static bool operator !=(Either<TL, TR> left, Either<TL, TR> right) => !left.Equals(right);

        /// <inheritdoc/>
        public T Match<T>(Func<TL, T> left, Func<TR, T> right) => this.IsLeft ? left(this.Left) : right(this.Right);

        /// <inheritdoc/>
        IEither<TL, TOut> IEither<TL, TR>.Select<TOut>(Func<TR, TOut> f) => this.Select(f);

        /// <inheritdoc cref="IEither{TL, TR}.Select{TOut}" />
        public Either<TL, TOut> Select<TOut>(Func<TR, TOut> f) => this.Match<Either<TL, TOut>>(left => left, right => f(right));

        /// <summary>
        /// Applies the <paramref name="selector"/> to this and flattens.
        /// </summary>
        /// <param name="selector">The function to apply to the right side of the <see cref="Either{TL,TR}"/>.</param>
        /// <typeparam name="TOut">The output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="Either{TL,TR}"/>.</returns>
        public Either<TL, TOut> SelectMany<TOut>(Func<TR, Either<TL, TOut>> selector) => this.Match(left => left, selector);

        /// <inheritdoc/>
        public void ForEach(Action<TR> right)
        {
            if (this.IsRight)
                right(this.Right);
        }

        /// <inheritdoc/>
        public void ForEach(Action<TL> left, Action<TR> right)
        {
            if (this.IsLeft)
                left(this.Left);
            else
                right(this.Right);
        }

        /// <inheritdoc/>
        public IEnumerator<TR> GetEnumerator() => this.RightOption.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public bool Equals(Either<TL, TR> that) => this.LeftOption == that.LeftOption && this.RightOption == that.RightOption;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Either<TL, TR> other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => this.LeftOption.GetHashCode() + this.RightOption.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => this.Match(x => $"Left<{typeof(TL).Name}>({x})", x => $"Right<{typeof(TR).Name}>({x})");
    }

    /// <summary>
    /// Companion class of <see cref="Either{TL,TR}"/>.
    /// </summary>
    [PublicAPI]
    public static class Either
    {
        /// <summary>
        /// Explicit conversion of a <typeparamref name="TL"/> type object to an either.
        /// </summary>
        /// <param name="left">The value to wrap in an <see cref="Either{TL,TR}"/>.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <returns>An <see cref="Either{TL,TR}"/>.</returns>
        public static Either<TL, TR> OfLeft<TL, TR>(TL left)
            => left;

        /// <summary>
        /// Explicit conversion of a <typeparamref name="TR"/> type object to an either.
        /// </summary>
        /// <param name="right">The value to wrap in an <see cref="Either{TL,TR}"/>.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <returns>An <see cref="Either{TL,TR}"/>.</returns>
        public static Either<TL, TR> OfRight<TL, TR>(TR right)
            => right;

        /// <summary>
        /// Applies the <paramref name="selector"/> to the right side and flattens.
        /// </summary>
        /// <param name="this">The <see cref="IEither{TL,TR}"/> to select on.</param>
        /// <param name="selector">The function to apply to the right side of the <see cref="IEither{TL,TR}"/>.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}"/>.</typeparam>
        /// <typeparam name="TOut">The output type of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IEither{TL,TR}"/>.</returns>
        public static IEither<TL, TOut> SelectMany<TL, TR, TOut>(this IEither<TL, TR> @this, Func<TR, IEither<TL, TOut>> selector)
            => @this.Match(x => OfLeft<TL, TOut>(x), selector);
    }
}