// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        public AuditConfiguration FormatType<T>(Func<T, object> formatter)
        {
#if !NETSTANDARD1_3
            Func<object, object> func = o =>
            {
                T obj;
                try
                {
                    obj = (T)o;
                }
                catch (Exception e)
                {
                    // Return initial object in case the cast doesn't work (such as null object)
                    return o;
                }

                return formatter(obj);

            };
#else
            Func<object, object> func = o => formatter((T) o);
#endif
            EntityValueFormatters.Add((x, s, v) => v != null && (v.GetType() == typeof(T) || 
            (
                typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>) && v.GetType() == typeof(T).GetGenericArguments()[0]
            )) ? func : null);

            return this;
        }
    }
}