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
        /// <summary>Excludes (by entity instance) from the audit all entities which satisfy the predicate.</summary>
        /// <param name="excludeEntityPredicate">The exclude entity predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration ExcludeByInstance(Func<object, bool> excludeEntityPredicate)
        {
            ExcludeIncludeByInstanceEntityPredicates.Add(x => excludeEntityPredicate(x.Entity) ? (bool?)false : null);
            return this;
        }
    }
}