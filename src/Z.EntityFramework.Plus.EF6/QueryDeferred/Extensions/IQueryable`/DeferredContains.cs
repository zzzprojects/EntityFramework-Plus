// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        public static QueryDeferred<bool> DeferredContains<TSource>(this IQueryable<TSource> source, TSource item)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<bool>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Contains, source, item),
                    new[] {source.Expression, Expression.Constant(item, typeof (TSource))}
                    ));
        }

        public static QueryDeferred<bool> DeferredContains<TSource>(this IQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<bool>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Contains, source, item, comparer),
                    new[] {source.Expression, Expression.Constant(item, typeof (TSource)), Expression.Constant(comparer, typeof (IEqualityComparer<TSource>))}
                    ));
        }
    }
}