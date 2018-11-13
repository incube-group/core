using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using InCube.Core.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Implements an option data type. this in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(NullableRef<>))]
    public readonly struct NullableRef<T> : IOption<T>, IInvariantOption<T, NullableRef<T>> 
        where T : class
    {
        private readonly T _value;

        private NullableRef(T value)
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

        public bool Equals(NullableRef<T> that) =>
            !HasValue && !that.HasValue ||
            HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(_value, that._value);

        public override bool Equals(object obj)
        {
            if (obj == null) return !HasValue;
            return obj is NullableRef<T> option && Equals(option);
        }

        public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public override string ToString() => HasValue ? $"Some({Value})" : "None";

        public static bool operator ==(NullableRef<T> c1, NullableRef<T> c2) => c1.Equals(c2);

        public static bool operator !=(NullableRef<T> c1, NullableRef<T> c2) => !(c1 == c2);

        public static explicit operator T(NullableRef<T> nullable) => nullable.Value;

        public static implicit operator NullableRef<T>(T value) => new NullableRef<T>(value);

        public static implicit operator NullableRef<T>(Option<T> option) => 
            option.GetValueOrDefault();

        public static implicit operator Option<T>(NullableRef<T> nullable) =>
            nullable.ToOption();

        public static implicit operator NullableRef<T>(NullableRef<Nothing> _) => default;

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
            this.HasValue ? Option.Some(f(_value)) : default;

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) => 
            HasValue ? f(_value) : default;

        public NullableRef<TOut> Select<TOut>(Func<T, TOut> f) where TOut : class =>
            HasValue ? f(_value) : default;

        public NullableRef<TOut> SelectMany<TOut>(Func<T, NullableRef<TOut>> f) where TOut : class =>
            HasValue ? f(_value) : default;

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);

        public NullableRef<T> Where(Func<T, bool> p) => !HasValue || p(_value) ? this : default;

        public NullableRef<TD> Cast<TD>() where TD : class, T =>
            SelectMany(x => x is TD d ? new NullableRef<TD>(d) : default);

        public int Count => HasValue ? 1 : 0;

        public NullableRef<T> OrElse([NotNull] Func<NullableRef<T>> @default) =>
            HasValue ? this : @default();

        public NullableRef<T> OrElse(NullableRef<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>  
            HasValue && comparer.Equals(_value, elem);
    }

    public static class NullableRef
    {
        #region Construction 

        public static readonly NullableRef<Nothing> None = default;

        public static NullableRef<T> Empty<T>() where T : class => default;

        public static NullableRef<T> Some<T>([NotNull] T value) where T : class => 
            CheckNotNull(value, nameof(value));

        #endregion

        #region Conversion

        public static NullableRef<T> ToNullable<T>(this T value) where T : class => value;

        #endregion

        #region Flattening

        public static NullableRef<T> Flatten<T>(this in Option<NullableRef<T>> self) where T : class =>
            self.HasValue ? self.Value : None;

        public static NullableRef<T> Flatten<T>(this in NullableRef<T>? self) where T : class =>
            self ?? None;

        #endregion

        #region Projection

        public static TOut? Select<T, TOut>(this NullableRef<T> @this, Func<T, TOut> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value) : default;

        public static TOut? SelectMany<T, TOut>(this NullableRef<T> @this, Func<T, TOut?> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value) : default;

        public static NullableRef<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value).ToNullable() : default;

        public static NullableRef<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, NullableRef<TOut>> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value) : None;

        #endregion
    }
}
