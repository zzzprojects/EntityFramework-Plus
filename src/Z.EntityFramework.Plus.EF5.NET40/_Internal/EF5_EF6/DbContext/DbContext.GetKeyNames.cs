// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT
#if EF5 || EF6

using System.Data.Entity;
using System.Linq;
using System.Reflection;

#if EF5
using System.Data.Metadata.Edm;

#elif EF6
using System.Data.Entity.Core.Metadata.Edm;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class DbContextExtensions
    {
        public static string[] GetKeyNames<T>(this DbContext context) where T : class
        {
            var set = context.Set(typeof (T));

            var internalSetField = set.GetType().GetField("_internalSet", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalSet = internalSetField.GetValue(set);

            var entitySetField = internalSet.GetType().GetProperty("EntitySet", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var entitySet = (EntitySet) entitySetField.GetValue(internalSet, null);

            var keyNames = entitySet.ElementType.KeyMembers
                .Select(k => k.Name)
                .ToArray();

            return keyNames;
        }
    }
}

#endif
#endif