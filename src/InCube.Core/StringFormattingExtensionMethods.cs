using System;
using System.Collections.Generic;
using System.Text;

namespace InCube.Core
{
    public static class StringFormattingExtensionMethods
    {
        public static string ToMatrixString(this double[,] matrix, string stringFormat = "e2")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
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

        public static string ToArrayString(this double[] array, string stringFormat = "e2")
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

        public static string ToArrayString(this int[] array, string stringFormat = "######")
        {
            var sb = new StringBuilder();
            foreach (var d in array)
            {
                var stringValue = $" {d.ToString()}";
                var padded = $"{stringValue,5}";
                sb.Append(padded);
            }

            return sb.ToString();
        }
    }
}
