// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchDeleteExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that deletes all rows asynchronously from the query
        ///     without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to delete rows from without retrieving entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> DeleteAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Delete(null), cancellationToken);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that deletes all rows from the query without
        ///     retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to delete rows from without retrieving entities.</param>
        /// <param name="batchDeleteBuilder">The batch delete builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> DeleteAsync<T>(this IQueryable<T> query, Action<BatchDelete> batchDeleteBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Delete(batchDeleteBuilder), cancellationToken);
        }
    }
}

#endif