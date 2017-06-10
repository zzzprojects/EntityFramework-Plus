// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_FILTER
#if EFCORE

using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Z.EntityFramework.Plus
{
    public static partial class DbContextExtensions
    {
        public static bool IsEFCore2x(this DbContext context)
        {
            return false;
        }
    }
}

#endif
#endif