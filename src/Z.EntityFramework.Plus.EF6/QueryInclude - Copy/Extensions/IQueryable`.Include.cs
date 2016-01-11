using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static class QueryIncludeExtensions
    {
        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Func<T, IEnumerable<T2>> path, bool useOptimizedInclude) where T : class
        {
            IQueryable<T> query;
            if (useOptimizedInclude)
            {
                query = null;
            }
            else
            {
                query = null;
            }

            return query;
        }

        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, Expression<Func<T2, bool>> predicate) where T : class where T2 : class
        {
            var context = source.GetObjectQuery().Context;
            var objectSetQuery = context.CreateObjectSet<T2>().Where(predicate);

            var includeQueryable = new QueryIncludeQueryable<T, T2>(source, selector, objectSetQuery);
            return includeQueryable;
        }

        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, Func<IQueryable<T2>, IQueryable<T2>> includeQuery) where T : class where T2 : class
        {
            var context = source.GetObjectQuery().Context;
            var objectSetQuery = includeQuery(context.CreateObjectSet<T2>());

            var includeQueryable = new QueryIncludeQueryable<T, T2>(source, selector, objectSetQuery);
            return includeQueryable;
        }
    }
}