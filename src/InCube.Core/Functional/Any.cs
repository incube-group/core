using System;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    public readonly struct Any<T> : IEquatable<Any<T>>
    {
        public Any(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public static implicit operator Any<T>(T value) => new Any<T>(value);

        public static implicit operator T(Any<T> any) => any.Value;

        public bool Equals(Any<T> other) => EqualityComparer<T>.Default.Equals(this.Value, other.Value);

        public override bool Equals(object obj) => 
            obj is Any<T> other && Equals(other);

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(this.Value);
    }

    public static class Any
    {
        public static Any<T> ToAny<T>(this T t) => t;

        public static TOut Apply<T, TOut>(this T self, Func<T, TOut> f) => f(self);

        public static void Apply<T>(this T self, Action<T> f) => f(self);

    }
}