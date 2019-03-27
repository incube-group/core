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

    /// <summary>
    /// A covariant range type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRange<out T> : IHasMin<T>, IHasMax<T>
    {
    }

    public interface IInvariantRange<T, TRange> : IRange<T>, IEquatable<TRange> where TRange : IInvariantRange<T, TRange>
    {
        bool Contains(T x);

        bool Contains(TRange that);
        
        bool OverlapsWith(TRange that);
        
        TRange IntersectWith(TRange that);
    }
}
