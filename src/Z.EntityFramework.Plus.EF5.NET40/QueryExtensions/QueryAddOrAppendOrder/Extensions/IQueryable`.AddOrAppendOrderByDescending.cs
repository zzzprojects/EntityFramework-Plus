// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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
            return new QueryAddOrAppendOrderExpressionVisitor<TSource, TKey>().OrderBy(query, keySelector);
        }

        public static IQueryable<TSource> AddOrAppendOrderByDescending<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            return new QueryAddOrAppendOrderExpressionVisitor<TSource, TKey>().OrderBy(query, keySelector, comparer);
        }
    }
}