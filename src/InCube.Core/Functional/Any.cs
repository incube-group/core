using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InCube.Core.Functional
{
    /// <summary>
    /// A struct wrapper for any type (class or struct). The use is primarily to allow reference types to be used as
    /// <see cref="Nullable{T}" />.
    /// </summary>
    /// <seealso cref="Option{T}" />
    /// <typeparam name="T">Wrapped type.</typeparam>
    [SuppressMessage("Managed Binary Analysis", "CA2225: Operator overloads have named alternates", Justification = "Methods are in static companion class.")]
    [Serializable]
    internal readonly struct Any<T> : IEquatable<Any<T>>
    {
        public Any(T value) => this.Value = value;

        public T Value { get; }

        public static implicit operator Any<T>(T value) => new(value);

        public static implicit operator T(Any<T> any) => any.Value;

        public static bool operator ==(Any<T> left, Any<T> right) => left.Equals(right);

        public static bool operator !=(Any<T> left, Any<T> right) => !(left == right);

        public bool Equals(Any<T> other) => EqualityComparer<T>.Default.Equals(this.Value, other.Value);

        public override bool Equals(object? obj) => obj is Any<T> other && this.Equals(other);

        public override int GetHashCode() => this.Value is null ? 0 : EqualityComparer<T>.Default.GetHashCode(this.Value);
    }

    /// <summary>
    /// Companion class of <see cref="Any{T}"/>.
    /// </summary>
    public static class Any
    {
        internal static Any<T> ToAny<T>(this T t) => t;

        internal static TOut Apply<T, TOut>(this T self, Func<T, TOut> f) => f(self);

        internal static void Apply<T>(this T self, Action<T> f) => f(self);
    }
}