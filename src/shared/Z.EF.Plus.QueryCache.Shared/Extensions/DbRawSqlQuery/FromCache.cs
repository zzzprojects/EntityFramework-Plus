// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Collections.Generic;
using System.Linq;

#if EF5 || EF6
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.Caching;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryCacheExtensions
    {
#if EF6

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached yet, the query
        ///     is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache the result.</param>
        /// <param name="policy">The eviction details for the cached result.</param>
        /// <param name="tags">(Optional) The tag list that can be used to expire cached result.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this DbRawSqlQuery<T> query, CacheItemPolicy policy, params string[] tags) where T : class
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return query.ToList();
            }
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Get<List<T>>(key);

            if (item == null)
            {
                item = query.ToList();
                item = QueryCacheManager.AddOrGetExisting(key, item, policy) ?? item;
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (IEnumerable<T>)item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached yet, the query
        ///     is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache the result.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="tags">(Optional) The tag list that can be used to expire cached result.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this DbRawSqlQuery<T> query, DateTimeOffset absoluteExpiration, params string[] tags) where T : class
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return query.ToList();
            }

            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Get<List<T>>(key);

            if (item == null)
            {
                item = query.ToList();
                item = QueryCacheManager.AddOrGetExisting(key, item, absoluteExpiration) ?? item;
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (IEnumerable<T>)item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached yet, the query
        ///     is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache the result.</param>
        /// <param name="tags">The tag list that can be used to expire cached result.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this DbRawSqlQuery<T> query, params string[] tags) where T : class
        {
            return query.FromCache(QueryCacheManager.DefaultCacheItemPolicy, tags);
        }
#endif
    }
}