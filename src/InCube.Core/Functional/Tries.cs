using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    [Serializable]
    public readonly struct Try<T> : IEnumerable<T>
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

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue) yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString() => HasValue ? $"Success({Value})" : $"Failure({_exception})";

        public static implicit operator Option<T>(Try<T> t) => t._value;
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

        public static Try<Exception> Failed<T>(this Try<T> self) => Try(() => self.Exception);

        public static T2 Match<T2, T1>(this Try<T1> self, Func<Exception, T2> failure, Func<T1, T2> success) =>
            self.HasValue ? success(self.Value) : failure(self.Exception);

        public static Try<T2> Transform<T2, T1>(this Try<T1> self, Func<Exception, Try<T2>> failure,
            Func<T1, Try<T2>> success) =>
            self.HasValue ? success(self.Value) : failure(self.Exception);

        public static Option<T> ToOption<T>(this Try<T> self) => self;

        public static T GetValueOrDefault<T>(this Try<T> self, Func<Exception, T> @default) =>
            self.HasValue ? self.Value : @default(self.Exception);

        public static T GetValueOrDefault<T>(this Try<T> self, Func<T> @default) => 
            self.ToOption().GetValueOrDefault(@default);

        public static T GetValueOrDefault<T>(this Try<T> self, T @default) =>
            self.ToOption().GetValueOrDefault(@default);

        public static T GetValueOrDefault<T>(this Try<T> self) where T : class => 
            self.GetValueOrDefault(default(T));

        public static Try<T> OrElse<T>(this Try<T> self, Func<Exception, Try<T>> @default) =>
            self.HasValue ? self : @default(self.Exception);

        public static Try<T> OrElse<T>(this Try<T> self, Func<Try<T>> @default) =>
            self.HasValue ? self : @default();

        public static Try<T> OrElse<T>(this Try<T> self, Try<T> @default) =>
            self.HasValue ? self : @default;

        public static T? ToNullable<T>(this Try<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        public static Try<T> Flatten<T>(this Try<Try<T>> self) =>
            self.HasValue ? self.Value : Failure<T>(self.Exception);

        public static bool Contains<T>(this Try<T> self, T elem) => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this Try<T> self, T elem, IEqualityComparer<T> comparer) => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static Try<T2> Select<T2, T1>(this Try<T1> self, Func<T1, T2> f) =>
            self.HasValue ? Try(() => f(self.Value)) : Failure<T2>(self.Exception);

        public static Try<T2> SelectMany<T2, T1>(this Try<T1> self, Func<T1, Try<T2>> f) =>
            self.HasValue ? f(self.Value): Failure<T2>(self.Exception);

        public static Try<T> Where<T>(this Try<T> self, Func<T, bool> p) =>
            !self.HasValue || p(self.Value)
                ? self
                : Failure<T>(new ArgumentException("Predicate does not hold for value " + self.Value));

        public static bool Any<T>(this Try<T> self) => self.ToOption().Any();

        public static bool Any<T>(this Try<T> self, Func<T, bool> p) => self.ToOption().Any(p);

        public static bool All<T>(this Try<T> self, Func<T, bool> p) => self.ToOption().All(p);

        public static void ForEach<T>(this Try<T> self, Action<T> action)
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this Try<T> self, Action failure, Action<T> success)
        {
            if (self.HasValue) success(self.Value);
            else failure();
        }

        public static void ForEach<T>(this Try<T> self, Action<Exception> failure, Action<T> success)
        {
            if (self.HasValue) success(self.Value);
            else failure(self.Exception);
        }
    }
}
