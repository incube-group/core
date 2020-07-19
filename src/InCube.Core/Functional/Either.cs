using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    [PublicAPI]
    public readonly struct Either<TL, TR> : IEither<TL, TR>, IEquatable<Either<TL, TR>>
    {
        private readonly object value;

        private Either(object value, bool left, bool right)
        {
            this.value = value;
            this.IsLeft = left;
            this.IsRight = right;
        }

        public bool IsLeft { get; }

        public bool IsRight { get; }

        public TL Left =>
            this.IsLeft ? (TL)this.value : 
            throw new NotSupportedException($"Either is not Left<{typeof(TL)}>, but Right<{typeof(TR)}>");

        public TR Right =>
            this.IsRight ? (TR)this.value : 
            throw new NotSupportedException($"Either is not Right<{typeof(TR)}>, but Left<{typeof(TL)}>");

        public Option<TL> LeftOption => this.IsLeft ? Option.Some((TL)this.value) : Option.None;

        public Option<TR> RightOption => this.IsRight ? Option.Some((TR)this.value) : Option.None;

        public Type Type => this.IsLeft ? typeof(TL) : typeof(TR);

        IOption<TL> IEither<TL, TR>.LeftOption => this.LeftOption;

        IOption<TR> IEither<TL, TR>.RightOption => this.RightOption;

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

        public T Match<T>(Func<TL, T> left, Func<TR, T> right) => this.IsLeft ? left(this.Left) : right(this.Right);

        IEither<TL, TOut> IEither<TL, TR>.Select<TOut>(Func<TR, TOut> f) => this.Select(f);

        public Either<TL, TOut> Select<TOut>(Func<TR, TOut> f) => this.Match<Either<TL, TOut>>(left => left, right => f(right));

        public Either<TL, TOut> SelectMany<TOut>(Func<TR, Either<TL, TOut>> f) => this.Match(left => left, f);

        public void ForEach(Action<TR> right)
        {
            if (this.IsRight) right(this.Right);
        }

        public void ForEach(Action<TL> left, Action<TR> right)
        {
            if (this.IsLeft) left(this.Left);
            else right(this.Right);
        }

        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);

        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);

        public IEnumerator<TR> GetEnumerator() => this.RightOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Equals(Either<TL, TR> that)
        {
            return this.LeftOption == that.LeftOption && this.RightOption == that.RightOption;
        }

        public override bool Equals(object obj) => 
            obj is Either<TL, TR> other && this.Equals(other);

        public override int GetHashCode() => this.LeftOption.GetHashCode() + this.RightOption.GetHashCode();

        public override string ToString() => this.Match(left: x => $"Left<{typeof(TL).Name}>({x})", right: x => $"Right<{typeof(TR).Name}>({x})");

        public static bool operator ==(Either<TL, TR> left, Either<TL, TR> right) => left.Equals(right);

        public static bool operator !=(Either<TL, TR> left, Either<TL, TR> right) => !left.Equals(right);
    }

    [PublicAPI]
    public static class Either
    {
        public static Either<TL, TR> OfLeft<TL, TR>(TL left) => left;

        public static Either<TL, TR> OfRight<TL, TR>(TR right) => right;

        // ReSharper disable once ConvertClosureToMethodGroup
        public static IEither<TL, TOut> SelectMany<TL, TR, TOut>(this IEither<TL, TR> @this, Func<TR, IEither<TL, TOut>> f) => @this.Match(l => OfLeft<TL, TOut>(l), f);
    }
}