// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_INCLUDEFILTER || QUERY_INCLUDEOPTIMIZED
#if EF6 && NET45
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

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