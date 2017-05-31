// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public class QueryIncludeOptimizedManager
    {
        static QueryIncludeOptimizedManager()
        {
            AllowQueryBatch = true;
            AllowIncludeSubPath = false;
        }

        public static bool AllowQueryBatch { get; set; }
        public static bool AllowIncludeSubPath { get; set; }
        // public static Func<DbContext, DbContext> DbContextFactory { get; set; }
    }
}