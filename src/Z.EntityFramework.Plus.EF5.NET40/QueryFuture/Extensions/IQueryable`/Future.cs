// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFutureExtensions
    {
        /// <summary>
        ///     Defer the execution of the <paramref name="query" /> and batch the query command with other
        ///     future queries. The batch is executed when a future query requires a database round trip.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">
        ///     The query to defer the execution of and to add in the batch of future
        ///     queries.
        /// </param>
        /// <returns>
        ///     The QueryFutureEnumerable&lt;TEntity&gt; added to the batch of futures queries.
        /// </returns>
        public static QueryFutureEnumerable<T> Future<T>(this IQueryable<T> query)
        {
#if EF5 || EF6
            var objectQuery = query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureEnumerable<T>(futureBatch, objectQuery);
#elif EF7
            var context = query.GetDbContext();
            var futureBatch = QueryFutureManager.AddOrGetBatch(context);
            var futureQuery = new QueryFutureEnumerable<T>(futureBatch, query);
#endif
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}