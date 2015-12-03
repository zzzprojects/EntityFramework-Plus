// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;

namespace Z.EntityFramework.Plus
{
    public static partial class FromCacheExtensions
    {
        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="options">The cache entry options to use to cache the query.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tag to use for the cache
        ///     key and expiration.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, MemoryCacheEntryOptions options, params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var result = Task.Run(() =>
            {
                object item;
                if (!QueryCacheManager.Cache.TryGetValue(key, out item))
                {
                    item = query.AsNoTracking().ToList();
                    item = QueryCacheManager.Cache.Set(key, item, options);
                    QueryCacheManager.AddCacheTag(key, tags);
                }

                return (IEnumerable<T>) item;
            });

            return result;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache if possible. Otherwise, materialize
        ///     asynchronously the query and cache the result
        ///     before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache.</param>
        /// <param name="tags">A variable-length parameters list containing tags to use for cache expiration.</param>
        /// <returns>The result of the query.</returns>
        public static async Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, params string[] tags) where T : class
        {
            return await query.FromCacheAsync(QueryCacheManager.DefaultMemoryCacheEntryOptions, tags);
        }
    }
}