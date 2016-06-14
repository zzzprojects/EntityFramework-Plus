// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

#if EF5
using System.Data.EntityClient;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class to own future queries in a batch</summary>
#if QUERY_INCLUDEOPTIMIZED
    internal class QueryFutureBatch
#else
    public class QueryFutureBatch
#endif

    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context related to the query future batched.</param>
#if EF5 || EF6
        public QueryFutureBatch(ObjectContext context)
#elif EFCORE
        public QueryFutureBatch(DbContext context)
#endif
        {
            Context = context;
            Queries = new List<BaseQueryFuture>();
        }

        /// <summary>Gets or sets the context related to the query future batched.</summary>
        /// <value>The context related to the query future batched.</value>
#if EF5 || EF6
        public ObjectContext Context { get; set; }
#elif EFCORE
        public DbContext Context { get; set; }
#endif

        /// <summary>Gets or sets deferred query lists waiting to be executed.</summary>
        /// <value>The deferred queries list waiting to be executed.</value>
        public List<BaseQueryFuture> Queries { get; set; }

        /// <summary>Executes deferred query lists.</summary>
        public void ExecuteQueries()
        {
#if EF5 || EF6
            var connection = (EntityConnection)Context.Connection;
#elif EFCORE
            var connection = Context.Database.GetDbConnection();
#endif
            var command = CreateCommandCombined();

            var ownConnection = false;

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    ownConnection = true;
                }

                using (command)
                {
#if EF5
                    using (var reader = command.ExecuteReader())
                    {
                        foreach (var query in Queries)
                        {
                            query.SetResult(reader);
                            reader.NextResult();
                        }
}
#elif EF6
                    var interceptionContext = Context.GetInterceptionContext();
                    using (var reader = DbInterception.Dispatch.Command.Reader(command, new DbCommandInterceptionContext(interceptionContext)))
                    {
                        foreach (var query in Queries)
                        {
                            query.SetResult(reader);
                            reader.NextResult();
                        }
                    }
#elif EFCORE
                    using (var reader = command.ExecuteReader())
                    {
                        var createEntityDataReader = new CreateEntityDataReader(reader);
                        foreach (var query in Queries)
                        {
                            query.SetResult(createEntityDataReader);
                            reader.NextResult();
                        }
                    }
#endif
                }

                Queries.Clear();
            }
            finally
            {
                if (ownConnection)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>Creates a new command combining deferred queries.</summary>
        /// <returns>The combined command created from deferred queries.</returns>
        protected DbCommand CreateCommandCombined()
        {
            var command = Context.CreateStoreCommand();

            var sb = new StringBuilder();
            var queryCount = 1;

            foreach (var query in Queries)
            {
                // GENERATE SQL
#if EF5 || EF6
                var sql = query.Query.ToTraceString();
                var parameters = query.Query.Parameters;

                // UPDATE parameter name
                foreach (var parameter in parameters)
                {
                    var oldValue = parameter.Name;
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = newValue;
                    dbParameter.Value = parameter.Value;
                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    sql = sql.Replace("@" + oldValue, "@" + newValue);
                }
#elif EFCORE

                RelationalQueryContext queryContext;
                var queryCommand = query.CreateExecutorAndGetCommand(out queryContext);
                var sql = queryCommand.CommandText;
                var parameters = queryCommand.Parameters;

                // UPDATE parameter name
                foreach (var parameter in queryContext.ParameterValues)
                {
                    var oldValue = parameter.Key;
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = newValue;
                    dbParameter.Value = parameter.Value;
                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    sql = sql.Replace("@" + oldValue, "@" + newValue);
                }
#endif



                sb.AppendLine(string.Concat("-- EF+ Query Future: ", queryCount, " of ", Queries.Count));
                sb.AppendLine(sql);
                sb.Append(";"); // SQL Server, SQL Azure, MySQL
                sb.AppendLine();
                sb.AppendLine();

                queryCount++;
            }

            command.CommandText = sb.ToString();

            return command;
        }
    }
}