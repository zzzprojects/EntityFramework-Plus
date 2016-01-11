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

#endif

namespace Z.EntityFramework.Plus
{
    public class QueryFutureBatch
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context.</param>
        public QueryFutureBatch(ObjectContext context)
        {
            Context = context;
            Queries = new List<QueryFutureBase>();
        }

        /// <summary>Gets or sets the context.</summary>
        /// <value>The context.</value>
        public ObjectContext Context { get; set; }

        /// <summary>Gets or sets the query pending.</summary>
        /// <value>The query pending.</value>
        public List<QueryFutureBase> Queries { get; set; }

        /// <summary>Executes the queries operation.</summary>
        public void ExecuteQueries()
        {
            var connection = (EntityConnection) Context.Connection;
            var command = CreateCommand();

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
#elif EF6
                    var interceptionContext = Context.GetInterceptionContext();
                    using (var reader = DbInterception.Dispatch.Command.Reader(command, new DbCommandInterceptionContext(interceptionContext)))
#endif
                    {
                        foreach (var query in Queries)
                        {
                            query.SetResult(reader);
                            reader.NextResult();
                        }
                    }
                }
            }
            finally
            {
                QueryFutureManager.RemoveBatch(this);

                if (ownConnection)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>Creates the command.</summary>
        /// <returns>The new command.</returns>
        protected DbCommand CreateCommand()
        {
            var command = Context.CreateStoreCommand();

            var sb = new StringBuilder();
            var queryCount = 1;

            foreach (var query in Queries)
            {
                // GENERATE SQL
                var sql = query.Query.ToTraceString();

                // UPDATE parameter name
                foreach (var parameter in query.Query.Parameters)
                {
                    var oldValue = parameter.Name;
                    var newValue = string.Concat("z", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = newValue;
                    dbParameter.Value = parameter.Value;
                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    sql = sql.Replace("@" + oldValue, "@" + newValue);
                }

                sb.AppendLine(string.Concat("-- Future Query (", queryCount, "/", Queries.Count, ")"));
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