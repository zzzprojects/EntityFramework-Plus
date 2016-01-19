// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Common;
#if EF5
using System.Data.EntityClient;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class ObjectContextExtensions
    {
        /// <summary>An ObjectContext extension method that creates store command .</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The new store command from the context.</returns>
        public static DbCommand CreateStoreCommand(this ObjectContext context)
        {
            var entityConnection = (EntityConnection) context.Connection;
            var command = entityConnection.StoreConnection.CreateCommand();
            command.Transaction = entityConnection.GetStoreTransaction();

            if (context.CommandTimeout.HasValue)
            {
                command.CommandTimeout = context.CommandTimeout.Value;
            }

            return command;
        }
    }
}