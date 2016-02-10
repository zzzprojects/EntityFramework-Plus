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
        ///     Change from "EntityModified" to "EntitySoftAdded" for all entities which satisfy the predicate.
        /// </summary>
        /// <param name="softAddPredicate">The soft add predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration SoftAdded(Func<object, bool> softAddPredicate)
        {
            SoftAddedPredicates.Add(softAddPredicate);
            return this;
        }

        /// <summary>
        ///     Change from "EntityModified" to "EntitySoftAdded" for all entities of 'T' type or entities which the
        ///     type derive from 'T' and which satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type to soft add entity.</typeparam>
        /// <param name="softAddPredicate">The soft add predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration SoftAdded<T>(Func<T, bool> softAddPredicate) where T : class
        {
            SoftAddedPredicates.Add(o =>
            {
                var entity = o as T;
                return entity != null && softAddPredicate(entity);
            });

            return this;
        }
    }
}