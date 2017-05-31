// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    /// <summary>Base class for query include optimized child.</summary>
    public abstract class BaseQueryIncludeOptimizedChild
    {
        /// <summary>Gets or sets a value indicating whether this object is lazy.</summary>
        /// <value>true if this object is lazy, false if not.</value>
        public bool IsLazy { get; set; }

        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        public virtual void CreateIncludeQuery(IQueryable rootQuery)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Gets the filter.</summary>
        /// <returns>The filter.</returns>
        public abstract Expression GetFilter();

        /// <summary>Gets filtered query.</summary>
        /// <param name="query">The query.</param>
        /// <returns>The filtered query.</returns>
        public abstract IQueryable GetFilteredQuery(IQueryable query);
    }
}