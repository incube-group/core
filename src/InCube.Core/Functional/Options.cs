using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using InCube.Core.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Defines utility operations on instances of <see cref="Option{T}"/> and <see cref="Nullable{T}"/>.
    /// </summary>
    public static class Options
    {
        public static readonly Option<Nothing> None = default;

        public static Option<T> Some<T>([NotNull] T value) => new Option<T>(value);

        public static Option<T> ToOption<T>(this T value) where T : class =>
            value != null ? Some(value) : None;

        public static Option<T> ToOption<T>(this in T? value) where T : struct =>
            value.HasValue ? Some(value.Value) : None;

        public static Option<T> ToOption<T>(this IOption<T> value) =>
            value.HasValue ? Some(value.Value) : None;

        [Obsolete("remove unnecessary call to " + nameof(ToOption))]
        public static Option<T> ToOption<T>(this in Option<T> value) => value;

        public static Option<T> Empty<T>() => None;

        public static TOut Match<TOut, TIn>(this in TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn: struct =>
            self.HasValue ? some(self.Value) : none();

        public static T GetValueOrDefault<T>(this in T? self, Func<T> @default) where T : struct =>
            self ?? @default();

        public static Option<T> OrElse<T>(this in Option<T> self, Func<Option<T>> @default) =>
            self.HasValue ? self : @default();

        public static Option<T> OrElse<T>(this in Option<T> self, in Option<T> @default) =>
            self.HasValue ? self : @default;

        public static T? OrElse<T>(this in T? self, Func<T?> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this in T? self, T? @default) where T : struct =>
            self ?? @default;

        public static T? ToNullable<T>(this T self) where T : struct => self;

        public static T? ToNullable<T>(this in Option<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        public static Option<T> Flatten<T>(this in Option<Option<T>> self) =>
            self.HasValue ? self.Value : None;

        public static T? Flatten<T>(this in Option<T?> self) where T : struct =>
            self.HasValue ? self.Value : null;

        public static Option<T> Flatten<T>(this in Option<T>? self) =>
            self ?? None;

        public static bool Contains<T>(this in Option<T> self, T elem) => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in T? self, T elem) where T: struct => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in Option<T> self, T elem, IEqualityComparer<T> comparer) => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static bool Contains<T>(this in T? self, T elem, IEqualityComparer<T> comparer) where T: struct => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static Option<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) where TIn : struct =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static Option<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Option<TOut>> f) where TIn : struct =>
            self.HasValue ? f(self.Value): None;

        public static TOut? SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, TOut?> f) where TIn : struct where TOut: struct =>
            self.HasValue ? f(self.Value): default;

        public static T? Where<T>(this in T? self, Func<T, bool> p) where T: struct =>
            !self.HasValue || p(self.Value) ? self : default;

        public static bool Any<T>(this in T? self) where T : struct => self.HasValue;

        public static bool Any<T>(this in T? self, Func<T, bool> p) where T : struct => self.HasValue && p(self.Value);

        public static bool All<T>(this in T? self, Func<T, bool> p) where T : struct => !self.HasValue || p(self.Value);

        public static void ForEach<T>(this in T? self, Action<T> action) where T : struct
        {
            if (self.HasValue)
            {
                action(self.Value);
            }
        }

        public static void ForEach<T>(this in T? self, Action none, Action<T> some) where T : struct
        {
            if (self.HasValue)
            {
                some(self.Value);
            }
            else
            {
                none();
            }
        }
    }

    /// <summary>
    /// Represents the Bottom type, i.e., the type of the none option.
    /// </summary>
    // ReSharper disable once ConvertToStaticClass
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Nothing
    {
        private Nothing()
        {
            // no instantiation
        }
    }

    /// <summary>
    /// A covariant version of <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOption<out T> : IEnumerable<T>
    {
        bool HasValue { get; }

        T Value { get; }

        TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some);

        T GetValueOrDefault();

        Option<TOut> Select<TOut>(Func<T, TOut> f);

        Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> f);

        bool Any();

        bool Any(Func<T, bool> p);

        bool All(Func<T, bool> p);

        void ForEach(Action<T> action);

        void ForEach(Action none, Action<T> some);

        IOption<T> Where(Func<T, bool> p);
    }

    /// <summary>
    /// Implements an option data type. this in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// both struct and class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable] [JsonConverter(typeof(GenericOptionJsonConverter))]
    public readonly struct Option<T>: IEquatable<Option<T>>, IOption<T>
    {
        private readonly T _value;

        internal Option(T value)
        {
            HasValue = true;
            _value = value;
        }

        public bool HasValue { get; }

        public T Value => HasValue ? _value : throw new InvalidOperationException("None.Get");

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue) yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Option<T> that) =>
            !HasValue && !that.HasValue ||
            HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(_value, that._value);

        public override bool Equals(object obj)
        {
            if (obj == null) return !HasValue;
            return obj is Option<T> option && Equals(option);
        }

        public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public override string ToString() => HasValue ? $"Some({Value})" : "None";

        public static bool operator ==(Option<T> c1, Option<T> c2) => c1.Equals(c2);

        public static bool operator !=(Option<T> c1, Option<T> c2) => !(c1 == c2);

        public static explicit operator T(Option<T> value) => value.Value;

        /// <summary>
        /// Note that this operation involves boxing.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Option<T>(T value) => value != null ? new Option<T>(value) : default;

        public static implicit operator Option<T>(Option<Nothing> _) => default;

        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => HasValue ? some(_value) : none();

        public T GetValueOrDefault() => GetValueOrDefault(default(T));

        public T GetValueOrDefault(Func<T> @default) => HasValue ? _value : @default();

        public T GetValueOrDefault(T @default) => HasValue ? _value : @default;

        public bool Any() => HasValue;

        public bool Any(Func<T, bool> p) => HasValue && p(_value);


        public bool All(Func<T, bool> p) => !HasValue || p(_value);

        public void ForEach(Action<T> action)
        {
            if (HasValue)
            {
                action(_value);
            }
        }


        public void ForEach(Action none, Action<T> some)
        {
            if (HasValue)
            {
                some(_value);
            }
            else
            {
                none();
            }
        }

        public Option<TOut> Select<TOut>(Func<T, TOut> f) => HasValue ? Options.Some(f(_value)) : default;

        public Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> f) => HasValue ? f(_value) : default;

        public Option<T> Where(Func<T, bool> p) => !HasValue || p(_value) ? this : default;

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);

        public Option<TD> Cast<TD>() where TD : T => SelectMany(x => x is TD d ? Options.Some(d) : default);
    }

}