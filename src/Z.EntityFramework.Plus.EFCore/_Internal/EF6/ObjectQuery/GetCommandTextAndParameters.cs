// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE || QUERY_FILTER || QUERY_FUTURE || QUERY_INCLUDEOPTIMIZED
#if EF6
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static Tuple<string, DbParameterCollection> GetCommandTextAndParameters(this ObjectQuery objectQuery)
        {
            var stateField = objectQuery.GetType().BaseType.GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
            var state = stateField.GetValue(objectQuery);
            var getExecutionPlanMethod = state.GetType().GetMethod("GetExecutionPlan", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExecutionPlan = getExecutionPlanMethod.Invoke(state, new object[] {null});
            var prepareEntityCommandMethod = getExecutionPlan.GetType().GetMethod("PrepareEntityCommand", BindingFlags.NonPublic | BindingFlags.Instance);

            var sql = "";
            using (var entityCommand = (EntityCommand) prepareEntityCommandMethod.Invoke(getExecutionPlan, new object[] {objectQuery.Context, objectQuery.Parameters}))
            {
                var getCommandDefinitionMethod = entityCommand.GetType().GetMethod("GetCommandDefinition", BindingFlags.NonPublic | BindingFlags.Instance);
                var getCommandDefinition = getCommandDefinitionMethod.Invoke(entityCommand, new object[0]);

                var prepareEntityCommandBeforeExecutionMethod = getCommandDefinition.GetType().GetMethod("PrepareEntityCommandBeforeExecution", BindingFlags.NonPublic | BindingFlags.Instance);
                var prepareEntityCommandBeforeExecution = (DbCommand) prepareEntityCommandBeforeExecutionMethod.Invoke(getCommandDefinition, new object[] {entityCommand});

                var commandDispatcherField = DbInterception.Dispatch.Command.GetType().GetField("_internalDispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
                var commandDispatcher = commandDispatcherField.GetValue(DbInterception.Dispatch.Command);

                var interceptorsField = commandDispatcher.GetType().GetField("_interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
                var interceptors = (List<IDbCommandInterceptor>) interceptorsField.GetValue(commandDispatcher);

                interceptors.ForEach(i => i.ReaderExecuting(prepareEntityCommandBeforeExecution, new DbCommandInterceptionContext<DbDataReader>(objectQuery.Context.GetInterceptionContext())));

                sql = prepareEntityCommandBeforeExecution.CommandText;
                var parameters = prepareEntityCommandBeforeExecution.Parameters;

                return new Tuple<string, DbParameterCollection>(sql, parameters);
            }
        }
    }
}

#endif
#endif