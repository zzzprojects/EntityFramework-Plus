// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;

namespace Z.EntityFramework.Plus
{
#if QUERY_INCLUDEOPTIMIZED
    internal static partial class QueryFutureExtensions
#else
    public static partial class QueryFutureExtensions
#endif
    {
        /// <summary>
        ///     Defer the execution of the <paramref name="query" /> and batch the query command with other
        ///     future queries. The batch is executed when a future query requires a database round trip.
        /// </summary>
        /// <typeparam name="TResult">The type of the query result.</typeparam>
        /// <param name="query">
        ///     The query to defer the execution of and to add in the batch of future
        ///     queries.
        /// </param>
        /// <returns>
        ///     The QueryFutureValue&lt;TResult,TResult&gt; added to the batch of futures queries.
        /// </returns>
        public static QueryFutureValue<TResult> FutureValue<TResult>(this IQueryable<TResult> query)
        {
#if EF5 || EF6
            var objectQuery = query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, objectQuery);
#elif EFCORE
            QueryFutureBatch futureBatch;
            QueryFutureValue<TResult> futureQuery;
            if (query.IsInMemoryQueryContext())
            {
                var context = query.GetInMemoryContext();
                futureBatch = QueryFutureManager.AddOrGetBatch(context);
                futureBatch.IsInMemory = true;
                futureQuery = new QueryFutureValue<TResult>(futureBatch, query);
            }
            else
            {
                var context = query.GetDbContext();
                futureBatch = QueryFutureManager.AddOrGetBatch(context);
                futureQuery = new QueryFutureValue<TResult>(futureBatch, query);
            }
#endif
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}