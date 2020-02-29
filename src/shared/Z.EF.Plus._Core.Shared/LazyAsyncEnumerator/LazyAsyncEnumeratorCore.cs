// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_INCLUDEFILTER || QUERY_INCLUDEOPTIMIZED
#if EFCORE
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    internal class LazyAsyncEnumerator<T> : IAsyncEnumerator<T>
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

        public void Dispose()
        {
            if (_objectResultAsyncEnumerator != null)
            {
                _objectResultAsyncEnumerator.Dispose();
            }
        }




#if EFCORE_2X
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_objectResultAsyncEnumerator != null)
            {
                return Task.FromResult(_objectResultAsyncEnumerator.MoveNext());
            }

            return FirstMoveNextAsync(cancellationToken);
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
#elif EFCORE_3X

        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_objectResultAsyncEnumerator != null)
            {
                return Task.FromResult(_objectResultAsyncEnumerator.MoveNext());
            }

            return FirstMoveNextAsync();
        }
        public ValueTask<bool> MoveNextAsync()
        {
            if (_objectResultAsyncEnumerator != null)
            {
                
                return new ValueTask<bool>(Task.FromResult(_objectResultAsyncEnumerator.MoveNext()));
            }

            return new ValueTask<bool>(FirstMoveNextAsync());
        }

        private async Task<bool> FirstMoveNextAsync()
        {
            var objectResult = await _getObjectResultAsync(CancellationToken.None).ConfigureAwait(false);
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
#endif


    }
}

#endif
#endif