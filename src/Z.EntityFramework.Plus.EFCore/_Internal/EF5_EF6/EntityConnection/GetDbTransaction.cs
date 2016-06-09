// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL
#if EF5 || EF6
using System.Data.Common;
using System.Reflection;
#if EF5
using System.Data.EntityClient;

#elif EF6
using System.Data.Entity.Core.EntityClient;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>An EntityConnection extension method that gets database transaction.</summary>
        /// <param name="entityConnection">The @this to act on.</param>
        /// <returns>The database transaction.</returns>
        public static DbTransaction GetDbTransaction(this EntityConnection entityConnection)
        {
            object entityTransaction = entityConnection.GetEntityTransaction();

            if (entityTransaction == null)
                return null;

            var transaction = entityTransaction.GetType().GetField("_storeTransaction", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entityTransaction);
            return (DbTransaction) transaction;
        }
    }
}

#endif
#endif