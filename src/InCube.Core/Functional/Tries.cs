using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    public interface ITry<out T> : IEnumerable<T>
    {
        bool HasValue { get; }

        T Value { get; }

        Exception Exception { get; }

        IOption<T> AsOption { get; }

        Try<Exception> Failed();

        TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success);

        T GetValueOrDefault();

        Try<TOut> Select<TOut>(Func<T, TOut> f);

        Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> success);

        Try<TOut> Transform<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success);

        ITry<T> Where(Func<T, bool> p);

        bool Any();

        bool Any(Func<T, bool> p);

        bool All(Func<T, bool> p);

        void ForEach(Action<T> action);

        void ForEach(Action failure, Action<T> success);

        void ForEach(Action<Exception> failure, Action<T> success);
    }

    [Serializable]
    public readonly struct Try<T> : ITry<T>
    {
        private readonly Option<T> _value;
        private readonly Exception _exception;

        internal Try(T value)
        {
            _value = Options.Some(value);
            _exception = null;
        }

        internal Try(Exception exception)
        {
            _value = Options.None;
            _exception = exception;
        }

        public bool HasValue => _value.HasValue;

        public T Value => HasValue ? _value.Value : throw Exception;

        public Exception Exception =>
            HasValue ? throw new InvalidOperationException("Try is success") : _exception.ToOption().Value;

        public IEnumerator<T> GetEnumerator() => this._value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => HasValue ? $"Success({Value})" : $"Failure({_exception})";

        public static implicit operator Option<T>(Try<T> t) => t._value;

        public Option<T> AsOption => this;

        IOption<T> ITry<T>.AsOption => this.AsOption;

        public Try<Exception> Failed()
        {
            var self = this; // in order to capture this in the following lambda
            return Tries.Try(() => self.Exception);
        }

        public TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success) =>
            HasValue ? success(_value.Value) : failure(Exception);

        public T GetValueOrDefault() => this.GetValueOrDefault(default(T));

        public Try<TOut> Select<TOut>(Func<T, TOut> f)
        {
            var self = this; // in order to capture this in the following lambda
            return HasValue ? Tries.Try(() => f(self._value.Value)) : Tries.Failure<TOut>(Exception);
        }

        public Try<TOut> SelectMany<TOut>(Func<T, Try<TOut>> f) =>
            this.HasValue ? f(this.Value) : Tries.Failure<TOut>(this.Exception);

        public Try<TOut> Transform<TOut>(Func<Exception, Try<TOut>> failure, Func<T, Try<TOut>> success) =>
            HasValue ? success(_value.Value) : failure(Exception);

        public Try<T> Where(Func<T, bool> p) =>
            !this.HasValue || p(_value.Value)
                ? this
                : Tries.Failure<T>(new ArgumentException("Predicate does not hold for value " + _value.Value));

        ITry<T> ITry<T>.Where(Func<T, bool> p) => this.Where(p);

        public bool Any() => AsOption.Any();

        public bool Any(Func<T, bool> p) => AsOption.Any(p);

        public bool All(Func<T, bool> p) => AsOption.All(p);

        public void ForEach(Action<T> action)
        {
            if (HasValue)
            {
                action(_value.Value);
            }
        }

        public void ForEach(Action failure, Action<T> success)
        {
            if (HasValue)
            {
                success(_value.Value);
            }
            else
            {
                failure();
            }
        }

        public void ForEach(Action<Exception> failure, Action<T> success)
        {
            if (HasValue)
            {
                success(_value.Value);
            }
            else
            {
                failure(Exception);
            }
        }
    }

    public static class Tries
    {
        public static Try<T> Success<T>(T t) => new Try<T>(t);

        public static Try<T> Failure<T>(Exception ex) => new Try<T>(ex);

        public static Try<T> Try<T>(Func<T> f)
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

        public static T GetValueOrDefault<T>(this in Try<T> self, Func<Exception, T> @default) =>
            self.HasValue ? self.Value : @default(self.Exception);

        public static T GetValueOrDefault<T>(this in Try<T> self, Func<T> @default) => 
            self.AsOption.GetValueOrDefault(@default);

        public static T GetValueOrDefault<T>(this in Try<T> self, T @default) =>
            self.AsOption.GetValueOrDefault(@default);

        public static Try<T> OrElse<T>(this in Try<T> self, Func<Exception, Try<T>> @default) =>
            self.HasValue ? self : @default(self.Exception);

        public static Try<T> OrElse<T>(this in Try<T> self, Func<Try<T>> @default) =>
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
