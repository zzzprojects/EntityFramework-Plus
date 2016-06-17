// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_FILTER
using System;
using System.Reflection;

#if EF5
using System.Data.Entity;
using System.Data.Objects;
using System.Reflection;

#elif EF6
using System.Data.Entity;
using System.Reflection;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static Type GetDbSetElementType(this Type type)
        {
#if EF5 || EF6
            try
            {
                var setInterface =
                    (type.IsGenericType && typeof (IDbSet<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                        ? type
                        : type.GetInterface(typeof (IDbSet<>).FullName);


                return setInterface.GetGenericArguments()[0];
            }
            catch (AmbiguousMatchException)
            {
                // Thrown if collection type implements IDbSet or IObjectSet<> more than once
            }
            return null;
#elif EFCORE

#if NETSTANDARD1_3
            return type.GetTypeInfo().GenericTypeArguments[0];
#else
            return type.GetGenericArguments()[0];
#endif
#endif
        }
    }
}
#endif