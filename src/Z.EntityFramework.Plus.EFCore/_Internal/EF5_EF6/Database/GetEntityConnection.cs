// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6

#if EF5
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Reflection;

#elif EF6
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Reflection;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static EntityConnection GetEntityConnection(this Database @this)
        {
            object internalContext = @this.GetType().GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(@this);

            MethodInfo getObjectContext = internalContext.GetType().GetMethod("GetObjectContextWithoutDatabaseInitialization", BindingFlags.Public | BindingFlags.Instance);
            var objectContext = (ObjectContext)getObjectContext.Invoke(internalContext, null);
            DbConnection entityConnection = objectContext.Connection;

            return (EntityConnection)entityConnection;
        }
    }
}

#endif
#endif