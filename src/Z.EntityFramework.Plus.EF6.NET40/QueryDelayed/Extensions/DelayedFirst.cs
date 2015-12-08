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
        public static QueryDelayed<TSource> DelayedFirst<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<TSource>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.First, source),
                    source.Expression
                    ));
        }

        public static QueryDelayed<TSource> DelayedFirst<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            return new QueryDelayed<TSource>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.First, source, predicate),
                    new[] {source.Expression, Expression.Quote(predicate)}
                    ));
        }
    }
}