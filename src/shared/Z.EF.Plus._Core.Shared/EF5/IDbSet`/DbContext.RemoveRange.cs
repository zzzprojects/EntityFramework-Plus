// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL
#if EF5
using System.Collections.Generic;
using System.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public static partial class IDbSetExtensions
    {
        public static void RemoveRange<T>(this IDbSet<T> sets, IEnumerable<T> source) where T : class
        {
            foreach (var item in source)
            {
                sets.Remove(item);
            }
        }
    }
}

#endif
#endif