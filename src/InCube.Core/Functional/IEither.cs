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
}