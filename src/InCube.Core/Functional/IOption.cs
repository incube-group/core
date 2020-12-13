using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// A covariant version of <see cref="Option{T}"/>, <see cref="Maybes"/> and <see cref="Nullable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type wrapped in the option.</typeparam>
    [PublicAPI]
    public interface IOption<out T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets a value indicating whether or not the <see cref="IOption{T}"/> has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the value of the option.
        /// </summary>
        T? Value { get; }

        /// <summary>
        /// Maps the option in one of two ways -- <paramref name="some"/> or <paramref name="none"/> -- depending on whether the option has a value or not respectively.
        /// </summary>
        /// <param name="none">The function to apply in case the option has a value.</param>
        /// <param name="some">The function to apply in case the option does not have a value.</param>
        /// <typeparam name="TOut">The outputy type of both functions.</typeparam>
        /// <returns>A <typeparamref name="TOut"/>.</returns>
        TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some);

        /// <summary>
        /// Gets the value of the option, or the default value for <see cref="T"/>.
        /// </summary>
        /// <returns>A <see cref="T"/>.</returns>
        [Pure]
        T? GetValueOrDefault();

        /// <summary>
        /// Applies the <paramref name="selector"/> to the <see cref="Value"/> if it is not null.
        /// </summary>
        /// <param name="selector">The function to apply.</param>
        /// <typeparam name="TOut">The output type of the function.</typeparam>
        /// <returns>An <see cref="IOption{T}"/> of <typeparamref name="TOut"/>.</returns>
        IOption<TOut> Select<TOut>(Func<T, TOut> selector);

        /// <summary>
        /// Applies the <paramref name="selector"/> and flattens.
        /// </summary>
        /// <param name="selector">The selector to apply.</param>
        /// <typeparam name="TOut">The output of the <paramref name="selector"/>.</typeparam>
        /// <returns>An <see cref="IOption{T}"/> of <typeparamref name="TOut"/>.</returns>
        IOption<TOut> SelectMany<TOut>(Func<T, IOption<TOut>> selector);

        /// <summary>
        /// Returns a value indicating whether or not the option has a value or not.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool Any();

        /// <summary>
        /// Returns a value indicating whether or not the <see cref="IOption{T}"/> has a value, and this value satisfies the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate that the value has to satisfy.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        bool Any(Func<T, bool> predicate);

        /// <summary>
        /// Returns a value indicating whether or not the <see cref="IOption{T}"/> doesn't have a value, or the value satisfies the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate that the value has to satisfy, if it exists.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        bool All(Func<T, bool> predicate);

        /// <summary>
        /// Performs the <paramref name="action"/> if the <see cref="IOption{T}"/> has a value.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        void ForEach(Action<T> action);

        /// <summary>
        /// Performs the <paramref name="some"/> if the <see cref="IOption{T}"/> has a value, otherwise performs <paramref name="none"/>.
        /// </summary>
        /// <param name="none">The action to perform in case the <see cref="IOption{T}"/> doesn't have a value.</param>
        /// <param name="some">The action to perform in case the <see cref="IOption{T}"/> has a value.</param>
        void ForEach(Action none, Action<T> some);

        /// <summary>
        /// Excludes value from <see cref="IOption{T}"/> if value doesn't satisfy the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate that the value has to satisfy.</param>
        /// <returns>An <see cref="IOption{T}"/>.</returns>
        IOption<T> Where(Func<T, bool> predicate);
    }
}