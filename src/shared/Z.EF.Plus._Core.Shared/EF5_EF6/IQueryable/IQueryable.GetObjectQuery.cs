// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE || QUERY_FILTER
#if EF5 || EF6
using System;
using System.Data.Entity;
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
        /// <param name="query">The query to get the ObjectQuery from.</param>
        /// <returns>The ObjectQuery from the query.</returns>
        internal static ObjectQuery GetObjectQuery(this IQueryable query)
        {
            // CHECK ObjectQuery
            var objectQuery = query as ObjectQuery;
            if (objectQuery != null)
            {
                return objectQuery;
            }

            // CHECK DbQuery
            var dbQuery = query as DbQuery;

            if (dbQuery != null)
            {
                var internalQueryProperty = dbQuery.GetType().GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = internalQueryProperty.GetValue(dbQuery, null);
                var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
                var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

                objectQuery = objectQueryContext as ObjectQuery;

                return objectQuery;
            }

            var type = query.GetType();

            if (query.GetType().IsGenericType && query.GetType().GetGenericTypeDefinition() == typeof(DbQuery<>))
            {
                var internalQueryProperty = typeof(DbQuery<>).MakeGenericType(query.ElementType).GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = internalQueryProperty.GetValue(query, null);
                var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
                var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

                objectQuery = objectQueryContext as ObjectQuery;

                return objectQuery;
            }

            if (query.GetType().IsGenericType && query.GetType().GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                var internalQueryProperty = typeof(DbSet<>).MakeGenericType(query.ElementType).GetProperty("InternalQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                var internalQuery = internalQueryProperty.GetValue(query, null);
                var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
                var objectQueryContext = objectQueryContextProperty.GetValue(internalQuery, null);

                objectQuery = objectQueryContext as ObjectQuery;

                return objectQuery;
            }

            throw new Exception(ExceptionMessage.GeneralException);
        }
    }
}

#endif
#endif