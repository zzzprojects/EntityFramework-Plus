// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE || QUERY_FILTER || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>An ObjectContext extension method that gets interception context.</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The interception context from the ObjectContext.</returns>
        public static DbInterceptionContext GetInterceptionContext(this ObjectContext context)
        {
            var interceptionContextProperty = context.GetType().GetProperty("InterceptionContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (DbInterceptionContext) interceptionContextProperty.GetValue(context, null);
        }
    }
}

#endif
#endif