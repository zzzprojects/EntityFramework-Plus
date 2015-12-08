// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for future queries.</summary>
    public static class QueryFutureManager
    {
        /// <summary>Static constructor.</summary>
        static QueryFutureManager()
        {
            Cache = MemoryCache.Default;
            DefaultCacheItemPolicy = new CacheItemPolicy {SlidingExpiration = TimeSpan.FromMinutes(5)};
        }

        /// <summary>Gets or sets the cache.</summary>
        /// <value>The cache.</value>
        public static ObjectCache Cache { get; set; }

        /// <summary>Gets or sets the default cache item policy.</summary>
        /// <value>The default cache item policy.</value>
        public static CacheItemPolicy DefaultCacheItemPolicy { get; set; }

        /// <summary>Adds an or get batch.</summary>
        /// <param name="context">The context.</param>
        /// <returns>A FutureQueryBatch.</returns>
        public static QueryFutureBatch AddOrGetBatch(ObjectContext context)
        {
            var key = RuntimeHelpers.GetHashCode(context).ToString();
            var newQuery = new QueryFutureBatch(context);
            return (QueryFutureBatch) Cache.AddOrGetExisting(key, newQuery, DefaultCacheItemPolicy) ?? newQuery;
        }

        /// <summary>Removes the batch described by context.</summary>
        /// <param name="batch">The context.</param>
        public static void RemoveBatch(QueryFutureBatch batch)
        {
            var key = RuntimeHelpers.GetHashCode(batch.Context).ToString();
            Cache.Remove(key);
        }
    }
}