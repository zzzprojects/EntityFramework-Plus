 // Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;
#if NET45 
using System.Data.Entity.Infrastructure;
#endif

#elif EFCORE
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class for query future value.</summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
#if QUERY_INCLUDEOPTIMIZED
    internal class QueryFutureValue<TResult> : BaseQueryFuture
#else
    public class QueryFutureValue<TResult> : BaseQueryFuture
#endif
    {
        /// <summary>The result of the query future.</summary>
        private TResult _result;

        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">
        ///     The query to defer the execution and to add in the batch of future
        ///     queries.
        /// </param>
#if EF5 || EF6
        public QueryFutureValue(QueryFutureBatch ownerBatch, ObjectQuery query)
#elif EFCORE
        public QueryFutureValue(QueryFutureBatch ownerBatch, IQueryable query)
#endif
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets the value of the future query.</summary>
        /// <value>The value of the future query.</value>
        public TResult Value
        {
            get
            {
                if (!HasValue)
                {
                    OwnerBatch.ExecuteQueries();
                }

                return _result;
            }
        }

#if NET45 
        /// <summary>Gets the value of the future query.</summary>
        /// <value>The value of the future query.</value>
        public async Task<TResult> ValueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
	        cancellationToken.ThrowIfCancellationRequested();
			if (!HasValue)
            {
#if EF6
                if (Query.Context.IsInMemoryEffortQueryContext())
                {
                    OwnerBatch.ExecuteQueries();
                }
                else
                {
                    await OwnerBatch.ExecuteQueriesAsync(cancellationToken).ConfigureAwait(false);
                }
#else
                await OwnerBatch.ExecuteQueriesAsync(cancellationToken).ConfigureAwait(false);
#endif
			}

			return _result;
        }
#endif

                /// <summary>Sets the result of the query deferred.</summary>
                /// <param name="reader">The reader returned from the query execution.</param>
        public override void SetResult(DbDataReader reader)
        {
            if (reader.GetType().FullName.Contains("Oracle"))
            {
                var reader2 = new QueryFutureOracleDbReader(reader);
                reader = reader2;
            }
  
            var enumerator = GetQueryEnumerator<TResult>(reader);
            using (enumerator)
            {
                enumerator.MoveNext();
                _result = enumerator.Current;

            }
           
            // Enumerate on first item only
          
            HasValue = true;
        }


#if EFCORE
        public QueryDeferred<TResult> InMemoryDeferredQuery;

        public override void ExecuteInMemory()
        {
            if (InMemoryDeferredQuery != null)
            {
                _result = InMemoryDeferredQuery.Execute();
                HasValue = true;
            }
            else
            {
                var enumerator = Query.GetEnumerator();

                // Enumerate on first item only
                enumerator.MoveNext();
                _result = (TResult)enumerator.Current;

                HasValue = true;
            }
        }
#endif
        public override void GetResultDirectly()
        {
            var query = (IQueryable<TResult>) Query;
            var value = query.Provider.Execute<TResult>(query.Expression);

            _result = value;
            HasValue = true;
        }

#if NET45

#if EF6
        public override async Task GetResultDirectlyAsync(CancellationToken cancellationToken)
#else
        public override Task GetResultDirectlyAsync(CancellationToken cancellationToken)
#endif
        {
		    cancellationToken.ThrowIfCancellationRequested();

#if EF6
			var query = (IQueryable<TResult>)Query;
			var value = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync<TResult>(query.Expression, cancellationToken).ConfigureAwait(false);
	
			_result = value;
		    HasValue = true;
#else
		    GetResultDirectly();
            return Task.FromResult(0);
#endif

	    }
#endif


        internal void GetResultDirectly(IQueryable<TResult> query)
        {
            var value = query.Provider.Execute<TResult>(query.Expression);

            _result = value;
            HasValue = true;
        }

        /// <summary>
        /// Performs an implicit conversion from QueryFutureValue to TResult.
        /// </summary>
        /// <param name="futureValue">The future value.</param>
        /// <returns>The result of forcing this lazy value.</returns>
        public static implicit operator TResult(QueryFutureValue<TResult> futureValue)
        {
            if (futureValue == null)
            {
                return default(TResult);
            }

            return futureValue.Value;
        }
    }
}