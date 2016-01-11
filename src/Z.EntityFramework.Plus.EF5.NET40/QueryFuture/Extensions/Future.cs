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
        ///     An IQueryable&lt;TEntity&gt; extension method that futures the given query.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <returns>A QueryFutureEnumerable&lt;TEntity&gt;</returns>
        public static QueryFutureEnumerable<TEntity> Future<TEntity>(this IQueryable<TEntity> query)
        {
            var objectQuery = query.GetObjectQuery();
            var futureBatch = QueryFutureManager.AddOrGetBatch(objectQuery.Context);
            var futureQuery = new QueryFutureEnumerable<TEntity>(futureBatch, objectQuery);
            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }
    }
}