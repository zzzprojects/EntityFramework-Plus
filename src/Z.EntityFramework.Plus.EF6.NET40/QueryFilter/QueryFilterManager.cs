// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for query filters.</summary>
    public static class QueryFilterManager
    {
        /// <summary>The generic filter context lock.</summary>
        private static readonly object GenericFilterContextLock = new object();

        /// <summary>Static constructor.</summary>
        static QueryFilterManager()
        {
            CacheGenericFilterContext = new Dictionary<string, QueryFilterContext>();
            CacheWeakFilterContext = new ConditionalWeakTable<DbContext, QueryFilterContext>();
            CacheWeakFilterQueryable = new ConditionalWeakTable<IQueryable, IQueryFilterQueryable>();
        }

        /// <summary>Gets or sets the dictionary containing generic filter context for a DbContext.FullName.</summary>
        /// <value>The dictionary containing generic filter context for a DbContext.FullName.</value>
        public static Dictionary<string, QueryFilterContext> CacheGenericFilterContext { get; set; }

        /// <summary>Gets or sets the weak table containing filter context for a specified context.</summary>
        /// <value>The weak table containing filter context for a specified context.</value>
        public static ConditionalWeakTable<DbContext, QueryFilterContext> CacheWeakFilterContext { get; set; }

        /// <summary>Gets or sets the weak table containing filter queryable for a specified query.</summary>
        /// <value>The weak table containing filter queryable for a specified query.</value>
        public static ConditionalWeakTable<IQueryable, IQueryFilterQueryable> CacheWeakFilterQueryable { get; set; }

        /// <summary>Adds or gets the generic filter context associated with the context.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The generic filter context associated with the context.</returns>
        public static QueryFilterContext AddOrGetGenericFilterContext(DbContext context)
        {
            var key = context.GetType().FullName;
            QueryFilterContext filterContext;

            if (!CacheGenericFilterContext.TryGetValue(key, out filterContext))
            {
                lock (GenericFilterContextLock)
                {
                    if (!CacheGenericFilterContext.TryGetValue(key, out filterContext))
                    {
                        filterContext = new QueryFilterContext(context, true);
                        CacheGenericFilterContext.Add(key, filterContext);
                    }
                }
            }

            return filterContext;
        }

        /// <summary>Adds or get the filter context associated with the context.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The filter context associated with the context.</returns>
        public static QueryFilterContext AddOrGetFilterContext(DbContext context)
        {
            QueryFilterContext filterContext;

            if (!CacheWeakFilterContext.TryGetValue(context, out filterContext))
            {
                filterContext = new QueryFilterContext(context);
                CacheWeakFilterContext.Add(context, filterContext);
            }

            return filterContext;
        }

        /// <summary>Gets the filter queryable associated with the query.</summary>
        /// <param name="query">The query.</param>
        /// <returns>The filter queryable associated with the query.</returns>
        public static IQueryFilterQueryable GetFilterQueryable(IQueryable query)
        {
            IQueryFilterQueryable filterQueryable;
            CacheWeakFilterQueryable.TryGetValue(query, out filterQueryable);
            return filterQueryable;
        }
    }
}