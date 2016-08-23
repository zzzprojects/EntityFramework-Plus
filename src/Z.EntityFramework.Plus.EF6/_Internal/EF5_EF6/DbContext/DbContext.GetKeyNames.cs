// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT
#if EF5 || EF6

#if EF5
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class DbContextExtensions
    {
        public static string[] GetKeyNames<T>(this DbContext context) where T : class
        {
            var objectSet = ((IObjectContextAdapter) context).ObjectContext.CreateObjectSet<T>();
            var keyNames = objectSet.EntitySet.ElementType.KeyMembers
                .Select(k => k.Name)
                .ToArray();
            return keyNames;
        }
    }
}

#endif
#endif