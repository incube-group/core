using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

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

        public static Option<T> ToOption<T>(this in Option<T> self) => self;

        public static Option<T> Empty<T>() => None;

        public static TOut Match<TOut, TIn>(this in Option<TIn> self, Func<TOut> none, Func<TIn, TOut> some) =>
            self.HasValue ? some(self.Value) : none();

        public static TOut Match<TOut, TIn>(this in TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn: struct =>
            self.HasValue ? some(self.Value) : none();

        public static T GetValueOrDefault<T>(this in Option<T> self, Func<T> @default) =>
            self.HasValue ? self.Value : @default();

        public static T GetValueOrDefault<T>(this in Option<T> self, T @default) =>
            self.HasValue ? self.Value : @default;

        public static T GetValueOrDefault<T>(this in T? self, Func<T> @default) where T : struct =>
            self ?? @default();

        public static T GetValueOrDefault<T>(this in Option<T> self) => self.GetValueOrDefault(default(T));

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

        public static bool Contains<T>(this in Option<T> self, T elem) => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in T? self, T elem) where T: struct => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in Option<T> self, T elem, IEqualityComparer<T> comparer) => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static bool Contains<T>(this in T? self, T elem, IEqualityComparer<T> comparer) where T: struct => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static Option<TOut> Select<TIn, TOut>(this in Option<TIn> self, Func<TIn, TOut> f) =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static Option<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) where TIn : struct =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static Option<TOut> SelectMany<TIn, TOut>(this in Option<TIn> self, Func<TIn, Option<TOut>> f) =>
            self.HasValue ? f(self.Value): None;

        public static Option<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Option<TOut>> f) where TIn : struct =>
            self.HasValue ? f(self.Value): None;

        public static TOut? SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, TOut?> f) where TIn : struct where TOut: struct =>
            self.HasValue ? f(self.Value): default;

        public static Option<T> Where<T>(this in Option<T> self, Func<T, bool> p) =>
            !self.HasValue || p(self.Value) ? self : None;

        public static T? Where<T>(this in T? self, Func<T, bool> p) where T: struct =>
            !self.HasValue || p(self.Value) ? self : default;

        public static bool Any<T>(this in Option<T> self) => self.HasValue;

        public static bool Any<T>(this in T? self) where T : struct => self.HasValue;

        public static bool Any<T>(this in Option<T> self, Func<T, bool> p) => self.HasValue && p(self.Value);

        public static bool All<T>(this in Option<T> self, Func<T, bool> p) => !self.HasValue || p(self.Value);

        public static bool All<T>(this in T? self, Func<T, bool> p) where T : struct => !self.HasValue || p(self.Value);

        public static void ForEach<T>(this in Option<T> self, Action<T> action)
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this in T? self, Action<T> action) where T : struct
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this in Option<T> self, Action none, Action<T> some)
        {
            if (self.HasValue) some(self.Value);
            else none();
        }

        public static void ForEach<T>(this in T? self, Action none, Action<T> some) where T : struct
        {
            if (self.HasValue) some(self.Value);
            else none();
        }

        /// <summary>
        /// Compiler supported safe upcasting. Ideally, this should be implemented as an implicit conversion, but
        /// this is currently not supported by the compiler (https://github.com/dotnet/csharplang/issues/534).
        /// </summary>
        /// <typeparam name="TU">The </typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="_">Helps the compiler to infer the type parameter<code>TU</code>. Clients will
        /// generally pass <code>default(TU)</code>.</param>
        /// <returns></returns>
        public static Option<TU> Upcast<TU, T>(this in Option<T> self, TU _ = default) where T : TU =>
            self.HasValue ? Some<TU>(self.Value) : None;
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
    /// Implements an option data type. this in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// both struct and class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct Option<T>: IEnumerable<T>, IEquatable<Option<T>>
    {
        private readonly T _value;

        internal Option([NotNull] T value)
        {
            Debug.Assert(value != null, nameof(value) + " is null");
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
    }

}