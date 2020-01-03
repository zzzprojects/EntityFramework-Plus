// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
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
        /// <summary>Gets the filter associated with the specified key from the context.</summary>
        /// <param name="context">The context filtered.</param>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <returns>The filter associated with the specified key from the context.</returns>
#if EF6
        public static AliasBaseQueryFilter DbSetFilter(this DbContext context, object key)
#else
        public static AliasBaseQueryFilter Filter(this DbContext context, object key)
#endif

        {
            var filterContext = AliasQueryFilterManager.AddOrGetFilterContext(context);

            return filterContext.GetFilter(key);
        }

        /// <summary>
        ///     Creates and return a filter added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="context">The context to filter.</param>
        /// <param name="queryFilter">The query filter to apply to the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the context.</returns>
#if EF6
        public static AliasBaseQueryFilter DbSetFilter<T>(this DbContext context, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
#else
        public static AliasBaseQueryFilter Filter<T>(this DbContext context, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
#endif
        {
#if EF6
            return context.DbSetFilter(Guid.NewGuid(), queryFilter, isEnabled);

#else
            return context.Filter(Guid.NewGuid(), queryFilter, isEnabled);
#endif
        }

        /// <summary>
        ///     Creates and return a filter associated with the specified key added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="context">The context filtered.</param>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <param name="queryFilter">The query filter to apply to the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the context.</returns>
#if EF6
        public static AliasBaseQueryFilter DbSetFilter<T>(this DbContext context, object key, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
#else
        public static AliasBaseQueryFilter Filter<T>(this DbContext context, object key, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
#endif
        {
            var filterContext = AliasQueryFilterManager.AddOrGetFilterContext(context);

            var filter = filterContext.AddFilter(key, queryFilter);

            if (isEnabled)
            {
                filter.Enable();
            }

            return filter;
        }
    }
}