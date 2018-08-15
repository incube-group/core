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
    }
}
