// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

#if EF5 
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        /// <summary>Excludes (by entity entry) from the audit all entities which satisfy the predicate.</summary>
        /// <param name="excludeEntityPredicate">The exclude entity predicate.</param>
        /// <returns>An AuditConfiguration.</returns>
#if EF5 || EF6
        public AuditConfiguration ExcludeByEntry(Func<ObjectStateEntry, bool> excludeEntityPredicate)
#elif EFCORE
        public AuditConfiguration ExcludeByEntry(Func<EntityEntry, bool> excludeEntityPredicate)
#endif

        {
            ExcludeIncludeByInstanceEntityPredicates.Add(x => excludeEntityPredicate(x) ? (bool?) false : null);
            return this;
        }
    }
}