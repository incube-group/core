using JetBrains.Annotations;

namespace InCube.Core.Collections
{
    /// <summary>
    /// A collection of extension methods related to arrays.
    /// </summary>
    [PublicAPI]
    public static class Arrays
    {
        /// <summary>
        /// Creates a constant array.
        /// </summary>
        /// <param name="length">The legnth of the outpout array.</param>
        /// <param name="value">The value to set at every index.</param>
        /// <typeparam name="T">The type of the output array.</typeparam>
        /// <returns>An array filled with a single object.</returns>
        public static T[] Constant<T>(this int length, T value)
            where T : notnull
        {
            var result = new T[length];
            result.Set(value);
            return result;
        }

        /// <summary>
        /// Creates a constant array.
        /// </summary>
        /// <param name="length">The legnth of the outpout array.</param>
        /// <param name="value">The value to set at every index.</param>
        /// <typeparam name="T">The type of the output array.</typeparam>
        /// <returns>An array filled with a single object.</returns>
        public static T?[] ConstantNullable<T>(this int length, T? value)
        {
            var result = new T[length];
            result.Set(value);
            return result;
        }

        /// <summary>
        /// Sets all the elements of the <paramref name="array"/> to the <see cref="value"/>.
        /// </summary>
        /// <param name="array">The array to modify.</param>
        /// <param name="value">The value to modify the array with.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        public static void Set<T>(this T[] array, T value)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = value;
        }
    }
}