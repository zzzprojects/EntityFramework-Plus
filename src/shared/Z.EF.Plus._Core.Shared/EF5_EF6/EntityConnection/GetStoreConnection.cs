// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_CACHE
#if EF5 || EF6
using System.Data.Common;
using System.Reflection;
#if EF5
using System.Data.EntityClient;

#elif EF6
using System.Data.Entity.Core.EntityClient;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
     
        public static DbConnection GetStoreConnection(this EntityConnection entityConnection)
        {
            var storeConnectionProperty = entityConnection.GetType().GetProperty("StoreConnection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (DbConnection)storeConnectionProperty.GetValue(entityConnection, null);
        }
    }
}

#endif
#endif