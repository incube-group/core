using System;
using JetBrains.Annotations;

namespace InCube.Core.Functional
{
    /// <summary>
    /// Wrapper around a result that can be either some type of an exception
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public interface ITry<out T> : IOption<T>
    {
        Exception Exception { get; }

        IOption<T> AsOption { get; }

        IEither<Exception, T> AsEither { get; }

        Try<Exception> Failed();

        TOut Match<TOut>(Func<Exception, TOut> failure, Func<T, TOut> success);

        new ITry<TOut> Select<TOut>(Func<T, TOut> f);

        ITry<TOut> SelectMany<TOut>(Func<T, ITry<TOut>> success);

        ITry<TOut> SelectMany<TOut>(Func<Exception, ITry<TOut>> failure, Func<T, ITry<TOut>> success);

        new ITry<T> Where(Func<T, bool> p);

        void ForEach(Action<Exception> failure, Action<T> success);
    }
}