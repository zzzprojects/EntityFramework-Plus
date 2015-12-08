// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDelayedExtensions
    {
        public static QueryDelayed<TSource> DelayedMax<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<TSource>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Max, source),
                    source.Expression));
        }

        public static QueryDelayed<TResult> DelayedMax<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<TResult>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Max, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }
    }
}