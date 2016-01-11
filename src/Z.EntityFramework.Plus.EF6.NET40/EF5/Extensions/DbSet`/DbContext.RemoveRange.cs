// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if EF5
using System.Collections.Generic;
using System.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public static partial class EF5Extensions
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