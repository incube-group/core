using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Invariant method extensions of <see cref="IOption{T}" />.
    /// </summary>
    /// <typeparam name="T">Type of the wrapped optional.</typeparam>
    /// <typeparam name="TOpt">Type of the option.</typeparam>
    [PublicAPI]
    public interface IInvariantOption<T, TOpt> : IOption<T>, IEquatable<TOpt>
        where TOpt : IInvariantOption<T, TOpt>
    {
        /// <summary>
        /// Attempts to generate back up value for option, if necessary.
        /// </summary>
        /// <param name="default">Generator for backup value.</param>
        /// <returns>An <see cref="IInvariantOption{T,TOpt}" />.</returns>
        TOpt OrElse(Func<TOpt> @default);

        /// <summary>
        /// Attempts to fill option with back up value, if necessary.
        /// </summary>
        /// <param name="default">The backup value.</param>
        /// <returns>An <see cref="IInvariantOption{T,TOpt}" />.</returns>
        TOpt OrElse(TOpt @default);

        /// <summary>
        /// Extracts value of <see cref="IInvariantOption{T,TOpt}" /> or yields default value.
        /// </summary>
        /// <param name="default">Generator for default value.</param>
        /// <returns>An object of the underlying type.</returns>
        T GetValueOr(Func<T> @default);

        /// <summary>
        /// Extracts value of <see cref="IInvariantOption{T,TOpt}" /> or yields default value.
        /// </summary>
        /// <param name="default">The default value.</param>
        /// <returns>An object of the underlying type.</returns>
        T GetValueOrDefault(T @default);

        /// <summary>
        /// Checks whether <see cref="IInvariantOption{T,TOpt}" /> has value and value is equal to given element.
        /// </summary>
        /// <param name="elem">The element to compare option value with if it exists.</param>
        /// <returns>
        /// True if <see cref="IInvariantOption{T,TOpt}" /> has value and it is equal to the provided element, false
        /// otherwise.
        /// </returns>
        bool Contains(T elem);

        /// <summary>
        /// Checks whether <see cref="IInvariantOption{T,TOpt}" /> has value and value is equal to given element, using provided
        /// comparer.
        /// </summary>
        /// <param name="elem">The element to compare option value with if it exists.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns>
        /// True if <see cref="IInvariantOption{T,TOpt}" /> has value and it is equal to the provided element, false
        /// otherwise.
        /// </returns>
        bool Contains(T elem, IEqualityComparer<T> comparer);

        /// <summary>
        /// Filters <see cref="IInvariantOption{T,TOpt}" /> to None if element doesn't satisfy predicate.
        /// </summary>
        /// <param name="predicate">Predicate to check.</param>
        /// <returns>An <see cref="IInvariantOption{T,TOpt}" />.</returns>
        new TOpt Where(Func<T, bool> predicate);
    }
}