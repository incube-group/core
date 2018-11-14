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
    /// Implements an option data type. This in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(CanBeNull<>))]
    public readonly struct CanBeNull<T> : IOption<T>, IInvariantOption<T, CanBeNull<T>> 
        where T : class
    {
        private readonly T _value;

        private CanBeNull(T value)
        {
            _value = value;
        }

        public bool HasValue => this._value != null;

        public T Value => HasValue ? _value : throw new InvalidOperationException("None.Get");

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue) yield return _value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(CanBeNull<T> that) =>
            !HasValue && !that.HasValue ||
            HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(_value, that._value);

        public override bool Equals(object obj)
        {
            if (obj == null) return !HasValue;
            return obj is CanBeNull<T> option && Equals(option);
        }

        public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public override string ToString() => HasValue ? $"Some({Value})" : "None";

        public static bool operator ==(CanBeNull<T> c1, CanBeNull<T> c2) => c1.Equals(c2);

        public static bool operator !=(CanBeNull<T> c1, CanBeNull<T> c2) => !(c1 == c2);

        public static explicit operator T(CanBeNull<T> nullable) => nullable.Value;

        public static implicit operator CanBeNull<T>(T value) => new CanBeNull<T>(value);

        public static implicit operator CanBeNull<T>(Option<T> option) => 
            option.GetValueOrDefault();

        public static implicit operator Option<T>(CanBeNull<T> nullable) =>
            nullable.ToOption();

        public static implicit operator CanBeNull<T>(CanBeNull<Nothing> _) => default(CanBeNull<T>);

        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) => 
            HasValue ? some(_value) : none();

        public T GetValueOrDefault() => GetValueOrDefault(default(T));

        public T GetValueOrDefault(T @default) => HasValue ? _value : @default;

        public T GetValueOrDefault([NotNull] Func<T> @default) =>
            HasValue ? _value : CheckNotNull(@default, nameof(@default)).Invoke();

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

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => 
            this.HasValue ? Option.Some(f(_value)) : Option.None;

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => 
            HasValue ? f(_value).ToOption() : Option.None;

        public CanBeNull<TOut> Select<TOut>(Func<T, TOut> f) where TOut : class =>
            HasValue ? f(_value) : default(CanBeNull<TOut>);

        public CanBeNull<TOut> SelectMany<TOut>(Func<T, CanBeNull<TOut>> f) where TOut : class =>
            HasValue ? f(_value) : default(CanBeNull<TOut>);

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);

        public CanBeNull<T> Where(Func<T, bool> p) => !HasValue || p(_value) ? this : default(CanBeNull<T>);

        public CanBeNull<TD> Cast<TD>() where TD : class, T =>
            SelectMany(x => x is TD d ? new CanBeNull<TD>(d) : default(CanBeNull<TD>));

        public int Count => HasValue ? 1 : 0;

        public CanBeNull<T> OrElse([NotNull] Func<CanBeNull<T>> @default) =>
            HasValue ? this : @default();

        public CanBeNull<T> OrElse(CanBeNull<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>  
            HasValue && comparer.Equals(_value, elem);
    }

    public static class CanBeNull
    {
        #region Construction 

        public static readonly CanBeNull<Nothing> None = default(CanBeNull<Nothing>);

        public static CanBeNull<T> Empty<T>() where T : class => default(CanBeNull<T>);

        public static CanBeNull<T> Some<T>([NotNull] T value) where T : class => 
            CheckNotNull(value, nameof(value));

        #endregion

        #region Conversion

        public static CanBeNull<T> ToNullable<T>(this T value) where T : class => value;

        #endregion

        #region Flattening

        public static CanBeNull<T> Flatten<T>(this in Option<CanBeNull<T>> self) where T : class =>
            self.HasValue ? self.Value : default(CanBeNull<T>);

        public static CanBeNull<T> Flatten<T>(this in CanBeNull<T>? self) where T : class =>
            self ?? default(CanBeNull<T>);

        #endregion

        #region Projection

        public static TOut? Select<T, TOut>(this CanBeNull<T> @this, Func<T, TOut> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value).ToNullable() : default(TOut?);

        public static TOut? SelectMany<T, TOut>(this CanBeNull<T> @this, Func<T, TOut?> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value) : default(TOut?);

        public static CanBeNull<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value).ToNullable() : default(CanBeNull<TOut>);

        public static CanBeNull<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, CanBeNull<TOut>> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value) : default(CanBeNull<TOut>);

        #endregion
    }
}
