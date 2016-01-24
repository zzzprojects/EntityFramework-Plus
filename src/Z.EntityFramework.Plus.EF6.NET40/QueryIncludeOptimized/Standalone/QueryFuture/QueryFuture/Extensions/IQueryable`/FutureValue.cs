// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.
#if STANDALONE
using System.Linq;

namespace Z.EntityFramework.Plus
{
    internal static partial class QueryFutureExtensions
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
        internal static QueryFutureValue<TResult> FutureValue<TResult>(this IQueryable<TResult> query)
        {
#if EF5 || EF6
            var objectQuery = query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, objectQuery);
#elif EF7
            var context = query.GetDbContext();
            var futureBatch = QueryFutureManager.AddOrGetBatch(context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, query);
#endif
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}
#endif