using System;
using System.Text;
using JetBrains.Annotations;

namespace InCube.Core
{
    /// <summary>
    /// Static convenience methods to check that a method or a constructor is invoked with proper parameter or not.
    /// </summary>
    public static class Preconditions
    {
        private static string StringValue(object obj)
        {
            return obj?.ToString() ?? "null";
        }

        private static string Format(string template, params object[] args)
        {
            template = StringValue(template); // null -> "null"

            // start substituting the arguments into the '{}' placeholders
            var builder = new StringBuilder(template.Length + 16 * args.Length);
            var templateStart = 0;
            var i = 0;
            while (i < args.Length)
            {
                var placeholderStart = template.IndexOf("{}", templateStart, StringComparison.Ordinal);
                if (placeholderStart == -1)
                {
                    break;
                }
                builder.Append(template, templateStart, placeholderStart - templateStart);
                builder.Append(args[i++]);
                templateStart = placeholderStart + 2;
            }
            builder.Append(template, templateStart, template.Length - templateStart);

            // if we run out of placeholders, append the extra args in square braces
            if (i < args.Length)
            {
                builder.Append(" [");
                builder.Append(args[i++]);
                while (i < args.Length)
                {
                    builder.Append(", ");
                    builder.Append(args[i++]);
                }
                builder.Append(']');
            }

            return builder.ToString();
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">if set to <c>true</c> [expression].</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">if set to <c>true</c> [expression].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument(bool expression, object errorMessage)
        {
            if (!expression)
            {
                throw new ArgumentException(StringValue(errorMessage));
            }
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument<T1>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1));
            }
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgumentF<T1>(bool b, [NotNull] string errorMessageTemplate, T1 p1)
        {
            if (!b)
            {
                throw new ArgumentException(string.Format(errorMessageTemplate, p1));
            }
        }
        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument<T1, T2>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2));
            }
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgumentF<T1, T2>(bool b, [NotNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (!b)
            {
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2));
            }
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument<T1, T2, T3>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2, p3));
            }
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgumentF<T1, T2, T3>(bool b, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (!b)
            {
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2, p3));
            }
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckArgument<T1, T2, T3, T4>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2, p3, p4));
            }
        }


        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgumentF<T1, T2, T3, T4>(bool b, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (!b)
            {
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2, p3, p4));
            }
        }

        public static T CheckNotNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException();
            }

            return reference;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">The reference.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNotNull<T>(T reference, [CanBeNull] object errorMessage)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(StringValue(errorMessage));
            }
            return reference;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNotNull<T, T1>(T obj, [CanBeNull] string errorMessageTemplate, T1 p1)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1));
            }
            return obj;
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNullF<T, T1>(T obj, [NotNull] string errorMessageTemplate, T1 p1)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1));
            }
            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNotNull<T, T1, T2>(T obj, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1, p2));
            }
            return obj;
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNullF<T, T1, T2>(T obj, [NotNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2));
            }
            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNotNull<T, T1, T2, T3>(T obj, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1, p2, p3));
            }
            return obj;
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNullF<T, T1, T2, T3>(T obj, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2, p3));
            }
            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNotNull<T, T1, T2, T3, T4>(T obj, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1, p2, p3, p4));
            }
            return obj;
        }

        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNullF<T, T1, T2, T3, T4>(T obj, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2, p3, p4));
            }
            return obj;
        }
    }
}
