// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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
            ElementType = typeof (T);
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
            return Filter((IQueryable<T>) query).Cast<TEntity>();
#elif EF7
            // TODO: Use the same code as (EF5 || EF6) once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
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