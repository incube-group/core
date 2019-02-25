using System;

namespace InCube.Core.Numerics
{
    public static class Comparables
    {
        public static T Min<T>(T x, T y) where T : IComparable<T> => x.CompareTo(y) <= 0 ? x : y;

        public static T Max<T>(T x, T y) where T : IComparable<T> => x.CompareTo(y) >= 0 ? x : y;
    }
}