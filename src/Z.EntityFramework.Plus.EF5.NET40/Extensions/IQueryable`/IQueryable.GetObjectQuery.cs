// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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
    public static partial class IQueryableExtensions
    {
        /// <summary>An IQueryable&lt;TEntity&gt; extension method that gets a context.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <returns>The context.</returns>
        public static ObjectQuery<TEntity> GetObjectQuery<TEntity>(this IQueryable<TEntity> query)
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