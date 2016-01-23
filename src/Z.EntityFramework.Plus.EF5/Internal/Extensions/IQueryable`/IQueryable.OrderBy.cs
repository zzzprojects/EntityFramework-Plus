// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    public static partial class IQueryableExtensions
    {
        internal static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            return Order(source, propertyName, true, true);
        }

        internal static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, string propertyName, IComparer<TKey> comparer)
        {
            return Order(source, propertyName, true, true, comparer);
        }
    }
}