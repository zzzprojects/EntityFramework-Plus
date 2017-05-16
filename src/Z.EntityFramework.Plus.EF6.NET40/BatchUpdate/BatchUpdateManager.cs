// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

#if EFCORE
using Microsoft.EntityFrameworkCore;
#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manage EF+ Batch Update Configuration.</summary>
    public class BatchUpdateManager
    {
        /// <summary>Gets or sets the batch update builder to change default configuration.</summary>
        /// <value>The batch update builder to change default configuration.</value>
        public static Action<BatchUpdate> BatchUpdateBuilder { get; set; }

#if EFCORE

        /// <summary>Gets or sets the factory to create an InMemory DbContext.</summary>
        /// <value>The factory to create an InMemory DbContext.</value>
        public static Func<DbContext> InMemoryDbContextFactory { get; set; }
#endif
    }
}