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
        /// <summary>A DelayedQuery&lt;TResult&gt; extension method that future value.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <returns>A FutureQueryValue&lt;TResult,TResult&gt;</returns>
        public static QueryFutureValue<TResult> FutureValue<TResult>(this QueryDeferred<TResult> query)
        {
            var objectQuery = query.Query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, objectQuery);
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }

        /// <summary>A DelayedQuery&lt;TResult&gt; extension method that future value.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <returns>A FutureQueryValue&lt;TResult,TResult&gt;</returns>
        public static QueryFutureValue<TResult> FutureValue<TResult>(this IQueryable<TResult> query)
        {
            var objectQuery = query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureValue<TResult>(futureBatch, objectQuery);
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}