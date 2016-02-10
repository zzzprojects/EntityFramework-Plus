// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchUpdateExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Update(updateFactory, null), cancellationToken);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression  without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <param name="batchUpdateBuilder">The batch delete builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, Action<BatchUpdate> batchUpdateBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Update(updateFactory, batchUpdateBuilder), cancellationToken);
        }
    }
}

#endif