using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Extension methods for tasks involving nullable types
    /// </summary>
    [PublicAPI]
    public static class Tasks
    {
        internal static Task<TOut> ApplyAsync<T, TOut>(this T self, Func<T, Task<TOut>> f) => f(self);

        internal static Task ApplyAsync<T>(this T self, Func<T, Task> f) where T : struct => f(self);

        /// <summary>
        /// Performs asynchronous action on input nullable type when it is not null
        /// </summary>
        /// <typeparam name="T">Type of input nullable</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="action">Asynchronous action to perform</param>
        public static Task ForEachAsync<T>(this in T? self, Func<T, Task> action) where T : struct => self is null ? Task.CompletedTask : self.Value.ApplyAsync(action);

        /// <summary>
        /// Performs an asynchronous action, or another asynchronous action, depending on whether the input nullable is null or not
        /// </summary>
        /// <typeparam name="T">Type of input nullable</typeparam>
        /// <param name="self">Input nullable value</param>
        /// <param name="none">Action to perform in case input is null</param>
        /// <param name="some">Action to perform in case input is not null</param>
        public static Task ForEachAsync<T>(this in T? self, Func<Task> none, Func<T, Task> some) where T : struct => self.HasValue ? self.ForEachAsync(some) : none();

        /// <summary>
        /// Asynchronously maps input nullable value to an output <see cref="Maybe{T}"/>, when mapping itself returns <see cref="Maybe{T}"/>
        /// Equivalent to mapping and then flattening
        /// </summary>
        /// <typeparam name="TIn">Type of input nullable value</typeparam>
        /// <typeparam name="TOut">Type of output <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">The input nullable value to map</param>
        /// <param name="f">The asynchronous mapping</param>
        /// <returns>A task of the output <see cref="Maybe{T}"/></returns>
        public static Task<Maybe<TOut>> SelectManyAsync<TIn, TOut>(this in TIn? self, Func<TIn, Task<Maybe<TOut>>> f) where TIn : struct where TOut : class =>
            self is null ? Task.FromResult(Maybe<TOut>.None) : self.Value.ApplyAsync(f);

        /// <summary>
        /// Asynchronously maps input nullable value to an output nullable value, when mapping itself returns nullable value
        /// Equivalent to mapping and then flattening
        /// </summary>
        /// <typeparam name="TIn">Type of input nullable</typeparam>
        /// <typeparam name="TOut">Type of output nullable</typeparam>
        /// <param name="self">Input nullable value to map</param>
        /// <param name="f">The asynchronous mapping</param>
        /// <returns>A task of the output nullable</returns>
        public static Task<TOut?> SelectManyAsync<TIn, TOut>(this in TIn? self, Func<TIn, Task<TOut?>> f) where TIn : struct where TOut : struct =>
            self is null ? Task.FromResult(default(TOut?)) : self.Value.ApplyAsync(f);

        /// <summary>
        /// Asynchronously maps input nullable value to an output <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="TIn">Type of input nullable</typeparam>
        /// <typeparam name="TOut">Type of output <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">Input nullable value to map</param>
        /// <param name="f">Asynchronous mapping</param>
        /// <returns>A task of the output <see cref="Maybe{T}"/></returns>
        public static async Task<Maybe<TOut>> SelectAsync<TIn, TOut>(this TIn? self, Func<TIn, Task<TOut>> f) where TIn : struct where TOut : class =>
            self.HasValue ? await f(self.Value).ConfigureAwait(false) : Maybe<TOut>.None;

        /// <summary>
        /// Asynchronously maps input <see cref="Maybe{T}"/> to an output <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="TIn">Type of input <see cref="Maybe{T}"/></typeparam>
        /// <typeparam name="TOut">Type of output <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">Input <see cref="Maybe{T}"/></param>
        /// <param name="f">The mapping</param>
        /// <returns>A task of <see cref="Maybe{T}"/></returns>
        public static async Task<Maybe<TOut>> SelectAsync<TIn, TOut>(this Maybe<TIn> self, Func<TIn, Task<TOut>> f) where TOut : class where TIn : class =>
            self.HasValue ? await self.Value!.ApplyAsync(f).ConfigureAwait(false) : Maybe<TOut>.None;

        /// <summary>
        /// Asynchronously maps input <see cref="Maybe{T}"/> to an output <see cref="Maybe{T}"/> when mapping itself returns a <see cref="Maybe{T}"/>
        /// Equivalent to mapping and then flattening
        /// </summary>
        /// <typeparam name="TIn">Type of input <see cref="Maybe{T}"/></typeparam>
        /// <typeparam name="TOut">Type of output <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">The input <see cref="Maybe{T}"/></param>
        /// <param name="f">The mapping</param>
        /// <returns>A task of <see cref="Maybe{T}"/></returns>
        public static Task<Maybe<TOut>> SelectManyAsync<TIn, TOut>(this in Maybe<TIn> self, Func<TIn, Task<Maybe<TOut>>> f) where TOut : class where TIn : class =>
            self.HasValue ? self.Value!.ApplyAsync(f) : Task.FromResult(Maybe<TOut>.None);

        /// <summary>
        /// Asynchronously tries to get alternative value to input <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">Input <see cref="Maybe{T}"/></param>
        /// <param name="default">Generator for alternative <see cref="Maybe{T}"/></param>
        /// <returns>A task of <see cref="Maybe{T}"/></returns>
        public static Task<Maybe<T>> OrElseAsync<T>(this in Maybe<T> self, Func<Task<Maybe<T>>> @default) where T : class => self.HasValue ? Task.FromResult(self) : @default();

        /// <summary>
        /// Asynchronously performs an action, or another action, depending on whether or not input <see cref="Maybe{T}"/> has a value
        /// </summary>
        /// <typeparam name="T">Type of input <see cref="Maybe{T}"/></typeparam>
        /// <param name="self">The input <see cref="Maybe{T}"/></param>
        /// <param name="none">Asynchronous action to perform when input <see cref="Maybe{T}"/> doesn't have a value</param>
        /// <param name="some">Asynchronous action to perform when input <see cref="Maybe{T}"/> has a value</param>
        public static Task ForEachAsync<T>(this in Maybe<T> self, Func<Task> none, Func<T, Task> some) where T : class => !self.HasValue ? none() : self.ForEachAsync(some);

        /// <summary>
        /// Asynchronously performs action on value of input <see cref="Option{T}"/>, if any
        /// </summary>
        /// <param name="self">The input <see cref="Option{T}"/></param>
        /// <param name="action">The action to perform</param>
        /// <typeparam name="T">The type of the input <see cref="Option{T}"/></typeparam>
        public static Task ForEachAsync<T>(this in Option<T> self, Func<T, Task> action) => self.AsAny.ForEachAsync(x => action(x));

        /// <summary>
        /// Asynchronously performs one of two actions, depending on whether input <see cref="Option{T}"/> has a value or not
        /// </summary>
        /// <param name="self">The input <see cref="Option{T}"/></param>
        /// <param name="none">The action to perform in case input is None</param>
        /// <param name="some">The action to perform in case input is Some</param>
        /// <typeparam name="T">Type of the input <see cref="Option{T}"/></typeparam>
        public static Task ForEachAsync<T>(this in Option<T> self, Func<Task> none, Func<T, Task> some) => self.AsAny.ForEachAsync(none, x => some(x));

        /// <summary>
        /// Asynchronously performs one of two mappings of the input <see cref="Either{TL,TR}"/> depending on whether or not it is Right or Left
        /// </summary>
        /// <param name="self">Input <see cref="Either{TL,TR}"/></param>
        /// <param name="left">Action to perform in case <see cref="Either{TL,TR}"/> is Left</param>
        /// <param name="right">Action to perform in case <see cref="Either{TL,TR}"/> is Right</param>
        /// <typeparam name="TL">Left type of <see cref="Either{TL,TR}"/></typeparam>
        /// <typeparam name="TR">Right type of <see cref="Either{TL,TR}"/></typeparam>
        /// <typeparam name="TOut">Output type of both mappings</typeparam>
        /// <returns>A task of the output type of the given mappings</returns>
        public static Task<TOut> MatchAsync<TL, TR, TOut>(in this Either<TL, TR> self, Func<TL, Task<TOut>> left, Func<TR, Task<TOut>> right) =>
            self.IsLeft ? left(self.Left) : right(self.Right);

        /// <summary>
        /// Asynchronously performs one of two actions depending on whether the input <see cref="Either{TL,TR}"/> is Right or Left
        /// </summary>
        /// <param name="self">The input <see cref="Either{TL,TR}"/></param>
        /// <param name="left">The action to perform in case input is Left</param>
        /// <param name="right">The action to perform in case input is Right</param>
        /// <typeparam name="TL">Left type of input</typeparam>
        /// <typeparam name="TR">Right type of input</typeparam>
        public static Task ForEachAsync<TL, TR>(this in Either<TL, TR> self, Func<TL, Task> left, Func<TR, Task> right) =>
            self.IsLeft ? left(self.Left) : right(self.Right);

        public static Task<Either<TL, TOut>> SelectAsync<TL, TR, TOut>(this in Either<TL, TR> self, Func<TR, Task<TOut>> f) =>
            self.Match<Task<Either<TL, TOut>>>(left => Task.FromResult(new Either<TL, TOut>(left)), async right => await f(right));

        public static Task ForEachAsync<T>(this in Maybe<T> self, Func<T, Task> action) where T : class => self.Value?.Apply(action) ?? Task.CompletedTask;

        public static async Task<TOut> MatchAsync<TIn, TOut>(this Maybe<TIn> self, Func<Task<TOut>> none, Func<TIn, Task<TOut>> some) where TIn : class =>
            self.Value == null ? (await none().ConfigureAwait(false)).ToAny() : (await self.Value.ApplyAsync(some).ConfigureAwait(false)).ToAny();
        
        public static Task<TOut?> ApplyNullableAsync<TIn, TOut>(this TIn? input, Func<TIn, Task<TOut?>> map) where TIn : struct where TOut : class =>
            input is null ? Task.FromResult(default(TOut?)) : map(input.Value);
    }
}