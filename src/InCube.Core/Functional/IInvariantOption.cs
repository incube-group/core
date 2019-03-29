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
    public interface IInvariantOption<T, TOpt> : IOption<T>, IEquatable<TOpt> where TOpt : IInvariantOption<T, TOpt>
    {
        TOpt OrElse([NotNull] Func<TOpt> @default);
        
        TOpt OrElse(TOpt @default);

        T GetValueOr([NotNull] Func<T> @default);
        
        T GetValueOrDefault(T @default);

        bool Contains(T elem);

        bool Contains(T elem, IEqualityComparer<T> comparer);

        new TOpt Where(Func<T, bool> p);
    }
}