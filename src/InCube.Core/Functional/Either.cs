using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Represents the union of two distinct types, namely, <typeparamref name="TL"/> and <typeparamref name="TR"/>.
    ///
    /// This union type is right-biased if used as an <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="TL">The <see cref="Left"/> type.</typeparam>
    /// <typeparam name="TR">The <see cref="Right"/> type.</typeparam>
    public interface IEither<out TL, out TR>: IEnumerable<TR>
    {
        bool IsLeft { get; }
        bool IsRight { get; }

        TL Left { get; }

        TR Right { get; }

        IOption<TL> LeftOption { get; }

        IOption<TR> RightOption { get; }

        T Match<T>(Func<TL, T> left, Func<TR, T> right);

        void ForEach(Action<TL> left, Action<TR> right);

        Type Type { get; }
    }

    public readonly struct Either<TL, TR> : IEither<TL, TR>
    {
        private readonly object _value;

        private Either(object value, bool left, bool right)
        {
            _value = value;
            IsLeft = left;
            IsRight = right;
        }

        public bool IsLeft { get; }
        public bool IsRight { get; }

        public TL Left => IsLeft ? (TL) _value : 
            throw new NotSupportedException($"not a {typeof(TL)}, but a {typeof(TR)}");

        public TR Right => IsRight ? (TR) _value : 
            throw new NotSupportedException($"not a {typeof(TR)}, but a {typeof(TL)}");

        public Option<TL> LeftOption => IsLeft ? Options.Some((TL) _value) : Options.None;

        public Option<TR> RightOption => IsRight ? Options.Some((TR) _value) : Options.None;

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
    }

}