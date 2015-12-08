// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    public static partial class ObjectContextExtensions
    {
        public static DbInterceptionContext GetInterceptionContext(this ObjectContext context)
        {
            var interceptionContextProperty = context.GetType().GetProperty("InterceptionContext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (DbInterceptionContext) interceptionContextProperty.GetValue(context, null);
        }
    }
}
#endif