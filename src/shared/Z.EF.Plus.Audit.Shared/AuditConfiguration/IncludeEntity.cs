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
        /// <summary>Includes from the audit all entities which satisfy the predicate.</summary>
        /// <param name="includeEntityPredicate">The include entity predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration Include(Func<object, bool> includeEntityPredicate)
        {
            ExcludeIncludeEntityPredicates.Add(x => includeEntityPredicate(x) ? (bool?) true : null);
            return this;
        }

        /// <summary>Includes from the audit all entities of 'T' type or entities which the type derive from 'T'.</summary>
        /// <typeparam name="T">Generic type to include.</typeparam>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration Include<T>()
        {
            ExcludeIncludeEntityPredicates.Add(x => x is T ? (bool?) true : null);
            return this;
        }
    }
}