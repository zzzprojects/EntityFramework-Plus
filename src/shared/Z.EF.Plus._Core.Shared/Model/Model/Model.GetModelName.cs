// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System.Data.Entity;

namespace Z.EntityFramework.Plus.Internal
{
    internal static partial class Model
    {
        /// <summary>
        ///     A DbContext extension method that gets model name.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The model name.</returns>
        internal static string GetModelName(this DbContext @this)
        {
            // Only EntityConnection contains model
            var connectionString = @this.Database.GetEntityConnection().ConnectionString;
            var end = connectionString.IndexOf(".msl") - 1;

            if (end <= -1)
            {
                return null;
            }

            var start = connectionString.Substring(0, end).LastIndexOf("/") + 1;
            var modelName = connectionString.Substring(start, end - start + 1);
            return modelName;
        }
    }
}

#endif
#endif