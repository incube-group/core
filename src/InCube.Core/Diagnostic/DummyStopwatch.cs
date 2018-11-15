using System;

namespace InCube.Core.Diagnostic
{
    public interface IStopwatch
    {
        void Start();

        void Stop();
        
        void Reset();
        
        void Restart();
        
        bool IsRunning { get; }
        
        TimeSpan Elapsed { get; }
        
        long ElapsedMilliseconds { get; }
        
        long ElapsedTicks { get; }
    }

    public sealed class DummyStopwatch : IStopwatch
    {
        public static readonly DummyStopwatch Instance = new DummyStopwatch();

        private const int ZeroTicks = 0;
        private static readonly TimeSpan EmptyTimeSpan = new TimeSpan(ZeroTicks);

        private DummyStopwatch()
        {
        }

        public void Start()
        {
        }

        public static DummyStopwatch StartNew() => Instance;

        public void Stop()
        {
        }

        public void Reset()
        {
        }

        public void Restart()
        {
        }

        public bool IsRunning => false;

        public TimeSpan Elapsed => EmptyTimeSpan;

        public long ElapsedMilliseconds => ZeroTicks;

        public long ElapsedTicks => ZeroTicks;

        public static long GetTimestamp() => ZeroTicks;
    }
}
