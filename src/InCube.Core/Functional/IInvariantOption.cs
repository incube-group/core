using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Invariant method extensions of <see cref="IOption{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TOpt"></typeparam>
    public interface IInvariantOption<T, TOpt> : IEquatable<TOpt> where TOpt : IOption<T>
    {
        TOpt OrElse([NotNull] Func<TOpt> @default);
        TOpt OrElse(TOpt @default);

        T GetValueOrDefault([NotNull] Func<T> @default);
        T GetValueOrDefault(T @default);

        bool Contains(T elem);
        bool Contains(T elem, IEqualityComparer<T> comparer);

        TOpt Where(Func<T, bool> p);

        Any<T>? AsAny { get; }
    }
}