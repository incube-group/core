using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace InCube.Core
{
    /// <summary>
    /// Helper class for dealing with many <see cref="IDisposable"/>s.
    /// </summary>
    [PublicAPI]
    public class Disposables : IDisposable
    {
        public delegate void Disposer();

        private readonly Stack<Disposer> unmanaged = new Stack<Disposer>();
        private readonly Stack<IDisposable> managed = new Stack<IDisposable>();
        private bool disposed;

        public void Add(Disposer disposer)
        {
            Preconditions.CheckArgument(!this.disposed, "disposed");
            Preconditions.CheckNotNull(disposer);
            this.unmanaged.Push(disposer);
        }

        public T Add<T>(T disposable) where T : IDisposable
        {
            Preconditions.CheckArgument(!this.disposed, "disposed");
            Preconditions.CheckNotNull(disposable);
            this.managed.Push(disposable);
            return disposable;
        }

        public T Create<T>(Func<T> creator) where T : IDisposable
        {
            return this.Add(creator());
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
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(ex);
                }
            }
            return exceptions;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

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
                        {
                            exceptions = new List<Exception>();
                        }
                        exceptions.Add(ex);
                    }
                }
            }

            this.disposed = true;

            if (exceptions != null)
            {
                if (exceptions.Count == 1)
                {
                    throw exceptions[0];
                }

                throw new AggregateException(exceptions);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposables()
        {
            this.Dispose(false);
        }
    }
}