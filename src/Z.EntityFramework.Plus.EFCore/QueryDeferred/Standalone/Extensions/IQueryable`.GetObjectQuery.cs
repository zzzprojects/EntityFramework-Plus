// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if STANDALONE && (EF5 || EF6)

using System.Linq;
using System.Reflection;
#if EF5
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        private static ObjectQuery<TEntity> GetObjectQuery<TEntity>(this IQueryable<TEntity> query)
        {
            // CHECK ObjectQuery
            var objectQuery = query as ObjectQuery<TEntity>;
            if (objectQuery != null)
            {
                return objectQuery;
            }

            // CHECK DbQuery
            var dbQuery = query as DbQuery<TEntity>;

            if (dbQuery == null)
            {
                return null;
            }

            var internalQueryProperty = dbQuery.GetType().GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQuery = internalQueryProperty.GetValue(dbQuery, null);
            var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
            var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

            objectQuery = objectQueryContext as ObjectQuery<TEntity>;

            return objectQuery;
        }
    }
}

#endif