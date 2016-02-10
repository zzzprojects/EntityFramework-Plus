// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchUpdateExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update expression.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory) where T : class
        {
            return query.Update(updateFactory, null);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update expression.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
            var batchUpdate = new BatchUpdate();

            if (BatchUpdateManager.BatchUpdateBuilder != null)
            {
                BatchUpdateManager.BatchUpdateBuilder(batchUpdate);
            }

            if (batchUpdateBuilder != null)
            {
                batchUpdateBuilder(batchUpdate);
            }

            return batchUpdate.Execute(query, updateFactory);
        }
    }
}