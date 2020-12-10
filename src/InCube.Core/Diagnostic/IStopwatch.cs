using System;

namespace InCube.Core.Diagnostic
{
    /// <summary>
    /// Interface exposing the common operations of a stopwatch.
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        /// Gets a value indicating whether or not the stopwatch is timing.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the time that the stopwatch has timed.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the time that the stopwatch as timed in milliseconds.
        /// </summary>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Gets the time that the stopwatch as timed in ticks.
        /// </summary>
        long ElapsedTicks { get; }

        /// <summary>
        /// Starts timing.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops timing.
        /// </summary>
        void Stop();

        /// <summary>
        /// Resets timing.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resets and starts timing.
        /// </summary>
        void Restart();
    }
}