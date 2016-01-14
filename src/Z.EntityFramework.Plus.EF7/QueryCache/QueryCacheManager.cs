// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if EF5 || EF6
using System.Runtime.Caching;

#elif EF7
using Microsoft.Extensions.Caching.Memory;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manage EF+ Query Cache Configuration.</summary>
    public static class QueryCacheManager
    {
        /// <summary>Static constructor.</summary>
        static QueryCacheManager()
        {
#if EF5 || EF6
            Cache = MemoryCache.Default;
            DefaultCacheItemPolicy = new CacheItemPolicy();
#elif EF7
            Cache = new MemoryCache(new MemoryCacheOptions());
            DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();
#endif
            CachePrefix = "Z.EntityFramework.Plus.QueryCacheManager;";
            CacheTags = new ConcurrentDictionary<string, List<string>>();
        }

#if EF5 || EF6
    /// <summary>Gets or sets the cache to use for QueryCacheExtensions extension methods.</summary>
    /// <value>The cache to use for QueryCacheExtensions extension methods.</value>
        public static ObjectCache Cache { get; set; }

        /// <summary>Gets or sets the default cache item policy to use when no policy is specified.</summary>
        /// <value>The default cache item policy to use when no policy is specified.</value>
        public static CacheItemPolicy DefaultCacheItemPolicy { get; set; }
#elif EF7
        /// <summary>Gets or sets the cache to use for the QueryCacheExtensions extension methods.</summary>
        /// <value>The cache to use for the QueryCacheExtensions extension methods.</value>
        public static IMemoryCache Cache { get; set; }

        /// <summary>Gets or sets the default memory cache entry options to use when no policy is specified.</summary>
        /// <value>The default memory cache entry options to use when no policy is specified.</value>
        public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; set; }
#endif

        /// <summary>Gets or sets the cache prefix to use to create the cache key.</summary>
        /// <value>The cache prefix to use to create the cache key.</value>
        public static string CachePrefix { get; set; }

        /// <summary>Gets the dictionary cache tags used to store tags and corresponding cached keys.</summary>
        /// <value>The cache tags used to store tags and corresponding cached keys.</value>
        public static ConcurrentDictionary<string, List<string>> CacheTags { get; }

        /// <summary>Adds cache tags corresponding to a cached key in the CacheTags dictionary.</summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="tags">A variable-length parameters list containing tags corresponding to the <paramref name="cacheKey" />.</param>
        internal static void AddCacheTag(string cacheKey, params string[] tags)
        {
            foreach (var tag in tags)
            {
                CacheTags.AddOrUpdate(tag, x => new List<string> {cacheKey}, (x, list) =>
                {
                    if (!list.Contains(x))
                    {
                        list.Add(x);
                    }

                    return list;
                });
            }
        }

        /// <summary>Expire all cached keys linked to specified tags.</summary>
        /// <param name="tags">
        ///     A variable-length parameters list containing tag to expire linked cache
        ///     key.
        /// </param>
        public static void ExpireTag(params string[] tags)
        {
            foreach (var tag in tags)
            {
                List<string> list;
                if (CacheTags.TryRemove(tag, out list))
                {
                    foreach (var item in list)
                    {
                        Cache.Remove(item);
                    }
                }
            }
        }

        /// <summary>Gets cached keys used to cache or retrieve a query from the QueryCacheManager.</summary>
        /// <param name="query">The query to cache or retrieve from the QueryCacheManager.</param>
        /// <param name="tags">A variable-length parameters list containing tags to create the cache key.</param>
        /// <returns>The cache key used to cache or retrieve a query from the QueryCacheManager.</returns>
        internal static string GetCacheKey(IQueryable query, string[] tags)
        {
            return CachePrefix + string.Join(";", tags) + query;
        }

#if EF5 || EF6
    /// <summary>Gets cached keys used to cache or retrieve a query from the QueryCacheManager.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="query">The query to cache or retrieve from the QueryCacheManager.</param>
    /// <param name="tags">A variable-length parameters list containing tags to create the cache key.</param>
    /// <returns>The cache key used to cache or retrieve a query from the QueryCacheManager.</returns>
        internal static string GetCacheKey<T>(QueryDeferred<T> query, string[] tags)
        {
            return CachePrefix + string.Join(";", tags) + query.Query.Expression;
        }
#endif
    }
}