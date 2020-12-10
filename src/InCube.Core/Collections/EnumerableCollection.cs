using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static InCube.Core.Preconditions;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Extension methods of the <see cref="EnumerableCollection"/> class.
    /// </summary>
    [PublicAPI]
    public static class EnumerableCollection
    {
        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> to an <see cref="EnumerableCollection"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="count">The number of elements in the collection.</param>
        /// <typeparam name="T">The type of the gollection.</typeparam>
        /// <returns>An <see cref="EnumerableCollection"/>.</returns>
        public static EnumerableCollection<T> ToCollection<T>(this IEnumerable<T> enumerable, int count) => new(enumerable, count);
    }

    /// <summary>
    /// Wraps an <see cref="IEnumerable{T}" /> into a read only <see cref="ICollection{T}" /> by adding a count argument.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    [PublicAPI]
    public class EnumerableCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableCollection{T}" /> class.
        /// </summary>
        /// <param name="enumerable">The source to base the collection on.</param>
        /// <param name="count">The number of elements in the collection.</param>
        public EnumerableCollection(IEnumerable<T> enumerable, int count)
        {
            this.enumerable = enumerable;
            this.Count = count;
        }

        /// <summary>
        /// Gets the count of the number of elements in the collection.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets a value indicating whether or not this collection is readonly. It is.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}" />.</returns>
        public IEnumerator<T> GetEnumerator() => this.enumerable.GetEnumerator();

        /// <summary>
        /// Explicit implementation of <see cref="IEnumerable{T}.GetEnumerator" />.
        /// </summary>
        /// <returns>AN <see cref="IEnumerator{T}" />.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// NOT SUPPORTED.
        /// </summary>
        /// <param name="item">Doesn't matter.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        [Obsolete("NOT SUPPORTED.")]
        public void Add(T item) => throw new NotSupportedException();

        /// <summary>
        /// NOT SUPPORTED.
        /// </summary>
        [Obsolete("NOT SUPPORTED.")]
        public void Clear() => throw new NotSupportedException();

        /// <summary>
        /// Whether or not the <paramref name="item" /> is contained in this collection.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>A boolean indicating whether or not the item is contained in this collection.</returns>
        public bool Contains(T item) => this.enumerable.Contains(item);

        /// <summary>
        /// Copies the elements of this collection to the <paramref name="array" /> starting at the <paramref name="arrayIndex" />.
        /// </summary>
        /// <param name="array">The target of the copy.</param>
        /// <param name="arrayIndex">The index to start copying at in the <paramref name="array" />.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckArgument(this.Count <= array.Length - arrayIndex, "insufficient space for {0} elements in array of length {1} at index {2}", this.Count, array.Length, arrayIndex);
            var i = -1;
            foreach (var item in this.enumerable)
                array[++i] = item;
        }

        /// <summary>
        /// NO SUPPORTED.
        /// </summary>
        /// <param name="item">Doesn't matter.</param>
        /// <returns>Never returns, always throws.</returns>
        [Obsolete("NOT SUPPORTED.")]
        public bool Remove(T item) => throw new NotSupportedException();
    }
}