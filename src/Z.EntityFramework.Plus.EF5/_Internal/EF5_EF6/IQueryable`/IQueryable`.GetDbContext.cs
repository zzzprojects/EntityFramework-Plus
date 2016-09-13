// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Data.Entity;
using System.Linq;

#if EF5
using System.Reflection;

#elif EF6

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        internal static DbContext GetDbContext<T>(this IQueryable<T> query)
        {
#if EF5
            var provider = query.Provider;
            var internalContextProperty = provider.GetType().GetProperty("InternalContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalContext = internalContextProperty.GetValue(provider, null);

            var ownerProperty = internalContext.GetType().GetProperty("Owner", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var owner = ownerProperty.GetValue(internalContext, null);
            return (DbContext) owner;
#elif EF6
            return query.GetObjectQuery().Context.GetDbContext();
#endif
        }
    }
}

#endif
#endif