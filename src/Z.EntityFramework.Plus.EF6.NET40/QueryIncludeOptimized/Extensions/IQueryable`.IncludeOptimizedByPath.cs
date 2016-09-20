// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryIncludeOptimizedExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that include and filter related entities with a optimized SQL.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to filter included related entities.</param>
        /// <param name="navigationProperties">The navigation properties to include.</param>
        /// <returns>An IQueryable&lt;T&gt; that include and filter related entities.</returns>
        public static IQueryable<T> IncludeOptimizedByPath<T>(this IQueryable<T> query, string navigationProperties)
        {
            return QueryIncludeOptimizedByPath.IncludeOptimizedByPath(query, navigationProperties);
        }
    }
}