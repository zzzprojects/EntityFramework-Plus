// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
#if EF5
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A query filter queryable.</summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public class QueryFilterQueryable<TEntity> : IQueryFilterQueryable
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context.</param>
        /// <param name="filterSet">Set the filter set associated with the filter queryable.</param>
        /// <param name="originalQuery">The original query.</param>
        public QueryFilterQueryable(DbContext context, QueryFilterSet filterSet, IQueryable<TEntity> originalQuery)
        {
            Context = context;
            Filters = new List<IQueryFilter>();
            FilterSet = filterSet;
            OriginalQuery = originalQuery;
        }

        /// <summary>Gets or sets the filters used by the filter queryable.</summary>
        /// <value>The filters used by the filter queryable.</value>
        public List<IQueryFilter> Filters { get; set; }

        /// <summary>Gets or sets the filter set associated with the filter queryable.</summary>
        /// <value>The filter set associated with the filter queryable.</value>
        public QueryFilterSet FilterSet { get; set; }

        /// <summary>Gets or sets the original query.</summary>
        /// <value>The original query.</value>
        public IQueryable<TEntity> OriginalQuery { get; set; }

        /// <summary>Gets or sets the context associated with the filter queryable.</summary>
        /// <value>The context associated with the filter queryable.</value>
        public DbContext Context { get; set; }

        /// <summary>Disable the specified filter for the filter queryable.</summary>
        /// <param name="filter">The filter to disable.</param>
        public void DisableFilter(IQueryFilter filter)
        {
            if (Filters.Remove(filter))
            {
                UpdateInternalQuery();
            }
        }

        /// <summary>Enables the specified filter for the filter queryable.</summary>
        /// <param name="filter">The filter to enable.</param>
        public void EnableFilter(IQueryFilter filter)
        {
            if (!Filters.Contains(filter))
            {
                Filters.Add(filter);
                UpdateInternalQuery();
            }
        }

        /// <summary>Gets original query.</summary>
        /// <returns>The original query.</returns>
        public object GetOriginalQuery()
        {
            return OriginalQuery;
        }

        /// <summary>Updates the internal query.</summary>
        public void UpdateInternalQuery()
        {
            object query = OriginalQuery;

            foreach (var filter in Filters)
            {
                query = filter.ApplyFilter<TEntity>(query);
            }

#if EF5 || EF6
            FilterSet.UpdateInternalQueryCompiled.Value(Context, (ObjectQuery)query);

#elif EF7
            // todo: Use the same code as (EF5 || EF6) once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            FilterSet.UpdateInternalQuery(Context, query);

#endif
        }
    }
}