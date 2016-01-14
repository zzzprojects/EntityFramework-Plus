// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        public static QueryDeferred<TSource> DeferredAggregate<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (func == null)
                throw Error.ArgumentNull("func");

            return new QueryDeferred<TSource>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Aggregate, source, func),
                    new[] {source.Expression, Expression.Quote(func)}
                    ));
        }

        public static QueryDeferred<TAccumulate> DeferredAggregate<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (func == null)
                throw Error.ArgumentNull("func");

            return new QueryDeferred<TAccumulate>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Aggregate, source, seed, func),
                    new[] {source.Expression, Expression.Constant(seed), Expression.Quote(func)}
                    ));
        }

        public static QueryDeferred<TResult> DeferredAggregate<TSource, TAccumulate, TResult>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (func == null)
                throw Error.ArgumentNull("func");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<TResult>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Aggregate, source, seed, func, selector),
                    source.Expression,
                    Expression.Constant(seed),
                    Expression.Quote(func),
                    Expression.Quote(selector)
                    ));
        }
    }
}