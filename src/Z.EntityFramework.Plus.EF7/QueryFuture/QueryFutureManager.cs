// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Runtime.CompilerServices;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Manage EF+ Query Future Configuration.</summary>
    public static class QueryFutureManager
    {
        /// <summary>Static constructor.</summary>
        static QueryFutureManager()
        {
#if EF5 || EF6
            CacheWeakFutureBatch = new ConditionalWeakTable<ObjectContext, QueryFutureBatch>();
#elif EF7
            CacheWeakFutureBatch = new ConditionalWeakTable<DbContext, QueryFutureBatch>();
#endif
        }

        /// <summary>Gets or sets the weak table used to cache future batch associated to a context.</summary>
        /// <value>The weak table used to cache future batch associated to a context.</value>
#if EF5 || EF6
        public static ConditionalWeakTable<ObjectContext, QueryFutureBatch> CacheWeakFutureBatch { get; set; }
#elif EF7
        public static ConditionalWeakTable<DbContext, QueryFutureBatch> CacheWeakFutureBatch { get; set; }
#endif

        /// <summary>Adds or gets the future batch associated to the context.</summary>
        /// <param name="context">The context used to cache the future batch.</param>
        /// <returns>The future batch associated to the context.</returns>
#if EF5 || EF6
        public static QueryFutureBatch AddOrGetBatch(ObjectContext context)
#elif EF7
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