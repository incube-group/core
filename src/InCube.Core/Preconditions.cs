using System;
using JetBrains.Annotations;

namespace InCube.Core
{
    /// <summary>
    /// Static convenience methods to check that a method or a constructor is invoked with proper parameter or not.
    /// The design of this class has been strongly influenced by
    /// <see href="https://github.com/google/guava/blob/master/guava/src/com/google/common/base/Preconditions.java">Preconditions</see>
    /// in
    /// <see href="https://github.com/google/guava">Google Guava</see>.
    /// </summary>
    public static class Preconditions
    {
        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">if set to <c>true</c> [expression].</param>
        /// <exception cref="ArgumentException">Throws when false.</exception>
        public static void CheckArgument(bool expression)
        {
            if (!expression)
                throw new ArgumentException();
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">if set to <c>true</c> [expression].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="ArgumentException">Throws when false.</exception>
        public static void CheckArgument(bool expression, object errorMessage)
        {
            if (!expression)
                throw new ArgumentException(StringValue(errorMessage));
        }

        /// <summary>
        /// Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <exception cref="ArgumentException">Throws when false.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgument<T1>(bool b, [NotNull] string errorMessageTemplate, T1 p1)
        {
            if (!b)
                throw new ArgumentException(string.Format(errorMessageTemplate, p1));
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
        /// <exception cref="ArgumentException">Throws when false.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgument<T1, T2>(bool b, [NotNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (!b)
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2));
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
        /// <exception cref="ArgumentException">Throws when false.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgument<T1, T2, T3>(bool b, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (!b)
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2, p3));
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
        /// <exception cref="ArgumentException">Throws when false.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static void CheckArgument<T1, T2, T3, T4>(
            bool b,
            [NotNull] string errorMessageTemplate,
            T1 p1,
            T2 p2,
            T3 p3,
            T4 p4)
        {
            if (!b)
                throw new ArgumentException(string.Format(errorMessageTemplate, p1, p2, p3, p4));
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <typeparam name="T">Type of object to check for null.</typeparam>
        /// <returns>A <typeparamref name="T" />.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="reference"/> is null.</exception>
        public static T CheckNotNull<T>(T reference)
        {
            if (reference == null)
                throw new ArgumentNullException();

            return reference;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T">Type of object to check for null.</typeparam>
        /// <param name="reference">The reference.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A <typeparamref name="T" />.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="reference"/> is null.</exception>
        public static T CheckNotNull<T>(T reference, [CanBeNull] object errorMessage)
        {
            if (reference == null)
                throw new ArgumentNullException(StringValue(errorMessage));

            return reference;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T">Type of object to check for null.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <returns>A <typeparamref name="T" />.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="obj"/> is null.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNull<T, T1>(T obj, [NotNull] string errorMessageTemplate, T1 p1)
        {
            if (obj == null)
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1));

            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T">The type of the object to check for null.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>A <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="obj"/> is null.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNull<T, T1, T2>(T obj, [NotNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (obj == null)
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2));

            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T">The type of the object to check for null.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="errorMessageTemplate">The error message template.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns>A <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="obj"/> is null.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNull<T, T1, T2, T3>(T obj, [NotNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (obj == null)
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2, p3));

            return obj;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <typeparam name="T">The type of the object to check for null.</typeparam>
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
        /// <returns>A <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="obj"/> is null.</exception>
        [StringFormatMethod("errorMessageTemplate")]
        public static T CheckNotNull<T, T1, T2, T3, T4>(
            T obj,
            [NotNull] string errorMessageTemplate,
            T1 p1,
            T2 p2,
            T3 p3,
            T4 p4)
        {
            if (obj == null)
                throw new ArgumentNullException(string.Format(errorMessageTemplate, p1, p2, p3, p4));

            return obj;
        }

        private static string StringValue(object? obj) => obj?.ToString() ?? "null";
    }
}