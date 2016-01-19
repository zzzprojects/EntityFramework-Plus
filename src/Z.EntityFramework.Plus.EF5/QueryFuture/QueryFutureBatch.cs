// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class to own future queries in a batch</summary>
    public class QueryFutureBatch
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context related to the query future batched.</param>
#if EF5 || EF6
        public QueryFutureBatch(ObjectContext context)
#elif EF7
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
#elif EF7
        public DbContext Context { get; set; }
#endif

        /// <summary>Gets or sets deferred query lists waiting to be executed.</summary>
        /// <value>The deferred queries list waiting to be executed.</value>
        public List<BaseQueryFuture> Queries { get; set; }

        /// <summary>Executes deferred query lists.</summary>
        public void ExecuteQueries()
        {
#if EF5 || EF6
            var connection = (EntityConnection) Context.Connection;
#elif EF7
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
#elif EF7
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
#elif EF7
                var queryCommand = query.CreateExecutorAndGetCommand();
                var sql = queryCommand.CommandText;
                var parameters = queryCommand.Parameters;
#endif

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

                sb.AppendLine(string.Concat("-- EF+ Query Future: ", queryCount, " of ", Queries.Count));
                sb.AppendLine(sql);
                sb.AppendLine();
                sb.AppendLine();

                queryCount++;
            }

            command.CommandText = sb.ToString();

            return command;
        }
    }
}