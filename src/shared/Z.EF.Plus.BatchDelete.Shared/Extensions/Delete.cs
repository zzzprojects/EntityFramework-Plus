// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchDeleteExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that deletes all rows from the query without
        ///     retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to delete rows from without retrieving entities.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Delete<T>(this IQueryable<T> query) where T : class
        {
            return query.Delete(null);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that deletes all rows from the query without
        ///     retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to delete rows from without retrieving entities.</param>
        /// <param name="batchDeleteBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Delete<T>(this IQueryable<T> query, Action<BatchDelete> batchDeleteBuilder) where T : class
        {
            var batchDelete = new BatchDelete();

            if (BatchDeleteManager.BatchDeleteBuilder != null)
            {
                BatchDeleteManager.BatchDeleteBuilder(batchDelete);
            }

            if (batchDeleteBuilder != null)
            {
                batchDeleteBuilder(batchDelete);
            }

            return batchDelete.Execute(query);
        }
    }
}