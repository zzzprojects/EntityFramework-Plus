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
        public static QueryDeferred<double> DeferredAverage(this IQueryable<int> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double?> DeferredAverage(this IQueryable<int?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double> DeferredAverage(this IQueryable<long> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double?> DeferredAverage(this IQueryable<long?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<float> DeferredAverage(this IQueryable<float> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<float>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<float?> DeferredAverage(this IQueryable<float?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<float?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double> DeferredAverage(this IQueryable<double> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double?> DeferredAverage(this IQueryable<double?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<decimal> DeferredAverage(this IQueryable<decimal> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<decimal>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<decimal?> DeferredAverage(this IQueryable<decimal?> source)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<decimal?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source), source.Expression));
        }

        public static QueryDeferred<double> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double?> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<float> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<float>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<float?> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<float?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double?> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<double?> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<double?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<decimal> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<decimal>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }

        public static QueryDeferred<decimal?> DeferredAverage<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (selector == null)
                throw Error.ArgumentNull("selector");

            return new QueryDeferred<decimal?>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Average, source, selector),
                    new[] {source.Expression, Expression.Quote(selector)}
                    ));
        }
    }
}