// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;

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
    /// <summary>A class for query filter.</summary>
    /// <typeparam name="T">The type of the filter element.</typeparam>
#if EF6
    public class QueryDbSetFilter<T> : BaseQueryDbSetFilter
#else
    public class QueryFilter<T> : BaseQueryFilter
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="ownerFilterContext">The context that owns his filter.</param>
        /// <param name="filter">The filter.</param>
#if EF6
        public QueryDbSetFilter(AliasQueryFilterContext ownerFilterContext, Func<IQueryable<T>, IQueryable<T>> filter)
#else
        public QueryFilter(AliasQueryFilterContext ownerFilterContext, Func<IQueryable<T>, IQueryable<T>> filter)
#endif
        {
            ElementType = typeof(T);
            Filter = filter;
            OwnerFilterContext = ownerFilterContext;
        }

        /// <summary>Gets or sets the filter.</summary>
        /// <value>The filter.</value>
        public Func<IQueryable<T>, IQueryable<T>> Filter { get; set; }

        /// <summary>Apply the filter on the query and return the new filtered query.</summary>
        /// <param name="query">The query to filter.</param>
        /// <returns>The new query filered query.</returns>
        public override object ApplyFilter<TEntity>(object query)
        {
#if EF5 || EF6
            return Filter((IQueryable<T>)query).Cast<TEntity>();
#elif EFCORE
            // TODO: Use the same code as (EF5 || EF6) once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            if(AliasQueryFilterManager.ForceCast)
            {
                return Filter((IQueryable<T>) query).Cast<TEntity>();
            }

            return Filter((IQueryable<T>) query);
#endif
        }

        /// <summary>Gets the filter.</summary>
        /// <returns>The filter.</returns>
        public override object GetFilter()
        {
            return Filter;
        }

        /// <summary>Makes a deep copy of this filter.</summary>
        /// <param name="filterContext">The filter context that owns the filter copy.</param>
        /// <returns>A copy of this filter.</returns>
        public override AliasBaseQueryFilter Clone(AliasQueryFilterContext filterContext)
        {
#if EF6
            return new QueryDbSetFilter<T>(filterContext, Filter);
#else
            return new QueryFilter<T>(filterContext, Filter);
#endif
        }
    }
}