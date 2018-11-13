using System;
using System.Collections;
using System.Collections.Generic;
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
    [Serializable] [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Option<>))]
    public readonly struct Option<T>: IOption<T>, IInvariantOption<T, Option<T>>
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
            if (HasValue) yield return _value;
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

        public static implicit operator Option<T>(T value) => 
            typeof(T).IsValueType || value != null ? new Option<T>(value) : default;

        public static implicit operator Option<T>(Option<Nothing> _) => default;

        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => HasValue ? some(_value) : none();

        public T GetValueOrDefault() => GetValueOrDefault(default(T));

        public T GetValueOrDefault([NotNull] Func<T> @default) =>
            HasValue ? _value : CheckNotNull(@default, nameof(@default)).Invoke();

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

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => this.Select(f);

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => SelectMany(x => f(x).ToOption());

        public Option<TOut> Select<TOut>(Func<T, TOut> f) => HasValue ? Option.Some(f(_value)) : default;

        public Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> f) => HasValue ? f(_value) : default;

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);
        
        public Option<T> Where(Func<T, bool> p) => !HasValue || p(_value) ? this : default;

        public Option<TD> Cast<TD>() where TD : T => SelectMany(x => x is TD d ? Option.Some(d) : default);

        public int Count => HasValue ? 1 : 0;

        public Option<T> OrElse([NotNull] Func<Option<T>> @default) =>
            HasValue ? this : @default();

        public Option<T> OrElse(Option<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>
            HasValue && comparer.Equals(_value, elem);
    }

    /// <summary>
    /// Defines utility operations on instances of <see cref="Option{T}"/>.
    /// </summary>
    public static class Option
    {
        #region Construction 
        
        public static readonly Option<Nothing> None = default;
        
        public static Option<T> Empty<T>() => default;

        public static Option<T> Some<T>([NotNull] T value) => new Option<T>(value);

        public static Option<T?> Some<T>([CanBeNull] T? value) where T : struct => new Option<T?>(value);

        #endregion

        #region Conversion

        public static Option<T> ToOption<T>(this T value) where T : class =>
            value != null ? Some(value) : default;

        public static Option<T> ToOption<T>(this in T? value) where T : struct =>
            value.HasValue ? Some(value.Value) : default;

        public static Option<T> ToOption<T>(this NullableRef<T> value) where T : class =>
            value.HasValue ? Some(value.Value) : default;

        public static Option<T> ToOption<T>(this IOption<T> value) =>
            value is Option<T> opt ? opt : value.HasValue ? Some(value.Value) : default;

        [Obsolete("remove unnecessary call to " + nameof(ToOption))]
        public static Option<T> ToOption<T>(this in Option<T> value) => value;

        public static T? ToNullable<T>(this in Option<T> self) where T : struct =>
            self.HasValue ? new T?(self.Value) : null;

        #endregion

        #region Flattening
        
        public static Option<T> Flatten<T>(this in Option<Option<T>> self) =>
            self.HasValue ? self.Value : default;

        public static T? Flatten<T>(this in Option<T?> self) where T : struct =>
            self.HasValue ? self.Value : null;

        public static Option<T> Flatten<T>(this in Option<T>? self) =>
            self ?? default;

        #endregion
    }
}