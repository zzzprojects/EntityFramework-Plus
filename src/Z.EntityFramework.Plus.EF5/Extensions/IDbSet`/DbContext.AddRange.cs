// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.


#if EF5
using System.Collections.Generic;
using System.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public static partial class EF5Extensions
    {
        public static void AddRange<T>(this IDbSet<T> sets, IEnumerable<T> source) where T : class
        {
            foreach (var item in source)
            {
                sets.Add(item);
            }
        }
    }
}

#endif