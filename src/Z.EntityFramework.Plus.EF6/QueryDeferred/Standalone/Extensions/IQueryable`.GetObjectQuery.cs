// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


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