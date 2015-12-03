// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Z.EntityFramework.Plus
{
    /// <summary>Query cache manager for global configuration and methods.</summary>
    public class QueryCacheManager
    {
        /// <summary>Static constructor.</summary>
        static QueryCacheManager()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
            DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();
            CachePrefix = "Z.EntityFramework.Plus.QueryCacheManager;";
            CacheTags = new ConcurrentDictionary<string, List<string>>();
        }

        /// <summary>Gets or sets the cache to use for the QueryCacheExtensions methods.</summary>
        /// <value>The cache to use for the QueryCacheExtensions methods.</value>
        public static IMemoryCache Cache { get; set; }

        /// <summary>Gets or sets the default memory cache entry options to use when no policy is specified.</summary>
        /// <value>The default memory cache entry options to use when no policy is specified.</value>
        public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; set; }

        /// <summary>Gets or sets the cache prefix to use to create the cache key.</summary>
        /// <value>The cache prefix to use to create the cache key.</value>
        public static string CachePrefix { get; set; }

        /// <summary>Gets the dictionary cache tags used to store link between tag and cache key.</summary>
        /// <value>The cache tags used to store link between tag and cache key.</value>
        public static ConcurrentDictionary<string, List<string>> CacheTags { get; }

        /// <summary>Adds cache tag to link to a cache key in the CacheTags dictionary'.</summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="tags">A variable-length parameters list containing tag to link to the <paramref name="cacheKey" />.</param>
        public static void AddCacheTag(string cacheKey, params string[] tags)
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

        /// <summary>Expire all cache key linked to specified tags.</summary>
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

        /// <summary>Gets cache key used to cache or retrieve query from the QueryCacheManager.</summary>
        /// <param name="query">The query to cache or retrieve from the QueryCacheManager.</param>
        /// <param name="tags">A variable-length parameters list containing tag to create the cache key.</param>
        /// <returns>The cache key used to cache or retrieve query from the QueryCacheManager.</returns>
        public static string GetCacheKey(IQueryable query, string[] tags)
        {
            return CachePrefix + string.Join(";", tags) + query;
        }
    }
}