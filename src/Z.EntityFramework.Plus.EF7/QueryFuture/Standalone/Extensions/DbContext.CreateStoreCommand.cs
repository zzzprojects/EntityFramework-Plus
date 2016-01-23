// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.



#if STANDALONE && EF7
using System.Data.Common;
using Microsoft.Data.Entity;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFutureExtensions
    {
        /// <summary>An ObjectContext extension method that creates store command .</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The new store command from the context.</returns>
        internal static DbCommand CreateStoreCommand(this DbContext context)
        {
            var entityConnection = context.Database.GetDbConnection();
            var command = entityConnection.CreateCommand();
            //command.Transaction = entityConnection.GetStoreTransaction();

            var commandTimeout = context.Database.GetCommandTimeout();
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        }
    }
}
#endif