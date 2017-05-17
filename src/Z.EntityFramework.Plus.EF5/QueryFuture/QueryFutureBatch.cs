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
using System.Linq.Expressions;
using System.Reflection;
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

        public bool IsInMemory { get; set; }
#endif

        /// <summary>Gets or sets deferred query lists waiting to be executed.</summary>
        /// <value>The deferred queries list waiting to be executed.</value>
        public List<BaseQueryFuture> Queries { get; set; }

        /// <summary>Executes deferred query lists.</summary>
        public void ExecuteQueries()
        {
            if (Queries.Count == 0)
            {
                // Already all executed
                return;
            }

#if EFCORE
            if (IsInMemory)
            {
                foreach (var query in Queries)
                {
                    query.ExecuteInMemory();
                }
                Queries.Clear();
                return;
            }
#endif

            if (Queries.Count == 1)
            {
                Queries[0].GetResultDirectly();
                Queries.Clear();
                return;
            }

#if EF5 || EF6
            var connection = (EntityConnection)Context.Connection;
#elif EFCORE
            if (IsInMemory)
            {
                foreach (var query in Queries)
                {
                    query.ExecuteInMemory();
                }
                return;
            }

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

            var isOracle = command.GetType().FullName.Contains("Oracle.DataAccess");
            var isOracleManaged = command.GetType().FullName.Contains("Oracle.ManagedDataAccess");
            var isOracleDevArt = command.GetType().FullName.Contains("Devart");

            foreach (var query in Queries)
            {
                // GENERATE SQL
#if EF5
                var sql = query.Query.ToTraceString();
                var parameters = query.Query.Parameters;

                // UPDATE parameter name
                foreach (var parameter in parameters)
                {
                    var oldValue = parameter.Name;
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.CopyFrom(parameter, newValue);

                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    sql = sql.Replace("@" + oldValue, "@" + newValue);
                }
#elif EF6
                var commandTextAndParameter = query.Query.GetCommandTextAndParameters();


                var sql = commandTextAndParameter.Item1;
                var parameters = commandTextAndParameter.Item2;

                // UPDATE parameter name
                foreach (DbParameter parameter in parameters)
                {
                    var oldValue = parameter.ParameterName;
                    if (oldValue.StartsWith("@"))
                    {
                        oldValue = oldValue.Substring(1);
                    }
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.CopyFrom(parameter, newValue);

                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    if (isOracle || isOracleManaged || isOracleDevArt)
                    {
                        sql = sql.Replace(":" + oldValue, ":" + newValue);
                    }
                    else
                    {
                        sql = sql.Replace("@" + oldValue, "@" + newValue);
                    }
                }
#elif EFCORE

                RelationalQueryContext queryContext;
                var queryCommand = query.CreateExecutorAndGetCommand(out queryContext);
                var sql = queryCommand.CommandText;
                var parameters = queryCommand.Parameters;

                // UPDATE parameter name
                foreach (var relationalParameter in queryCommand.Parameters)
                {
                    var parameter = queryContext.ParameterValues[relationalParameter.InvariantName];

                    var oldValue = relationalParameter.InvariantName;
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.CopyFrom(relationalParameter, parameter, newValue);

                    command.Parameters.Add(dbParameter);

                    // REPLACE parameter with new value
                    if (isOracle || isOracleManaged || isOracleDevArt)
                    {
                        sql = sql.Replace(":" + oldValue, ":" + newValue);
                    }
                    else
                    {
                        sql = sql.Replace("@" + oldValue, "@" + newValue);
                    }
                }
#endif



                sb.AppendLine(string.Concat("-- EF+ Query Future: ", queryCount, " of ", Queries.Count));

                if (isOracle || isOracleManaged || isOracleDevArt)
                {
                    var parameterName = "zzz_cursor_" + queryCount;
                    sb.AppendLine("open :" + parameterName + " for " + sql);
                    var param = command.CreateParameter();
                    param.ParameterName = parameterName;
                    param.Direction = ParameterDirection.Output;
                    param.Value = DBNull.Value;

                    if (isOracle)
                    {
#if NETSTANDARD1_3
                        SetOracleDbType(command.GetType().GetTypeInfo().Assembly, param, 121);
#else
                        SetOracleDbType(command.GetType().Assembly, param, 121);
#endif
                    }
                    else if (isOracleManaged)
                    {
#if NETSTANDARD1_3
                        SetOracleManagedDbType(command.GetType().GetTypeInfo().Assembly, param, 121);
#else
                        SetOracleManagedDbType(command.GetType().Assembly, param, 121);
#endif
                    }
                    else if (isOracleDevArt)
                    {
#if NETSTANDARD1_3
                        SetOracleDevArtDbType(command.GetType().GetTypeInfo().Assembly, param, 7);
#else
                        SetOracleDevArtDbType(command.GetType().Assembly, param, 7);
#endif
                    }


                    command.Parameters.Add(param);
                }
                else
                {
                    sb.AppendLine(sql);
                }


                sb.Append(";"); // SQL Server, SQL Azure, MySQL
                sb.AppendLine();
                sb.AppendLine();

                queryCount++;
            }

            command.CommandText = sb.ToString();

            if (isOracle || isOracleManaged || isOracleDevArt)
            {
                var bindByNameProperty = command.GetType().GetProperty("BindByName") ?? command.GetType().GetProperty("PassParametersByName");
                bindByNameProperty.SetValue(command, true, null);

                command.CommandText = "BEGIN" + Environment.NewLine + command.CommandText + Environment.NewLine + "END;";
            }

            return command;
        }

        private static Action<DbParameter, object> _SetOracleDbType;
        private static Action<DbParameter, object> _SetOracleManagedDbType;
        private static Action<DbParameter, object> _SetOracleDevArtDbType;

        public static void SetOracleManagedDbType(Assembly assembly, DbParameter dbParameter, object type)
        {
            if (_SetOracleManagedDbType == null)
            {
                var dbtype = assembly.GetType("Oracle.ManagedDataAccess.Client.OracleDbType");
                var dbParameterType = assembly.GetType("Oracle.ManagedDataAccess.Client.OracleParameter");
                var propertyInfo = dbParameter.GetType().GetProperty("OracleDbType");

                var parameter = Expression.Parameter(typeof(DbParameter));
                var parameterConvert = Expression.Convert(parameter, dbParameterType);
                var parameterValue = Expression.Parameter(typeof(object));
                var parameterValueConvert = Expression.Convert(parameterValue, dbtype);

                var property = Expression.Property(parameterConvert, propertyInfo);
                var expression = Expression.Assign(property, parameterValueConvert);

                _SetOracleManagedDbType = Expression.Lambda<Action<DbParameter, object>>(expression, parameter, parameterValue).Compile();
            }

            _SetOracleManagedDbType(dbParameter, type);
        }

        public static void SetOracleDbType(Assembly assembly, DbParameter dbParameter, object type)
        {
            if (_SetOracleDbType == null)
            {
                var dbtype = assembly.GetType("Oracle.DataAccess.Client.OracleDbType");
                var dbParameterType = assembly.GetType("Oracle.DataAccess.Client.OracleParameter");
                var propertyInfo = dbParameter.GetType().GetProperty("OracleDbType");

                var parameter = Expression.Parameter(typeof(DbParameter));
                var parameterConvert = Expression.Convert(parameter, dbParameterType);
                var parameterValue = Expression.Parameter(typeof(object));
                var parameterValueConvert = Expression.Convert(parameterValue, dbtype);

                var property = Expression.Property(parameterConvert, propertyInfo);
                var expression = Expression.Assign(property, parameterValueConvert);

                _SetOracleDbType = Expression.Lambda<Action<DbParameter, object>>(expression, parameter, parameterValue).Compile();
            }

            _SetOracleDbType(dbParameter, type);
        }

        public static void SetOracleDevArtDbType(Assembly assembly, DbParameter dbParameter, object type)
        {
            if (_SetOracleDevArtDbType == null)
            {
                var dbtype = assembly.GetType("Devart.Data.Oracle.OracleDbType");
                var dbParameterType = assembly.GetType("Devart.Data.Oracle.OracleParameter");
                var propertyInfo = dbParameter.GetType().GetProperty("OracleDbType");

                var parameter = Expression.Parameter(typeof(DbParameter));
                var parameterConvert = Expression.Convert(parameter, dbParameterType);
                var parameterValue = Expression.Parameter(typeof(object));
                var parameterValueConvert = Expression.Convert(parameterValue, dbtype);

                var property = Expression.Property(parameterConvert, propertyInfo);
                var expression = Expression.Assign(property, parameterValueConvert);

                _SetOracleDevArtDbType = Expression.Lambda<Action<DbParameter, object>>(expression, parameter, parameterValue).Compile();
            }

            _SetOracleDevArtDbType(dbParameter, type);
        }
    }
}