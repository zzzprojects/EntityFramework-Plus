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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if EF5
using System.Data.EntityClient;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

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

            bool allowQueryBatch = QueryFutureManager.AllowQueryBatch;
#if EFCORE
            var databaseCreator = Context.Database.GetService<IDatabaseCreator>();

            var assembly = databaseCreator.GetType().GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;

            // We deactivated temporary some QueryFuture for EF Core as they don't work correctly            
            // We need to still make them "work" for IncludeFilter feature
            var isMySqlPomelo = assemblyName == "Pomelo.EntityFrameworkCore.MySql";
            var isOracle = assemblyName == "Oracle.EntityFrameworkCore" || assemblyName == "Devart.Data.Oracle.Entity.EFCore";
            if (allowQueryBatch && (isOracle || isMySqlPomelo))
            {
                allowQueryBatch = false;
            }
#endif

            if (!allowQueryBatch)
            {
                foreach (var query in Queries)
                {
                    query.GetResultDirectly();
                }

                Queries.Clear();
                return;
            }

            var ownConnection = false;

#if EF5 || EF6
            var connection = (EntityConnection)Context.Connection;

#if EF6
            if (Context.IsInMemoryEffortQueryContext())
            {
                foreach (var query in Queries)
                {
                    query.GetResultDirectly();
                }

                Queries.Clear();
                return;
            }
#endif
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

            var firstQuery = Queries[0];

            if (connection.State != ConnectionState.Open)
            {
                Context.Database.OpenConnection();
                ownConnection = true;
            }
#endif
            var command = CreateCommandCombined();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
#if EFCORE
                    // connection opened before
#else
                    connection.Open();
#endif
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
#if EFCORE
                    Context.Database.CloseConnection();
#else
                    connection.Close();
#endif
                }
            }

#if EFCORE
            if(firstQuery.RestoreConnection != null)
            {
                firstQuery.RestoreConnection();
            }
#endif
        }

#if NET45
        /// <summary>Executes deferred query lists.</summary>
        public async Task ExecuteQueriesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                await Queries[0].GetResultDirectlyAsync(cancellationToken).ConfigureAwait(false);
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

            var firstQuery = Queries[0];
#endif
            var command = CreateCommandCombined();

            var ownConnection = false;

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    ownConnection = true;
                }

                using (command)
                {
#if EF5
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        foreach (var query in Queries)
                        {
                            query.SetResult(reader);
                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                        }
					}
#elif EF6
					var interceptionContext = Context.GetInterceptionContext();
                    using (var reader = await DbInterception.Dispatch.Command.ReaderAsync(command, new DbCommandInterceptionContext(interceptionContext), cancellationToken).ConfigureAwait(false))
                    {
                        foreach (var query in Queries)
                        {
                            query.SetResult(reader);
                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
#elif EFCORE
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var createEntityDataReader = new CreateEntityDataReader(reader);
                        foreach (var query in Queries)
                        {
                            query.SetResult(createEntityDataReader);
                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
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

#if EFCORE
            if (firstQuery.RestoreConnection != null)
            {
                firstQuery.RestoreConnection();
            }
#endif
        }
#endif

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

#if EFCORE_3X
            // foreach is broken need stop and new Foreach, a for is better here, but I don't know if is possible Include with logique with new IncludeOptimized in a Where logic or other. In theory I guess yes, in true I don't know.
            // For now I try without check that.
            for (int i = 0; i < Queries.Count;i++)
            {
                var query = Queries.ElementAt(i);
                // first check is because parano.
                if (query.GetType().FullName.Contains("QueryFutureEnumerable") && query.Query != null && query.Query.GetType().FullName.Contains("Z.EntityFramework.Plus.QueryIncludeOptimizedParentQueryable"))
                {
                    var futurType = query.GetType();
                    var typeIncludeOptimized = query.Query.GetType();
                    var methodPrepareQuery = typeIncludeOptimized.GetMethod("PrepareQuery", BindingFlags.Instance | BindingFlags.NonPublic);
                    var propertyChilds = typeIncludeOptimized.GetProperty("Childs", BindingFlags.Instance | BindingFlags.Public);

                    // call futur.  
                    methodPrepareQuery.Invoke(query.Query, null);
                    var childs = propertyChilds.GetValue(query.Query);

                    query.Childs = (dynamic)childs;
                    query.IsIncludeOptimizedNullCollectionNeeded = true;
                }
            }
#endif


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
                string invariantName = null;

                if (parameters.Count == 1 && parameters[0] is CompositeRelationalParameter compositeRelationalParameter)
                {
                    invariantName = parameters[0].InvariantName;
                    parameters = compositeRelationalParameter.RelationalParameters;
                }

                int i = 0;
                object value;
                MethodInfo methodeConvertFromProvider;
                object convertToProvider; 

                // UPDATE parameter name
                foreach (var relationalParameter in parameters)
                {
                    value = null;
                    var parameter = queryContext.ParameterValues[invariantName ?? relationalParameter.InvariantName];

                    // logic FROM BatchUpdate.cs
                    methodeConvertFromProvider = null;
                    convertToProvider = null;

                    var propertyRelationalTypeMapping = relationalParameter.GetType().GetProperty("RelationalTypeMapping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    if (propertyRelationalTypeMapping != null)
                    {
                        var relationalTypeMapping = propertyRelationalTypeMapping.GetValue(relationalParameter);
                        var propertyConverter = relationalTypeMapping?.GetType().GetProperty("Converter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                        if (propertyConverter != null)
                        {
                            var converter = propertyConverter.GetValue(relationalTypeMapping);
                            var propertyConvertToProvider = converter?.GetType().GetProperty("ConvertToProvider", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                            if (propertyConvertToProvider != null)
                            {
                                convertToProvider = propertyConvertToProvider.GetValue(converter);
                                methodeConvertFromProvider = convertToProvider?.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                            }
                            else
                            {
                                var spatialPropertyConverter = relationalTypeMapping?.GetType().GetProperty("SpatialConverter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                                if (spatialPropertyConverter != null)
                                {
                                    var converterSpatial = spatialPropertyConverter.GetValue(relationalTypeMapping);
                                    var spatialPropertyConvertToProvider = converterSpatial?.GetType().GetProperty("ConvertToProvider", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                                    if (spatialPropertyConvertToProvider != null)
                                    {
                                        convertToProvider = spatialPropertyConvertToProvider.GetValue(converterSpatial);
                                        methodeConvertFromProvider = convertToProvider?.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                                    }
                                }
                            }
                        }
                    }


                    if (invariantName != null && parameter is object[] objectArray)
                    {
                        value = objectArray[i];
                        i++;
                    }

                    var oldValue = relationalParameter.InvariantName;
                    var newValue = string.Concat("Z_", queryCount, "_", oldValue);

                    // CREATE parameter
                    var dbParameter = command.CreateParameter();
                    dbParameter.CopyFrom(relationalParameter, value ?? parameter, newValue);

                    if (methodeConvertFromProvider != null)
                    {
                        dbParameter.Value = methodeConvertFromProvider.Invoke(convertToProvider, new[] { dbParameter.Value });
                    }

                    if (dbParameter.Value == null || dbParameter.Value.GetType() != typeof(object[]) || ((object[])dbParameter.Value).Count() != 0)
                    {
                        command.Parameters.Add(dbParameter);
                    }

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