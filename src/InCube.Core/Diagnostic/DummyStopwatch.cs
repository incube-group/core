using System;
using System.Diagnostics;

namespace InCube.Core.Diagnostic
{
    public sealed class DummyStopwatch {
        public static readonly DummyStopwatch Instance = new DummyStopwatch();
        
        private const int ZeroTicks = 0;
        private static readonly TimeSpan EmptyTimeSpan = new TimeSpan(ZeroTicks);

        private DummyStopwatch(){}

        public void Start() {}

        public static DummyStopwatch StartNew() => Instance;

        public void Stop() {}

        public void Reset() {}

        public void Restart() {}

        public bool IsRunning => false;

        public TimeSpan Elapsed => EmptyTimeSpan;

        public long ElapsedMilliseconds => ZeroTicks;

        public long ElapsedTicks => ZeroTicks;

        public static long GetTimestamp() => ZeroTicks;
    }

    public static class Stopwatches
    {
        public static void WriteLine(this Stopwatch watch, string name) => 
            Console.WriteLine($"{name} time: {watch.Elapsed}");

        public static void WriteLine(this DummyStopwatch watch, string name)
        {
        }
    }
}
