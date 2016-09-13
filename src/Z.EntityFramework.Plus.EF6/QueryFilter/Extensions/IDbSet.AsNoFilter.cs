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
        /// <summary>Return the orginal query before the context was filtered.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The filtered query from which the original query should be retrieved.</param>
        /// <returns>The orginal query before the context was filtered.</returns>
#if EF5 || EF6
        public static IQueryable<T> AsNoFilter<T>(this IDbSet<T> query) where T : class
#elif EFCORE
        public static IQueryable<T> AsNoFilter<T>(this DbSet<T> query) where T : class
#endif
        {
            var queryFilterQueryable = QueryFilterManager.GetFilterQueryable(query);
            return queryFilterQueryable != null ? (IQueryable<T>)queryFilterQueryable.OriginalQuery : query;
        }
    }
}
#endif