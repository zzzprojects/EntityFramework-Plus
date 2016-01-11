// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Interface for query filter queryable.</summary>
    public interface IQueryFilterQueryable
    {
        /// <summary>Gets or sets the context associated to the query.</summary>
        /// <value>The context associated to the query.</value>
        DbContext Context { get; set; }

        /// <summary>Disables the filter on the associated query.</summary>
        /// <param name="filter">The filter to disable.</param>
        void DisableFilter(IQueryFilter filter);

        /// <summary>Enables the filter on the associated query.</summary>
        /// <param name="filter">The filter to enable.</param>
        void EnableFilter(IQueryFilter filter);

        /// <summary>Gets original query on the associated query.</summary>
        /// <returns>The original query on the associated query.</returns>
        object GetOriginalQuery();
    }
}