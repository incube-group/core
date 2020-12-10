using System;
using System.Threading;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Helper for creating and applying lambda functions.
    /// </summary>
    [PublicAPI]
    public static class Functions
    {
        /// <summary>
        /// Infers the return type of a functional expression.
        /// </summary>
        /// <typeparam name="T">type being inferred.</typeparam>
        /// <param name="func">a function to evaluate.</param>
        /// <returns>The result of the <paramref name="func"/>.</returns>
        public static T Invoke<T>(Func<T> func) => func();

        /// <summary>
        /// Alternative syntax for ternary operator.
        /// </summary>
        /// <typeparam name="T">Type of output of both branches.</typeparam>
        /// <param name="condition">Condition to check to choose a branch.</param>
        /// <param name="ifBranch">True branch function.</param>
        /// <param name="elseBranch">False branch function.</param>
        /// <returns>A T object.</returns>
        public static T If<T>(bool condition, Func<T> ifBranch, Func<T> elseBranch) =>
            condition ? ifBranch() : elseBranch();

        /// <summary>
        /// Shorthand for 'using' syntax when we want to return something out of the block.
        /// </summary>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="func">The code to run with disposable.</param>
        /// <returns>The output of <paramref name="func"/>.</returns>
        public static TOut WithDisposables<TOut>(Func<Disposables, TOut> func)
        {
            using var disposables = new Disposables();
            return func(disposables);
        }

        /// <summary>
        /// Tries to map any type to a super-type. Returns input in case result is None.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="self">Input to map.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A TOut object.</returns>
        public static TOut ApplyOpt<TIn, TOut>(this TIn self, Func<TIn, Option<TOut>> f)
            where TIn : TOut =>
            f(self).GetValueOrDefault(self);

        /// <summary>
        /// Tries to map a reference type to a super-type. Returns input in case result is None.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="self">Input to map.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A TOut object.</returns>
        public static TOut ApplyOpt<TIn, TOut>(this TIn self, Func<TIn, Maybe<TOut>> f)
            where TOut : class
            where TIn : TOut =>
            f(self).GetValueOrDefault(self);

        /// <summary>
        /// Tries to map a value type to a new value, if it returns null, rolls back to the input value.
        /// </summary>
        /// <typeparam name="T">Type of value to map, and of nullable output.</typeparam>
        /// <param name="self">Value to map.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A T value.</returns>
        public static T ApplyOpt<T>(this T self, Func<T, T?> f)
            where T : struct =>
            f(self).GetValueOrDefault(self);

        /// <summary>
        /// Tries to map a reference type to a new value, if it returns null, rolls back to the input value.
        /// </summary>
        /// <typeparam name="T">Type of value to map, and of nullable output.</typeparam>
        /// <param name="self">Value to map.</param>
        /// <param name="f">Mapping.</param>
        /// <returns>A T value.</returns>
        public static T ApplyOpt<T>(this T self, Func<T, T?> f)
            where T : class =>
            f(self).GetValueOrDefault(self);

        /// <summary>
        /// Syntactic sugar for creating a <see cref="Lazy{T}"/>.
        /// </summary>
        /// <typeparam name="T">Value type of the lazy.</typeparam>
        /// <param name="func">Generator function for the value.</param>
        /// <param name="lazyThreadSafetyMode">Thread safety mode.</param>
        /// <returns>A <see cref="Lazy{T}"/>.</returns>
        public static Lazy<T> Lazy<T>(Func<T> func, LazyThreadSafetyMode lazyThreadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication) => new Lazy<T>(func, lazyThreadSafetyMode);
    }
}
