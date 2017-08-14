// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !QUERY_INCLUDEOPTIMIZED
namespace Z.EntityFramework.Plus
{
    public static partial class QueryFutureExtensions
    {
        /// <summary>
        ///     Defer the execution of the <paramref name="query" /> and batch the query command with other
        ///     future queries. The batch is executed when a future query requires a database round trip.
        /// </summary>
        /// <typeparam name="TResult">The type of the query result.</typeparam>
        /// <param name="query">The query to defer the execution and to add in the batch of future queries.</param>
        /// <returns>
        ///     The QueryFutureValue&lt;TResult,TResult&gt; added to the batch of futures queries.
        /// </returns>
        public static QueryFutureValue<TResult> FutureValue<TResult>(this QueryDeferred<TResult> query)
        {
            if (!QueryFutureManager.AllowQueryBatch)
            {
                var futureValue = new QueryFutureValue<TResult>(null, null);
                futureValue.GetResultDirectly(query.Query);
                return futureValue;
            }

#if EF5 || EF6
            var objectQuery = query.Query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, objectQuery);
#elif EFCORE
            QueryFutureBatch futureBatch;
            QueryFutureValue<TResult> futureQuery;
            if (query.Query.IsInMemoryQueryContext())
            {
                var context = query.Query.GetInMemoryContext();
                futureBatch = QueryFutureManager.AddOrGetBatch(context);
                futureBatch.IsInMemory = true;
                futureQuery = new QueryFutureValue<TResult>(futureBatch, query.Query);
                futureQuery.InMemoryDeferredQuery = query;
            }
            else
            {
                var context = query.Query.GetDbContext();
                futureBatch = QueryFutureManager.AddOrGetBatch(context);
                futureQuery = new QueryFutureValue<TResult>(futureBatch, query.Query);
            }
#endif
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}
#endif