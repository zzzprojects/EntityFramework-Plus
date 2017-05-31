// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Entity;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryInterceptorFilterExtensions
    {
        /// <summary>Gets the filter associated with the specified key from the context.</summary>
        /// <param name="context">The context filtered.</param>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <returns>The filter associated with the specified key from the context.</returns>
        public static BaseQueryFilterInterceptor Filter(this DbContext context, object key)
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);
            return filterContext.GetFilter(key);
        }

        /// <summary>
        ///     Creates and return a filter added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="context">The context to filter.</param>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilterInterceptor Filter<T>(this DbContext context, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true) where T : class
        {
            return context.Filter(Guid.NewGuid(), queryFilter, isEnabled);
        }

        /// <summary>
        ///     Creates and return a filter associated with the specified key added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="context">The context filtered.</param>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilterInterceptor Filter<T>(this DbContext context, object key, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true) where T : class
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);
            var filter = filterContext.AddFilter(key, queryFilter);

            if (isEnabled)
            {
                filter.Enable();
            }

            return filter;
        }
    }
}