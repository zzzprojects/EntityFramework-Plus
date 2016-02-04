// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static object GetObjectQuery(this object set, Type elementType)
        {
            var dbQueryGenericType = typeof (DbQuery<>).MakeGenericType(elementType);
            var internalQueryField = dbQueryGenericType.GetField("_internalQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var internalQuery = internalQueryField.GetValue(set);

            var objectQueryContextProperty = internalQuery.GetType().GetProperty("ObjectQuery", BindingFlags.Public | BindingFlags.Instance);
            var objectQuery = objectQueryContextProperty.GetValue(internalQuery, null);

            return objectQuery;
        }
    }
}

#endif