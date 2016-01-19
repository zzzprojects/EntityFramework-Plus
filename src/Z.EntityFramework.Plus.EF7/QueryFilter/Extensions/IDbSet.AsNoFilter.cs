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
        /// <summary>Return the orginal query before the context was filtered.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The filtered query from which the original query should be retrieved.</param>
        /// <returns>The orginal query before the context was filtered.</returns>
#if EF5 || EF6
        public static IQueryable<T> AsNoFilter<T>(this IDbSet<T> query) where T : class
#elif EF7
        public static IQueryable<T> AsNoFilter<T>(this DbSet<T> query) where T : class
#endif
        {
            var queryFilterQueryable = QueryFilterManager.GetFilterQueryable(query);
            return queryFilterQueryable != null ? (IQueryable<T>) queryFilterQueryable.OriginalQuery : query;
        }
    }
}