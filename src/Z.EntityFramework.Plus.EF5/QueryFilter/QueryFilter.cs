// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter.</summary>
    /// <typeparam name="T">The type of the filter element.</typeparam>
    public class QueryFilter<T> : BaseQueryFilter
    {
        /// <summary>Constructor.</summary>
        /// <param name="ownerFilterContext">The context that owns his filter.</param>
        /// <param name="filter">The filter.</param>
        public QueryFilter(QueryFilterContext ownerFilterContext, Func<IQueryable<T>, IQueryable<T>> filter)
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
            if(QueryFilterManager.ForceCast)
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
        public override BaseQueryFilter Clone(QueryFilterContext filterContext)
        {
            return new QueryFilter<T>(filterContext, Filter);
        }
    }
}
#endif