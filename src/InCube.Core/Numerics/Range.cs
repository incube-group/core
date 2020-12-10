using System;
using System.Collections.Generic;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Numerics
{
    /// <summary>
    /// A range type representing a closed interval.
    /// </summary>
    /// <typeparam name="T">The type of the lower and upper bound.</typeparam>
    public readonly struct Range<T> : IInvariantRange<T, Range<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Range{T}"/> struct.
        /// </summary>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        [JsonConstructor]
        public Range(T min, T max)
        {
            Preconditions.CheckArgument(min.CompareTo(max) <= 0, "min {0} > max {1}", min, max);
            this.Min = min;
            this.Max = max;
        }

        public Range<T> With(Option<T> min = default, Option<T> max = default) => new(min.GetValueOrDefault(this.Min), max.GetValueOrDefault(this.Max));

        /// <inheritdoc/>
        public T Min { get; }

        /// <inheritdoc/>
        public T Max { get; }

        public static bool operator ==(Range<T> left, Range<T> right) => Equals(left, right);

        public static bool operator !=(Range<T> left, Range<T> right) => !(left == right);

        /// <inheritdoc/>
        public bool Contains(T x) => this.Min.CompareTo(x) <= 0 && x.CompareTo(this.Max) <= 0;

        /// <inheritdoc/>
        public bool Contains(Range<T> range) => this.Min.CompareTo(range.Min) <= 0 && range.Max.CompareTo(this.Max) <= 0;

        /// <inheritdoc/>
        public bool OverlapsWith(Range<T> range) => this.Min.CompareTo(range.Max) <= 0 && range.Min.CompareTo(this.Max) <= 0;

        /// <inheritdoc/>
        public Range<T> IntersectWith(Range<T> range) => Comparables.Max(this.Min, range.Min).ToRange(Comparables.Min(this.Max, range.Max));

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator Range<T>((T Min, T Max) that) =>
#pragma warning restore CA2225 // Operator overloads have named alternates
            that.Min.ToRange(that.Max);

        public void Deconstruct(out T min, out T max)
        {
            min = this.Min;
            max = this.Max;
        }

        /// <inheritdoc/>
        public bool Equals(Range<T> that) => this.Min.CompareTo(that.Min) == 0 && this.Max.CompareTo(that.Max) == 0;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Range<T> range && this.Equals(range);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(this.Min) * 397) ^ EqualityComparer<T>.Default.GetHashCode(this.Max);
            }
        }

        public override string ToString() => $"[{this.Min}, {this.Max}]";
    }
}