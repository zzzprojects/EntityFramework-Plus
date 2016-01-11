// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>
        ///     Returns a new query where the entities are filtered by using filter from specified keys.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="keys">A variable-length parameters list containing keys associated to filter to use.</param>
        /// <returns>The new query where the entities are filtered by using filter from specified keys.</returns>
#if EF5 || EF6
        public static IQueryable<TEntity> Filter<TEntity>(this IDbSet<TEntity> query, params object[] keys) where TEntity : class
#elif EF7
        public static IQueryable<TEntity> Filter<TEntity>(this DbSet<TEntity> query, params object[] keys) where TEntity : class
#endif
        {
            var queryFilterQueryable = QueryFilterManager.GetFilterQueryable(query);
            var context = queryFilterQueryable != null ? queryFilterQueryable.Context : query.GetDbContext();
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);
            return filterContext.ApplyFilter(query, keys);
        }
    }
}