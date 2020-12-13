using System;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Wrapper around a result that can be either some type of an exception.
    /// </summary>
    /// <typeparam name="T">Type of the wrapped object.</typeparam>
    [PublicAPI]
    public interface ITry<out T> : IOption<T>
    {
        /// <summary>
        /// Gets the exception.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets <see cref="Option{T}.None" /> if exception, else <see cref="Option.Some{T}(T)" />.
        /// </summary>
        IOption<T> AsOption { get; }

        /// <summary>
        /// Gets either the <see cref="Exception" /> or the value.
        /// </summary>
        IEither<Exception, T> AsEither { get; }

        /// <summary>
        /// Checks whether or not the <see cref="ITry{T}" /> has failed.
        /// </summary>
        /// <returns>The exception.</returns>
        Try<Exception> Failed();

        /// <summary>
        /// Applies one of the two selectors <paramref name="failure" /> or <paramref name="success" /> depending on whether or not
        /// the <see cref="ITry{T}" /> is a failure or a success respectively.
        /// </summary>
        /// <param name="failure">The selector in case of failure.</param>
        /// <param name="success">The selector in case of success.</param>
        /// <typeparam name="TOut">The output type of both selectors.</typeparam>
        /// <returns>A <typeparamref name="TOut" /> instance.</returns>
        TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success);

        /// <summary>
        /// Applies the <paramref name="selector" /> to the <see cref="ITry{T}" /> if it is a success.
        /// </summary>
        /// <param name="selector">The function to apply.</param>
        /// <typeparam name="TOut">The output type of the function to apply.</typeparam>
        /// <returns>A <typeparamref name="TOut" /> instance.</returns>
        new ITry<TOut> Select<TOut>(Func<T, TOut> selector);

        /// <summary>
        /// Applies the <paramref name="selector" /> to the <see cref="ITry{T}" /> if it is a success to return another
        /// <see cref="ITry{T}" />, and then flattens the result.
        /// </summary>
        /// <param name="selector">The selector, which can fail again.</param>
        /// <typeparam name="TOut">The output type of the selector.</typeparam>
        /// <returns>A <see cref="ITry{T}" /> of <typeparamref name="TOut" />.</returns>
        ITry<TOut> SelectMany<TOut>(Func<T, ITry<TOut>> selector);

        /// <summary>
        /// Applies one of the two selectors <paramref name="failure" /> or <paramref name="success" /> depending on whether or not
        /// the <see cref="ITry{T}" /> is a failure or a success respectively.
        /// Both of these can fail (again).
        /// </summary>
        /// <param name="failure">The selector in case of failure.</param>
        /// <param name="success">The selector in case of success.</param>
        /// <typeparam name="TOut">The output type of both selectors.</typeparam>
        /// <returns>A <see cref="ITry{T}" /> of <typeparamref name="TOut" />.</returns>
        ITry<TOut> SelectMany<TOut>(Func<Exception, ITry<TOut>> failure, Func<T, ITry<TOut>> success);

        /// <summary>
        /// Fails if <paramref name="predicate" /> is false or if this was already a failure..
        /// </summary>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>A new <see cref="ITry{T}" />.</returns>
        new ITry<T> Where(Func<T, bool> predicate);

        /// <summary>
        /// Performs one of the two actions <paramref name="failure" /> or <paramref name="success" /> depending on whether or not
        /// the <see cref="ITry{T}" /> is a failure or a success respectively.
        /// </summary>
        /// <param name="failure">The action in case of failure.</param>
        /// <param name="success">The action in case of success.</param>
        void ForEach(Action<Exception> failure, Action<T> success);
    }
}