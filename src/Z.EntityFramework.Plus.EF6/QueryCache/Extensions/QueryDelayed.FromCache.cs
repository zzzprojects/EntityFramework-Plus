// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Runtime.Caching;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryCacheExtensions
    {
        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="policy">The policy to use to cache the query.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tag to use for the cache
        ///     key and expiration.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static T FromCache<T>(this QueryDelayed<T> query, CacheItemPolicy policy, params string[] tags)
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Cache.Get(key);

            if (item == null)
            {
                item = query.Execute();
                QueryCacheManager.Cache.AddOrGetExisting(key, item, policy);
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (T) item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tag to use for the cache
        ///     key and expiration.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static T FromCache<T>(this QueryDelayed<T> query, DateTimeOffset absoluteExpiration, params string[] tags)
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Cache.Get(key);

            if (item == null)
            {
                item = query.Execute();
                QueryCacheManager.Cache.AddOrGetExisting(key, item, absoluteExpiration);
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (T) item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tag to use for the cache
        ///     key and expiration.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static T FromCache<T>(this QueryDelayed<T> query, params string[] tags)
        {
            return query.FromCache(QueryCacheManager.DefaultCacheItemPolicy, tags);
        }
    }
}