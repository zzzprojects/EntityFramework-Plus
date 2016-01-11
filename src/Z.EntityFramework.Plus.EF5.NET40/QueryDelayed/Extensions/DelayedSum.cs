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
    public static partial class QueryDelayedExtensions
    {
        public static QueryDelayed<int> DelayedSum(this IQueryable<int> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<int>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<int?> DelayedSum(this IQueryable<int?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<int?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<long> DelayedSum(this IQueryable<long> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<long>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<long?> DelayedSum(this IQueryable<long?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<long?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<float> DelayedSum(this IQueryable<float> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<float?> DelayedSum(this IQueryable<float?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<double> DelayedSum(this IQueryable<double> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<double?> DelayedSum(this IQueryable<double?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<decimal> DelayedSum(this IQueryable<decimal> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<decimal?> DelayedSum(this IQueryable<decimal?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source), source.Expression));
        }

        public static QueryDelayed<int> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<int>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<int?> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<int?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<long> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<long>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<long?> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<long?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<float> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<float?> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double?> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<decimal> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<decimal?> DelayedSum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Sum, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }
    }
}