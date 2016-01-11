// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>A query filter.</summary>
    /// <typeparam name="T">Generic type to filter.</typeparam>
    public class QueryFilter<T> : IQueryFilter
    {
        /// <summary>Create a new QueryFilter.</summary>
        /// <param name="ownerFilterContext">The context that owns his filter.</param>
        /// <param name="predicate">The filter delegate.</param>
        public QueryFilter(QueryFilterContext ownerFilterContext, Func<IQueryable<T>, IQueryable<T>> predicate)
        {
            ElementType = typeof (T);
            OwnerFilterContext = ownerFilterContext;
            Predicate = predicate;
        }

        /// <summary>Gets or sets the filter context that owns this filter.</summary>
        /// <value>The owner filter context.</value>
        public QueryFilterContext OwnerFilterContext { get; set; }

        /// <summary>Gets or sets the filter predicate.</summary>
        /// <value>The filter predicate.</value>
        public Func<IQueryable<T>, IQueryable<T>> Predicate { get; set; }

        /// <summary>Gets or sets the type of the filter element.</summary>
        /// <value>The type of the filter element.</value>
        public Type ElementType { get; set; }

        /// <summary>Returns a new query where the entities are filtered by using the filter.</summary>
        /// <param name="query">The query to filter.</param>
        /// <returns>The new query where the entities are filtered by using the filter.</returns>
        public object ApplyFilter<TEntity>(object query)
        {
#if EF5 || EF6
            return Predicate((IQueryable<T>) query).Cast<TEntity>();
#elif EF7
    // todo: Use the same code as (EF5 || EF6) once EF team fix the cast issue: https://github.com/aspnet/EntityFramework/issues/3736
            return Predicate((IQueryable<T>) query);
#endif
        }

        /// <summary>Disables this filter.</summary>
        public void Disable()
        {
            Disable(null);
        }

        /// <summary>Disables this filter on the speficied type.</summary>
        /// <typeparam name="TEntity">Type of the entity to disable the filter.</typeparam>
        public void Disable<TEntity>()
        {
            Disable(typeof (TEntity));
        }

        /// <summary>Disable this filter on the specified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to disable the filter on.</param>
        public void Disable(params Type[] types)
        {
            OwnerFilterContext.DisableFilter(this, types);
        }

        /// <summary>Enables this filter.</summary>
        public void Enable()
        {
            Enable(null);
        }

        /// <summary>Enables this filter on the specified type.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        public void Enable<TEntity>()
        {
            Enable(typeof (TEntity));
        }

        /// <summary>Enables this filter on the spcified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to enable the filter on.</param>
        public void Enable(params Type[] types)
        {
            OwnerFilterContext.EnableFilter(this, types);
        }

        /// <summary>Gets the filter predicate.</summary>
        /// <returns>The filter predicate.</returns>
        public object GetPredicate()
        {
            return Predicate;
        }
    }
}