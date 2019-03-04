using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    public readonly struct Try<T> : ITry<T>, IInvariantOption<T, Try<T>>
    {
        private readonly Maybe<Exception> exception;

        internal Try(T value)
        {
            AsOption = Option.Some(value);
            this.exception = Maybe.None;
        }

        internal Try(Exception exception)
        {
            AsOption = Option.None;
            this.exception = Maybe.Some(exception);
        }

        public Option<T> AsOption { get; }

        IOption<T> ITry<T>.AsOption => AsOption;

        public static implicit operator Option<T>(Try<T> t) => t.AsOption;

        public Either<Exception, T> AsEither => this;

        IEither<Exception, T> ITry<T>.AsEither => AsEither;

        public static implicit operator Either<Exception, T>(Try<T> t) =>
            t.Match(Either.OfLeft<Exception, T>, Either.OfRight<Exception, T>);

        public bool HasValue => AsOption.HasValue;

        [SuppressMessage("Design",
            "CA1065: Do not raise exceptions in unexpected locations",
            Justification = "by design")]
        public T Value
        {
            get
            {
                var @this = this;
                return AsOption.GetValueOrDefault(() => throw @this.Exception);
            }
        }

        public Exception Exception =>
            HasValue ? throw new InvalidOperationException("Try is success") : this.exception.Value;

        public IEnumerator<T> GetEnumerator() => this.AsOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Equals(Try<T> that) =>
            AsOption.Equals(that.AsOption) && this.exception.Equals(that.exception);

        public override bool Equals(object obj) => 
            obj is Try<T> other && Equals(other);

        public override int GetHashCode() => 
            AsOption.GetHashCode() + this.exception.GetHashCode();

        public static bool operator ==(Try<T> left, Try<T> right) => left.Equals(right);

        public static bool operator !=(Try<T> left, Try<T> right) => !left.Equals(right);

        public override string ToString() => HasValue ? $"Success({Value})" : $"Failure({this.exception.GetValueOrDefault()})";

        public Try<Exception> Failed()
        {
            var self = this; // in order to capture this in the following lambda
            return Try.Do(() => self.Exception);
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
            Match(Try.Failure<TOut>, value => Try.Do(() => f(value)));

        public Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> f) =>
            Match(Try.Failure<TOut>, f);

        public Try<TOut> SelectMany<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success) =>
            Match(failure, success);

        public Try<T> Where(Func<T, bool> p) =>
            !this.HasValue || p(AsOption.Value)
                ? this
                : Try.Failure<T>(new ArgumentException("Predicate does not hold for value " + AsOption.Value));

        public Any<T>? AsAny => AsOption.AsAny;

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
