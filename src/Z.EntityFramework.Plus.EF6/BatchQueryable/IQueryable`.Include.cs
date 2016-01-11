using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    internal static class QueryIncludeExtensions2
    {
        /// <summary>An IQueryable&lt;T&gt; extension method that includes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="useOptimizedInclude">true to use optimized include.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, bool useOptimizedInclude) where T : class where T2 : class
        {
            if (useOptimizedInclude)
            {
                var includeOrderedQueryable = source as BatchOrderedQueryable<T>;
                ObjectQuery objectQuery;
                if (includeOrderedQueryable == null)
                {
                    includeOrderedQueryable = new BatchOrderedQueryable<T>(source);

                    var batch = new BatchQuery
                    {
                        Context = source.GetObjectQuery().Context
                    };

                    includeOrderedQueryable.OwnerBatch = batch;
                    batch.Queries.Add(includeOrderedQueryable);

                    objectQuery = source.GetObjectQuery();
                }
                else
                {
                    objectQuery = includeOrderedQueryable.ObjectQuery;
                }

                // CREATE include query
                var includeQuery2 = objectQuery.Context.CreateObjectSet<T2>();
                Func<ObjectQuery, IQueryable<T2>> queryFactory = x => includeQuery2.Intersect(((ObjectQuery<T>) x).SelectMany(selector));
                var includeQueryable2 = new BatchOrderedQueryable<T2>(queryFactory);

                // Link
                includeOrderedQueryable.OwnerBatch.Queries.Add(includeQueryable2);
                includeQueryable2.OwnerParent = includeOrderedQueryable;
                includeOrderedQueryable.Childs.Add(includeQueryable2);

                return includeOrderedQueryable;
            }

            return source.Include(selector);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that includes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, Expression<Func<T2, bool>> predicate) where T : class where T2 : class
        {
            var includeOrderedQueryable = source as BatchOrderedQueryable<T>;
            ObjectQuery objectQuery;
            if (includeOrderedQueryable == null)
            {
                includeOrderedQueryable = new BatchOrderedQueryable<T>(source);

                var batch = new BatchQuery
                {
                    Context = source.GetObjectQuery().Context
                };

                includeOrderedQueryable.OwnerBatch = batch;
                batch.Queries.Add(includeOrderedQueryable);

                objectQuery = source.GetObjectQuery();
            }
            else
            {
                objectQuery = includeOrderedQueryable.ObjectQuery;
            }

            // CREATE include query
            var includeQuery = objectQuery.Context.CreateObjectSet<T2>().Where(predicate);
            Func<ObjectQuery, IQueryable<T2>> queryFactory = x => includeQuery.Intersect(((ObjectQuery<T>) x).SelectMany(selector));
            var includeQueryable2 = new BatchOrderedQueryable<T2>(queryFactory);

            // Link
            includeOrderedQueryable.OwnerBatch.Queries.Add(includeQueryable2);
            includeQueryable2.OwnerParent = includeOrderedQueryable;
            // includeOrderedQueryable.Childs.Add(includeQueryable2);

            

            return includeOrderedQueryable;
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that includes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="includeQuery">The include query.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, Func<IQueryable<T2>, IQueryable<T2>> includeQuery) where T : class where T2 : class
        {
            var includeOrderedQueryable = source as BatchOrderedQueryable<T>;
            ObjectQuery objectQuery;
            if (includeOrderedQueryable == null)
            {
                includeOrderedQueryable = new BatchOrderedQueryable<T>(source);

                var batch = new BatchQuery
                {
                    Context = source.GetObjectQuery().Context
                };

                includeOrderedQueryable.OwnerBatch = batch;
                batch.Queries.Add(includeOrderedQueryable);

                objectQuery = source.GetObjectQuery();
            }
            else
            {
                objectQuery = includeOrderedQueryable.ObjectQuery;
            }

            // CREATE include query
            var includeQuery2 = includeQuery(objectQuery.Context.CreateObjectSet<T2>());
            Func<ObjectQuery, IQueryable<T2>> queryFactory = x => includeQuery2.Intersect(((ObjectQuery<T>) x).SelectMany(selector));
            var includeQueryable2 = new BatchOrderedQueryable<T2>(queryFactory);

            // Link
            includeOrderedQueryable.OwnerBatch.Queries.Add(includeQueryable2);
            includeQueryable2.OwnerParent = includeOrderedQueryable;
            includeOrderedQueryable.Childs.Add(includeQueryable2);

            return includeOrderedQueryable;
        }
    }
}