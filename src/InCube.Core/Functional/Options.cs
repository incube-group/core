using System;
using System.Collections;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Defines utility operations on instances of <see cref="IOption{T}"/> and <see cref="Option{T}"/>. The
    /// duplication of methods for both types aims at avoiding unnecessary boxing where possible. 
    /// </summary>
    public static class Options
    {
        public static readonly Option<Nothing> None = default;

        public static Option<T> Some<T>(T value) => new Option<T>(value);

        public static Option<T> ToOption<T>(this T value) where T : class => 
            value != null ? Some(value) : None;

        public static Option<T> ToOption<T>(this T? value) where T : struct =>
            value.HasValue ? Some(value.Value) : None;

        public static Option<T> ToOption<T>(this IOption<T> self) => self.HasValue ? Some(self.Value) : None;

        public static Option<T> ToOption<T>(this Option<T> self) => self;

        public static Option<T> Empty<T>() => None;

        public static TOut Match<TOut, TIn>(this IOption<TIn> self, Func<TOut> none, Func<TIn, TOut> some) =>
            self.HasValue ? some(self.Value) : none();

        public static TOut Match<TOut, TIn>(this Option<TIn> self, Func<TOut> none, Func<TIn, TOut> some) =>
            self.HasValue ? some(self.Value) : none();

        public static TOut Match<TOut, TIn>(this TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn: struct =>
            self.HasValue ? some(self.Value) : none();

        public static T GetValueOrDefault<T>(this IOption<T> self, Func<T> @default) =>
            self.HasValue ? self.Value : @default();

        public static T GetValueOrDefault<T>(this IOption<T> self, T @default) =>
            self.HasValue ? self.Value : @default;

        public static T GetValueOrDefault<T>(this Option<T> self, Func<T> @default) =>
            self.HasValue ? self.Value : @default();

        public static T GetValueOrDefault<T>(this Option<T> self, T @default) =>
            self.HasValue ? self.Value : @default;

        public static IOption<T> OrElse<T>(this IOption<T> self, Func<IOption<T>> @default) =>
            self.HasValue ? self : @default();

        public static IOption<T> OrElse<T>(this IOption<T> self, IOption<T> @default) =>
            self.HasValue ? self : @default;

        public static IOption<T> OrElse<T>(this IOption<T> self, Func<Option<T>> @default) =>
            self.HasValue ? self : @default();

        public static IOption<T> OrElse<T>(this IOption<T> self, Option<T> @default) =>
            self.HasValue ? self : @default;

        public static IOption<T> OrElse<T>(this Option<T> self, Func<IOption<T>> @default) =>
            self.HasValue ? self : @default();

        public static IOption<T> OrElse<T>(this Option<T> self, IOption<T> @default) =>
            self.HasValue ? self : @default;

        public static Option<T> OrElse<T>(this Option<T> self, Func<Option<T>> @default) =>
            self.HasValue ? self : @default();

        public static Option<T> OrElse<T>(this Option<T> self, Option<T> @default) =>
            self.HasValue ? self : @default;

        public static T? OrElse<T>(this T? self, Func<T?> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this T? self, T? @default) where T : struct =>
            self ?? @default;

        public static T OrNull<T>(this IOption<T> self) where T : class => self.GetValueOrDefault(default(T));

        public static T OrNull<T>(this Option<T> self) where T : class => self.GetValueOrDefault(default(T));

        public static T? ToNullable<T>(this IOption<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        public static T? ToNullable<T>(this Option<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        public static IOption<TInner> Flatten<TInner, TOption>(this IOption<TOption> self) where TOption : IOption<TInner> =>
            self.HasValue ? (IOption<TInner>) self.Value : Empty<TInner>();

        public static IOption<TInner> Flatten<TInner, TOption>(this Option<TOption> self) where TOption : IOption<TInner> =>
            self.HasValue ? (IOption<TInner>) self.Value : Empty<TInner>();

        public static Option<T> Flatten<T>(this IOption<Option<T>> self) =>
            self.HasValue ? self.Value : None;

        public static Option<T> Flatten<T>(this Option<Option<T>> self) =>
            self.HasValue ? self.Value : None;

        public static bool Contains<T>(this IOption<T> self, T elem) =>
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this Option<T> self, T elem) => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this T? self, T elem) where T: struct => 
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this IOption<T> self, T elem, IEqualityComparer<T> comparer) => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static bool Contains<T>(this Option<T> self, T elem, IEqualityComparer<T> comparer) => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static bool Contains<T>(this T? self, T elem, IEqualityComparer<T> comparer) where T: struct => 
            self.HasValue && comparer.Equals(self.Value, elem);

        public static Option<TOut> Select<TOut, TIn>(this IOption<TIn> self, Func<TIn, TOut> f) =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static Option<TOut> Select<TOut, TIn>(this Option<TIn> self, Func<TIn, TOut> f) =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static Option<TOut> Select<TOut, TIn>(this TIn? self, Func<TIn, TOut> f) where TIn : struct =>
            self.HasValue ? Some(f(self.Value)) : None;

        public static IOption<TOut> SelectMany<TOut, TIn>(this IOption<TIn> self, Func<TIn, IOption<TOut>> f) =>
            self.HasValue ? f(self.Value): Empty<TOut>();

        public static IOption<TOut> SelectMany<TOut, TIn>(this IOption<TIn> self, Func<TIn, Option<TOut>> f) =>
            self.HasValue ? f(self.Value): Empty<TOut>();

        public static IOption<TOut> SelectMany<TOut, TIn>(this Option<TIn> self, Func<TIn, IOption<TOut>> f) =>
            self.HasValue ? f(self.Value): Empty<TOut>();

        public static Option<TOut> SelectMany<TOut, TIn>(this Option<TIn> self, Func<TIn, Option<TOut>> f) =>
            self.HasValue ? f(self.Value): None;

        public static Option<TOut> SelectMany<TOut, TIn>(this TIn? self, Func<TIn, Option<TOut>> f) where TIn : struct =>
            self.HasValue ? f(self.Value): None;

        public static IOption<TOut> SelectMany<TOut, TIn>(this TIn? self, Func<TIn, IOption<TOut>> f) where TIn : struct =>
            self.HasValue ? f(self.Value): Empty<TOut>();

        public static TOut? SelectMany<TOut, TIn>(this TIn? self, Func<TIn, TOut?> f) where TIn : struct where TOut: struct =>
            self.HasValue ? f(self.Value): default;

        public static IOption<T> Where<T>(this IOption<T> self, Func<T, bool> p) =>
            !self.HasValue || p(self.Value) ? self : Empty<T>();

        public static Option<T> Where<T>(this Option<T> self, Func<T, bool> p) =>
            !self.HasValue || p(self.Value) ? self : None;

        public static T? Where<T>(this T? self, Func<T, bool> p) where T: struct =>
            !self.HasValue || p(self.Value) ? self : default;

        public static bool Any<T>(this IOption<T> self) => self.HasValue;

        public static bool Any<T>(this Option<T> self) => self.HasValue;

        public static bool Any<T>(this T? self) where T: struct => self.HasValue;

        public static bool Any<T>(this IOption<T> self, Func<T, bool> p) => self.HasValue && p(self.Value);

        public static bool Any<T>(this Option<T> self, Func<T, bool> p) => self.HasValue && p(self.Value);

        public static bool All<T>(this IOption<T> self, Func<T, bool> p) => !self.HasValue || p(self.Value);

        public static bool All<T>(this Option<T> self, Func<T, bool> p) => !self.HasValue || p(self.Value);

        public static bool All<T>(this T? self, Func<T, bool> p) where T: struct => !self.HasValue || p(self.Value);

        public static void ForEach<T>(this IOption<T> self, Action<T> action)
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this Option<T> self, Action<T> action)
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this T? self, Action<T> action) where T : struct
        {
            if (self.HasValue) action(self.Value);
        }

        public static void ForEach<T>(this Option<T> self, Action none, Action<T> some)
        {
            if (self.HasValue) some(self.Value);
            else none();
        }

        public static void ForEach<T>(this IOption<T> self, Action none, Action<T> some)
        {
            if (self.HasValue) some(self.Value);
            else none();
        }

        public static void ForEach<T>(this T? self, Action none, Action<T> some) where T : struct
        {
            if (self.HasValue) some(self.Value);
            else none();
        }

        public static T ApplyOpt<T>(this T self, Func<T, Option<T>> f) => f(self).GetValueOrDefault(self);

        public static T ApplyOpt<T>(this T self, Func<T, IOption<T>> f) => f(self).GetValueOrDefault(self);

        public static T ApplyOpt<T>(this T self, Func<T, T?> f) where T : struct => f(self).GetValueOrDefault(self);
    }

    public interface IHasValue
    {
        bool HasValue { get; }
    }

    /// <summary>
    /// Represents the interface of an optional value. The purpose of this interface is primarily to support options
    /// with covariant parameter types. Refer directly to <see cref="Option{T}"/> when covariance is not required so as
    /// to avoid unnecessary boxing.
    /// </summary>
    public interface IOption<out T>: IHasValue, IEnumerable<T>
    {
        T Value { get; }
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
    /// Implements an option data type. This is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// both struct and class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct Option<T>: IOption<T>, IEquatable<Option<T>>, IEquatable<IOption<T>>
    {
        private readonly T _value;

        internal Option(T value)
        {
            HasValue = true;
            _value = value;
        }

        public bool HasValue { get; }
        
        public T Value => HasValue ? _value : throw new InvalidOperationException("None.Get");

        public static explicit operator T(Option<T> value)
        {
            return value.Value;
        }

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
            HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(Value, that.Value);

        public bool Equals(IOption<T> that) => Equals(that.ToOption());

        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            if (obj is Option<T> option) return Equals(option);

            if (obj is IOption<T> ioption) return Equals(ioption);

            if (!HasValue && obj is IHasValue that) return !that.HasValue;

            return false;
        }

        public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public static bool operator ==(Option<T> c1, Option<T> c2) => c1.Equals(c2);

        public static bool operator !=(Option<T> c1, Option<T> c2) => !(c1 == c2);

        public static bool operator ==(Option<T> c1, IOption<T> c2) => c2 != null && c1.Equals(c2);

        public static bool operator !=(Option<T> c1, IOption<T> c2) => !(c1 == c2);

        public static bool operator ==(IOption<T> c1, Option<T> c2) => c1 != null && c1.Equals(c2);

        public static bool operator !=(IOption<T> c1, Option<T> c2) => !(c1 == c2);

        public static implicit operator Option<T>(Option<Nothing> _) => default;

        public override string ToString() => HasValue ? $"Some({Value})" : "None";
    }
}