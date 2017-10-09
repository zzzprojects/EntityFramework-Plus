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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

#if EF5
using System.Data.Objects;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

#elif EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

#elif EFCORE
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class to batch delete.</summary>
    public class BatchDelete
    {
        /// <summary>The command text template.</summary>
        internal const string CommandTextTemplate = @"
DELETE
FROM    A {Hint}
FROM    {TableName} AS A
        INNER JOIN ( {Select}
                    ) AS B ON {PrimaryKeys}

SELECT @@ROWCOUNT
";
        internal const string CommandTextSqlCeTemplate = @"
DELETE
FROM    {TableName}
WHERE EXISTS ( SELECT 1 FROM ({Select}) AS B
               WHERE {PrimaryKeys}
           )
";
        internal const string CommandTextOracleTemplate = @"
DELETE
FROM    {TableName}
WHERE EXISTS ( SELECT 1 FROM ({Select}) B
               WHERE {PrimaryKeys}
           )
";

        internal const string CommandTextSQLiteTemplate = @"
DELETE
FROM    {TableName}
WHERE EXISTS ( SELECT 1 FROM ({Select}) B
               WHERE {PrimaryKeys}
           )
";

        /// <summary>The command text postgre SQL template.</summary>
        internal const string CommandTextPostgreSQLTemplate = @"
DELETE FROM {TableName} AS A
USING ( {Select} ) AS B WHERE {PrimaryKeys}
";

        /// <summary>The command text postgre SQL template.</summary>
        internal const string CommandTextTemplate_MySql = @"
DELETE A
FROM {TableName} AS A
INNER JOIN ( {Select} ) AS B ON {PrimaryKeys}
";

        /// <summary>The command text template with WHILE loop.</summary>
        internal const string CommandTextWhileTemplate = @"
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @totalRowAffected = 0

WHILE @rowAffected IS NULL
    OR @rowAffected > 0
    BEGIN
        DELETE TOP ({Top})
        FROM    A {Hint}
        FROM    {TableName} AS A
                INNER JOIN ( {Select}
                           ) AS B ON {PrimaryKeys}

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected
    END

SELECT  @totalRowAffected
";

        /// <summary>The command text template with DELAY and WHILE loop</summary>
        internal const string CommandTextWhileDelayTemplate = @"
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @totalRowAffected = 0

WHILE @rowAffected IS NULL
    OR @rowAffected > 0
    BEGIN
        IF @rowAffected IS NOT NULL
            BEGIN
                WAITFOR DELAY '{Delay}'
            END

        DELETE TOP ({Top})
        FROM    A {Hint}
        FROM    {TableName} AS A
                INNER JOIN ( {Select}
                           ) AS B ON {PrimaryKeys}

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected
    END

SELECT  @totalRowAffected
";

        /// <summary>Default constructor.</summary>
        public BatchDelete()
        {
            BatchSize = 4000;
        }

        /// <summary>Gets or sets the size of the batch.</summary>
        /// <value>The size of the batch.</value>
        public int BatchSize { get; set; }

        /// <summary>Gets or sets the batch delay interval in milliseconds (The wait time between batch).</summary>
        /// <value>The batch delay interval in milliseconds (The wait time between batch).</value>
        public int BatchDelayInterval { get; set; }

        /// <summary>Gets or sets a value indicating whether the query use table lock.</summary>
        /// <value>True if use table lock, false if not.</value>
        public bool UseTableLock { get; set; }

        /// <summary>Gets or sets the DbCommand before being executed.</summary>
        /// <value>The DbCommand before being executed.</value>
        public Action<DbCommand> Executing { get; set; }

        /// <summary>Executes the batch delete operation.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query used to execute the batch operation.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute<T>(IQueryable<T> query) where T : class
        {
            // FIX query with visitor
            {
                var visitor = new BatchDeleteVisitor();
                visitor.Visit(query.Expression);

                if (visitor.HasOrderBy)
                {
                    query = query.Take(int.MaxValue);
                }

                if (visitor.HasTake || visitor.HasSkip)
                {
                    BatchSize = 0;
                }
            }

            string expression = query.Expression.ToString();

            if (Regex.IsMatch(expression, @"\.Where\(\w+ => False\)"))
            {
                return 0;
            }

            // GET model and info
#if EF5 || EF6
            var dbContext = query.GetDbContext();

#if EF6
            if (dbContext.IsInMemoryEffortQueryContext())
            {
                var context = query.GetDbContext();

                var list = query.ToList();
                context.Set<T>().RemoveRange(list);
                context.SaveChanges();
                return list.Count;
            }
#endif

            var model = dbContext.GetModel();
            var entity = model.Entity<T>();
            var keys = entity.Info.Key.PropertyRefs;

            // SELECT keys names
            var queryKeys = query.SelectByName(keys.Select(x => x.Name).ToList());
            var innerObjectQuery = queryKeys.GetObjectQuery();
             
            // CREATE command
            var command = CreateCommand(innerObjectQuery, entity);

            // WHERE 1 = 0
            if (command == null)
            {
                return 0;
            }

            // EXECUTE
            var ownConnection = false;

            try
            {
                if (innerObjectQuery.Context.Connection.State != ConnectionState.Open)
                {
                    ownConnection = true;
                    innerObjectQuery.Context.Connection.Open();
                }

                if (Executing != null)
                {
                    Executing(command);
                }

#if EF5
                if (command.GetType().Name == "NpgsqlCommand")
                {
                    command.CommandText = command.CommandText.Replace("[", "\"").Replace("]", "\"");
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else if (command.Connection.GetType().Name.Contains("MySql"))
                {
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else if (command.Connection.GetType().Name.Contains("Oracle"))
                {
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else if (command.GetType().Name == "SqlCeCommand")
                {
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else
                {
                    var rowAffecteds = (int)command.ExecuteScalar();
                    return rowAffecteds;
                }
#elif EF6
                var interceptionContext = new DbCommandInterceptionContext(dbContext.GetObjectContext().GetInterceptionContext());

                if (command.GetType().Name == "NpgsqlCommand")
                {
                    command.CommandText = command.CommandText.Replace("[", "\"").Replace("]", "\"");
                    int totalRowAffecteds = DbInterception.Dispatch.Command.NonQuery(command, interceptionContext);
                    return totalRowAffecteds;
                }
                else if (command.Connection.GetType().Name.Contains("MySql"))
                {
                    int totalRowAffecteds = DbInterception.Dispatch.Command.NonQuery(command, interceptionContext);
                    return totalRowAffecteds;
                }
                else if (command.Connection.GetType().Name.Contains("Oracle") || command.Connection.GetType().Name.Contains("SQLite"))
                {
                    int totalRowAffecteds = DbInterception.Dispatch.Command.NonQuery(command, interceptionContext);
                    return totalRowAffecteds;
                }
                else if (command.GetType().Name == "SqlCeCommand")
                {
                    int totalRowAffecteds = DbInterception.Dispatch.Command.NonQuery(command, interceptionContext);
                    return totalRowAffecteds;
                }
                else
                {
                    var rowAffecteds = (int)DbInterception.Dispatch.Command.Scalar(command, interceptionContext);
                    return rowAffecteds;
                }
#endif

            }
            finally
            {
                if (ownConnection && innerObjectQuery.Context.Connection.State != ConnectionState.Closed)
                {
                    innerObjectQuery.Context.Connection.Close();
                }
            }
#elif EFCORE
            if (BatchDeleteManager.InMemoryDbContextFactory != null && query.IsInMemoryQueryContext())
            {
                var context = BatchDeleteManager.InMemoryDbContextFactory();

                var list = query.ToList();
                context.RemoveRange(list);
                context.SaveChanges();
                return list.Count;
            }

            var dbContext = query.GetDbContext();
            var entity = dbContext.Model.FindEntityType(typeof(T));
            var keys = entity.GetKeys().ToList()[0].Properties;

            var queryKeys = query.SelectByName(keys.Select(x => x.Name).ToList());

            // CREATE command
            var command = CreateCommand(queryKeys, entity);

            // EXECUTE
            var ownConnection = false;

            try
            {
                if (dbContext.Database.GetDbConnection().State != ConnectionState.Open)
                {
                    ownConnection = true;
                    dbContext.Database.OpenConnection();
                }

                if (Executing != null)
                {
                    Executing(command);
                }

                if (command.GetType().Name == "NpgsqlCommand")
                {
                    command.CommandText = command.CommandText.Replace("[", "\"").Replace("]", "\"");
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else if (command.Connection.GetType().Name.Contains("MySql"))
                {
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }
                else if (command.GetType().Name.Contains("Sqlite"))
                {
                    int totalRowAffecteds = command.ExecuteNonQuery();
                    return totalRowAffecteds;
                }

                var rowAffecteds = (int)command.ExecuteScalar();
                return rowAffecteds;
            }
            finally
            {
                if (ownConnection && dbContext.Database.GetDbConnection().State != ConnectionState.Closed)
                {
                    dbContext.Database.CloseConnection();
                }
            }
#endif
            }

#if EF5 || EF6
        /// <summary>Creates a command to execute the batch operation.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="entity">The schema entity.</param>
        /// <param name="visitor">The visitor.</param>
        /// <returns>The new command to execute the batch operation.</returns>
        internal DbCommand CreateCommand<T>(ObjectQuery query, SchemaEntityType<T> entity)
        {
            // GET command
            var command = query.Context.CreateStoreCommand();

            bool isPostgreSql = command.GetType().Name == "NpgsqlCommand";
            bool isMySql = command.GetType().FullName.Contains("MySql");
            var isSqlCe = command.GetType().Name == "SqlCeCommand";
            var isOracle = command.GetType().Namespace.Contains("Oracle");
            var isSQLite = command.GetType().Namespace.Contains("SQLite");

            // Oracle BindByName
            if (isOracle)
            {
                var bindByNameProperty = command.GetType().GetProperty("BindByName") ?? command.GetType().GetProperty("PassParametersByName");
                if (bindByNameProperty != null)
                {
                    bindByNameProperty.SetValue(command, true, null);
                }
            }

            // GET mapping
            var mapping = entity.Info.EntityTypeMapping.MappingFragment;
            var store = mapping.StoreEntitySet;

            string tableName;

            if (isMySql)
            {
                tableName = string.Concat("`", store.Table, "`");
            }
            else if (isSqlCe)
            {
                tableName = string.Concat("[", store.Table, "]");
            }
            else if (isSQLite)
            {
                tableName = string.Concat("\"", store.Table, "\"");
            }
            else if (isOracle)
            {
                tableName = string.IsNullOrEmpty(store.Schema) || store.Schema == "dbo" ?
string.Concat("\"", store.Table, "\"") :
string.Concat("\"", store.Schema, "\".\"", store.Table, "\"");
            }
            else
            {
                tableName = string.IsNullOrEmpty(store.Schema) ?
    string.Concat("[", store.Table, "]") :
    string.Concat("[", store.Schema, "].[", store.Table, "]");
            }

            // GET keys mappings
            var columnKeys = new List<string>();
            foreach (var propertyKey in entity.Info.Key.PropertyRefs)
            {
                var mappingProperty = mapping.ScalarProperties.Find(x => x.Name == propertyKey.Name);

                if (mappingProperty == null)
                {
                    throw new Exception(string.Format(ExceptionMessage.BatchOperations_PropertyNotFound, propertyKey.Name));
                }

                columnKeys.Add(mappingProperty.ColumnName);
            }

            // GET command text template
            var commandTextTemplate = isPostgreSql ?
                CommandTextPostgreSQLTemplate :
                isOracle ?
                    CommandTextOracleTemplate :
                    isMySql ?
                        CommandTextTemplate_MySql :
                        isSqlCe ?
                            CommandTextSqlCeTemplate :
                            isSQLite ?
                                CommandTextSQLiteTemplate :
                            BatchSize > 0 ?
                                BatchDelayInterval > 0 ?
                                    CommandTextWhileDelayTemplate :
                                    CommandTextWhileTemplate :
                                CommandTextTemplate;

            // GET inner query
            var customQuery = query.GetCommandTextAndParameters();

            if (customQuery.Item1.EndsWith("WHERE 1 = 0", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            if (isPostgreSql && customQuery.Item1.EndsWith("WHERE TRUE = FALSE", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var querySelect = customQuery.Item1;

            // GET primary key join
            string primaryKeys;
            if (isSqlCe || isOracle || isSQLite)
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".", EscapeName(x, isMySql, isOracle), " = B.", EscapeName(x, isMySql, isOracle), "")));
            }
            else
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.", EscapeName(x, isMySql, isOracle), " = B.", EscapeName(x, isMySql, isOracle), "")));
            }

            // REPLACE template
            commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                .Replace("{Select}", querySelect)
                .Replace("{PrimaryKeys}", primaryKeys)
                .Replace("{Top}", BatchSize.ToString())
                .Replace("{Delay}", TimeSpan.FromMilliseconds(BatchDelayInterval).ToString(@"hh\:mm\:ss\:fff"))
                .Replace("{Hint}", UseTableLock ? "WITH ( TABLOCK )" : "");

            // CREATE command
            command.CommandText = commandTextTemplate;

            // ADD Parameter
            var parameterCollection = customQuery.Item2;
#if EF5
            foreach (ObjectParameter parameter in parameterCollection)
            {
                var param = command.CreateParameter();
                param.CopyFrom(parameter);

                command.Parameters.Add(param);
            }
#elif EF6
            foreach (DbParameter parameter in parameterCollection)
            {
                var param = command.CreateParameter();
                param.CopyFrom(parameter);

                command.Parameters.Add(param);
            }
#endif

            return command;
        }
#elif EFCORE
        public DbCommand CreateCommand(IQueryable query, IEntityType entity)
        {
            var context = query.GetDbContext();

            var databaseCreator = context.Database.GetService<IDatabaseCreator>();

            var assembly = databaseCreator.GetType().GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;

            MethodInfo dynamicProviderEntityType = null;
            MethodInfo dynamicProviderProperty = null;

            bool isSqlServer = false;
            bool isPostgreSQL = false;
            bool isMySql = false;
            bool isMySqlPomelo = false;
            bool isSQLite = false;

            if (assemblyName == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                isSqlServer = true;
                var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerMetadataExtensions");
                dynamicProviderEntityType = type.GetMethod("SqlServer", new[] { typeof(IEntityType) });
                dynamicProviderProperty = type.GetMethod("SqlServer", new[] { typeof(IProperty) });
            }
            else if (assemblyName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                isPostgreSQL = true;
                var type = assembly.GetType("Microsoft.EntityFrameworkCore.NpgsqlMetadataExtensions");
                dynamicProviderEntityType = type.GetMethod("Npgsql", new[] { typeof(IEntityType) });
                dynamicProviderProperty = type.GetMethod("Npgsql", new[] { typeof(IProperty) });
            }
            else if (assemblyName == "MySql.Data.EntityFrameworkCore")
            {
                isMySql = true;
                var type = assembly.GetType("MySQL.Data.EntityFrameworkCore.MySQLMetadataExtensions");
                dynamicProviderEntityType = type.GetMethod("MySQL", new[] { typeof(IEntityType) });
                dynamicProviderProperty = type.GetMethod("MySQL", new[] { typeof(IProperty) });
            }
            else if (assemblyName == "Pomelo.EntityFrameworkCore.MySql")
            {
                isMySqlPomelo = true;
                var type = assembly.GetType("Microsoft.EntityFrameworkCore.MySqlMetadataExtensions");
                dynamicProviderEntityType = type.GetMethod("MySql", new[] { typeof(IEntityType) });
                dynamicProviderProperty = type.GetMethod("MySql", new[] { typeof(IProperty) });
            }
            else if (assemblyName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                isSQLite = true;

                // CHANGE all for this one?
                dynamicProviderEntityType = typeof(RelationalMetadataExtensions).GetMethod("Relational", new[] { typeof(IEntityType) });
                dynamicProviderProperty = typeof(RelationalMetadataExtensions).GetMethod("Relational", new[] { typeof(IProperty) });
            }
            else
            {
                throw new Exception(string.Format(ExceptionMessage.Unsupported_Provider, assemblyName));
            }

            
            string tableName = "";
            var columnKeys = new List<string>();
            string primaryKeys = "";

            if (isSqlServer)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.IsNullOrEmpty(sqlServer.Schema) ?
                    string.Concat("[", sqlServer.TableName, "]") :
                    string.Concat("[", sqlServer.Schema, "].[", sqlServer.TableName, "]");

                // GET keys mappings
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.[", x, "] = B.[", x, "]")));
            }
            else if (isPostgreSQL)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.IsNullOrEmpty(sqlServer.Schema) ?
                    string.Concat("\"", sqlServer.TableName, "\"") :
                    string.Concat("\"", sqlServer.Schema, "\".\"", sqlServer.TableName, "\"");

                // GET keys mappings
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.\"", x, "\" = B.\"", x, "\"")));
            }
            else if (isMySqlPomelo)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("`", sqlServer.TableName, "`");

                // GET keys mappings
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.`", x, "` = B.`", x, "`")));
            }
            else if (isMySql)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("`", sqlServer.TableName, "`");

                // GET keys mappings
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.`", x, "` = B.`", x, "`")));
            }
            else if (isSQLite)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("\"", sqlServer.TableName, "\"");

                // GET keys mappings
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + "." + "\"" + x + "\"", " = B.\"", x, "\"")));
            }


            // GET command text template
            var commandTextTemplate = isPostgreSQL ?
                CommandTextPostgreSQLTemplate :
                isMySql || isMySqlPomelo ? 
                CommandTextTemplate_MySql :
                isSQLite ?
                CommandTextSQLiteTemplate :
                BatchSize > 0 ?
                BatchDelayInterval > 0 ?
                    CommandTextWhileDelayTemplate :
                    CommandTextWhileTemplate :
                CommandTextTemplate;

            // GET inner query
#if EFCORE
            RelationalQueryContext queryContext;
            var relationalCommand = query.CreateCommand(out queryContext);
#else
            var relationalCommand = query.CreateCommand();
#endif
            var querySelect = relationalCommand.CommandText;


            // REPLACE template
            commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                .Replace("{Select}", querySelect)
                .Replace("{PrimaryKeys}", primaryKeys)
                .Replace("{Top}", BatchSize.ToString())
                .Replace("{Delay}", TimeSpan.FromMilliseconds(BatchDelayInterval).ToString(@"hh\:mm\:ss\:fff"))
                .Replace("{Hint}", UseTableLock ? "WITH ( TABLOCK )" : "");

            // CREATE command
            var command = query.GetDbContext().CreateStoreCommand();
            command.CommandText = commandTextTemplate;

#if EFCORE
            // ADD Parameter
            foreach (var relationalParameter in relationalCommand.Parameters)
            {
                var parameter = queryContext.ParameterValues[relationalParameter.InvariantName];

                var param = command.CreateParameter();
                param.CopyFrom(relationalParameter, parameter);

                command.Parameters.Add(param);
            }
#else
            // ADD Parameter
            var parameterCollection = relationalCommand.Parameters;
            foreach (var parameter in parameterCollection)
            {
                var param = command.CreateParameter();
                param.CopyFrom(parameter);

                command.Parameters.Add(param);
            }
#endif

            return command;
        }
#endif
        public string EscapeName(string name, bool isMySql, bool isOracle)
        {
            return isMySql ? string.Concat("`", name, "`") :
                isOracle ? string.Concat("\"", name, "\"") :
                    string.Concat("[", name, "]");
        }
    }
}