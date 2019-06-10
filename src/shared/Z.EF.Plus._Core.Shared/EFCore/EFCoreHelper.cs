// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Sql;
namespace Z.EntityFramework.Plus
{
    internal static partial class EFCoreHelper
    {
#if EFCORE_3X
        internal static bool IsVersion3x = true;
        internal static bool IsVersion3xPreview5 = true;
#else
        internal static bool IsVersion3x = false;
        internal static bool IsVersion3xPreview5 = false;
#endif
    }
}
#endif