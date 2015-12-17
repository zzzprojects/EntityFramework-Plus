// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;

#if EF5
using System.Data.Objects;

#elif EF6

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
#elif EF7

            return type.GetGenericArguments()[0];
#endif
        }
    }
}