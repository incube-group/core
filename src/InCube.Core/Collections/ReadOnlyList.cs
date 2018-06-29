using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Wraps an <see cref="IList{T}"/> into an <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    public struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _wrapped;

        public ReadOnlyList(IList<T> wrapped)
        {
            _wrapped = wrapped;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public int Count => _wrapped.Count;

        public T this[int index] => _wrapped[index];

    }

}