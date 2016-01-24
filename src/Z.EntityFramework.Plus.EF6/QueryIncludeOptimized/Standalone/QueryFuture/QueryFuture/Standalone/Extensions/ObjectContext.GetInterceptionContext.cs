// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.



#if STANDALONE && EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class QueryFutureExtensions
    {
        /// <summary>An ObjectContext extension method that gets interception context.</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The interception context from the ObjectContext.</returns>
        internal static DbInterceptionContext GetInterceptionContext(this ObjectContext context)
        {
            var interceptionContextProperty = context.GetType().GetProperty("InterceptionContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (DbInterceptionContext) interceptionContextProperty.GetValue(context, null);
        }
    }
}

#endif