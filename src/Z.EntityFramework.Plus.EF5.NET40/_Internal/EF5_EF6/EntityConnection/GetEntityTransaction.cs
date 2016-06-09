// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF5 || EF6
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
        /// <summary>An EntityConnection extension method that gets the entity transaction.</summary>
        /// <param name="entityConnection">The entity connection to act on.</param>
        /// <returns>The entity transaction from the entity connection.</returns>
        public static EntityTransaction GetEntityTransaction(this EntityConnection entityConnection)
        {
            var entityTransaction = entityConnection.GetType().GetField("_currentTransaction", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entityConnection);

            return (EntityTransaction) entityTransaction;
        }
    }
}

#endif
#endif