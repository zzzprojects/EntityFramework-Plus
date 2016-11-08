// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class to manage query filter options.</summary>
    public static class QueryFilterManager
    {
        /// <summary>The generic filter context lock.</summary>
        private static readonly object GenericFilterContextLock = new object();

        /// <summary>Static constructor.</summary>
        static QueryFilterManager()
        {
            CacheGenericFilterContext = new Dictionary<string, QueryFilterContext>();
            CacheWeakFilterContext = new ConditionalWeakTable<DbContext, QueryFilterContext>();
            CacheWeakFilterQueryable = new ConditionalWeakTable<IQueryable, BaseQueryFilterQueryable>();
            GlobalFilters = new Dictionary<object, BaseQueryFilter>();
            GlobalInitializeFilterActions = new List<Tuple<BaseQueryFilter, Action<BaseQueryFilter>>>();
        }

#if EFCORE
        /// <summary>Gets or sets if the method cast should be forced.</summary>
        /// <value>True if the method cast should be forced, otherwise false.</value>
        public static bool ForceCast { get; set; }
#endif

        /// <summary>Gets the global filters.</summary>
        /// <value>The global filters.</value>
        public static Dictionary<object, BaseQueryFilter> GlobalFilters { get; }

        /// <summary>Gets or sets the global initialize filter actions.</summary>
        /// <value>The global initialize filter actions.</value>
        public static List<Tuple<BaseQueryFilter, Action<BaseQueryFilter>>> GlobalInitializeFilterActions { get; set; }

        /// <summary>Gets or sets the dictionary containing generic filter context information for a DbContext.FullName.</summary>
        /// <value>The dictionary containing generic filter context information for a DbContext.FullName.</value>
        public static Dictionary<string, QueryFilterContext> CacheGenericFilterContext { get; set; }

        /// <summary>Gets or sets the weak table containing filter context for a specified context.</summary>
        /// <value>The weak table containing filter context for a specified context.</value>
        public static ConditionalWeakTable<DbContext, QueryFilterContext> CacheWeakFilterContext { get; set; }

        /// <summary>Gets or sets the weak table containing filter queryable for a specified query.</summary>
        /// <value>The weak table containing filter queryable for a specified query.</value>
        public static ConditionalWeakTable<IQueryable, BaseQueryFilterQueryable> CacheWeakFilterQueryable { get; set; }

        /// <summary>Adds or gets the generic filter context associated with the context.</summary>
        /// <param name="context">The context associated to the filter context.</param>
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
        /// <param name="context">The context associated with the filter context.</param>
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
        /// <param name="query">The query associated with the filter queryable.</param>
        /// <returns>The filter queryable associated with the query.</returns>
        public static BaseQueryFilterQueryable GetFilterQueryable(IQueryable query)
        {
            BaseQueryFilterQueryable filterQueryable;
            CacheWeakFilterQueryable.TryGetValue(query, out filterQueryable);
            return filterQueryable;
        }

        /// <summary>Gets the filter associated with the specified key from the context.</summary>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <returns>The filter associated with the specified key from the context.</returns>
        public static BaseQueryFilter Filter(object key)
        {
            BaseQueryFilter filter;
            GlobalFilters.TryGetValue(key, out filter);

            return filter;
        }

        /// <summary>
        ///     Creates and return a filter added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilter Filter<T>(Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
        {
            return Filter(Guid.NewGuid(), queryFilter, isEnabled);
        }

        /// <summary>
        ///     Creates and return a filter associated with the specified key added for the context.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="key">The filter key associated to the filter.</param>
        /// <param name="queryFilter">The query filter to apply to the the context.</param>
        /// <param name="isEnabled">true if the filter is enabled.</param>
        /// <returns>The filter created and added to the the context.</returns>
        public static BaseQueryFilter Filter<T>(object key, Func<IQueryable<T>, IQueryable<T>> queryFilter, bool isEnabled = true)
        {
            BaseQueryFilter filter;
            if (!GlobalFilters.TryGetValue(key, out filter))
            {
                filter = new QueryFilter<T>(null, queryFilter) { IsDefaultEnabled = isEnabled };
                GlobalFilters.Add(key, filter);
            }

            return filter;
        }

        /// <summary>Initilize global filter in the context.</summary>
        /// <param name="context">The context to initialize global filter on.</param>
        public static void InitilizeGlobalFilter(DbContext context)
        {
            var cloneDictionary = new Dictionary<BaseQueryFilter, BaseQueryFilter>();

            var filterContext = AddOrGetFilterContext(context);

            foreach (var filter in GlobalFilters)
            {
                var clone = filter.Value.Clone(filterContext);
                filterContext.Filters.Add(filter.Key, clone);
                if (filter.Value.IsDefaultEnabled)
                {
                    clone.Enable();
                }

                cloneDictionary.Add(filter.Value, clone);
            }

            foreach (var initlizeAction in GlobalInitializeFilterActions)
            {
                initlizeAction.Item2(cloneDictionary[initlizeAction.Item1]);
            }
        }
    }
}
#endif