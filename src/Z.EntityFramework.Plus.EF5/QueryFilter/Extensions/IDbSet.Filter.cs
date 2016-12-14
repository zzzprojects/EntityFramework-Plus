// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>
        ///     Filter the query using context filters associated with specified keys.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to filter using context filters associated with specified keys.</param>
        /// <param name="keys">
        ///     A variable-length parameters list containing keys associated to context filters to use to filter the
        ///     query.
        /// </param>
        /// <returns>The query filtered using context filters associated with specified keys.</returns>
#if EF5 || EF6
        public static IQueryable<T> Filter<T>(this IDbSet<T> query, params object[] keys) where T : class
#elif EFCORE
        public static IQueryable<T> Filter<T>(this DbSet<T> query, params object[] keys) where T : class
#endif
        {
            var queryFilterQueryable = QueryFilterManager.GetFilterQueryable(query);
            var nonQueryFilter = queryFilterQueryable != null ? (IQueryable<T>)queryFilterQueryable.OriginalQuery : query;

            var context = queryFilterQueryable != null ? queryFilterQueryable.Context : query.GetDbContext();
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);

            return filterContext.ApplyFilter(nonQueryFilter, keys);
        }
    }
}
#endif