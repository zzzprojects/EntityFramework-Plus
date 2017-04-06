// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if EF5 || EF6
using System;
using System.Data.Entity;
using System.Runtime.Caching;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryCacheExtensions
    {
#if EF5
    /// <summary>
    ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
    ///     yet, the query is materialized asynchronously and cached before being returned.
    /// </summary>
    /// <typeparam name="T">The generic type of the query.</typeparam>
    /// <param name="query">The query to cache in the QueryCacheManager.</param>
    /// <param name="policy">The policy to use to cache the query.</param>
    /// <param name="tags">
    ///     A variable-length parameters list containing tags to expire cached
    ///     entries.
    /// </param>
    /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, CacheItemPolicy policy, params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var result = Task.Run(() =>
            {
                var item = QueryCacheManager.Cache.Get(key);

                if (item == null)
                {
                    item = query.AsNoTracking().ToList();
                    item = QueryCacheManager.Cache.AddOrGetExisting(key, item, policy) ?? item;
                    QueryCacheManager.AddCacheTag(key, tags);
                }

                return (IEnumerable<T>) item;
            });

            return result;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, DateTimeOffset absoluteExpiration, params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var result = Task.Run(() =>
            {
                var item = QueryCacheManager.Cache.Get(key);

                if (item == null)
                {
                    item = query.AsNoTracking().ToList();
                    item = QueryCacheManager.Cache.AddOrGetExisting(key, item, absoluteExpiration) ?? item;
                    QueryCacheManager.AddCacheTag(key, tags);
                }

                return (IEnumerable<T>) item;
            });

            return result;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, params string[] tags) where T : class
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultCacheItemPolicy, tags);
        }
#elif EF6
        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="policy">The policy to use to cache the query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static async Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, CacheItemPolicy policy, CancellationToken cancellationToken = default(CancellationToken), params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Cache.Get(key);

            if (item == null)
            {
                item = await query.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
                item = QueryCacheManager.Cache.AddOrGetExisting(key, item, policy) ?? item;
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (IEnumerable<T>) item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="policy">The policy to use to cache the query.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, CacheItemPolicy policy, params string[] tags) where T : class
        {
            return query.FromCacheAsync(policy, default(CancellationToken), tags);
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static async Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default(CancellationToken), params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            var item = QueryCacheManager.Cache.Get(key);

            if (item == null)
            {
                item = await query.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
                item = QueryCacheManager.Cache.AddOrGetExisting(key, item, absoluteExpiration) ?? item;
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (IEnumerable<T>) item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, DateTimeOffset absoluteExpiration, params string[] tags) where T : class
        {
            return query.FromCacheAsync(absoluteExpiration, default(CancellationToken), tags);
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, params string[] tags) where T : class
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultCacheItemPolicy, default(CancellationToken), tags);
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized asynchronously and cached before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken), params string[] tags) where T : class
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultCacheItemPolicy, cancellationToken, tags);
        }
#elif EFCORE
        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="options">The cache entry options to use to cache the query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static async Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, MemoryCacheEntryOptions options, CancellationToken cancellationToken = default(CancellationToken), params string[] tags) where T : class
        {
            var key = QueryCacheManager.GetCacheKey(query, tags);

            object item;
            if (!QueryCacheManager.Cache.TryGetValue(key, out item))
            {
                item = await query.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
                item = QueryCacheManager.Cache.Set(key, item, options);
                QueryCacheManager.AddCacheTag(key, tags);
            }

            return (IEnumerable<T>) item;
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache. If the query is not cached
        ///     yet, the query is materialized and cached before being returned.
        /// </summary>
        /// <typeparam name="T">The generic type of the query.</typeparam>
        /// <param name="query">The query to cache in the QueryCacheManager.</param>
        /// <param name="options">The cache entry options to use to cache the query.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, MemoryCacheEntryOptions options, params string[] tags) where T : class
        {
            return query.FromCacheAsync(options, default(CancellationToken), tags);
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache if possible. Otherwise, materialize
        ///     asynchronously the query and cache the result
        ///     before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, params string[] tags) where T : class
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultMemoryCacheEntryOptions, default(CancellationToken), tags);
        }

        /// <summary>
        ///     Return the result of the <paramref name="query" /> from the cache if possible. Otherwise,
        ///     materialize asynchronously the query and cache the result before being returned.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tags">
        ///     A variable-length parameters list containing tags to expire cached
        ///     entries.
        /// </param>
        /// <returns>The result of the query.</returns>
        public static Task<IEnumerable<T>> FromCacheAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken), params string[] tags) where T : class
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultMemoryCacheEntryOptions, cancellationToken, tags);
        }
#endif
    }
}

#endif