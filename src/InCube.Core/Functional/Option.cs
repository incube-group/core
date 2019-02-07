using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InCube.Core.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Implements an option data type. this in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// both struct and class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Option<>))]
    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    public readonly struct Option<T> : IOption<T>, IInvariantOption<T, Option<T>>
    {
        internal Option(T value)
        {
            AsAny = value;
        }

        internal Option(in Any<T>? any)
        {
            AsAny = any;
        }

        public Any<T>? AsAny { get; }

        /// <see cref="Nullable{T}.HasValue"/>
        public bool HasValue => AsAny.HasValue;

        /// <see cref="Nullable{T}.Value"/>
        /// <exception cref="InvalidOperationException">If the maybe is undefined.</exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T Value => AsAny.Value;

        public IEnumerator<T> GetEnumerator()
        {
            if (AsAny.HasValue)
            {
                yield return AsAny.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Option<T> that) => Nullable.Equals(this.AsAny, that.AsAny);
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return !HasValue;
            return obj is Option<T> option && Equals(option);
        }

        public override int GetHashCode() => AsAny.GetHashCode();

        public override string ToString()
        {
            var typeName = typeof(T).Name;
            return Select(x => $"Some<${typeName}>({x})").GetValueOrDefault(() => $"None<{typeName}>");
        }

        public static bool operator ==(Option<T> c1, Option<T> c2) => c1.Equals(c2);

        public static bool operator !=(Option<T> c1, Option<T> c2) => !(c1 == c2);

        public static explicit operator T(Option<T> value) => value.Value;

        public static implicit operator Option<T>(T value) => 
            typeof(T).IsValueType || !ReferenceEquals(value, null) ? new Option<T>(value) : default(Option<T>);

        [SuppressMessage("Usage",
            "CA1801: Review unused parameters",
            Justification = "Need parameter for complying with implicit conversion operator.")]
        public static implicit operator Option<T>(Option<Nothing> x) => default(Option<T>);

        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) =>
            AsAny.Select(x => some(x).ToAny()) ?? none();

        public T GetValueOrDefault() => AsAny.GetValueOrDefault();

        public T GetValueOrDefault(T @default) => AsAny ?? @default;

        public T GetValueOrDefault(Func<T> @default) => 
            AsAny ?? CheckNotNull(@default, nameof(@default)).Invoke();

        public bool Any() => HasValue;

        public bool Any(Func<T, bool> p) => AsAny.Any(x => p(x));

        public bool All(Func<T, bool> p) => AsAny.All(x => p(x));

        public void ForEach(Action<T> action)
        {
            AsAny.ForEach(x => action(x));
        }

        public void ForEach(Action none, Action<T> some)
        {
            AsAny.ForEach(none, x => some(x));
        }

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => 
            Select(f);

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => 
            SelectMany(x => f(x).ToOption());

        public Option<TOut> Select<TOut>(Func<T, TOut> f) =>
            new Option<TOut>(AsAny.Select(x => f(x).ToAny()));

        public Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> f) =>
            new Option<TOut>(AsAny.SelectMany(x => f(x).AsAny));

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);
        
        public Option<T> Where(Func<T, bool> p) => Any(p) ? this : default(Option<T>);

        public Option<TD> Cast<TD>() where TD : T => 
            SelectMany(x => x is TD d ? Option.Some(d) : default(Option<TD>));

        public int Count => HasValue ? 1 : 0;

        public Option<T> OrElse(Func<Option<T>> @default) =>
            HasValue ? this : @default();

        public Option<T> OrElse(Option<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => 
            Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>
            AsAny.Select(x => comparer.Equals(x.Value, elem)) ?? false;

        public static readonly Option<T> Empty = default(Option<T>);
    }

    /// <summary>
    /// Defines utility operations on instances of <see cref="Option{T}"/>.
    /// </summary>
    public static class Option
    {
        #region Construction 
        
        public static readonly Option<Nothing> None = default(Option<Nothing>);

        public static Option<T> Some<T>([NotNull] T value) => new Option<T>(value);

        public static Option<T?> Some<T>([CanBeNull] T? value) where T : struct => new Option<T?>(value);

        #endregion

        #region Conversion

        public static Option<T> ToOption<T>(this T value) where T : class =>
            value?.Apply(Some) ?? default(Option<T>);

        public static Option<T> ToOption<T>(this in T? value) where T : struct =>
            value?.Apply(Some) ?? default(Option<T>);

        public static Option<T> ToOption<T>(this in Any<T>? any) => new Option<T>(any);

        public static Option<T> ToOption<T>(this Maybe<T> maybe) where T : class => maybe;

        public static Option<T> ToOption<T>(this IOption<T> value) =>
            value is Option<T> opt ? opt : value.HasValue ? Some(value.Value) : default(Option<T>);

        [Obsolete("remove unnecessary call to " + nameof(ToOption))]
        public static Option<T> ToOption<T>(this in Option<T> value) => value;

        public static T? ToNullable<T>(this in Option<T> self) where T : struct =>
            self.AsAny?.Apply(x => x.Value);

        #endregion

        #region Flattening
        
        public static Option<T> Flatten<T>(this in Option<Option<T>> self) =>
            self.AsAny?.Apply(x => x.Value) ?? default(Option<T>);

        public static T? Flatten<T>(this in Option<T?> self) where T : struct =>
            self.AsAny?.Apply(x => x.Value);

        public static Option<T> Flatten<T>(this in Option<T>? self) =>
            self ?? default(Option<T>);

        #endregion
    }
}