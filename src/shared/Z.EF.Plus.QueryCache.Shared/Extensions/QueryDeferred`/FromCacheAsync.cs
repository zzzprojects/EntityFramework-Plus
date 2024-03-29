﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if EF5 || EF6
using System.Runtime.Caching;

#elif EFCORE
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, CacheItemPolicy policy, params string[] tags)
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return Task.Run(() =>
                {
                   return query.Execute();
                });
            }

            var key = QueryCacheManager.GetCacheKey(query, tags, true);

            var result = Task.Run(() =>
            {
                var item = QueryCacheManager.GetDeferred(key);

                if (item == null)
                {
                    item = query.Execute();
                    item = QueryCacheManager.AddOrGetExistingDeferred<T>(key, item ?? DBNull.Value, policy) ?? item;
                    QueryCacheManager.AddCacheTag(key, tags);
				    QueryCacheManager.AddCacheTag(key, typeof(T).Name + QueryCacheManager.CacheTypeSuffix);
                }

                item = item.IfDbNullThenNull();

                return (T) item;
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, DateTimeOffset absoluteExpiration, params string[] tags)
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return Task.Run(() =>
                {
                    return query.Execute();
                });
            }

            var key = QueryCacheManager.GetCacheKey(query, tags, true);

            var result = Task.Run(() =>
            {
                var item = QueryCacheManager.GetDeferred(key);

                if (item == null)
                {
                    item = query.Execute();
                    item = QueryCacheManager.AddOrGetExistingDeferred<T>(key, item ?? DBNull.Value, absoluteExpiration) ?? item;
                    QueryCacheManager.AddCacheTag(key, tags);
					QueryCacheManager.AddCacheTag(key, typeof(T).Name + QueryCacheManager.CacheTypeSuffix);
                }

                item = item.IfDbNullThenNull();

                return (T) item;
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, params string[] tags)
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
        public static async Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, CacheItemPolicy policy, CancellationToken cancellationToken = default(CancellationToken), params string[] tags)
        { 
            if (!QueryCacheManager.IsEnabled)
            {
                return await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }

            var key = QueryCacheManager.GetCacheKey(query, tags, true);

            var item = QueryCacheManager.GetDeferred(key);

            if (item == null)
            {
                using (var handler = new QueryCacheItemTracker().Initialize(query))
                {
                    item = await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                    item = QueryCacheManager.AddOrGetExistingDeferred<T>(key, item ?? DBNull.Value, policy) ?? item;
                    QueryCacheManager.AddCacheTag(handler, key, tags);
                    QueryCacheManager.AddCacheTag(key, typeof(T).Name + QueryCacheManager.CacheTypeSuffix);
                }
            }

            item = item.IfDbNullThenNull();

            return (T) item;
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, CacheItemPolicy policy, params string[] tags)
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
        public static async Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default(CancellationToken), params string[] tags)
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }

            var key = QueryCacheManager.GetCacheKey(query, tags, true);

            var item = QueryCacheManager.GetDeferred(key);

            if (item == null)
            {
                using (var handler = new QueryCacheItemTracker().Initialize(query))
                {
                    item = await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                    item = QueryCacheManager.AddOrGetExistingDeferred<T>(key, item ?? DBNull.Value, absoluteExpiration) ?? item;
                    QueryCacheManager.AddCacheTag(handler, key, tags);
                    QueryCacheManager.AddCacheTag(key, typeof(T).Name + QueryCacheManager.CacheTypeSuffix);
                }
            }

            item = item.IfDbNullThenNull();

            return (T) item;
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, DateTimeOffset absoluteExpiration, params string[] tags)
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, params string[] tags)
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, CancellationToken cancellationToken = default(CancellationToken), params string[] tags)
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
        public static async Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, MemoryCacheEntryOptions options, CancellationToken cancellationToken = default(CancellationToken), params string[] tags)
        {
            if (!QueryCacheManager.IsEnabled)
            {
                return await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }

            var key = QueryCacheManager.GetCacheKey(query, tags, true);

            object item;
            if (!QueryCacheManager.Cache.TryGetValue(key, out item))
            {
                item = await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                item = QueryCacheManager.Cache.Set(key, item ?? DBNull.Value, options);
                QueryCacheManager.AddCacheTag(key, tags);
                QueryCacheManager.AddCacheTag(key, typeof(T).Name + QueryCacheManager.CacheTypeSuffix); 
            }

            item = item.IfDbNullThenNull();

            return (T) item;
        }

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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, MemoryCacheEntryOptions options, params string[] tags)
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, params string[] tags)
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
        public static Task<T> FromCacheAsync<T>(this QueryDeferred<T> query, CancellationToken cancellationToken = default(CancellationToken), params string[] tags)
        {
            return query.FromCacheAsync(QueryCacheManager.DefaultMemoryCacheEntryOptions, cancellationToken, tags);
        }
#endif
        }
}

#endif