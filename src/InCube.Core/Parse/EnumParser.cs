using System;
using System.Linq;
using JetBrains.Annotations;

namespace InCube.Core.Parse
{
    /// <summary>
    /// Utilities to handle enums.
    /// </summary>
    [PublicAPI]
    public static class EnumParser
    {
        /// <summary>
        /// Parses a string to an enum value. If unsuccessful, an exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type of enum to parse to.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// The parsed enum.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws when <typeparamref name="T" /> is not an enum type and when the string cannot be parsed.
        /// </exception>
        public static T ParseEnum<T>(this string value)
            where T : struct
        {
            var enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new InvalidOperationException($"Attempted to parse '{value}' to type '{enumType.Name}', which is not an enum type!");

            var isAnInteger = int.TryParse(value, out var dummy);

            var parsedSuccessfully = Enum.TryParse<T>(value, true, out var parsed);

            if (isAnInteger || !parsedSuccessfully)
            {
                var enumValues = (T[])enumType.GetEnumValues();
                var validOptions = string.Join(", ", enumValues.Select(x => $"'{x.ToString()}'"));
                throw new InvalidOperationException($"Could not parse string '{value}' as enum value of '{enumType.Name}'. Valid options would be {validOptions}.");
            }

            return parsed;
        }

        /// <summary>
        /// Parses a string to a nullable enum value. If unsuccessful, an exception is thrown.
        /// </summary>
        /// <typeparam name="TNullable">The type of enum to parse to.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed enum.</returns>
        public static TNullable? ParseNullableEnum<TNullable>(this string value)
            where TNullable : struct
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return value.ParseEnum<TNullable>();
        }

        /// <summary>
        /// Attempts to parse the <paramref name="value"/> as a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to parse.</param>
        /// <typeparam name="TEnum">The result enum type.</typeparam>
        /// <returns>A nullable <typeparamref name="TEnum"/>.</returns>
        /// <exception cref="InvalidOperationException">Throws when <typeparamref name="TEnum" /> is not an enum type and when the string cannot be parsed.</exception>
        public static TEnum? ParseEnumOrNull<TEnum>(this string value)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);

            if (!enumType.IsEnum)
                throw new InvalidOperationException($"Attempted to parse '{value}' to type '{enumType.Name}', which is not an enum type!");

            int dummy;
            var isAnInteger = int.TryParse(value, out dummy);

            var parsedSuccessfully = Enum.TryParse(value, true, out TEnum parsed);

            if (isAnInteger || !parsedSuccessfully)
                return null;

            return parsed;
        }
    }
}