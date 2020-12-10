using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace InCube.Core.Test.Functional
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleType",
        Justification = "Companion object.")]
    internal class Boxed<T> : IEquatable<Boxed<T>> where T : struct
    {
        [JsonConstructor]
        internal Boxed(T value)
        {
            this.Value = value;
        }

        public T Value { get; }

        public static implicit operator T(Boxed<T> boxed) => boxed.Value;

        public static implicit operator Boxed<T>(T value) => new Boxed<T>(value);

        public bool Equals(Boxed<T> that)
        {
            if (ReferenceEquals(null, that))
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(this.Value, that.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Boxed<T>)obj);
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => $"{this.Value}";
    }

    internal static class Boxed
    {
        public static Boxed<T> Of<T>(T value) where T : struct => value;
    }
}