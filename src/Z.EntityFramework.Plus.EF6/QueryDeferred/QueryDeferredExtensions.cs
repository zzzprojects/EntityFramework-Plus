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
using System.Reflection;
#if EF5
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

#elif EF6

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        private static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
        {
            return f.Method;
        }

        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.Method;
        }

        private static MethodInfo GetMethodInfo<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f, T1 unused1, T2 unused2, T3 unused3)
        {
            return f.Method;
        }

        private static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> f, T1 unused1, T2 unused2, T3 unused3, T4 unused4)
        {
            return f.Method;
        }

        private static Expression GetSourceExpression<TSource>(IEnumerable<TSource> source)
        {
            var q = source as IQueryable<TSource>;
            if (q != null) return q.Expression;
            return Expression.Constant(source, typeof (IEnumerable<TSource>));
        }

#if STANDALONE

        private static ObjectQuery<TEntity> GetObjectQuery<TEntity>(this IQueryable<TEntity> query)
        {
            // CHECK ObjectQuery
            var objectQuery = query as ObjectQuery<TEntity>;
            if (objectQuery != null)
            {
                return objectQuery;
            }

            // CHECK DbQuery
            var dbQuery = query as DbQuery<TEntity>;

            if (dbQuery == null)
            {
                return null;
            }

            var internalQueryProperty = dbQuery.GetType().GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQuery = internalQueryProperty.GetValue(dbQuery, null);
            var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
            var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

            objectQuery = objectQueryContext as ObjectQuery<TEntity>;

            return objectQuery;
        }

#endif
    }
}