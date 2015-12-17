// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>Gets the filter from the context associated with the specified key.</summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The filter key.</param>
        /// <returns>The filter from the context associated with the specified key.</returns>
        public static IQueryFilter Filter(this DbContext context, object key)
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);
            return filterContext.GetFilter(key);
        }

        /// <summary>
        ///     Creates and Gets the filter from the context associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Generic type to filter.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="predicate">The filter predicate.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter from the context associated with the specified key.</returns>
        public static IQueryFilter Filter<T>(this DbContext context, Func<IQueryable<T>, IQueryable<T>> predicate, bool isEnabled = true)
        {
            return context.Filter(Guid.NewGuid(), predicate, isEnabled);
        }

        /// <summary>
        ///     Creates and Gets the filter from the context associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Generic type to filter.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="key">The filter key.</param>
        /// <param name="predicate">The filter predicate.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter from the context associated with the specified key.</returns>
        public static IQueryFilter Filter<T>(this DbContext context, object key, Func<IQueryable<T>, IQueryable<T>> predicate, bool isEnabled = true)
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);
            var filter = filterContext.AddFilter(key, predicate);

            if (isEnabled)
            {
                filter.Enable();
            }

            return filter;
        }
    }
}