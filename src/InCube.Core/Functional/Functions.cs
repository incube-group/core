using System;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Helper for creating and applying lambda functions.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Infers the return type of a functional expression.
        /// </summary>
        /// <typeparam name="T">type being inferred</typeparam>
        /// <param name="func">a function to evaluate</param>
        /// <returns></returns>
        [PublicAPI]
        public static T Invoke<T>(Func<T> func) => func();

        public static T If<T>(bool condition, Func<T> ifBranch, Func<T> elseBranch) =>
            condition 
                ? ifBranch() 
                : elseBranch();

        [PublicAPI]
        public static T WithDisposables<T>(Func<Disposables, T> func)
        {
            using (var disposables = new Disposables())
            {
                return func(disposables);
            }
        }

        [PublicAPI]
        public static T ApplyOpt<T>(this T self, Func<T, Option<T>> f) => f(self).GetValueOrDefault(self);

        [PublicAPI]
        public static T ApplyOpt<T>(this T self, Func<T, T?> f) where T : struct => f(self).GetValueOrDefault(self);

        [PublicAPI]
        public static Lazy<T> Lazy<T>(Func<T> func) => new Lazy<T>(func);
    }
}
