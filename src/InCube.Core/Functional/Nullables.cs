using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Extension methods for <see cref="Nullable{T}"/> types.
    /// </summary>
    [PublicAPI]
    public static class Nullables
    {
        #region Conversion

        /// <summary>
        /// Converts a non-nullable value type to a nullable value type
        /// </summary>
        /// <typeparam name="T">The underlying value type</typeparam>
        /// <param name="self">Input non-nullable value type</param>
        /// <returns>Nullable value type</returns>
        public static T? ToNullable<T>(this T self) where T : struct => self;

        #endregion

        #region IOption (Monad) like extensions
        
        /// <summary>
        /// Tries to get value out of nullable value type, or generates default using input function
        /// </summary>
        /// <typeparam name="T">Underlying type</typeparam>
        /// <param name="self">The input nullable value type</param>
        /// <param name="default">Default generating function</param>
        /// <returns>Non-null value type</returns>
        public static T GetValueOr<T>(this in T? self, Func<T> @default) where T : struct => self ?? @default();

        /// <summary>
        /// Provides alternate nullable value generating function to an input nullable value
        /// </summary>
        /// <typeparam name="T">Underlying value type</typeparam>
        /// <param name="self">The input nullable value type</param>
        /// <param name="default">Alternative generating function</param>
        /// <returns>Nullable value type</returns>
        public static T? OrElse<T>(this in T? self, Func<T?> @default) where T : struct => self ?? @default();

        /// <summary>
        /// Provides an alternate nullable value to an input nullable value
        /// </summary>
        /// <typeparam name="T">Underlying value type</typeparam>
        /// <param name="self">The input nullable value</param>
        /// <param name="default">Alternate nullable value</param>
        /// <returns>A nullable value type</returns>
        public static T? OrElse<T>(this in T? self, T? @default) where T : struct => self ?? @default;

        /// <summary>
        /// Executes one of two mappings to an output type depending on whether the input nullable value type is null or not
        /// </summary>
        /// <typeparam name="TOut">Output type of both mappings</typeparam>
        /// <typeparam name="TIn">Type of nullable value input</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="none">Mapping in case nullable is null</param>
        /// <param name="some">Mapping in case nullable is not null</param>
        /// <returns>An object of the output type</returns>
        public static TOut Match<TOut, TIn>(this in TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn : struct => self.Select(x => some(x).ToAny()) ?? none();

        /// <summary>
        /// Checks whether a nullable has a value and is equal to the input element using the default equality comparer
        /// </summary>
        /// <typeparam name="T">The type of the nullable</typeparam>
        /// <param name="self">The input nullable</param>
        /// <param name="elem">The element to check against the nullable if it's not null</param>
        /// <returns>True if nullable is not null and it's equal to the input element, false otherwise</returns>
        public static bool Contains<T>(this in T? self, T elem) where T : struct => self.Contains(elem, EqualityComparer<T>.Default);

        /// <summary>
        /// Checks whether a nullable has a value and is equal to the input element using the input equality comparer
        /// </summary>
        /// <typeparam name="T">Underlying value type</typeparam>
        /// <param name="self">The input nullable</param>
        /// <param name="elem">The element to compare with</param>
        /// <param name="comparer">The comparer to use</param>
        /// <returns>True if nullable is not null and it's equal to the input element, false otherwise</returns>
        public static bool Contains<T>(this in T? self, T elem, IEqualityComparer<T> comparer) where T : struct => self.Select(x => comparer.Equals(x, elem)) ?? false;

        /// <summary>
        /// Filters a nullable : if it is null, returns null, if it is not null, checks whether is meets the predicate, if it doesn't returns null, otherwise returns the same nullable
        /// </summary>
        /// <typeparam name="T">The type of the nullable value</typeparam>
        /// <param name="self">The input nullable</param>
        /// <param name="p">The predicate to check</param>
        /// <returns>A nullable value type</returns>
        public static T? Where<T>(this in T? self, Func<T, bool> p) where T : struct => self.Any(p) ? self : default;

        /// <summary>
        /// Checks whether a nullable value type is null or not
        /// </summary>
        /// <typeparam name="T">The nullable value type</typeparam>
        /// <param name="self">The input nullable value type</param>
        /// <returns>True if input is not null, false otherwise</returns>
        public static bool Any<T>(this in T? self) where T : struct => self.HasValue;

        /// <summary>
        /// Checks whether input nullable is non-null and its value satisfies the given predicate
        /// </summary>
        /// <typeparam name="T">Underlying nullable value type</typeparam>
        /// <param name="self">Nullable to check</param>
        /// <param name="p">Predicate to check with</param>
        /// <returns>True if input is not null and predicate is satisfied, false otherwise</returns>
        public static bool Any<T>(this in T? self, Func<T, bool> p) where T : struct => self?.Apply(p) ?? false;

        /// <summary>
        /// Checks whether input nullable is null or its value satisfied the given predicate
        /// </summary>
        /// <typeparam name="T">Type of input nullable value</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="p">Predicate to check value against</param>
        /// <returns>True if null or predicate is true, false otherwise</returns>
        public static bool All<T>(this in T? self, Func<T, bool> p) where T : struct => self?.Apply(p) ?? true;

        /// <summary>
        /// Performs given action on nullable value, if it is not null
        /// </summary>
        /// <typeparam name="T">Type of input nullable value</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="action">The action to perform</param>
        public static void ForEach<T>(this in T? self, Action<T> action) where T : struct => self?.Apply(action);

        /// <summary>
        /// Performs one action or another depending on whether input nullable value is null or not
        /// </summary>
        /// <typeparam name="T">Type of input nullable value</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="none">Action to perform in case input is null</param>
        /// <param name="some">Action to perform in case input is not null</param>
        public static void ForEach<T>(this in T? self, Action none, Action<T> some) where T : struct
        {
            self.ForEach(some);
            if (!self.HasValue)
            {
                none();
            }
        }

        /// <summary>
        /// Maps input nullable value to an output nullable value
        /// </summary>
        /// <typeparam name="TIn">Type of input nullable value</typeparam>
        /// <typeparam name="TOut">Type of output nullable value</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="f">Mapping</param>
        /// <returns>A nullable value</returns>
        public static TOut? Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) 
            where TIn : struct where TOut : struct =>
            self?.Apply(f);

        /// <summary>
        /// Maps input nullable value to an output nullable value, when mapping can return null
        /// </summary>
        /// <typeparam name="TIn">Type of input nullable value</typeparam>
        /// <typeparam name="TOut">Type of output nullable value</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="f">Mapping</param>
        /// <returns>A nullable value</returns>
        public static TOut? SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, TOut?> f) 
            where TIn : struct where TOut : struct =>
            self?.Apply(f);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="map"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        public static TOut? ApplyNullable<TIn, TOut>(this TIn? input, Func<TIn, TOut?> map) where TIn : struct where TOut : class =>
            input is null ? null : map(input.Value);

        /// <summary>
        /// Gets first element of sequence that satisfies given predicate, or null if no such element exists
        /// </summary>
        /// <param name="inputs">Input enumerable to look through</param>
        /// <param name="predicate">Predicate to check elements against</param>
        /// <typeparam name="TIn">Type of input enumerable</typeparam>
        /// <returns>A nullable value type object</returns>
        public static TIn? FirstNullable<TIn>(this IEnumerable<TIn> inputs, Func<TIn, bool> predicate) where TIn : struct
        {
            using var enumerator = inputs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    return enumerator.Current;
            }

            return null;
        }

        #endregion

        #region NullableReferences

        /// <summary>
        /// Gets value of nullable, or a default value if it is null
        /// </summary>
        /// <typeparam name="T">The type of the input and output</typeparam>
        /// <param name="nullable">The nullable to check</param>
        /// <param name="default">The default to use in case input is null</param>
        /// <returns>A T object</returns>
        public static T GetValueOrDefault<T>(this T? nullable, T @default) where T : class => nullable ?? @default;

        #endregion
    }
}