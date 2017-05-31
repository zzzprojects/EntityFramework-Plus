// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System;
using System.Collections.Generic;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A base class for query filter queryable.</summary>
    public abstract class BaseQueryFilterQueryable
    {
        /// <summary>Gets or sets the context associated to the filter queryable.</summary>
        /// <value>The context associated to the filter queryable.</value>
        public DbContext Context { get; set; }

        /// <summary>Gets or sets the filters used by the filter queryable.</summary>
        /// <value>The filters used by the filter queryable.</value>
        public List<BaseQueryFilter> Filters { get; set; }

        /// <summary>Gets or sets the filter set associated with the filter queryable.</summary>
        /// <value>The filter set associated with the filter queryable.</value>
        public QueryFilterSet FilterSet { get; set; }

        /// <summary>Gets or sets the original query.</summary>
        /// <value>The original query.</value>
        public object OriginalQuery { get; set; }

        /// <summary>Disables the filter on the associated query.</summary>
        /// <param name="filter">The filter to disable on the associated query.</param>
        public void DisableFilter(BaseQueryFilter filter)
        {
            if (Filters.Remove(filter))
            {
                UpdateInternalQuery();
            }
        }

        /// <summary>Enables the filter on the associated query.</summary>
        /// <param name="filter">The filter to enable on the associated query.</param>
        public void EnableFilter(BaseQueryFilter filter)
        {
            if (!Filters.Contains(filter))
            {
                Filters.Add(filter);
                UpdateInternalQuery();
            }
        }

        /// <summary>Updates the internal query.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        public virtual void UpdateInternalQuery()
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }
    }
}
#endif