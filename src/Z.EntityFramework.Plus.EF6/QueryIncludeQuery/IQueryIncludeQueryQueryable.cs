// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>Interface for query include query queryable.</summary>
    public interface IQueryIncludeQueryQueryable
    {
        /// <summary>Query and filter included related entities.</summary>
        /// <param name="query">The query.</param>
        /// <returns>An IQueryable which included related entities are filtered.</returns>
        IQueryable Select(IQueryable query);
    }
}