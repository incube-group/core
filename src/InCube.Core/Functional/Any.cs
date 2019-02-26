using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InCube.Core.Functional
{
    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    [Serializable]
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

        public static bool operator ==(Any<T> left, Any<T> right) => left.Equals(right);

        public static bool operator !=(Any<T> left, Any<T> right) => !(left == right);
    }

    public static class Any
    {
        public static Any<T> ToAny<T>(this T t) => t;

        internal static TOut Apply<T, TOut>(this T self, Func<T, TOut> f) => f(self);

        internal static void Apply<T>(this T self, Action<T> f) => f(self);
    }
}