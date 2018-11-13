using System;
using System.Collections.Generic;

namespace InCube.Core.Functional
{
    public static class Nullables
    {
        public static T? ToNullable<T>(this T self) where T : struct => self;

        public static T GetValueOrDefault<T>(this in T? self, Func<T> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this in T? self, Func<T?> @default) where T : struct =>
            self ?? @default();

        public static T? OrElse<T>(this in T? self, T? @default) where T : struct =>
            self ?? @default;

        public static TOut Match<TOut, TIn>(this in TIn? self, Func<TOut> none, Func<TIn, TOut> some) where TIn : struct =>
            self.HasValue ? some(self.Value) : none();

        public static bool Contains<T>(this in T? self, T elem) where T : struct =>
            self.Contains(elem, EqualityComparer<T>.Default);

        public static bool Contains<T>(this in T? self, T elem, IEqualityComparer<T> comparer) where T : struct =>
            self.HasValue && comparer.Equals(self.Value, elem);

        public static T? Where<T>(this in T? self, Func<T, bool> p) where T : struct =>
            !self.HasValue || p(self.Value) ? self : default;

        public static bool Any<T>(this in T? self) where T : struct => self.HasValue;

        public static bool Any<T>(this in T? self, Func<T, bool> p) where T : struct => self.HasValue && p(self.Value);

        public static bool All<T>(this in T? self, Func<T, bool> p) where T : struct => !self.HasValue || p(self.Value);

        public static void ForEach<T>(this in T? self, Action<T> action) where T : struct
        {
            if (self.HasValue)
            {
                action(self.Value);
            }
        }

        public static void ForEach<T>(this in T? self, Action none, Action<T> some) where T : struct
        {
            if (self.HasValue)
            {
                some(self.Value);
            }
            else
            {
                none();
            }
        }

        public static TOut? Select<TIn, TOut>(this in TIn? self, Func<TIn, TOut> f) where TIn : struct where TOut : struct =>
            self.HasValue ? f(self.Value) : default;

        public static TOut? SelectMany<TIn, TOut>(this in TIn? self, Func<TIn, TOut?> f) where TIn : struct where TOut : struct =>
            self.HasValue ? f(self.Value) : default;
    }
}