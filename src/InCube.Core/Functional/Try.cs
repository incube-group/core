using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    [Serializable]
    public readonly struct Try<T> : ITry<T>
    {
        private readonly Exception _exception;

        internal Try(T value)
        {
            AsOption = Option.Some(value);
            _exception = null;
        }

        internal Try(Exception exception)
        {
            AsOption = Option.None;
            _exception = exception;
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
            HasValue ? throw new InvalidOperationException("Try is success") : _exception.ToOption().Value;

        public IEnumerator<T> GetEnumerator() => this.AsOption.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => HasValue ? $"Success({Value})" : $"Failure({_exception})";

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

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) =>
            AsOption.Select(f);

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) =>
            ((IOption<T>)AsOption).SelectMany(f);

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
    }

    public static class Try
    {
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

        public static Try<T> ToTry<T>(this ITry<T> value) =>
            value is Try<T> @try ? @try : value.HasValue ? Success(value.Value) : Failure<T>(value.Exception);

        public static T GetValueOrDefault<T>(this in Try<T> self, [NotNull] Func<Exception, T> @default) =>
            self.HasValue ? self.Value : @default(self.Exception);

        public static T GetValueOrDefault<T>(this in Try<T> self, [NotNull] Func<T> @default) => 
            self.AsOption.GetValueOrDefault(@default);

        public static T GetValueOrDefault<T>(this in Try<T> self, T @default) =>
            self.AsOption.GetValueOrDefault(@default);

        public static Try<T> OrElse<T>(this in Try<T> self, [NotNull] Func<Exception, Try<T>> @default) =>
            self.HasValue ? self : @default(self.Exception);

        public static Try<T> OrElse<T>(this in Try<T> self, [NotNull] Func<Try<T>> @default) =>
            self.HasValue ? self : @default();

        public static Try<T> OrElse<T>(this in Try<T> self, Try<T> @default) =>
            self.HasValue ? self : @default;

        public static T? ToNullable<T>(this in Try<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        public static Try<T> Flatten<T>(this in Try<Try<T>> self) =>
            self.HasValue ? self.Value : Failure<T>(self.Exception);

        public static bool Contains<T>(this in Try<T> self, T elem) => 
            self.AsOption.Contains(elem);

        public static bool Contains<T>(this in Try<T> self, T elem, IEqualityComparer<T> comparer) =>
            self.AsOption.Contains(elem, comparer);
    }
}
