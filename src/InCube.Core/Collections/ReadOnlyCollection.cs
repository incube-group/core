using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Collections
{
    /// <summary>
    /// Wraps an <see cref="ICollection{T}"/> into an <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the collection</typeparam>
    public struct ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _wrapped;

        public ReadOnlyCollection(ICollection<T> wrapped)
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

    }

}