using System;
using System.Collections.Generic;

namespace InCube.Core
{
    /// <summary>
    /// Helper class for dealing with many <see cref="IDisposable"/>s.
    /// </summary>
    public class Disposables : IDisposable
    {
        public delegate void Disposer();

        private readonly Stack<Disposer> _unmanaged = new Stack<Disposer>();
        private readonly Stack<IDisposable> _managed = new Stack<IDisposable>();
        private bool _disposed = false;

        public void Add(Disposer disposer)
        {
            Preconditions.CheckArgument(!_disposed, "disposed");
            Preconditions.CheckNotNull(disposer);
            _unmanaged.Push(disposer);
        }

        public T Add<T>(T disposable) where T : IDisposable
        {
            Preconditions.CheckArgument(!_disposed, "disposed");
            Preconditions.CheckNotNull(disposable);
            _managed.Push(disposable);
            return disposable;
        }

        public T Create<T>(Func<T> creator) where T : IDisposable
        {
            return Add(creator());
        }

        private List<Exception> ReleaseUnmanagedResources()
        {
            List<Exception> exceptions = null;
            while (_unmanaged.Count > 0)
            {
                var disposer = _unmanaged.Pop();
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
            if (_disposed) return;

            var exceptions = ReleaseUnmanagedResources();
            if (disposing)
            {
                while (_managed.Count > 0)
                {
                    var disposable = _managed.Pop();
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

            _disposed = true;

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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposables()
        {
            Dispose(false);
        }
    }
}