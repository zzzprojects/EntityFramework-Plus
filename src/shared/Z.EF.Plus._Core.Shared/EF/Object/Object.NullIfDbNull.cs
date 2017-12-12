// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_CACHE

using System;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>An object extension method that return null if the value is DBNull.Value.</summary>
        /// <param name="item">The item to act on.</param>
        /// <returns>Null if the value is DBNull.Value.</returns>
        public static object IfDbNullThenNull(this object item)
        {
            if (item == DBNull.Value)
            {
                item = null;
            }

            return item;
        }
    }
}
#endif