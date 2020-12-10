using System;
using JetBrains.Annotations;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// Companion class of <see cref="Range{T}"/>
    /// </summary>
    [PublicAPI]
    public static class Ranges
    {
        public static Range<T> ToRange<T>(this T min, T max)
            where T : IComparable<T> =>
            new(min, max);

        public static Range<T> ToRange<T>(this IRange<T> range)
            where T : IComparable<T> =>
            range.Min.ToRange(range.Max);

        [Obsolete("unnecessary call")]
        public static Range<T> ToRange<T>(this Range<T> range)
            where T : IComparable<T> =>
            range;
    }
}