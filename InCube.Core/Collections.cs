using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InCube.Core
{

    public struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _wrapped;

        public ReadOnlyList(IList<T> wrapped)
        {
            this._wrapped = wrapped;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._wrapped.GetEnumerator();
        }

        public int Count => this._wrapped.Count;

        public T this[int index] => this._wrapped[index];

        public static implicit operator ReadOnlyList<T>(List<T> list)
        {
            return new ReadOnlyList<T>(list);
        }

        public static implicit operator ReadOnlyList<T>(T[] list)
        {
            return new ReadOnlyList<T>(list);
        }
    }

    public static class Collections
    {
        public static IReadOnlyDictionary<T, V> AsReadOnly<T, V>(this IDictionary<T, V> dict)
        {
            return new ReadOnlyDictionary<T, V>(dict);
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            switch (list)
            {
                case List<T> l:
                    return l;
                case T[] a:
                    return a;
                default:
                    return new ReadOnlyList<T>(list);
            }
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this List<T> list) => list;

        public static IReadOnlyList<T> AsReadOnly<T>(this T[] list) => list;

    }

}