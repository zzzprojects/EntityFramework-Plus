// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

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

#if EF6
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryDbSetFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryDbSetFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryDbSetFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryDbSetFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryDbSetFilterSet;
#else
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryFilterSet;
#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter queryable.</summary>
    /// <typeparam name="T">The type of elements of the filter queryable.</typeparam>
#if EF6
    public class QueryDbSetFilterQueryable<T> : BaseQueryDbSetFilterQueryable
#else
    public class QueryFilterQueryable<T> : BaseQueryFilterQueryable
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context associated to the filter queryable.</param>
        /// <param name="filterSet">The filter set associated with the filter queryable.</param>
        /// <param name="originalQuery">The original query.</param>
#if EF6
        public QueryDbSetFilterQueryable(DbContext context, AliasQueryFilterSet filterSet, IQueryable<T> originalQuery)
#else
        public QueryFilterQueryable(DbContext context, AliasQueryFilterSet filterSet, IQueryable<T> originalQuery)
#endif
        {
            Context = context;
            Filters = new List<AliasBaseQueryFilter>();
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