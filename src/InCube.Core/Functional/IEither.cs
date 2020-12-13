using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Represents the union of two distinct types, namely, <typeparamref name="TL" /> and <typeparamref name="TR" />.
    /// This union type is right-biased if used as an <see cref="IEnumerable" />.
    /// </summary>
    /// <typeparam name="TL">The <see cref="Left" /> type.</typeparam>
    /// <typeparam name="TR">The <see cref="Right" /> type.</typeparam>
    [PublicAPI]
    public interface IEither<out TL, out TR> : IEnumerable<TR>
    {
        /// <summary>
        /// Gets a value indicating whether or not the instance is of kind left.
        /// </summary>
        bool IsLeft { get; }

        /// <summary>
        /// Gets a value indicating whether or not the instance is of kind right.
        /// </summary>
        bool IsRight { get; }

        /// <summary>
        /// Gets the 'left' value.
        /// </summary>
        TL Left { get; }

        /// <summary>
        /// Gets the 'right' value.
        /// </summary>
        TR Right { get; }

        /// <summary>
        /// Gets 'left' value as an option.
        /// </summary>
        IOption<TL> LeftOption { get; }

        /// <summary>
        /// Gets 'right' value as an option.
        /// </summary>
        IOption<TR> RightOption { get; }

        /// <summary>
        /// Gets the right or left type depending if the either is right or left respectively.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Maps either left or right value to output type.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="left">The left mapping.</param>
        /// <param name="right">The right mapping.</param>
        /// <returns>An output type object.</returns>
        TOut Match<TOut>(Func<TL, TOut> left, Func<TR, TOut> right);

        /// <summary>
        /// Maps right to output type and preserves left.
        /// </summary>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="f">Mapping from right type to output type.</param>
        /// <returns>An <see cref="Either{TL,TR}" /> of left and output types.</returns>
        IEither<TL, TOut> Select<TOut>(Func<TR, TOut> f);

        /// <summary>
        /// Performs an action on right.
        /// </summary>
        /// <param name="right">Action to perform on right.</param>
        void ForEach(Action<TR> right);

        /// <summary>
        /// Performs an action on right or left.
        /// </summary>
        /// <param name="left">The left action.</param>
        /// <param name="right">The right action.</param>
        void ForEach(Action<TL> left, Action<TR> right);
    }
}