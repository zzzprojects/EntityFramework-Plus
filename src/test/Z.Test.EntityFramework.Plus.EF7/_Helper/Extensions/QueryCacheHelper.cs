// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using Microsoft.Data.Entity;
#if EF7

#endif

namespace Z.Test.EntityFramework.Plus
{
    public static class TestExtensions
    {
#if EF7
        public static DbContext GetObjectContext(this DbContext context)
        {
            return context;
        }
#endif
    }
}