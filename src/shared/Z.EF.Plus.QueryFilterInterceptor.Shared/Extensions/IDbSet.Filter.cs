// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryInterceptorFilterExtensions
    {
        /// <summary>
        ///     Filter the query using context filters associated with specified keys.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to filter using context filters associated with specified keys.</param>
        /// <param name="keys">
        ///     A variable-length parameters list containing keys associated to context filters to use to filter the
        ///     query.
        /// </param>
        /// <returns>The query filtered using context filters associated with specified keys.</returns>
        public static IQueryable<T> Filter<T>(this IDbSet<T> query, params object[] keys) where T : class
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(query.GetDbContext());
            var filterHook = QueryFilterManager.EnableFilterById;

            var sb = new StringBuilder();

            if (keys != null)
            {
                foreach (var key in keys)
                {
                    var filter = filterContext.GetFilter(key);
                    if (filter == null)
                    {
                        continue;
                    }
                    sb.Append(filter.UniqueKey);
                    sb.Append(";");
                }
            }

            return QueryFilterManager.HookFilter(query.AsNoFilter(), filterHook + sb);
        }
    }
}