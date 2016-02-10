// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        /// <summary>
        ///     Change from "EntityModified' to "EntitySoftDeleted" for all entities which satisfy the predicate.
        /// </summary>
        /// <param name="softDeletePredicate">The soft add predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration SoftDeleted(Func<object, bool> softDeletePredicate)
        {
            SoftDeletedPredicates.Add(softDeletePredicate);
            return this;
        }

        /// <summary>
        ///     Change from "EntityModified" to "EntitySoftDeleted" for all entities of 'T' type or entities which the
        ///     type derive from 'T' and which satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type to soft delete entity.</typeparam>
        /// <param name="softDeletePredicate">The soft delete predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration SoftDeleted<T>(Func<T, bool> softDeletePredicate) where T : class
        {
            SoftDeletedPredicates.Add(o =>
            {
                var entity = o as T;
                return entity != null && softDeletePredicate(entity);
            });

            return this;
        }
    }
}