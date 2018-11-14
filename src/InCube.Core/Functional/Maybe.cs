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
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Maybe<>))]
    public readonly struct Maybe<T> : IOption<T>, IInvariantOption<T, Maybe<T>> 
        where T : class
    {
        private readonly T _value;

        private Maybe(T value)
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

        public bool Equals(Maybe<T> that) =>
            !HasValue && !that.HasValue ||
            HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(_value, that._value);

        public override bool Equals(object obj)
        {
            if (obj == null) return !HasValue;
            return obj is Maybe<T> option && Equals(option);
        }

        public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public override string ToString() => HasValue ? $"Some({Value})" : "None";

        public static bool operator ==(Maybe<T> c1, Maybe<T> c2) => c1.Equals(c2);

        public static bool operator !=(Maybe<T> c1, Maybe<T> c2) => !(c1 == c2);

        public static explicit operator T(Maybe<T> nullable) => nullable.Value;

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        public static implicit operator Maybe<T>(Option<T> option) => 
            option.GetValueOrDefault();

        public static implicit operator Option<T>(Maybe<T> nullable) =>
            nullable.ToOption();

        public static implicit operator Maybe<T>(Maybe<Nothing> _) => default(Maybe<T>);

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

        public Maybe<TOut> Select<TOut>(Func<T, TOut> f) where TOut : class =>
            HasValue ? f(_value) : default(Maybe<TOut>);

        public Maybe<TOut> SelectMany<TOut>(Func<T, Maybe<TOut>> f) where TOut : class =>
            HasValue ? f(_value) : default(Maybe<TOut>);

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);

        public Maybe<T> Where(Func<T, bool> p) => !HasValue || p(_value) ? this : default(Maybe<T>);

        public Maybe<TD> Cast<TD>() where TD : class, T =>
            SelectMany(x => x is TD d ? new Maybe<TD>(d) : default(Maybe<TD>));

        public int Count => HasValue ? 1 : 0;

        public Maybe<T> OrElse([NotNull] Func<Maybe<T>> @default) =>
            HasValue ? this : @default();

        public Maybe<T> OrElse(Maybe<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>  
            HasValue && comparer.Equals(_value, elem);
    }

    public static class Maybe
    {
        #region Construction 

        public static readonly Maybe<Nothing> None = default(Maybe<Nothing>);

        public static Maybe<T> Empty<T>() where T : class => default(Maybe<T>);

        public static Maybe<T> Some<T>([NotNull] T value) where T : class => 
            CheckNotNull(value, nameof(value));

        #endregion

        #region Conversion

        public static Maybe<T> ToNullable<T>(this T value) where T : class => value;

        #endregion

        #region Flattening

        public static Maybe<T> Flatten<T>(this in Option<Maybe<T>> self) where T : class =>
            self.HasValue ? self.Value : default(Maybe<T>);

        public static Maybe<T> Flatten<T>(this in Maybe<T>? self) where T : class =>
            self ?? default(Maybe<T>);

        #endregion

        #region Projection

        public static TOut? Select<T, TOut>(this Maybe<T> @this, Func<T, TOut> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value).ToNullable() : default(TOut?);

        public static TOut? SelectMany<T, TOut>(this Maybe<T> @this, Func<T, TOut?> f)
            where T : class where TOut : struct =>
            @this.HasValue ? f(@this.Value) : default(TOut?);

        public static Maybe<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value).ToNullable() : default(Maybe<TOut>);

        public static Maybe<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Maybe<TOut>> f) 
            where TIn : struct where TOut : class =>
            self.HasValue ? f(self.Value) : default(Maybe<TOut>);

        #endregion
    }
}
