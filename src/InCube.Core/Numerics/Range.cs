using System;
using System.Collections.Generic;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// A range type representing a closed interval.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Range<T> : IInvariantRange<T, Range<T>> where T : IComparable<T>
    {
        [JsonConstructor]
        public Range(T min, T max)
        {
            Preconditions.CheckArgument(min.CompareTo(max) <= 0, "min {0} > max {1}", min, max);
            Min = min;
            Max = max;
        }

        public Range<T> With(Option<T> min = default, Option<T> max = default) =>
            new Range<T>(min.GetValueOrDefault(Min), max.GetValueOrDefault(Max));

        public T Min { get; }

        public T Max { get; }

        public bool Contains(T x) => Min.CompareTo(x) <= 0 && x.CompareTo(Max) <= 0;

        public bool Contains(Range<T> that) => 
            this.Min.CompareTo(that.Min) <= 0 && that.Max.CompareTo(this.Max) <= 0;

        public bool OverlapsWith(Range<T> that) =>
            this.Min.CompareTo(that.Max) <= 0 && that.Min.CompareTo(this.Max) <= 0;

        public Range<T> IntersectWith(Range<T> that) => 
            Comparables.Max(this.Min, that.Min).ToRange(Comparables.Min(this.Max, that.Max));

        #pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator Range<T>((T Min, T Max) that) =>
            #pragma warning restore CA2225 // Operator overloads have named alternates
            that.Min.ToRange(that.Max);

        public void Deconstruct(out T min, out T max)
        {
            min = Min;
            max = Max;
        }

        public bool Equals(Range<T> that) => 
            this.Min.CompareTo(that.Min) == 0 && this.Max.CompareTo(that.Max) == 0;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Range<T> range && this.Equals(range);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Min) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Max);
            }
        }

        public static bool operator ==(Range<T> left, Range<T> right) => Equals(left, right);

        public static bool operator !=(Range<T> left, Range<T> right) => !(left == right);

        public override string ToString() => $"[{Min}, {Max}]";
    }

    public static class Range
    {
        public static Range<T> ToRange<T>(this T min, T max) where T : IComparable<T> =>
            new Range<T>(min, max);

        public static Range<T> ToRange<T>(this IRange<T> range) where T : IComparable<T> =>
            range.Min.ToRange(range.Max);

        [Obsolete("unnecessary call")]
        public static Range<T> ToRange<T>(this Range<T> range) where T : IComparable<T> => range;
    }
}