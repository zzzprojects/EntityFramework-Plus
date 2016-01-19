// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE
using System;
using System.Reflection;
#if EF5
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>A Type extension method that gets database set element type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The database set element type.</returns>
        internal static Type GetDbSetElementType(this Type type)
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
#elif EF7
            return type.GetGenericArguments()[0];
#endif
        }
    }
}

#endif