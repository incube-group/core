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

        /**
         * Substitutes each {@code {}} in {@code template} with an argument. These are matched by
         * position: the first {@code {}} gets {@code args[0]}, etc. If there are more arguments than
         * placeholders, the unmatched arguments will be appended to the end of the formatted message in
         * square braces.
         *
         * @param template a non-null string containing 0 or more {@code {}} placeholders.
         * @param args the arguments to be substituted into the message template. Arguments are converted
         *     to strings using {@link string#valueOf(object)}. Arguments can be null.
         */
        // Note that this is somewhat-improperly used from Verify.java as well.
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

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * @param expression a bool expression
         * @throws ArgumentException if {@code expression} is false
         */
        public static void CheckArgument(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException();
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * @param expression a bool expression
         * @param errorMessage the exception message to use if the check fails; will be converted to a
         *     string using {@link string#valueOf(object)}
         * @throws ArgumentException if {@code expression} is false
         */
        public static void CheckArgument(bool expression, object errorMessage)
        {
            if (!expression)
            {
                throw new ArgumentException(StringValue(errorMessage));
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * @param expression a bool expression
         * @param errorMessageTemplate a template for the exception message should the check fail. The
         *     message is formed by replacing each {@code {}} placeholder in the template with an
         *     argument. These are matched by position - the first {@code {}} gets {@code
         *     errorMessageArgs[0]}, etc. Unmatched arguments will be appended to the formatted message in
         *     square braces. Unmatched placeholders will be left as-is.
         * @param errorMessageArgs the arguments to be substituted into the message template. Arguments
         *     are converted to strings using {@link string#valueOf(object)}.
         * @throws ArgumentException if {@code expression} is false
         * @throws ArgumentNullException if the check fails and either {@code errorMessageTemplate} or
         *     {@code errorMessageArgs} is null (don't let this happen)
         */
        public static void CheckArgument(
            bool expression,
            [CanBeNull] string errorMessageTemplate,
            [CanBeNull] params object[] errorMessageArgs)
        {
            if (!expression)
            {
                throw new ArgumentException(Format(errorMessageTemplate, errorMessageArgs));
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * <p>See {@link #CheckArgument(bool, string, params object[])} for details.
         */
        public static void CheckArgument<T1>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1));
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * <p>See {@link #CheckArgument(bool, string, params object[])} for details.
         */
        public static void CheckArgument<T1, T2>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2));
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * <p>See {@link #CheckArgument(bool, string, params object[])} for details.
         */
        public static void CheckArgument<T1, T2, T3>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2, p3));
            }
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the calling method.
         *
         * <p>See {@link #CheckArgument(bool, string, params object[])} for details.
         */
        public static void CheckArgument<T1, T2, T3, T4>(bool b, [CanBeNull] string errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (!b)
            {
                throw new ArgumentException(Format(errorMessageTemplate, p1, p2, p3, p4));
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

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * @param reference an object reference
         * @param errorMessage the exception message to use if the check fails; will be converted to a
         *     string using {@link String#valueOf(object)}
         * @return the non-null reference that was validated
         * @throws ArgumentNullException if {@code reference} is null
         */
        public static T CheckNotNull<T>(T reference, [CanBeNull] object errorMessage)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(StringValue(errorMessage));
            }
            return reference;
        }

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * @param reference an object reference
         * @param errorMessageTemplate a template for the exception message should the check fail. The
         *     message is formed by replacing each {@code %s} placeholder in the template with an
         *     argument. These are matched by position - the first {@code %s} gets {@code
         *     errorMessageArgs[0]}, etc. Unmatched arguments will be appended to the formatted message in
         *     square braces. Unmatched placeholders will be left as-is.
         * @param errorMessageArgs the arguments to be substituted into the message template. Arguments
         *     are converted to strings using {@link String#valueOf(object)}.
         * @return the non-null reference that was validated
         * @throws ArgumentNullException if {@code reference} is null
         */
        public static T CheckNotNull<T>(
            T reference, [CanBeNull] String errorMessageTemplate, [CanBeNull] params object[] errorMessageArgs)
        {
            if (reference == null)
            {
                // If either of these parameters is null, the right thing happens anyway
                throw new ArgumentNullException(Format(errorMessageTemplate, errorMessageArgs));
            }
            return reference;
        }

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * <p>See {@link #checkNotNull(object, String, params object[])} for details.
         */
        public static T CheckNotNull<T, T1>(T obj, [CanBeNull] String errorMessageTemplate, T1 p1)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1));
            }
            return obj;
        }

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * <p>See {@link #checkNotNull(object, String, params object[])} for details.
         */
        public static T CheckNotNull<T, T1, T2>(T obj, [CanBeNull] String errorMessageTemplate, T1 p1, T2 p2)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1, p2));
            }
            return obj;
        }

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * <p>See {@link #checkNotNull(object, String, params object[])} for details.
         */
        public static T CheckNotNull<T, T1, T2, T3>(T obj, [CanBeNull] String errorMessageTemplate, T1 p1, T2 p2, T3 p3)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1));
            }
            return obj;
        }

        /**
         * Ensures that an object reference passed as a parameter to the calling method is not null.
         *
         * <p>See {@link #checkNotNull(object, String, params object[])} for details.
         */
        public static T CheckNotNull<T, T1, T2, T3, T4>(T obj, [CanBeNull] String errorMessageTemplate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(Format(errorMessageTemplate, p1, p2, p3, p4));
            }
            return obj;
        }
    }
}
