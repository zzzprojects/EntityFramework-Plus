// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
#if EF5
using System.Runtime.Caching;
using System.Data.EntityClient;
using System.Data.SqlClient;

#elif EF6
using System.Data.Entity.Core.EntityClient;
using System.Runtime.Caching;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
#elif EFCORE
            Cache = new MemoryCache(new MemoryCacheOptions());
            DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions();
#endif
            CachePrefix = "Z.EntityFramework.Plus.QueryCacheManager;";
            CacheTags = new ConcurrentDictionary<string, List<string>>();
            IncludeConnectionInCacheKey = true;
        }

#if EF5 || EF6
        /// <summary>Gets or sets the cache to use for QueryCacheExtensions extension methods.</summary>
        /// <value>The cache to use for QueryCacheExtensions extension methods.</value>
        public static ObjectCache Cache { get; set; }

        /// <summary>The default cache item policy.</summary>
        private static CacheItemPolicy _defaultCacheItemPolicy;

        /// <summary>The cache item policy factory.</summary>
        private static Func<CacheItemPolicy> _cacheItemPolicyFactory;

        /// <summary>Gets or sets the default cache item policy to use when no policy is specified.</summary>
        /// <value>The default cache item policy to use when no policy is specified.</value>
        public static CacheItemPolicy DefaultCacheItemPolicy
        {
            get
            {
                if (_defaultCacheItemPolicy == null && CacheItemPolicyFactory != null)
                {
                    return CacheItemPolicyFactory();
                }

                return _defaultCacheItemPolicy;
            }
            set
            {
                _defaultCacheItemPolicy = value;
                _cacheItemPolicyFactory = null;
            }
        }

        /// <summary>Gets or sets the cache item policy factory.</summary>
        /// <value>The cache item policy factory.</value>
        public static Func<CacheItemPolicy> CacheItemPolicyFactory
        {
            get { return _cacheItemPolicyFactory; }
            set
            {
                _cacheItemPolicyFactory = value;
                _defaultCacheItemPolicy = null;
            }
        }

#elif EFCORE
    /// <summary>Gets or sets the cache to use for the QueryCacheExtensions extension methods.</summary>
    /// <value>The cache to use for the QueryCacheExtensions extension methods.</value>
        public static IMemoryCache Cache { get; set; }

        /// <summary>The default memory cache entry options.</summary>
        private static MemoryCacheEntryOptions _defaultMemoryCacheEntryOptions;

        /// <summary>The memory cache entry options factory.</summary>
        private static Func<MemoryCacheEntryOptions> _memoryCacheEntryOptionsFactory;

        /// <summary>Gets or sets the default memory cache entry options to use when no policy is specified.</summary>
        /// <value>The default memory cache entry options to use when no policy is specified.</value>
        public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions
        {
            get
            {
                if (_defaultMemoryCacheEntryOptions == null && MemoryCacheEntryOptionsFactory != null)
                {
                    return MemoryCacheEntryOptionsFactory();
                }

                return _defaultMemoryCacheEntryOptions;
            }
            set
            {
                _defaultMemoryCacheEntryOptions = value;
                _memoryCacheEntryOptionsFactory = null;
            }
        }

        /// <summary>Gets or sets the memory cache entry options factory.</summary>
        /// <value>The memory cache entry options factory.</value>
        public static Func<MemoryCacheEntryOptions> MemoryCacheEntryOptionsFactory
        {
            get { return _memoryCacheEntryOptionsFactory; }
            set
            {
                _memoryCacheEntryOptionsFactory = value;
                _defaultMemoryCacheEntryOptions = null;
            }
        }
#endif

        /// <summary>Gets or sets the cache prefix to use to create the cache key.</summary>
        /// <value>The cache prefix to use to create the cache key.</value>
        public static string CachePrefix { get; set; }

        /// <summary>Gets or sets the cache key factory.</summary>
        /// <value>The cache key factory.</value>
        public static Func<IQueryable, string[], string> CacheKeyFactory { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the connection in cache key should be included.
        /// </summary>
        /// <value>true if include connection in cache key, false if not.</value>
        public static bool IncludeConnectionInCacheKey { get; set; }

        /// <summary>Gets the dictionary cache tags used to store tags and corresponding cached keys.</summary>
        /// <value>The cache tags used to store tags and corresponding cached keys.</value>
        public static ConcurrentDictionary<string, List<string>> CacheTags { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this object use first tag as cache key.
        /// </summary>
        /// <value>true if use first tag as cache key, false if not.</value>
        public static bool UseFirstTagAsCacheKey { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this object use tag as cache key.
        /// </summary>
        /// <value>true if use tag as cache key, false if not.</value>
        public static bool UseTagsAsCacheKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is command information optional for cache
        /// key.
        /// </summary>
        /// <value>
        /// True if this object is command information optional for cache key, false if not.
        /// </value>
        public static bool IsCommandInfoOptionalForCacheKey { get; set; }

        /// <summary>Adds cache tags corresponding to a cached key in the CacheTags dictionary.</summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="tags">A variable-length parameters list containing tags corresponding to the <paramref name="cacheKey" />.</param>
        internal static void AddCacheTag(string cacheKey, params string[] tags)
        {
            foreach (var tag in tags)
            {
                CacheTags.AddOrUpdate(CachePrefix + tag, x => new List<string> {cacheKey}, (x, list) =>
                {
                    if (!list.Contains(cacheKey))
                    {
                        list.Add(cacheKey);
                    }

                    return list;
                });
            }
        }

#if !EFCORE
        /// <summary>Expire all cached objects && tag.</summary>
        public static void ExpireAll()
        {
            var list = new List<string>();

            foreach (var item in Cache)
            {
                if (item.Key.StartsWith(CachePrefix, StringComparison.InvariantCulture))
                {
                    list.Add(item.Key);
                }
            }

            foreach (var item in list)
            {
                Cache.Remove(item);
            }
        }
#endif

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
                if (CacheTags.TryRemove(CachePrefix + tag, out list))
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
        public static string GetCacheKey(IQueryable query, string[] tags)
        {
            if (CacheKeyFactory != null)
            {
                var cacheKey = CacheKeyFactory(query, tags);

                if (!string.IsNullOrEmpty(cacheKey))
                {
                    return cacheKey;
                }
            }

            var sb = new StringBuilder();

#if EF5 || EF6

            var queryCacheUniqueKeyMethod = query.GetType().GetMethod("GetQueryCacheUniqueKey", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (queryCacheUniqueKeyMethod != null)
            {
                var queryCacheUniqueKey = (string)queryCacheUniqueKeyMethod.Invoke(query, new object[] { tags });

                if (!string.IsNullOrEmpty(queryCacheUniqueKey))
                {
                    return queryCacheUniqueKey;
                }
            }

            if (IsCommandInfoOptionalForCacheKey && !UseFirstTagAsCacheKey && !UseTagsAsCacheKey)
            {
                throw new Exception(ExceptionMessage.QueryCache_IsCommandInfoOptionalForCacheKey_Invalid);
            }

            var objectQuery = IsCommandInfoOptionalForCacheKey ? query.GetObjectQuerySafe() : query.GetObjectQuery();

            sb.AppendLine(CachePrefix);

            if (IncludeConnectionInCacheKey && objectQuery != null)
            {
                sb.AppendLine(GetConnectionStringForCacheKey(query));
            }
#elif EFCORE
            RelationalQueryContext queryContext = null;
            
            var command = query.CreateCommand(out queryContext);

            sb.AppendLine(CachePrefix);

            if (IncludeConnectionInCacheKey)
            {
                sb.AppendLine(GetConnectionStringForCacheKey(queryContext));
            }
#endif

            if (UseFirstTagAsCacheKey)
            {
                if (tags == null || tags.Length == 0 || string.IsNullOrEmpty(tags[0]))
                {
                    throw new Exception(ExceptionMessage.QueryCache_FirstTagNullOrEmpty);
                }

                sb.AppendLine(tags[0]);
                return sb.ToString();
            }

            if (UseTagsAsCacheKey)
            {
                if (tags == null || tags.Length == 0 || tags.Any(string.IsNullOrEmpty))
                {
                    throw new Exception(ExceptionMessage.QueryCache_UseTagsNullOrEmpty);
                }

                sb.AppendLine(string.Join(";", tags));
                return sb.ToString();
            }

            sb.AppendLine(string.Join(";", tags));

#if EF5
            if (objectQuery != null)
            {
                sb.AppendLine(objectQuery.ToTraceString());

                foreach (var parameter in objectQuery.Parameters)
                {
                    sb.Append(parameter.Name);
                    sb.Append(";");
                    sb.Append(parameter.Value);
                    sb.AppendLine(";");
                }
            }

#elif EF6
            if (objectQuery != null)
            {
                var commandTextAndParameters = objectQuery.GetCommandTextAndParameters();
                sb.AppendLine(commandTextAndParameters.Item1);

                foreach (DbParameter parameter in commandTextAndParameters.Item2)
                {
                    sb.Append(parameter.ParameterName);
                    sb.Append(";");
                    sb.Append(parameter.Value);
                    sb.AppendLine(";");
                }
            }
#elif EFCORE

            sb.AppendLine(query.Expression.ToString());
            sb.AppendLine(command.CommandText);

            foreach (var parameter in queryContext.ParameterValues)
            {
                sb.Append(parameter.Key);
                sb.Append(";");
                sb.Append(parameter.Value);
                sb.AppendLine(";");
            }
#endif

            return sb.ToString();
        }

        public static string GetConnectionStringForCacheKey(IQueryable query)
        {
#if EF5 || EF6
            var connection = ((EntityConnection) query.GetObjectQuery().Context.Connection).GetStoreConnection();

            string connectionStringWithoutPassword = "";
            // Remove the password from the connection string
            {
                if (connection.ConnectionString != null)
                {
                    var list = new List<string>();

                    var keyValues = connection.ConnectionString.Split(';');

                    foreach (var keyValue in keyValues)
                    {
                        if (!string.IsNullOrEmpty(keyValue))
                        {
                            var key = keyValue.Split('=')[0].Trim().ToLowerInvariant();

                            if (key != "password" && key != "pwd")
                            {
                                list.Add(keyValue);
                            }
                        }
                    }

                    connectionStringWithoutPassword = string.Join(",", list);
                }
            }

            // FORCE database name in case "ChangeDatabase()" method is used
            var connectionString = string.Concat(connection.DataSource ?? "", 
                Environment.NewLine, 
                connection.Database ?? "",
                Environment.NewLine,
                connectionStringWithoutPassword ?? "");
            return connectionString;
#elif EFCORE
            RelationalQueryContext queryContext;
            var command = query.CreateCommand(out queryContext);
            return GetConnectionStringForCacheKey(queryContext);
#endif
        }

#if EFCORE
        public static string GetConnectionStringForCacheKey(RelationalQueryContext queryContext)
        {
            var connection = queryContext.Connection.DbConnection;

            string connectionStringWithoutPassword = "";
            // Remove the password from the connection string
            {
                if (connection.ConnectionString != null)
                {
                    var list = new List<string>();

                    var keyValues = connection.ConnectionString.Split(';');

                    foreach (var keyValue in keyValues)
                    {
                        if (!string.IsNullOrEmpty(keyValue))
                        {
                            var key = keyValue.Split('=')[0].Trim().ToLowerInvariant();

                            if (key != "password" && key != "pwd")
                            {
                                list.Add(keyValue);
                            }
                        }
                    }

                    connectionStringWithoutPassword = string.Join(",", list);
                }
            }

            // FORCE database name in case "ChangeDatabase()" method is used
            var connectionString = string.Concat(connection.DataSource ?? "", 
                Environment.NewLine, 
                connection.Database ?? "",
                Environment.NewLine,
                connectionStringWithoutPassword ?? "");
            return connectionString;
        }
#endif


        /// <summary>Gets cached keys used to cache or retrieve a query from the QueryCacheManager.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to cache or retrieve from the QueryCacheManager.</param>
        /// <param name="tags">A variable-length parameters list containing tags to create the cache key.</param>
        /// <returns>The cache key used to cache or retrieve a query from the QueryCacheManager.</returns>
        public static string GetCacheKey<T>(QueryDeferred<T> query, string[] tags)
        {
            return GetCacheKey(query.Query, tags);
        }
    }
}