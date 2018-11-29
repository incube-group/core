using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Represents the union of two distinct types, namely, <typeparamref name="TL"/> and <typeparamref name="TR"/>.
    ///
    /// This union type is right-biased if used as an <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="TL">The <see cref="Left"/> type.</typeparam>
    /// <typeparam name="TR">The <see cref="Right"/> type.</typeparam>
    public interface IEither<out TL, out TR> : IEnumerable<TR>
    {
        bool IsLeft { get; }

        bool IsRight { get; }

        TL Left { get; }

        TR Right { get; }

        IOption<TL> LeftOption { get; }

        IOption<TR> RightOption { get; }

        T Match<T>(Func<TL, T> left, Func<TR, T> right);

        IEither<TL, TOut> Select<TOut>(Func<TR, TOut> f);

        void ForEach(Action<TR> right);

        void ForEach(Action<TL> left, Action<TR> right);

        Type Type { get; }
    }

    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    public readonly struct Either<TL, TR> : IEither<TL, TR>, IEquatable<Either<TL, TR>>
    {
        private readonly object value;

        private Either(object value, bool left, bool right)
        {
            this.value = value;
            IsLeft = left;
            IsRight = right;
        }

        public bool IsLeft { get; }

        public bool IsRight { get; }

        public TL Left => IsLeft ? (TL)this.value : 
            throw new NotSupportedException($"Either is not Left<{typeof(TL)}>, but Right<{typeof(TR)}>");

        public TR Right => IsRight ? (TR)this.value : 
            throw new NotSupportedException($"Either is not Right<{typeof(TR)}>, but Left<{typeof(TL)}>");

        public Option<TL> LeftOption => IsLeft ? Option.Some((TL)this.value) : Option.None;

        public Option<TR> RightOption => IsRight ? Option.Some((TR)this.value) : Option.None;

        public Type Type => IsLeft ? typeof(TL) : typeof(TR);

        IOption<TL> IEither<TL, TR>.LeftOption => LeftOption;

        IOption<TR> IEither<TL, TR>.RightOption => RightOption;

        /// <summary>
        /// Construct an Either for a <see cref="Left"/> value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns></returns>
        public Either(TL value) : this(value, left: true, right: false)
        {}

        /// <summary>
        /// Construct an Either for a <see cref="Right"/> value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns></returns>
        public Either(TR value) : this(value, left: false, right: true)
        {}

        public T Match<T>(Func<TL, T> left, Func<TR, T> right) =>
            IsLeft ? left(Left) : right(Right);

        IEither<TL, TOut> IEither<TL, TR>.Select<TOut>(Func<TR, TOut> f) => Select(f);

        public Either<TL, TOut> Select<TOut>(Func<TR, TOut> f) =>
            Match<Either<TL, TOut>>(left => left, right => f(right));

        public Either<TL, TOut> SelectMany<TOut>(Func<TR, Either<TL, TOut>> f) => 
            Match(left => left, f);

        public void ForEach(Action<TR> right)
        {
            if (IsRight)
            {
                right(Right);
            }
        }

        public void ForEach(Action<TL> left, Action<TR> right)
        {
            if (IsLeft)
            {
                left(Left);
            }
            else
            {
                right(Right);
            }
        }

        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);

        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);

        public IEnumerator<TR> GetEnumerator() => RightOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Either<TL, TR> that)
        {
            return this.LeftOption == that.LeftOption && this.RightOption == that.RightOption;
        }

        public override bool Equals(object obj) => 
            obj is Either<TL, TR> other && Equals(other);

        public override int GetHashCode() => 
            LeftOption.GetHashCode() + RightOption.GetHashCode();

        public static bool operator ==(Either<TL, TR> left, Either<TL, TR> right) => left.Equals(right);

        public static bool operator !=(Either<TL, TR> left, Either<TL, TR> right) => !left.Equals(right);
    }

    public static class Either
    {
        public static Either<TL, TR> OfLeft<TL, TR>(TL left) => left;

        public static Either<TL, TR> OfRight<TL, TR>(TR right) => right;

        public static IEither<TL, TOut> SelectMany<TL, TR, TOut>(
            this IEither<TL, TR> @this,
            Func<TR, IEither<TL, TOut>> f) =>
            
            // ReSharper disable once ConvertClosureToMethodGroup
            @this.Match(l => OfLeft<TL, TOut>(l), f);
    }
}