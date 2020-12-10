using System;
using JetBrains.Annotations;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Extension methods for <see cref="Maybe{T}" />.
    /// </summary>
    [PublicAPI]
    public static class Maybes
    {
        /// <summary>
        /// Instantiates an empty <see cref="Maybe{T}" />.
        /// </summary>
        public static readonly Maybe<Nothing> None = Maybe<Nothing>.None;

        #region Conversion

        /// <summary>
        /// Converts reference type to a <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Maybe{T}" />.</typeparam>
        /// <param name="value">The value of the <see cref="Maybe{T}" />.</param>
        /// <returns>A <see cref="Maybe{T}" />.</returns>
        public static Maybe<T> ToMaybe<T>(this T? value)
            where T : class =>
            value;

        #endregion

        #region Construction

        /// <summary>
        /// Instantiates a non-empty <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Maybe{T}" />.</typeparam>
        /// <param name="value">Value of the <see cref="Maybe{T}" />.</param>
        /// <returns>A new <see cref="Maybe{T}" />.</returns>
        public static Maybe<T> Some<T>(T value)
            where T : class =>
            CheckNotNull(value, nameof(value));

        #endregion

        #region Flattening

        /// <summary>
        /// Converts <see cref="Option{T}" /> of <see cref="Maybe{T}" /> of a reference type to a <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="self">The <see cref="Option{T}" /> nested <see cref="Maybe{T}" />.</param>
        /// <returns>A <see cref="Maybe{T}" />.</returns>
        public static Maybe<T> Flatten<T>(this in Option<Maybe<T>> self)
            where T : class =>
            self.AsAny?.Apply(x => x.Value) ?? Maybe<T>.None;

        /// <summary>
        /// Converts nullable nested <see cref="Maybe{T}" /> to a <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Maybe{T}" />.</typeparam>
        /// <param name="self">The input nullable nested <see cref="Maybe{T}" />.</param>
        /// <returns>A <see cref="Maybe{T}" />.</returns>
        public static Maybe<T> Flatten<T>(this in Maybe<T>? self)
            where T : class =>
            self ?? Maybe<T>.None;

        #endregion

        #region Projection

        /// <summary>
        /// Maps a <see cref="Maybe{T}" /> to a nullable value type.
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping.</typeparam>
        /// <typeparam name="TOut">Output type of mapping.</typeparam>
        /// <param name="this">Input <see cref="Maybe{T}" />.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A nullable output value type.</returns>
        public static TOut? Select<TIn, TOut>(this Maybe<TIn> @this, Func<TIn, TOut> f)
            where TIn : class
            where TOut : struct =>
            @this.GetValueOrDefault()?.Apply(f);

        /// <summary>
        /// Maps <see cref="Maybe{T}" /> to a nullable output type, when mapping can output null.
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping.</typeparam>
        /// <typeparam name="TOut">Output type of mapping.</typeparam>
        /// <param name="this">Input <see cref="Maybe{T}" />.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A nullable output value type.</returns>
        public static TOut? SelectMany<TIn, TOut>(this Maybe<TIn> @this, Func<TIn, TOut?> f)
            where TIn : class
            where TOut : struct =>
            @this.GetValueOrDefault()?.Apply(f);

        /// <summary>
        /// Maps nullable value type to a <see cref="Maybe{T}" />.
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping.</typeparam>
        /// <typeparam name="TOut">Output type of mapping.</typeparam>
        /// <param name="self">Input nullable value type.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A <see cref="Maybe{T}" /> of the output type.</returns>
        public static Maybe<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f)
            where TIn : struct
            where TOut : class =>
            self?.Apply(f);

        /// <summary>
        /// Maps nullable value type to a <see cref="Maybe{T}" /> when mapping can output None.
        /// </summary>
        /// <typeparam name="TIn">Input type of mapping.</typeparam>
        /// <typeparam name="TOut">Output type of mapping.</typeparam>
        /// <param name="self">Input nullable value.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A <see cref="Maybe{T}" /> of the output type.</returns>
        public static Maybe<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Maybe<TOut>> f)
            where TIn : struct
            where TOut : class =>
            self?.Apply(f) ?? None;

        #endregion
    }
}