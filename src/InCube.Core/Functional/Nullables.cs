using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Extension methods for <see cref="Nullable{T}"/> types.
    /// </summary>
    public static class Nullables
    {
        #region Conversion

        public static T? ToNullable<T>(this T self) where T : struct => self;

        #endregion

        #region IOption (Monad) like extensions
        
        public static T GetValueOr<T>(this in T? self, [NotNull] Func<T> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this in T? self, [NotNull] Func<T?> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this in T? self, T? @default) where T : struct =>
            self ?? @default;

        public static TOut Match<TOut, TIn>(this in TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn : struct =>
            self.Select(x => some(x).ToAny()) ?? none();

        public static bool Contains<T>(this in T? self, T elem) where T : struct =>
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in T? self, T elem, IEqualityComparer<T> comparer) where T : struct =>
            self.Select(x => comparer.Equals(x, elem)) ?? false;

        public static T? Where<T>(this in T? self, Func<T, bool> p) where T : struct =>
            self.Any(p) ? self : default(T?);

        public static bool Any<T>(this in T? self) where T : struct => self.HasValue;

        public static bool Any<T>(this in T? self, Func<T, bool> p) where T : struct => self?.Apply(p) ?? false;

        public static bool All<T>(this in T? self, Func<T, bool> p) where T : struct => self?.Apply(p) ?? true;

        public static void ForEach<T>(this in T? self, Action<T> action) where T : struct
        {
            self?.Apply(action);
        }

        public static Task ForEachAsync<T>(this in T? self, Func<T, Task> action) where T : struct => self?.Apply(action);

        public static void ForEach<T>(this in T? self, Action none, Action<T> some) where T : struct
        {
            self.ForEach(some);
            if (!self.HasValue)
            {
                none();
            }
        }

        public static Task ForEachAsync<T>(this in T? self, Func<Task> none, Func<T, Task> some) where T : struct => self.HasValue ? self.ForEachAsync(some) : none();

        public static TOut? Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) 
            where TIn : struct where TOut : struct =>
            self?.Apply(f);

        public static TOut? SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, TOut?> f) 
            where TIn : struct where TOut : struct =>
            self?.Apply(f);

        public static Task<TOut?> SelectManyAsync<TIn, TOut>(this in TIn? self, Func<TIn, Task<TOut?>> f) 
            where TIn : struct where TOut : struct =>
            self?.Apply(f);

        public static Task<Maybe<TOut>> SelectManyAsync<TIn, TOut>(this in TIn? self, Func<TIn, Task<Maybe<TOut>>> f) 
            where TIn : struct where TOut : class =>
            self?.Apply(f);

        #endregion
    }
}