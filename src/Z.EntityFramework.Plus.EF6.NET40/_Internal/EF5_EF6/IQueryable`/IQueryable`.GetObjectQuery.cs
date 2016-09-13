// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_DELETE || BATCH_UPDATE || QUERY_DEFERRED || QUERY_FILTER || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF5 || EF6
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>An IQueryable&lt;TEntity&gt; extension method that get the ObjectQuery from the query.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to get the ObjectQuery from.</param>
        /// <returns>The ObjectQuery from the query.</returns>
        internal static ObjectQuery<T> GetObjectQuery<T>(this IQueryable<T> query)
        {
            // CHECK for ObjectQuery
            var objectQuery = query as ObjectQuery<T>;
            if (objectQuery != null)
            {
                return objectQuery;
            }

            // CHECK for DbQuery
            var dbQuery = query as DbQuery<T>;

            if (dbQuery == null)
            {
                var internalQueryProperty = query.GetType().GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);

                if (internalQueryProperty == null)
                {
                    throw new Exception(ExceptionMessage.GeneralException);
                }

                var internalQuery = internalQueryProperty.GetValue(query, null);
                var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
                var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

                objectQuery = objectQueryContext as ObjectQuery<T>;

                return objectQuery;
            }

            {
                var internalQueryProperty = dbQuery.GetType().GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = internalQueryProperty.GetValue(dbQuery, null);
                var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
                var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

                objectQuery = objectQueryContext as ObjectQuery<T>;

                return objectQuery;
            }

        }
    }
}

#endif
#endif