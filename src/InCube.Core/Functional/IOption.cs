using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static InCube.Core.Preconditions;

namespace InCube.Core.Functional
{
    /// <summary>
    /// A covariant version of <see cref="Option{T}"/> and <see cref="Nullable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOption<out T> : IReadOnlyCollection<T>
    {
        bool HasValue { get; }

        T Value { get; }

        TOut Match<TOut>(Func<TOut> none, Func<T, TOut> some);

        [Pure]
        T GetValueOrDefault();

        IOption<TOut> Select<TOut>(Func<T, TOut> f);

        IOption<TOut> SelectMany<TOut>(Func<T, IOption<TOut>> f);

        bool Any();

        bool Any(Func<T, bool> p);

        bool All(Func<T, bool> p);

        void ForEach(Action<T> action);

        void ForEach(Action none, Action<T> some);

        IOption<T> Where(Func<T, bool> p);
    }

    // ReSharper disable once InconsistentNaming
    public static class IOption
    {
        public static T GetValueOrDefault<T>(this IOption<T> self, T @default) =>
            self.HasValue ? self.Value : @default;

        public static T GetValueOrDefault<T>(this IOption<T> self, [NotNull] Func<T> @default) =>
            self.HasValue ? self.Value : CheckNotNull(@default, nameof(@default)).Invoke();
    }
}