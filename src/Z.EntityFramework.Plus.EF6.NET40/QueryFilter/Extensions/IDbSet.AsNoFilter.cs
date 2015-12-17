// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
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
        /// <summary>Return the orginal query before the filter was applied.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <returns>The orginal query before the filter was applied.</returns>
#if EF5 || EF6
        public static IQueryable<TEntity> AsNoFilter<TEntity>(this IDbSet<TEntity> query) where TEntity : class
#elif EF7
        public static IQueryable<TEntity> AsNoFilter<TEntity>(this DbSet<TEntity> query) where TEntity : class
#endif
        {
            var queryFilterQueryable = QueryFilterManager.GetFilterQueryable(query);
            return queryFilterQueryable != null ? (IQueryable<TEntity>) queryFilterQueryable.GetOriginalQuery() : query;
        }
    }
}