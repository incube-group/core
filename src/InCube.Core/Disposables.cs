using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core
{
    /// <summary>
    /// Helper class for dealing with many <see cref="IDisposable" />s.
    /// </summary>
    [PublicAPI]
    public class Disposables : IDisposable
    {
        private readonly Stack<IDisposable> managed = new();

        private readonly Stack<Disposer> unmanaged = new();

        private bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="Disposables"/> class.
        /// </summary>
        ~Disposables()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Delegate type of function used for disposing of resources.
        /// </summary>
        public delegate void Disposer();

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds a delegate for disposing of some resources.
        /// </summary>
        /// <param name="disposer">The <see cref="Disposer"/> to add.</param>
        public void Add(Disposer disposer)
        {
            Preconditions.CheckArgument(!this.disposed, "disposed");
            Preconditions.CheckNotNull(disposer);
            this.unmanaged.Push(disposer);
        }

        /// <summary>
        /// Adds some disposable resources to the <see cref="Disposables"/>.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable"/> to add.</param>
        /// <typeparam name="T">The type of the <paramref name="disposable"/>.</typeparam>
        /// <returns>The <paramref name="disposable"/>.</returns>
        public T Add<T>(T disposable)
            where T : IDisposable
        {
            Preconditions.CheckArgument(!this.disposed, "disposed");
            Preconditions.CheckNotNull(disposable);
            this.managed.Push(disposable);
            return disposable;
        }

        /// <summary>
        /// Adds some disposable resources to the <see cref="Disposables"/> via a creator function.
        /// </summary>
        /// <param name="creator">The function to use to create the <see cref="IDisposable"/>.</param>
        /// <typeparam name="T">The type of the <see cref="IDisposable"/>.</typeparam>
        /// <returns>The created <see cref="IDisposable"/>.</returns>
        public T Create<T>(Func<T> creator)
            where T : IDisposable =>
            this.Add(creator());

        /// <summary>
        /// Disposes of the <see cref="Disposables"/>.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose of the managed resources.</param>
        /// <exception cref="AggregateException">In case any of the disposing throws.</exception>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            var exceptions = this.ReleaseUnmanagedResources();
            if (disposing)
            {
                while (this.managed.Count > 0)
                {
                    var disposable = this.managed.Pop();
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (exceptions == null)
                            exceptions = new List<Exception>();
                        exceptions.Add(ex);
                    }
                }
            }

            this.disposed = true;

            if (exceptions != null)
            {
                if (exceptions.Count == 1)
                    throw exceptions[0];

                throw new AggregateException(exceptions);
            }
        }

        private List<Exception>? ReleaseUnmanagedResources()
        {
            List<Exception>? exceptions = null;
            while (this.unmanaged.Count > 0)
            {
                var disposer = this.unmanaged.Pop();
                try
                {
                    disposer();
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                        exceptions = new List<Exception>();
                    exceptions.Add(ex);
                }
            }

            return exceptions;
        }
    }
}