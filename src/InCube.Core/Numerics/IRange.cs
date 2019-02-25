using System;

namespace InCube.Core.Numerics
{
    public interface IHasMin<out T>
    {
        T Min { get; }
    }

    public interface IHasMax<out T>
    {
        T Max { get; }
    }

    public interface IRange<out T> : IHasMin<T>, IHasMax<T>
    {
    }

    public interface IInvariantRange<T, TRange> : IRange<T>, IEquatable<TRange> where TRange : IInvariantRange<T, TRange>
    {
        bool IsInside(T x);

        bool IsInside(TRange that);
        
        bool IsOverlapping(TRange that);
        
        TRange Intersection(TRange that);
    }

    public static class Ranges
    {
        public static bool IsInside<T>(this IRange<T> @this, T x) where T : IComparable<T> => 
            @this.Min.CompareTo(x) <= 0 && x.CompareTo(@this.Max) <= 0;

        public static bool IsInside<T>(this IRange<T> @this, IRange<T> that) where T : IComparable<T> =>
            @this.IsInside(that.Min) && @this.IsInside(that.Max);

        public static bool IsOverlapping<T>(this IRange<T> @this, IRange<T> that) where T : IComparable<T> =>
            @this.IsInside(that.Min) || @this.IsInside(that.Max) || that.IsInside(@this.Min) || that.IsInside(@this.Max);

        public static Range<T> Intersection<T>(this IRange<T> @this, IRange<T> that) where T : IComparable<T> =>
            ToRange(Comparables.Max(@this.Min, that.Min), Comparables.Min(@this.Max, that.Max));

        public static Range<T> ToRange<T>(this T min, T max) where T : IComparable<T> =>
            new Range<T>(min, max);

        public static Range<T> ToRange<T>(this IRange<T> range) where T : IComparable<T> =>
            range.Min.ToRange(range.Max);

        [Obsolete("unnecessary call")]
        public static Range<T> ToRange<T>(this Range<T> range) where T : IComparable<T> => range;
    }
}
