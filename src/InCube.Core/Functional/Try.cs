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
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    public readonly struct Try<T> : ITry<T>, IInvariantOption<T, Try<T>>
    {
        private readonly Maybe<Exception> exception;

        internal Try(T value)
        {
            this.AsOption = Option.Some(value);
            this.exception = Maybe.None;
        }

        internal Try(Exception exception)
        {
            this.AsOption = Option.None;
            this.exception = Maybe.Some(exception);
        }

        public Option<T> AsOption { get; }

        IOption<T> ITry<T>.AsOption => this.AsOption;

        public static implicit operator Option<T>(Try<T> t) => t.AsOption;

        public Either<Exception, T> AsEither => this;

        IEither<Exception, T> ITry<T>.AsEither => this.AsEither;

        public static implicit operator Either<Exception, T>(Try<T> t) =>
            t.Match(Either.OfLeft<Exception, T>, Either.OfRight<Exception, T>);

        public bool HasValue => this.AsOption.HasValue;

        public T Value
        {
            get
            {
                var @this = this;
                return this.AsOption.GetValueOr(() => throw new InvalidOperationException("Try failed", @this.Exception));
            }
        }

        public Exception Exception => this.HasValue ? throw new InvalidOperationException("Try is success") : this.exception.Value;

        public IEnumerator<T> GetEnumerator() => this.AsOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Equals(Try<T> that) => this.AsOption.Equals(that.AsOption) && this.exception.Equals(that.exception);

        public override bool Equals(object obj) => 
            obj is Try<T> other && this.Equals(other);

        public override int GetHashCode() => this.AsOption.GetHashCode() + this.exception.GetHashCode();

        public static bool operator ==(Try<T> left, Try<T> right) => left.Equals(right);

        public static bool operator !=(Try<T> left, Try<T> right) => !left.Equals(right);

        public override string ToString() =>
            this.Match(
            success: x => $"Success({x})",
            failure: ex => $"Failure({ex})");

        public Try<Exception> Failed()
        {
            var self = this; // in order to capture this in the following lambda
            return Try.Do(() => self.Exception);
        }

        public TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success) => this.HasValue ? success(this.AsOption.Value) : failure(this.Exception);

        public Task<TOut> Match<TOut>(Func<Exception, Task<TOut>> failure, Func<T, Task<TOut>> success) => this.HasValue ? success(this.AsOption.Value) : failure(this.Exception);

        TOut IOption<T>.Match<TOut>(Func<TOut> none, Func<T, TOut> some) => this.Match(_ => none(), some);

        public T GetValueOrDefault() => this.AsOption.GetValueOrDefault();

        public T GetValueOrDefault([NotNull] Func<Exception, T> @default) => this.HasValue ? this.Value : @default(this.Exception);

        public T GetValueOr(Func<T> @default) => this.AsOption.GetValueOr(@default);

        public T GetValueOrDefault(T @default) => this.AsOption.GetValueOrDefault(@default);

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => this.AsOption.Select(f);

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => this.AsOption.SelectMany(x => f(x).ToOption());

        ITry<TOut> ITry<T>.Select<TOut>(Func<T, TOut> f) => this.Select(f);

        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<T, ITry<TOut>> f) => this.SelectMany(x => f(x).ToTry());

        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<Exception, ITry<TOut>> failure, Func<T, ITry<TOut>> success) => this.SelectMany(x => failure(x).ToTry(), x => success(x).ToTry());

        public Try<TOut> Select<TOut>(Func<T, TOut> f) => this.Match(Try.Failure<TOut>, value => Try.Do(() => f(value)));

        public Task<Try<TOut>> Select<TOut>(Func<T, Task<TOut>> f) => this.Match(Try.FailureAsync<TOut>, value => Try.DoAsync(() => f(value)));

        public Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> f) => this.Match(Try.Failure<TOut>, f);

        public Task<Try<TOut>> SelectMany<TOut>(Func<T, Task<Try<TOut>>> f) => this.Match(failure: Try.FailureAsync<TOut>, success: f);

        public Try<TOut> SelectMany<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success) => this.Match(failure, success);

        public Task ForEachAsync(Func<Task> none, Func<T, Task> some) => this.AsOption.ForEachAsync(none, some);

        public Try<T> Where(Func<T, bool> p) =>
            !this.HasValue || p(this.AsOption.Value)
                ? this
                : Try.Failure<T>(new ArgumentException("Predicate does not hold for value " + this.AsOption.Value));

        ITry<T> ITry<T>.Where(Func<T, bool> p) => this.Where(p);

        IOption<T> IOption<T>.Where(Func<T, bool> p) => this.Where(p);

        public bool Any() => this.AsOption.Any();

        public bool Any(Func<T, bool> p) => this.AsOption.Any(p);

        public bool All(Func<T, bool> p) => this.AsOption.All(p);

        public void ForEach(Action<T> action) => this.AsOption.ForEach(action);

        public Task ForEachAsync(Func<T, Task> action) => this.AsOption.ForEachAsync(action);

        public void ForEach(Action failure, Action<T> success) => this.AsOption.ForEach(failure, success);

        public void ForEach(Action<Exception> failure, Action<T> success)
        {
            if (this.HasValue)
            {
                success(this.AsOption.Value);
            }
            else
            {
                failure(this.Exception);
            }
        }

        public int Count => this.AsOption.Count;

        public Try<T> OrElse([NotNull] Func<Exception, Try<T>> @default) => this.HasValue ? this : @default(this.Exception);

        public Try<T> OrElse(Func<Try<T>> @default) => this.HasValue ? this : @default();

        public Try<T> OrElse(Try<T> @default) => this.HasValue ? this : @default;

        public bool Contains(T elem) => this.Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) => this.AsOption.Contains(elem, comparer);

        /// <see cref="Nullable{T}.Value"/>
        /// <exception cref="InvalidOperationException">If this <see cref="Try"/> has an exception or the <paramref name="index"/> != 0.</exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T this[int index] => index == 0 ? this.Value : throw new InvalidOperationException();
    }

    public static class Try
    {
        #region Construction 

        public static Try<T> Success<T>(T t) => new Try<T>(t);

        public static Try<T> Failure<T>(Exception ex) => new Try<T>(ex);

        public static Task<Try<T>> FailureAsync<T>(Exception ex) => Task.Run(() => new Try<T>(ex));

        public static Try<T> Do<T>(Func<T> f)
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

        public static async Task<Try<T>> DoAsync<T>(Func<Task<T>> f)
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

        #endregion

        #region Conversion

        public static Try<T> ToTry<T>(this ITry<T> value) =>
            value is Try<T> @try ? @try : value.HasValue ? Success(value.Value) : Failure<T>(value.Exception);

        #endregion

        #region Flattening

        public static Try<T> Flatten<T>(this in Try<Try<T>> self) =>
            self.HasValue ? self.Value : Failure<T>(self.Exception);

        #endregion
    }
}
