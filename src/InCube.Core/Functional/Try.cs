using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    [Serializable]
    public readonly struct Try<T> : ITry<T>, IInvariantOption<T, Try<T>>
    {
        private readonly CanBeNull<Exception> _exception;

        internal Try(T value)
        {
            AsOption = Option.Some(value);
            _exception = CanBeNull.None;
        }

        internal Try(Exception exception)
        {
            AsOption = Option.None;
            _exception = CanBeNull.Some(exception);
        }

        public Option<T> AsOption { get; }

        IOption<T> ITry<T>.AsOption => AsOption;

        public static implicit operator Option<T>(Try<T> t) => t.AsOption;

        public Either<Exception, T> AsEither => this;

        IEither<Exception, T> ITry<T>.AsEither => AsEither;

        public static implicit operator Either<Exception, T>(Try<T> t) =>
            t.Match(Either<Exception, T>.OfLeft, Either<Exception, T>.OfRight);

        public bool HasValue => AsOption.HasValue;

        public T Value => HasValue ? AsOption.Value : throw Exception;

        public Exception Exception =>
            HasValue ? throw new InvalidOperationException("Try is success") : _exception.Value;

        public IEnumerator<T> GetEnumerator() => this.AsOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Equals(Try<T> that) =>
            AsOption.Equals(that.AsOption) && _exception.Equals(that._exception);

        public override string ToString() => HasValue ? $"Success({Value})" : $"Failure({_exception.GetValueOrDefault()})";

        public Try<Exception> Failed()
        {
            var self = this; // in order to capture this in the following lambda
            return Try.Execute(() => self.Exception);
        }

        public TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success) =>
            HasValue ? success(AsOption.Value) : failure(Exception);

        TOut IOption<T>.Match<TOut>(Func<TOut> none, Func<T, TOut> some) =>
            Match(_ => none(), some);

        public T GetValueOrDefault() => AsOption.GetValueOrDefault();

        public T GetValueOrDefault([NotNull] Func<Exception, T> @default) =>
            HasValue ? Value : @default(Exception);

        public T GetValueOrDefault([NotNull] Func<T> @default) =>
            AsOption.GetValueOrDefault(@default);

        public T GetValueOrDefault(T @default) =>
            AsOption.GetValueOrDefault(@default);

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) =>
            AsOption.Select(f);

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) =>
            AsOption.SelectMany(x => f(x).ToOption());

        ITry<TOut> ITry<T>.Select<TOut>(Func<T, TOut> f) =>
            Select(f);

        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<T, ITry<TOut>> f) =>
            SelectMany(x => f(x).ToTry());

        ITry<TOut> ITry<T>.SelectMany<TOut>(Func<Exception, ITry<TOut>> failure, Func<T, ITry<TOut>> success) =>
            SelectMany(x => failure(x).ToTry(), x => success(x).ToTry());

        public Try<TOut> Select<TOut>(Func<T, TOut> f) =>
            Match(Try.Failure<TOut>, value => Try.Execute(() => f(value)));

        public Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> f) =>
            Match(Try.Failure<TOut>, f);

        public Try<TOut> SelectMany<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success) =>
            Match(failure, success);

        public Try<T> Where(Func<T, bool> p) =>
            !this.HasValue || p(AsOption.Value)
                ? this
                : Try.Failure<T>(new ArgumentException("Predicate does not hold for value " + AsOption.Value));

        ITry<T> ITry<T>.Where(Func<T, bool> p) => this.Where(p);

        IOption<T> IOption<T>.Where(Func<T, bool> p) => this.Where(p);

        public bool Any() => AsOption.Any();

        public bool Any(Func<T, bool> p) => AsOption.Any(p);

        public bool All(Func<T, bool> p) => AsOption.All(p);

        public void ForEach(Action<T> action)
        {
            AsOption.ForEach(action);
        }

        public void ForEach(Action failure, Action<T> success)
        {
            AsOption.ForEach(failure, success);
        }

        public void ForEach(Action<Exception> failure, Action<T> success)
        {
            if (HasValue)
            {
                success(AsOption.Value);
            }
            else
            {
                failure(Exception);
            }
        }

        public int Count => AsOption.Count;

        public Try<T> OrElse([NotNull] Func<Exception, Try<T>> @default) =>
            HasValue ? this : @default(Exception);

        public Try<T> OrElse([NotNull] Func<Try<T>> @default) =>
            HasValue ? this : @default();

        public Try<T> OrElse(Try<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>
            AsOption.Contains(elem, comparer);
    }

    public static class Try
    {
        #region Construction 

        public static Try<T> Success<T>(T t) => new Try<T>(t);

        public static Try<T> Failure<T>(Exception ex) => new Try<T>(ex);

        public static Try<T> Execute<T>(Func<T> f)
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
