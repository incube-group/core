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
    /// Implements an option data type. This in is essentially an extension of <see cref="Nullable{T}"/> that works for
    /// class types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [JsonConverter(typeof(GenericOptionJsonConverter), typeof(Maybe<>))]
    [SuppressMessage("Managed Binary Analysis",
        "CA2225: Operator overloads have named alternates",
        Justification = "Methods are in static companion class.")]
    public readonly struct Maybe<T> : IInvariantOption<T, Maybe<T>>
        where T : class
    {
        private readonly T value;

        private Maybe(T value)
        {
            this.value = value;
        }

        public bool HasValue => !ReferenceEquals(this.value, null);

        public T Value => this.value ?? throw new InvalidOperationException("None.Get");

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return this.value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Maybe<T> that) =>
            !this.HasValue && !that.HasValue ||
            this.HasValue && that.HasValue && EqualityComparer<T>.Default.Equals(this.value, that.value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return !HasValue;
            return obj is Maybe<T> option && Equals(option);
        }

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(this.value);

        public override string ToString()
        {
            var typeName = typeof(T).Name;
            return Select(x => $"Some<${typeName}>({x})").GetValueOrDefault(() => $"None<{typeName}>");
        }

        public static bool operator ==(Maybe<T> c1, Maybe<T> c2) => c1.Equals(c2);

        public static bool operator !=(Maybe<T> c1, Maybe<T> c2) => !c1.Equals(c2);

        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        public static implicit operator Maybe<T>(Option<T> option) => 
            option.GetValueOrDefault();

        public static implicit operator Option<T>(Maybe<T> maybe) =>
            maybe.Select(x => x.ToAny()).ToOption();

        [SuppressMessage("Usage",
            "CA1801: Review unused parameters",
            Justification = "Need parameter for complying with implicit conversion operator.")]
        public static implicit operator Maybe<T>(Maybe<Nothing> x) => default(Maybe<T>);

        public TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some) =>
            this.value?.Apply(x => some(x).ToAny()) ?? none();

        public T GetValueOrDefault() => 
            this.value;

        public T GetValueOrDefault(T @default) => 
            this.value ?? @default;

        public T GetValueOrDefault(Func<T> @default) =>
            this.value ?? CheckNotNull(@default, nameof(@default)).Invoke();

        public bool Any() => HasValue;

        public bool Any(Func<T, bool> p) => this.value?.Apply(p) ?? false;

        public bool All(Func<T, bool> p) => this.value?.Apply(p) ?? true;

        public void ForEach(Action<T> action)
        {
            this.value?.Apply(action);
        }

        public void ForEach(Action none, Action<T> some)
        {
            ForEach(some);
            if (!HasValue)
            {
                none();
            }
        }

        IOption<TOut> IOption<T>.Select<TOut>(Func<T, TOut> f) => 
            (this.value?.Apply(x => f(x).ToAny())).ToOption();

        IOption<TOut> IOption<T>.SelectMany<TOut>(Func<T, IOption<TOut>> f) =>
            this.value?.Apply(f) ?? default(Option<TOut>);

        public Maybe<TOut> Select<TOut>(Func<T, TOut> f) where TOut : class =>
            this.value?.Apply(f); // implicit conversion

        public Maybe<TOut> SelectMany<TOut>(Func<T, Maybe<TOut>> f) where TOut : class =>
            this.value?.Apply(f) ?? default(Maybe<TOut>);

        IOption<T> IOption<T>.Where(Func<T, bool> p) => Where(p);

        public Maybe<T> Where(Func<T, bool> p) => !HasValue || p(this.value) ? this : default(Maybe<T>);

        public Maybe<TD> Cast<TD>() where TD : class, T =>
            SelectMany(x => x is TD d ? new Maybe<TD>(d) : default(Maybe<TD>));

        public int Count => HasValue ? 1 : 0;

        public Maybe<T> OrElse(Func<Maybe<T>> @default) =>
            HasValue ? this : @default();

        public Maybe<T> OrElse(Maybe<T> @default) =>
            HasValue ? this : @default;

        public bool Contains(T elem) => Contains(elem, EqualityComparer<T>.Default);

        public bool Contains(T elem, IEqualityComparer<T> comparer) =>  
            this.Select(x => comparer.Equals(x, elem)) ?? false;

        public static readonly Maybe<T> Empty = default(Maybe<T>);

        /// <see cref="Nullable{T}.Value"/>
        /// <exception cref="InvalidOperationException">If this <see cref="Maybe"/> is undefined or the <paramref name="index"/> != 0.</exception>
        // ReSharper disable once PossibleInvalidOperationException
        public T this[int index] => index == 0 ? Value : throw new InvalidOperationException();
    }

    public static class Maybe
    {
        #region Construction 

        public static readonly Maybe<Nothing> None = Maybe<Nothing>.Empty;

        public static Maybe<T> Some<T>([NotNull] T value) where T : class => 
            CheckNotNull(value, nameof(value));

        #endregion

        #region Conversion

        public static Maybe<T> ToMaybe<T>(this T value) where T : class => value;

        #endregion

        #region Flattening

        public static Maybe<T> Flatten<T>(this in Option<Maybe<T>> self) where T : class =>
            self.AsAny?.Apply(x => x.Value) ?? default(Maybe<T>);

        public static Maybe<T> Flatten<T>(this in Maybe<T>? self) where T : class =>
            self ?? default(Maybe<T>);

        #endregion

        #region Projection

        public static TOut? Select<T, TOut>(this Maybe<T> @this, Func<T, TOut> f)
            where T : class where TOut : struct =>
            @this.GetValueOrDefault()?.Apply(f);

        public static TOut? SelectMany<T, TOut>(this Maybe<T> @this, Func<T, TOut?> f)
            where T : class where TOut : struct =>
            @this.GetValueOrDefault()?.Apply(f);

        public static Maybe<TOut> Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f)
            where TIn : struct where TOut : class =>
            self?.Apply(f); // implicit conversion

        public static Maybe<TOut> SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, Maybe<TOut>> f)
            where TIn : struct where TOut : class =>
            self?.Apply(f) ?? default(Maybe<TOut>);

        #endregion
    }
}
