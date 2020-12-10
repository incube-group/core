using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Functional representation of a try/catch block. Holds either the result of the computation or an exception.
    /// </summary>
    /// <typeparam name="T">Type of success.</typeparam>
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    [PublicAPI]
    public readonly struct Try<T> : ITry<T>, IInvariantOption<T, Try<T>>
    {
        private readonly Maybe<Exception> exception;

        internal Try(T value)
        {
            this.AsOption = Option.Some(value);
            this.exception = Maybes.None;
        }

        internal Try(Exception exception)
        {
            this.AsOption = Option.None;
            this.exception = Maybes.Some(exception);
        }

        /// <summary>
        /// Gets the <see cref="Try{T}" /> as an option : <see cref="Option{T}.None" /> when exception,
        /// <see cref="Option.Some{T}(T)" /> otherwise.
        /// </summary>
        public Option<T> AsOption { get; }

        /// <inheritdoc />
        IOption<T> ITry<T>.AsOption => this.AsOption;

        /// <summary>
        /// Gets the <see cref="Try{T}" /> an an either: left in case of an exception, right otherwise.
        /// </summary>
        public Either<Exception, T> AsEither => this;

        /// <inheritdoc />
        public int Count => this.AsOption.Count;

        /// <inheritdoc />
        IEither<Exception, T> ITry<T>.AsEither => this.AsEither;

        /// <inheritdoc />
        public bool HasValue => this.AsOption.HasValue;

        /// <inheritdoc />
        public T Value
        {
            get
            {
                var @this = this;
                return this.AsOption.GetValueOr(() => throw new InvalidOperationException("Try failed", @this.Exception));
            }
        }

        /// <summary>
        /// Gets the exception, in case it's an exception, otherwise throws.
        /// </summary>
        public Exception Exception => this.HasValue ? throw new InvalidOperationException("Try is success") : this.exception.Value;

        /// <summary>
        /// <see cref="IReadOnlyList{T}.this" />.
        /// </summary>
        /// <see cref="Nullable{T}.Value" />
        /// <param name="index">Has to be zero.</param>
        /// <exception cref="InvalidOperationException">
        /// If this <see cref="Try" /> has an exception or the
        /// <paramref name="index" /> != 0.
        /// </exception>
        public T this[int index] => index == 0 ? this.Value : throw new InvalidOperationException();

        public static implicit operator Option<T>(Try<T> t) => t.AsOption;

        public static implicit operator Either<Exception, T>(Try<T> t) => t.Match(Either.OfLeft<Exception, T>, Either.OfRight<Exception, T>);

        public static bool operator ==(Try<T> left, Try<T> right) => left.Equals(right);

        public static bool operator !=(Try<T> left, Try<T> right) => !left.Equals(right);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this.AsOption.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool Equals(Try<T> that) => this.AsOption.Equals(that.AsOption) && this.exception.Equals(that.exception);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Try<T> other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.AsOption.GetHashCode() + this.exception.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.Match(success: x => $"Success({x})", failure: ex => $"Failure({ex})");

        /// <inheritdoc />
        public Try<Exception> Failed()
        {
            var self = this; // in order to capture this in the following lambda
            return Try.Do(() => self.Exception);
        }

        /// <inheritdoc />
        public TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success) => this.HasValue ? success(this.AsOption.Value) : failure(this.Exception);

        /// <summary>
        /// Handles failure and success case by applying <paramref name="failure" /> or <paramref name="success" /> respectively.
        /// </summary>
        /// <typeparam name="TOut">The output type of either of the selectors.</typeparam>
        /// <param name="failure">The selector to apply in case of failure.</param>
        /// <param name="success">The selector to apply in case of success.</param>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public Task<TOut> MatchAsync<TOut>(Func<Exception, Task<TOut>> failure, Func<T, Task<TOut>> success) => this.HasValue ? success(this.AsOption.Value) : failure(this.Exception);

        /// <inheritdoc />
        TOut IOption<T>.Match<TOut>(Func<TOut> none, Func<T, TOut> some) => this.Match(_ => none(), some);

        /// <inheritdoc />
        public T GetValueOrDefault() => this.AsOption.GetValueOrDefault();

        /// <summary>
        /// Handles the exception using <see cref="@default" />.
        /// </summary>
        /// <param name="default">The handler for the exception.</param>
        /// <returns>A <see cref="T" />.</returns>
        public T GetValueOrDefault(Func<Exception, T> @default) => this.HasValue ? this.Value : @default(this.Exception);

        /// <inheritdoc />
        public T GetValueOr(Func<T> @default) => this.AsOption.GetValueOr(@default);

        /// <inheritdoc />
        public T GetValueOrDefault(T @default) => this.AsOption.GetValueOrDefault(@default);

        /// <inheritdoc />
        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> selector) => this.AsOption.Select(selector);

        /// <inheritdoc />
        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> selector) => this.AsOption.SelectMany(x => selector(x).ToOption());

        /// <inheritdoc />
        ITry<TOut> ITry<T>.Select<TOut>(Func<T, TOut> selector) => this.Select(selector);

        /// <inheritdoc />
        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<T, ITry<TOut>> selector) => this.SelectMany(x => selector(x).ToTry());

        /// <inheritdoc />
        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<Exception, ITry<TOut>> failure, Func<T, ITry<TOut>> success) => this.SelectMany(x => failure(x).ToTry(), x => success(x).ToTry());

        /// <inheritdoc cref="ITry{T}.Select{TOut}" />
        public Try<TOut> Select<TOut>(Func<T, TOut> f)
            where TOut : notnull =>
            this.Match(Try.Failure<TOut>, value => Try.Do(() => f(value)));

        /// <inheritdoc cref="ITry{T}.Select{TOut}" />
        public Task<Try<TOut>> Select<TOut>(Func<T, Task<TOut>> f)
            where TOut : notnull =>
            this.MatchAsync(ex => Task.FromResult(Try.Failure<TOut>(ex)), value => Try.DoAsync(() => f(value)));

        /// <inheritdoc cref="ITry{T}.SelectMany{TOut}(System.Func{T,InCube.Core.Functional.ITry{TOut}})" />
        public Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> f)
            where TOut : notnull =>
            this.Match(Try.Failure<TOut>, f);

        /// <inheritdoc cref="ITry{T}.SelectMany{TOut}(System.Func{T,InCube.Core.Functional.ITry{TOut}})" />
        public Task<Try<TOut>> SelectMany<TOut>(Func<T, Task<Try<TOut>>> f)
            where TOut : notnull =>
            this.MatchAsync(ex => Task.FromResult(Try.Failure<TOut>(ex)), f);

        /// <inheritdoc cref="ITry{T}.SelectMany{TOut}(System.Func{T,InCube.Core.Functional.ITry{TOut}})" />
        public Try<TOut> SelectMany<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success)
            where TOut : notnull =>
            this.Match(failure, success);

        /// <summary>
        /// Performs the asynchronous action <paramref name="none" /> or <paramref name="some" /> depending on whether the
        /// <see cref="Try{T}" /> is an exception or not.
        /// </summary>
        /// <param name="none">The action to perform when exception.</param>
        /// <param name="some">The action to perform when not exception.</param>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public Task ForEachAsync(Func<Task> none, Func<T, Task> some) => this.AsOption.ForEachAsync(none, some);

        /// <inheritdoc />
        public Try<T> Where(Func<T, bool> predicate) => !this.HasValue || predicate(this.AsOption.Value) ? this : Try.Failure<T>(new ArgumentException("Predicate does not hold for value " + this.AsOption.Value));

        /// <inheritdoc />
        ITry<T> ITry<T>.Where(Func<T, bool> predicate) => this.Where(predicate);

        /// <inheritdoc />
        IOption<T> IOption<T>.Where(Func<T, bool> predicate) => this.Where(predicate);

        /// <inheritdoc />
        public bool Any() => this.AsOption.Any();

        /// <inheritdoc />
        public bool Any(Func<T, bool> predicate) => this.AsOption.Any(predicate);

        /// <inheritdoc />
        public bool All(Func<T, bool> predicate) => this.AsOption.All(predicate);

        /// <inheritdoc />
        public void ForEach(Action<T> action) => this.AsOption.ForEach(action);

        /// <summary>
        /// Performs the asynchronous <paramref name="action" /> with the success value if present.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public Task ForEachAsync(Func<T, Task> action) => this.AsOption.ForEachAsync(action);

        /// <inheritdoc />
        public void ForEach(Action failure, Action<T> success) => this.AsOption.ForEach(failure, success);

        /// <inheritdoc />
        public void ForEach(Action<Exception> failure, Action<T> success)
        {
            if (this.HasValue)
                success(this.AsOption.Value);
            else
                failure(this.Exception);
        }

        /// <inheritdoc cref="IInvariantOption{T,TOpt}.OrElse(System.Func{TOpt})" />
        public Try<T> OrElse(Func<Exception, Try<T>> @default) => this.HasValue ? this : @default(this.Exception);

        /// <inheritdoc />
        public Try<T> OrElse(Func<Try<T>> @default) => this.HasValue ? this : @default();

        /// <inheritdoc />
        public Try<T> OrElse(Try<T> @default) => this.HasValue ? this : @default;

        /// <inheritdoc />
        public bool Contains(T elem) => this.Contains(elem, EqualityComparer<T>.Default);

        /// <inheritdoc />
        public bool Contains(T elem, IEqualityComparer<T> comparer) => this.AsOption.Contains(elem, comparer);
    }

    /// <summary>
    /// Extension methods of <see cref="Try{T}" />s.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Downcasts a <see cref="ITry{T}" /> to a <see cref="Try{T}" />.
        /// </summary>
        /// <param name="value">The <see cref="ITry{T}" /> to cast.</param>
        /// <typeparam name="T">Type of the <paramref name="value" />.</typeparam>
        /// <returns>A <see cref="Try{T}" />.</returns>
        public static Try<T> ToTry<T>(this ITry<T> value)
            where T : notnull =>
            value is Try<T> @try ? @try : value.HasValue ? Success(value.Value) : Failure<T>(value.Exception);

        /// <summary>
        /// Converts a <see cref="Try{T}" /> of a <see cref="Try{T}" /> to a <see cref="Try{T}" />.
        /// </summary>
        /// <param name="self">The nested <see cref="Try{T}" />.</param>
        /// <typeparam name="T">The underlying type of <see cref="Try{T}" />.</typeparam>
        /// <returns>A <see cref="Try{T}" />.</returns>
        public static Try<T> Flatten<T>(this in Try<Try<T>> self) => self.HasValue ? self.Value : Failure<T>(self.Exception);

        /// <summary>
        /// Creates a <see cref="Try{T}" /> based on a success.
        /// </summary>
        /// <param name="value">Success value.</param>
        /// <typeparam name="T">Type of the <see cref="Try{T}" />.</typeparam>
        /// <returns>A <see cref="Try{T}" />.</returns>
        public static Try<T> Success<T>(T value) => new(value);

        /// <summary>
        /// Creates a <see cref="Try{T}" /> based on a failure.
        /// </summary>
        /// <param name="exception">Failure exception.</param>
        /// <typeparam name="T">Type of the <see cref="Try{T}" />.</typeparam>
        /// <returns>A <see cref="Try{T}" />.</returns>
        public static Try<T> Failure<T>(Exception exception) => new(exception);

        /// <summary>
        /// Executes <paramref name="f"/> and handles possible exceptions in a <see cref="Try{T}"/>.
        /// </summary>
        /// <param name="f">Function to execute.</param>
        /// <typeparam name="T">Return type of <paramref name="f"/>.</typeparam>
        /// <returns>A <see cref="Try{T}"/>.</returns>
        public static Try<T> Do<T>(Func<T> f)
            where T : notnull
        {
            try
            {
                return Success(f());
            }
            catch (Exception ex)
            {
                return Failure<T>(ex);
            }
        }

        /// <summary>
        /// Executes an asyncrhonous <paramref name="f"/> and handles possible exceptions in a <see cref="Try{T}"/>.
        /// </summary>
        /// <param name="f">Function to execute.</param>
        /// <typeparam name="T">Return type of <paramref name="f"/>.</typeparam>
        /// <returns>A <see cref="Try{T}"/>.</returns>
        public static async Task<Try<T>> DoAsync<T>(Func<Task<T>> f)
            where T : notnull
        {
            try
            {
                return Success(await f().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return Failure<T>(ex);
            }
        }
    }
}