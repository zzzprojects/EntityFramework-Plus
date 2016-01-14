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
        public static QueryDeferred<int> DeferredSum(this IQueryable<int> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<int>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<int?> DeferredSum(this IQueryable<int?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<int?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<long> DeferredSum(this IQueryable<long> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<long>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<long?> DeferredSum(this IQueryable<long?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<long?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<float> DeferredSum(this IQueryable<float> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<float?> DeferredSum(this IQueryable<float?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<double> DeferredSum(this IQueryable<double> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<double?> DeferredSum(this IQueryable<double?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<decimal> DeferredSum(this IQueryable<decimal> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<decimal?> DeferredSum(this IQueryable<decimal?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDeferred<int> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<int>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<int?> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<int?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<long> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<long>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<long?> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<long?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<float> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<float?> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double?> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<decimal> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<decimal?> DeferredSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }
    }
}