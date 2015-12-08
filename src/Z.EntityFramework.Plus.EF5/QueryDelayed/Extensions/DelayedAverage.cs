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
        public static QueryDelayed<double> DelayedAverage(this IQueryable<int> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double?> DelayedAverage(this IQueryable<int?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double> DelayedAverage(this IQueryable<long> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double?> DelayedAverage(this IQueryable<long?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<float> DelayedAverage(this IQueryable<float> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<float?> DelayedAverage(this IQueryable<float?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double> DelayedAverage(this IQueryable<double> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double?> DelayedAverage(this IQueryable<double?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<decimal> DelayedAverage(this IQueryable<decimal> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<decimal?> DelayedAverage(this IQueryable<decimal?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDelayed<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDelayed<double> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double?> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<float> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<float>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<float?> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<float?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double?> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<double?> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<double?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<decimal> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<decimal>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDelayed<decimal?> DelayedAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDelayed<decimal?>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }
    }
}