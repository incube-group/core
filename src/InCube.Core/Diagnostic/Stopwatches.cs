using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace InCube.Core.Diagnostic
{
    /// <summary>
    /// Extension methods for stopwatches.
    /// </summary>
    public static class Stopwatches
    {
        /// <summary>
        /// Writes the time elapsed for <paramref name="name"/>.
        /// </summary>
        /// <param name="watch">The <see cref="Stopwatch"/>.</param>
        /// <param name="name">Name of timed thing.</param>
        public static void WriteLine(this Stopwatch watch, string name) => Console.WriteLine($"{name} time: {watch.Elapsed}");

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="watch">Dummy stopwatch doing nothing.</param>
        /// <param name="name">Doesn't matter.</param>
        [SuppressMessage("Usage", "CA1801: Review unused parameters", Justification = "Need parameter for complying with signature of WriteLine(Stopwatch, string).")]
        public static void WriteLine(this DummyStopwatch watch, string name)
        {
        }
    }
}