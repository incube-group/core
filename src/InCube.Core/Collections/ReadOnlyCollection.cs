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

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.wrapped.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.wrapped.GetEnumerator();

        /// <inheritdoc/>
        public int Count => this.wrapped.Count;

        /// <inheritdoc/>
        public bool Equals(ReadOnlyCollection<T> that) => Equals(this.wrapped, that.wrapped);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is ReadOnlyCollection<T> that && this.Equals(that);

        /// <inheritdoc/>
        public override int GetHashCode() => this.wrapped?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right) =>
            left.Equals(right);

        public static bool operator !=(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right) =>
            !left.Equals(right);
    }
}