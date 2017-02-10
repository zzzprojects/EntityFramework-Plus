// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class for query future value.</summary>
    /// <typeparam name="TResult">The type of elements of the query.</typeparam>
#if QUERY_INCLUDEOPTIMIZED
    internal class QueryFutureEnumerable<T> : BaseQueryFuture, IEnumerable<T>
#else
    public class QueryFutureEnumerable<TResult> : BaseQueryFuture<TResult>, IEnumerable<TResult>
#endif
    {
        /// <summary>The result of the query future.</summary>
        private IEnumerable<TResult> _result;

        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">
        ///     The query to defer the execution and to add in the batch of future
        ///     queries.
        /// </param>
#if EF5 || EF6
        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, ObjectQuery<T> query)
#elif EFCORE
        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, IQueryable<TResult> query)
#endif
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        public IEnumerator<TResult> GetEnumerator()
        {
            if (!HasValue)
            {
                OwnerBatch.ExecuteQueries();
            }

            if (_result == null)
            {
                return new List<TResult>().GetEnumerator();
            }

            return _result.GetEnumerator();
        }


        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public override void SetResult(DbDataReader reader)
        {
            var enumerator = GetQueryEnumerator<TResult>(reader);

            SetResult(enumerator);
        }

        private void SetResult(IEnumerator<TResult> enumerator)
        {
            // Enumerate on all items
            var list = new List<TResult>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            _result = list;

            HasValue = true;
        }

        public override void GetResultDirectly()
        {
            var enumerator = Query.GetEnumerator();

            SetResult(enumerator);
        }
    }
}