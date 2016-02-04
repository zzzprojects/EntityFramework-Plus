// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static class ListExtensions
    {
        internal static int GetVersion<TSource>(this List<TSource> source)
        {
            var property = source.GetType().GetField("_version", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int) property.GetValue(source);
        }
    }
}