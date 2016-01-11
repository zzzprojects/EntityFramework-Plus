using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static class QueryIncludeExtensions
    {
        public static void Exclude2<T, T2>(this ObjectContext context, IEnumerable<T> source, Func<T, IEnumerable<T2>> selector, Func<IEnumerable<T2>, List<T2>> predicate) where T : class where T2 : class
        {
            foreach (var sourceItem in source)
            {
                var list = selector(sourceItem);
                var includedItems = predicate(list);
                var excludedItems = list.Except(includedItems).ToList();

                var ilist = list as ICollection<T2>;
                if (ilist != null)
                {
                    excludedItems.ForEach(x => ilist.Remove(x));
                }
                else
                {
                    // todo: fix array? probably not supported anyway by entity framework
                    throw new Exception("Unsupported Collection");
                }
            }
        }

        public static void Exclude<T, T2>(this ObjectContext context, IEnumerable<T> source, Func<T, IEnumerable<T2>> selector, Func<IEnumerable<T2>, List<T2>> predicate) where T : class where T2 : class
        {
            var includedList = new List<object>();
            var excludedList = new List<object>();
            foreach (var sourceItem in source)
            {
                var list = selector(sourceItem);
                var includedItems = predicate(list);
                includedList.AddRange(includedItems);
                excludedList.AddRange(list.Except(includedItems));
            }
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that includes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public static IQueryable<T> Include<T, T2>(this IQueryable<T> source, Expression<Func<T, IEnumerable<T2>>> selector, Func<T2, bool> predicate) where T : class where T2 : class
        {
            source = source.Include(selector);

            var includeOrderedQueryable = source as QueryIncludeQueryable<T>;
            ObjectQuery objectQuery;
            if (includeOrderedQueryable == null)
            {
                objectQuery = source.GetObjectQuery();

                includeOrderedQueryable = new QueryIncludeQueryable<T>(source);
            }
            else
            {
                objectQuery = includeOrderedQueryable.ObjectQuery;
            }

            // Include
            includeOrderedQueryable.OriginalQueryable = includeOrderedQueryable.OriginalQueryable.Include(selector);

            // Include Filter
            var includeFilter = new QueryIncludeFilter<T, T2>() { selector = selector.Compile(), predicate = enumerable => enumerable.Where(predicate).ToList()};
            includeOrderedQueryable.IncludeFilters.Add(includeFilter);
   

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