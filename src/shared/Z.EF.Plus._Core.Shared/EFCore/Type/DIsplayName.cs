// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if FULL || QUERY_INCLUDEOPTIMIZED
#if EFCORE
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;


namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
#if EFCORE_5X
        internal static string DisplayName(this Type type, bool fullName)
        {
            var displayNameMethod = typeof(DbContext).Assembly.GetType("System.SharedTypeExtensions").GetMethod("DisplayName");

#if EFCORE_6X
            return (string)displayNameMethod.Invoke(null, new object[] { type, fullName, false });
#else
            return (string)displayNameMethod.Invoke(null, new object[] {type, fullName });
#endif
        }
#endif
        }
}
#endif
#endif