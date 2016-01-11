// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;

namespace Z.EntityFramework.Plus
{
    /// <summary>Interface for query filter.</summary>
    public interface IQueryFilter
    {
        /// <summary>Gets or sets the type of the filter element.</summary>
        /// <value>The type of the filter element.</value>
        Type ElementType { get; set; }

        /// <summary>Returns a new query where the entities are filtered by using the filter.</summary>
        /// <param name="query">The query to filter.</param>
        /// <returns>The new query where the entities are filtered by using the filter.</returns>
        object ApplyFilter<TEntity>(object query);

        /// <summary>Disables this filter.</summary>
        void Disable();

        /// <summary>Disables this filter on the speficied type.</summary>
        /// <typeparam name="TEntity">Type of the entity to disable the filter.</typeparam>
        void Disable<TEntity>();

        /// <summary>Disable this filter on the specified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to disable the filter on.</param>
        void Disable(params Type[] types);

        /// <summary>Enables this filter.</summary>
        void Enable();

        /// <summary>Enables this filter on the specified type.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        void Enable<TEntity>();

        /// <summary>Enables this filter on the spcified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to enable the filter on.</param>
        void Enable(params Type[] types);

        /// <summary>Gets the filter predicate.</summary>
        /// <returns>The filter predicate.</returns>
        object GetPredicate();
    }
}