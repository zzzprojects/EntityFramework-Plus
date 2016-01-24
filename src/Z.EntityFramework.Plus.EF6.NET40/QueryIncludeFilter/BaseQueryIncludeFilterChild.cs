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
    }
}