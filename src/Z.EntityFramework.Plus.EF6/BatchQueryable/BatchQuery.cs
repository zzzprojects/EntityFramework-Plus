using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Text;

namespace Z.EntityFramework.Plus
{
    /// <summary>A batch query.</summary>
    public class BatchQuery
    {
        /// <summary>Default constructor.</summary>
        public BatchQuery()
        {
            Queries = new List<IBatchQueryable>();
        }

        /// <summary>Gets or sets the context.</summary>
        /// <value>The context.</value>
        public ObjectContext Context { get; set; }

        /// <summary>Gets or sets the queries.</summary>
        /// <value>The queries.</value>
        public List<IBatchQueryable> Queries { get; set; }

        /// <summary>Executes this object.</summary>
        public void Execute()
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
                if (ownConnection)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>Creates the command.</summary>
        /// <returns>The new command.</returns>
        public DbCommand CreateCommand()
        {
            var command = Context.CreateStoreCommand();

            var sb = new StringBuilder();
            var queryCount = 1;

            foreach (var query in Queries)
            {
                var objectQuery = query.GetObjectQuery();

                // GENERATE SQL
                var sql = objectQuery.ToTraceString();

                // UPDATE parameter name
                foreach (var parameter in objectQuery.Parameters)
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

                sb.AppendLine(string.Concat("-- Batch Queryable (", queryCount, "/", Queries.Count, ")"));
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