// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


using System;
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        public static IQueryable<T> SetFiltered<T>(this DbContext context) where T : class
        {
            var filterContext = QueryFilterManager.AddOrGetFilterContext(context);

            if (filterContext.FilterSetByType.ContainsKey(typeof(T)))
            {
                var set = filterContext.FilterSetByType[typeof(T)];

                if (set.Count == 1)
                {
                    return (IQueryable<T>)set[0].DbSetProperty.GetValue(context, null);
                }
                throw new Exception(ExceptionMessage.QueryFilter_SetFilteredManyFound);
            }

            throw new Exception(ExceptionMessage.QueryFilter_SetFilteredNotFound);
        }
    }
}

