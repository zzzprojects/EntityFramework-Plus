// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !QUERY_INCLUDEOPTIMIZED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryAddOrAppendOrderExtensions
    {
        public static IQueryable<T> AddOrAppendOrderByDescending<T>(this IQueryable<T> query, params string[] columns)
        {
            return new QueryAddOrAppendOrderExpressionVisitor<T>().OrderByDescending(query, columns);
        }

        public static IQueryable<T> AddOrAppendOrderByDescending<T, TKey>(this IQueryable<T> query, IComparer<TKey> comparer, params string[] columns)
        {
            return new QueryAddOrAppendOrderExpressionVisitor<T>().OrderByDescending(query, columns, comparer);
        }

        public static IQueryable<TSource> AddOrAppendOrderByDescending<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector)
        {
            return new QueryAddOrAppendOrderExpressionVisitor<TSource, TKey>().OrderByDescending(query, keySelector);
        }

        public static IQueryable<TSource> AddOrAppendOrderByDescending<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            return new QueryAddOrAppendOrderExpressionVisitor<TSource, TKey>().OrderByDescending(query, keySelector, comparer);
        }
    }
}
#endif