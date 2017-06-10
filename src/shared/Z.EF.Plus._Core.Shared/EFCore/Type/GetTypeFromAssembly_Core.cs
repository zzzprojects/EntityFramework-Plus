// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE || QUERY_FUTURE
#if EFCORE
using System;
using System.Reflection;


namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        internal static Type GetTypeFromAssembly_Core(this Type fromType, string name)
        {
#if NETSTANDARD1_3
            return fromType.GetTypeInfo().Assembly.GetType(name);
#else
            return fromType.Assembly.GetType(name);
#endif
        }
    }
}
#endif
#endif