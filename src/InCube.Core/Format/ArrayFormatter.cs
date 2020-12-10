using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace InCube.Core.Format
{
    /// <summary>
    /// Convenience extensions methods for formatting matrices and arrays.
    /// </summary>
    [PublicAPI]
    public static class ArrayFormatter
    {
        /// <summary>
        /// Formats a 2-dimensional array of doubles.
        /// </summary>
        /// <param name="matrix">The matrix to format.</param>
        /// <param name="stringFormat">The double string format string to use.</param>
        /// <returns>A string representation of the input 2-dimensional array.</returns>
        public static string ToMatrixString(this double[,] matrix, string stringFormat = "e2")
        {
            var sb = new StringBuilder();
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    var value = matrix[i, j];
                    var stringValue = $" {value.ToString(stringFormat)}";
                    var padded = $"{stringValue,11}";
                    sb.Append(padded);
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Pretty-prints an array of <see cref="double"/>s.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to pretty-print.</param>
        /// <param name="stringFormat">The format string to use for the individual <see cref="double"/>s.</param>
        /// <returns>A pretty <see cref="string"/>.</returns>
        public static string ToArrayString(this IReadOnlyList<double> array, string stringFormat = "e2")
        {
            var sb = new StringBuilder();
            foreach (var d in array)
            {
                var stringValue = $" {d.ToString(stringFormat)}";
                var padded = $"{stringValue,11}";
                sb.Append(padded);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Pretty-prints an array of <see cref="double"/>s.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to pretty-print.</param>
        /// <returns>A pretty <see cref="string"/>.</returns>
        public static string ToArrayString(this IReadOnlyList<int> array)
        {
            var sb = new StringBuilder();
            foreach (var d in array)
            {
                var stringValue = $" {d}";
                var padded = $"{stringValue,5}";
                sb.Append(padded);
            }

            return sb.ToString();
        }
    }
}