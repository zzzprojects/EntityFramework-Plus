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
    /// <summary>Base class for query include filter child.</summary>
    public abstract class BaseQueryIncludeFilterChild
    {
        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        /// <returns>The query to use to load related entities.</returns>
        public virtual IQueryable CreateIncludeQuery(IQueryable rootQuery)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Gets the filter.</summary>
        /// <returns>The filter.</returns>
        public abstract Expression GetFilter();
    }
}