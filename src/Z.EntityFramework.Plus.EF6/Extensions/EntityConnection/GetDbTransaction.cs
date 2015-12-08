// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: http://zzzprojects.uservoice.com/forums/283924-entity-framework-plus
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Common;
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