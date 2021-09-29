using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A collection of extension methods to work with <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    [PublicAPI]
    public static class AsyncEnumerables
    {
        /// <summary>
        /// Chunks an <see cref="IAsyncEnumerable{T}"/> into an <see cref="IAsyncEnumerable{T}"/> of lists.
        /// </summary>
        /// <param name="self">An <see cref="IAsyncEnumerable{T}"/>.</param>
        /// <param name="chunkSize">The size of the chunks.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of lists.</returns>
        public static async IAsyncEnumerable<IReadOnlyList<T>> Chunk<T>(this IAsyncEnumerable<T> self, int chunkSize)
            where T : class
        {
            if (chunkSize <= 0)
                throw new ArgumentException($"Chunksize {chunkSize} is not supported.", nameof(chunkSize));
            var buffer = new T[chunkSize];
            var counter = 0;
            await foreach (var t in self)
            {
                if (counter == chunkSize)
                {
                    yield return buffer.ToImmutableList();
                    buffer.Set(null);
                    counter = 0;
                }

                buffer[counter] = t;
                ++counter;
            }

            if (counter != 0)
                yield return buffer[..counter].ToImmutableList();
        }
    }
}