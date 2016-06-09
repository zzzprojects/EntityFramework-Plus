// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Runtime.CompilerServices;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manage EF+ Query Future Configuration.</summary>
#if QUERY_INCLUDEOPTIMIZED
    internal static class QueryFutureManager
#else
    public static class QueryFutureManager
#endif
    {
        /// <summary>Static constructor.</summary>
        static QueryFutureManager()
        {
#if EF5 || EF6
            CacheWeakFutureBatch = new ConditionalWeakTable<ObjectContext, QueryFutureBatch>();
#elif EFCORE
            CacheWeakFutureBatch = new System.Runtime.CompilerServices.ConditionalWeakTable<DbContext, QueryFutureBatch>();
#endif
        }

        /// <summary>Gets or sets the weak table used to cache future batch associated to a context.</summary>
        /// <value>The weak table used to cache future batch associated to a context.</value>
#if EF5 || EF6
        public static ConditionalWeakTable<ObjectContext, QueryFutureBatch> CacheWeakFutureBatch { get; set; }
#elif EFCORE
        public static System.Runtime.CompilerServices.ConditionalWeakTable<DbContext, QueryFutureBatch> CacheWeakFutureBatch { get; set; }
#endif

        /// <summary>Adds or gets the future batch associated to the context.</summary>
        /// <param name="context">The context used to cache the future batch.</param>
        /// <returns>The future batch associated to the context.</returns>
#if EF5 || EF6
        public static QueryFutureBatch AddOrGetBatch(ObjectContext context)
#elif EFCORE
        public static QueryFutureBatch AddOrGetBatch(DbContext context)
#endif
        {
            QueryFutureBatch futureBatch;

            if (!CacheWeakFutureBatch.TryGetValue(context, out futureBatch))
            {
                futureBatch = new QueryFutureBatch(context);
                CacheWeakFutureBatch.Add(context, futureBatch);
            }

            return futureBatch;
        }
    }
}