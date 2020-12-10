using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Wraps an <see cref="IList{T}"/> into an <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    public readonly struct ReadOnlyList<T> : IReadOnlyList<T>, IEquatable<ReadOnlyList<T>>
    {
        private readonly IList<T> wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> struct.
        /// </summary>
        /// <param name="wrapped">The <see cref="IList{T}"/> to wrap.</param>
        public ReadOnlyList(IList<T> wrapped)
        {
            this.wrapped = wrapped;
        }

        /// <inheritdoc/>
        public int Count => this.wrapped.Count;

        /// <inheritdoc/>
        public T this[int index] => this.wrapped[index];

        public static bool operator ==(ReadOnlyList<T> left, ReadOnlyList<T> right) => left.Equals(right);

        public static bool operator !=(ReadOnlyList<T> left, ReadOnlyList<T> right) => !left.Equals(right);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.wrapped.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.wrapped.GetEnumerator();

        /// <inheritdoc/>
        public bool Equals(ReadOnlyList<T> that) => Equals(this.wrapped, that.wrapped);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ReadOnlyList<T> that && this.Equals(that);

        /// <inheritdoc/>
        public override int GetHashCode() => this.wrapped.GetHashCode();
    }
}