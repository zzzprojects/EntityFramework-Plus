#if STANDALONE

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

#if NET45

namespace Z.EntityFramework.Plus
{
    internal class LazyAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly Func<CancellationToken, Task<IEnumerable<T>>> _getObjectResultAsync;
        private IEnumerator<T> _objectResultAsyncEnumerator;

        public LazyAsyncEnumerator(Func<CancellationToken, Task<IEnumerable<T>>> getObjectResultAsync)
        {
            _getObjectResultAsync = getObjectResultAsync;
        }

        public T Current
        {
            get
            {
                return _objectResultAsyncEnumerator == null
                    ? default(T)
                    : _objectResultAsyncEnumerator.Current;
            }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            if (_objectResultAsyncEnumerator != null)
            {
                _objectResultAsyncEnumerator.Dispose();
            }
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_objectResultAsyncEnumerator != null)
            {
                return Task.FromResult(_objectResultAsyncEnumerator.MoveNext());
            }

            return FirstMoveNextAsync(cancellationToken);
        }

        private async Task<bool> FirstMoveNextAsync(CancellationToken cancellationToken)
        {
            var objectResult = await _getObjectResultAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _objectResultAsyncEnumerator = objectResult.GetEnumerator();
            }
            catch
            {
                throw;
            }
            return _objectResultAsyncEnumerator.MoveNext();
        }
    }
}

#endif
#endif