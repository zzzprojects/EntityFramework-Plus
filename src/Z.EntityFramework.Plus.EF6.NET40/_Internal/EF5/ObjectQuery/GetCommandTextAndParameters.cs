// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_UPDATE || BATCH_DELETE
#if EF5
using System;
using System.Data.Objects;


namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static Tuple<string, ObjectParameterCollection> GetCommandTextAndParameters(this ObjectQuery objectQuery)
        {
            var sql = objectQuery.ToTraceString();
            var parameters = objectQuery.Parameters;

            return new Tuple<string, ObjectParameterCollection>(sql, parameters);
        }
    }
}

#endif
#endif