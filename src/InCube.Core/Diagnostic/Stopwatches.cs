using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace InCube.Core.Diagnostic
{
    public static class Stopwatches
    {
        public static void WriteLine(this Stopwatch watch, string name) =>
            Console.WriteLine($"{name} time: {watch.Elapsed}");

        [SuppressMessage("Usage",
            "CA1801: Review unused parameters",
            Justification = "Need parameter for complying with signature of WriteLine(Stopwatch, string).")]
        public static void WriteLine(this DummyStopwatch watch, string name)
        {
        }
    }
}