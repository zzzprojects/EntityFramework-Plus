// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_INCLUDEOPTIMIZED
#if EF5 || EF6
using System;
using System.Data.Common;

#if EF5
using System.Data.Objects;
using System.Data.Metadata.Edm;

#elif EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>An ObjectContext extension method that creates store command .</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The new store command from the context.</returns>
        public static EntitySet GetEntitySet<TEntity>(this ObjectContext context)
        {
            try
            {
                return (EntitySet)((dynamic)context).CreateObjectSet<TEntity>().EntitySet;
            }
            catch (Exception)
            {
                var type = typeof(TEntity);
                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    var method = typeof(InternalExtensions).GetMethod("GetEntitySet").MakeGenericMethod(type.BaseType);
                    return (EntitySet) method.Invoke(null, new object[] { context });
                }

                throw;
            }
        }
    }
}

#endif
#endif