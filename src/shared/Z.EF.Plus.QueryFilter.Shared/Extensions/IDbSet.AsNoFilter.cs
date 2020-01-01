// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

#if EF6
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryDbSetFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryDbSetFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryDbSetFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryDbSetFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryDbSetFilterSet;
#else
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryFilterSet;
#endif

namespace Z.EntityFramework.Plus
{
#if EF6
    public static partial class QueryDbSetFilterExtensions
#else
    public static partial class QueryFilterExtensions
#endif
    {
		/// <summary>Return the original query before the context was filtered.</summary>
		/// <typeparam name="T">The type of elements of the query.</typeparam>
		/// <param name="query">The filtered query from which the original query should be retrieved.</param>
		/// <returns>The original query before the context was filtered.</returns>
#if EF5
        public static IQueryable<T> AsNoFilter<T>(this IQueryable<T> query) where T : class
#elif EF6
        public static IQueryable<T> AsNoDbSetFilter<T>(this IQueryable<T> query) where T : class
#elif EFCORE
		public static IQueryable<T> AsNoFilter<T>(this IQueryable<T> query) where T : class
#endif
        {
            var queryFilterQueryable = AliasQueryFilterManager.GetFilterQueryable(query);

            return queryFilterQueryable != null ? (IQueryable<T>)queryFilterQueryable.OriginalQuery : query;
        }
    }
}