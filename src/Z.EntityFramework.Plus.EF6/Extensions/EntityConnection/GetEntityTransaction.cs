// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Reflection;
#if EF5
using System.Data.EntityClient;

#elif EF6
using System.Data.Entity.Core.EntityClient;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class EntityConnectionExtensions
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