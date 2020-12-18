namespace InCube.Core.Functional
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Companion class of <see cref="Either{TL,TR}" />.
    /// </summary>
    [PublicAPI]
    public static class Eithers
    {
        /// <summary>
        /// Explicit conversion of a <typeparamref name="TL" /> type object to an either.
        /// </summary>
        /// <param name="left">The value to wrap in an <see cref="Either{TL,TR}" />.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <returns>An <see cref="Either{TL,TR}" />.</returns>
        public static Either<TL, TR> OfLeft<TL, TR>(TL left)
            => left;

        /// <summary>
        /// Explicit conversion of a <typeparamref name="TR" /> type object to an either.
        /// </summary>
        /// <param name="right">The value to wrap in an <see cref="Either{TL,TR}" />.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <returns>An <see cref="Either{TL,TR}" />.</returns>
        public static Either<TL, TR> OfRight<TL, TR>(TR right)
            => right;

        /// <summary>
        /// Applies the <paramref name="selector" /> to the right side and flattens.
        /// </summary>
        /// <param name="this">The <see cref="IEither{TL,TR}" /> to select on.</param>
        /// <param name="selector">The function to apply to the right side of the <see cref="IEither{TL,TR}" />.</param>
        /// <typeparam name="TL">The left type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <typeparam name="TR">The right type of the <see cref="Either{TL,TR}" />.</typeparam>
        /// <typeparam name="TOut">The output type of the <paramref name="selector" />.</typeparam>
        /// <returns>An <see cref="IEither{TL,TR}" />.</returns>
        public static IEither<TL, TOut> SelectMany<TL, TR, TOut>(this IEither<TL, TR> @this, Func<TR, IEither<TL, TOut>> selector)
            => @this.Match(x => OfLeft<TL, TOut>(x), selector);

        /// <summary>
        /// Applies <paramref name="selector"/> to all the right sides, if all the <see cref="IEither{TL,TR}"/>s are right.
        /// </summary>
        /// <param name="either1">A first <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either2">A second <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="selector">A selector to apply to the right sides.</param>
        /// <typeparam name="TL">Type of the left sides.</typeparam>
        /// <typeparam name="TR1">Type of the first right side.</typeparam>
        /// <typeparam name="TR2">Type of the second right side.</typeparam>
        /// <typeparam name="TOut">Type of the output.</typeparam>
        /// <returns>A <see cref="Maybe{T}"/> of <typeparamref name="TOut"/>.</returns>
        public static Maybe<TOut> AllRight<TL, TR1, TR2, TOut>(
            this IEither<TL, TR1> either1,
            IEither<TL, TR2> either2,
            Func<TR1, TR2, TOut> selector)
            where TOut : class
        {
            return (either1, either2) switch
            {
                ({ IsRight: true }, { IsRight: true }) => selector(either1.Right, either2.Right),
                _ => Maybe<TOut>.None,
            };
        }

        /// <summary>
        /// Applies <paramref name="selector"/> to all the right sides, if all the <see cref="IEither{TL,TR}"/>s are right.
        /// </summary>
        /// <param name="either1">A first <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either2">A second <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either3">A third <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="selector">A selector to apply to the right sides.</param>
        /// <typeparam name="TL">Type of the left sides.</typeparam>
        /// <typeparam name="TR1">Type of the first right side.</typeparam>
        /// <typeparam name="TR2">Type of the second right side.</typeparam>
        /// <typeparam name="TR3">Type of the third right side.</typeparam>
        /// <typeparam name="TOut">Type of the output.</typeparam>
        /// <returns>A <see cref="Maybe{T}"/> of <typeparamref name="TOut"/>.</returns>
        public static Maybe<TOut> AllRight<TL, TR1, TR2, TR3, TOut>(
            this IEither<TL, TR1> either1,
            IEither<TL, TR2> either2,
            IEither<TL, TR3> either3,
            Func<TR1, TR2, TR3, TOut> selector)
            where TOut : class
        {
            return (either1, either2, either3) switch
            {
                ({ IsRight: true }, { IsRight: true }, { IsRight: true }) => selector(either1.Right, either2.Right, either3.Right),
                _ => Maybe<TOut>.None,
            };
        }

        /// <summary>
        /// Applies <paramref name="selector"/> to all the right sides, if all the <see cref="IEither{TL,TR}"/>s are right.
        /// </summary>
        /// <param name="either1">A first <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either2">A second <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either3">A third <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="either4">A fourth <see cref="IEither{TL,TR}"/>.</param>
        /// <param name="selector">A selector to apply to the right sides.</param>
        /// <typeparam name="TL">Type of the left sides.</typeparam>
        /// <typeparam name="TR1">Type of the first right side.</typeparam>
        /// <typeparam name="TR2">Type of the second right side.</typeparam>
        /// <typeparam name="TR3">Type of the third right side.</typeparam>
        /// <typeparam name="TR4">Type of the fourth right side.</typeparam>
        /// <typeparam name="TOut">Type of the output.</typeparam>
        /// <returns>A <see cref="Maybe{T}"/> of <typeparamref name="TOut"/>.</returns>
        public static Maybe<TOut> AllRight<TL, TR1, TR2, TR3, TR4, TOut>(
            this IEither<TL, TR1> either1,
            IEither<TL, TR2> either2,
            IEither<TL, TR3> either3,
            IEither<TL, TR4> either4,
            Func<TR1, TR2, TR3, TR4, TOut> selector)
            where TOut : class
        {
            return (either1, either2, either3, either4) switch
            {
                ({ IsRight: true }, { IsRight: true }, { IsRight: true }, { IsRight: true }) => selector(either1.Right, either2.Right, either3.Right, either4.Right),
                _ => Maybe<TOut>.None,
            };
        }
    }
}