using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static InCube.Core.Preconditions;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Wraps an <see cref="IEnumerable{T}"/> into a read only <see cref="ICollection{T}"/> by adding a count argument.
    /// </summary>
    /// <typeparam name="T">The type of the collection</typeparam>
    public struct EnumerableCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public EnumerableCollection(IEnumerable<T> enumerable, int count)
        {
            _enumerable = enumerable;
            Count = count;
            IsReadOnly = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _enumerable.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckArgument(Count <= array.Length - arrayIndex,
                "insufficient space for {} elements in array of length {} at index {}",
                Count, array.Length, arrayIndex);
            var i = -1;
            foreach (var item in _enumerable)
            {
                array[++i] = item;
            }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }

        private readonly IEnumerable<T> _enumerable;
    }

}