// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !EF6
using System.Collections.Generic;
using System.Linq;
#if EF5
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter queryable.</summary>
    /// <typeparam name="T">The type of elements of the filter queryable.</typeparam>
    public class QueryFilterQueryable<T> : BaseQueryFilterQueryable
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context associated to the filter queryable.</param>
        /// <param name="filterSet">The filter set associated with the filter queryable.</param>
        /// <param name="originalQuery">The original query.</param>
        public QueryFilterQueryable(DbContext context, QueryFilterSet filterSet, IQueryable<T> originalQuery)
        {
            Context = context;
            Filters = new List<BaseQueryFilter>();
            FilterSet = filterSet;
            OriginalQuery = originalQuery;
        }

        /// <summary>Updates the internal query.</summary>
        public override void UpdateInternalQuery()
        {
            var query = OriginalQuery;

            foreach (var filter in Filters)
            {
                query = filter.ApplyFilter<T>(query);
            }

#if EF5 || EF6
            FilterSet.UpdateInternalQueryCompiled.Value(Context, (ObjectQuery)query);

#elif EFCORE
            // todo: Use the same code as (EF5 || EF6) once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            FilterSet.UpdateInternalQuery(Context, query);
            //FilterSet.UpdateInternalQuery(Context, query);

#endif
        }
    }
}
#endif