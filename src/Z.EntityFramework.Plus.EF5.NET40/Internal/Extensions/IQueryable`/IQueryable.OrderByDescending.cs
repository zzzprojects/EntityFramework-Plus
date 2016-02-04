// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class IQueryableExtensions
    {
        internal static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            return Order(source, propertyName, true, false);
        }

        internal static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryable<TSource> source, string propertyName, IComparer<TKey> comparer)
        {
            return Order(source, propertyName, true, false, comparer);
        }
    }
}