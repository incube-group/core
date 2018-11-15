using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Wraps an <see cref="ICollection{T}"/> into an <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the collection</typeparam>
    public readonly struct ReadOnlyCollection<T> : IReadOnlyCollection<T>, IEquatable<ReadOnlyCollection<T>>
    {
        private readonly ICollection<T> wrapped;

        public ReadOnlyCollection(ICollection<T> wrapped)
        {
            this.wrapped = wrapped;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.wrapped.GetEnumerator();
        }

        public int Count => this.wrapped.Count;

        public bool Equals(ReadOnlyCollection<T> other) => 
            Equals(this.wrapped, other.wrapped);

        public override bool Equals(object obj) => 
            obj is ReadOnlyCollection<T> other && Equals(other);

        public override int GetHashCode() => 
            this.wrapped?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right) => 
            left.Equals(right);

        public static bool operator !=(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right) => 
            !left.Equals(right);
    }
}