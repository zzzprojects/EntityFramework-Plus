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
using System.Text.RegularExpressions;
#if EF5
using System.Data.Objects;
using System.Data.SqlClient;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

#elif EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Reflection;
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
    public class BatchUpdate
    {
        /// <summary>The command text template.</summary>
        internal const string CommandTextTemplate = @"
UPDATE A {Hint}
SET {SetValue}
FROM {TableName} AS A
INNER JOIN ( {Select}
           ) AS B ON {PrimaryKeys}
";

        internal const string CommandTextTemplateSqlCe = @"
UPDATE {TableName}
SET {SetValue}
WHERE EXISTS ( SELECT 1 FROM ({Select}) AS B
               WHERE {PrimaryKeys}
           )  
";
        internal const string CommandTextOracleTemplate = @"
UPDATE {TableName}
SET {SetValue}
WHERE EXISTS ( SELECT 1 FROM ({Select}) B
               WHERE {PrimaryKeys}
           )  
";

        internal const string CommandTextTemplate_PostgreSQL = @"
UPDATE {TableName}
SET {SetValue}
WHERE EXISTS ( SELECT 1 FROM ({Select}) B
               WHERE {PrimaryKeys}
           )  
";

        internal const string CommandTextTemplate_SQLite = @"
UPDATE {TableName}
SET {SetValue}
WHERE EXISTS ( SELECT 1 FROM ({Select}) B
               WHERE {PrimaryKeys}
           )  
";

        internal const string CommandTextTemplate_MySQL = @"
UPDATE {TableName} AS A
INNER JOIN ( {Select}
           ) AS B ON {PrimaryKeys}
SET {SetValue}
";

#if TODO
    /// <summary>The command text template with WHILE loop.</summary>
        internal const string CommandTextWhileTemplate = @"
DECLARE @rowAffected INT
DECLARE @totalRowAffected INT

SET @totalRowAffected = 0

WHILE @rowAffected IS NULL
    OR @rowAffected > 0
    BEGIN
        DELETE TOP ({Top})
        FROM    A
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
        FROM    A
        FROM    {TableName} AS A
                INNER JOIN ( {Select}
                           ) AS B ON {PrimaryKeys}

        SET @rowAffected = @@ROWCOUNT
        SET @totalRowAffected = @totalRowAffected + @rowAffected
    END

SELECT  @totalRowAffected
";

        /// <summary>Default constructor.</summary>
        public BatchUpdate()
        {
            BatchSize = 4000;
        }

        /// <summary>Gets or sets the size of the batch.</summary>
        /// <value>The size of the batch.</value>
        public int BatchSize { get; set; }

        /// <summary>Gets or sets the batch delay interval in milliseconds (The wait time between batch).</summary>
        /// <value>The batch delay interval in milliseconds (The wait time between batch).</value>
        public int BatchDelayInterval { get; set; }
#endif

        /// <summary>Gets or sets the DbCommand before being executed.</summary>
        /// <value>The DbCommand before being executed.</value>
        public Action<DbCommand> Executing { get; set; }

        /// <summary>Gets or sets a value indicating whether the query use table lock.</summary>
        /// <value>True if use table lock, false if not.</value>
        public bool UseTableLock { get; set; }

        /// <summary>Executes the batch delete operation.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query used to execute the batch operation.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute<T>(IQueryable<T> query, Expression<Func<T, T>> updateFactory) where T : class
        {
            // FIX query with visitor
            {
                var visitor = new BatchUpdateVisitor();
                visitor.Visit(query.Expression);

                if (visitor.HasOrderBy)
                {
                    query = query.Take(int.MaxValue);
                }
            }

            string expression = query.Expression.ToString();

            if (Regex.IsMatch(expression, @"\.Where\(\w+ => False\)"))
            {
                return 0;
            }

#if EF5 || EF6
            var dbContext = query.GetDbContext();

#if EF6
            if (dbContext.IsInMemoryEffortQueryContext())
            {
                var context = query.GetDbContext();

                var list = query.ToList();
                var compiled = updateFactory.Compile();
                var memberBindings = ((MemberInitExpression)updateFactory.Body).Bindings;
                var accessors = memberBindings
                    .Select(x => x.Member.Name)
                    .Select(x => new PropertyOrFieldAccessor(typeof(T).GetProperty(x, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)))
                    .ToList();

                foreach (var item in list)
                {
                    var newItem = compiled(item);

                    foreach (var accessor in accessors)
                    {
                        var value = accessor.GetValue(newItem);
                        accessor.SetValue(item, value);
                    }
                }

                context.SaveChanges();
                return list.Count;
            }
#endif

            var objectQuery = query.GetObjectQuery();

            // GET model and info
            var model = dbContext.GetModel();
            var entity = model.Entity<T>();

            // TODO: Select only key + lambda columns
            // var keys = entity.Info.Key.PropertyRefs;
            //var queryKeys = query.SelectByName(keys.Select(x => x.Name).ToList());
            //var innerObjectQuery = queryKeys.GetObjectQuery();
            var innerObjectQuery = objectQuery;

            // GET UpdateSetValues
            var values = GetInnerValues(query, updateFactory, entity);

            // CREATE command
            var command = CreateCommand(innerObjectQuery, entity, values);

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
                var rowAffecteds = command.ExecuteNonQuery();
#elif EF6
                var interceptionContext = new DbCommandInterceptionContext(dbContext.GetObjectContext().GetInterceptionContext());
                var rowAffecteds = DbInterception.Dispatch.Command.NonQuery(command, interceptionContext);
#endif
                return rowAffecteds;
            }
            finally
            {
                if (ownConnection && innerObjectQuery.Context.Connection.State != ConnectionState.Closed)
                {
                    innerObjectQuery.Context.Connection.Close();
                }
            }
#elif EFCORE
            if (BatchUpdateManager.InMemoryDbContextFactory != null && query.IsInMemoryQueryContext())
            {
                var context = BatchUpdateManager.InMemoryDbContextFactory();

                var list = query.ToList();
                var compiled = updateFactory.Compile();
                var memberBindings = ((MemberInitExpression)updateFactory.Body).Bindings;
                var accessors = memberBindings
                    .Select(x => x.Member.Name)
                    .Select(x => new PropertyOrFieldAccessor(typeof(T).GetProperty(x, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)))
                    .ToList();

                foreach (var item in list)
                {
                    var newItem = compiled(item);

                    foreach (var accessor in accessors)
                    {
                        var value = accessor.GetValue(newItem);
                        accessor.SetValue(item, value);
                    }
                }

                context.SaveChanges();
                return list.Count;
            }

            var dbContext = query.GetDbContext();
            var entity = dbContext.Model.FindEntityType(typeof(T));

            // TODO: Select only key + lambda columns
            //  var keys = entity.GetKeys().ToList()[0].Properties;
            //var queryKeys = query.SelectByName(keys.Select(x => x.Name).ToList());
            //var innerObjectQuery = queryKeys.GetObjectQuery();
            var queryKeys = query;

            // GET UpdateSetValues
            var values = GetInnerValues(query, updateFactory, entity);

            // CREATE command
            var command = CreateCommand(queryKeys, entity, values);

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

                var rowAffecteds = command.ExecuteNonQuery();
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
        /// <param name="query">The query.</param>
        /// <param name="entity">The schema entity.</param>
        /// <returns>The new command to execute the batch operation.</returns>
        internal DbCommand CreateCommand<T>(ObjectQuery query, SchemaEntityType<T> entity, List<Tuple<string, object>> values)
        {
            var objectParameters = values.Where(x => x.Item2 is ObjectParameter);
            values = values.Except(objectParameters).ToList();

            var command = query.Context.CreateStoreCommand();
            bool isMySql = command.GetType().FullName.Contains("MySql");
            var isSqlCe = command.GetType().Name == "SqlCeCommand";
            var isOracle = command.GetType().Namespace.Contains("Oracle");
            var isPostgreSQL = command.GetType().Name == "NpgsqlCommand";
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
            else if (isOracle)
            {
                tableName = string.IsNullOrEmpty(store.Schema) || store.Schema == "dbo" ?
                    string.Concat("\"", store.Table, "\"") :
                    string.Concat("\"", store.Schema, "\".\"", store.Table, "\"");
            }
            else if (isPostgreSQL)
            {
                tableName = string.IsNullOrEmpty(store.Schema) ?
                    string.Concat("\"", store.Table, "\"") :
                    string.Concat("\"", store.Schema, "\".\"", store.Table, "\"");
            }
            else if (isSQLite)
            {
                tableName = string.Concat("\"", store.Table, "\"");
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
            var commandTextTemplate =
#if TODO
                BatchSize > 0 ?
                BatchDelayInterval > 0 ?
                    CommandTextWhileDelayTemplate :
                    CommandTextWhileTemplate :
#endif
                isPostgreSQL ? CommandTextTemplate_PostgreSQL : 
                isOracle ? CommandTextOracleTemplate :
                isMySql ? CommandTextTemplate_MySQL : 
                isSqlCe ? CommandTextTemplateSqlCe :
                isSQLite ? CommandTextTemplate_SQLite :
                CommandTextTemplate;

            // GET inner query
            var customQuery = query.GetCommandTextAndParameters();

            if (customQuery.Item1.EndsWith("WHERE 1 = 0", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var querySelect = customQuery.Item1;

            // GET primary key join
            string primaryKeys;
            string setValues;

            if (isSqlCe)
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".", EscapeName(x, isMySql, isOracle, isPostgreSQL), " = B.", EscapeName(x, isMySql, isOracle, isPostgreSQL), "")));

                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = ", ((ConstantExpression) x.Item2).Value.ToString().Replace("B.[", "[")) :
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = @zzz_BatchUpdate_", i)));
            }
            else if (isOracle || isPostgreSQL)
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".", EscapeName(x, isMySql, isOracle, isPostgreSQL), " = B.", EscapeName(x, isMySql, isOracle, isPostgreSQL), "")));

                // GET updateSetValues
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = ", ((ConstantExpression)x.Item2).Value) :
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = :zzz_BatchUpdate_", i)));
            }
            else if (isSQLite)
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".", EscapeName(x, isMySql, isOracle, isPostgreSQL), " = B.", EscapeName(x, isMySql, isOracle, isPostgreSQL), "")));

                // GET updateSetValues
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = ", ((ConstantExpression)x.Item2).Value) :
                    string.Concat(EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = @zzz_BatchUpdate_", i)));
            }
            else
            {
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.", EscapeName(x, isMySql, isOracle, isPostgreSQL), " = B.", EscapeName(x, isMySql, isOracle, isPostgreSQL), "")));

                // GET updateSetValues
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                    string.Concat("A.", EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = ", ((ConstantExpression) x.Item2).Value) :
                    string.Concat("A.", EscapeName(x.Item1, isMySql, isOracle, isPostgreSQL), " = @zzz_BatchUpdate_", i)));
            }

            // REPLACE template
            commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                .Replace("{Select}", querySelect)
                .Replace("{PrimaryKeys}", primaryKeys)
                .Replace("{SetValue}", setValues)
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

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];

                if (value.Item2 is ConstantExpression)
                {
                    continue;
                }

                var parameterPrefix = isOracle ? ":" : "@";

                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterPrefix + "zzz_BatchUpdate_" + i;
                parameter.Value = values[i].Item2 ?? DBNull.Value;

                if (parameter is SqlParameter)
                {
                    var sqlParameter = (SqlParameter)parameter;
                    if (sqlParameter.DbType == DbType.DateTime)
                    {
                        sqlParameter.DbType = DbType.DateTime2;
                    }
                }

                command.Parameters.Add(parameter);
            }

            foreach (var value in objectParameters)
            {
                var objectParameter = (ObjectParameter)value.Item2;

                var parameter = command.CreateParameter();

                parameter.Value = objectParameter.Value ?? DBNull.Value;
                parameter.ParameterName = objectParameter.Name;
                command.Parameters.Add(parameter);
            }

            return command;
        }
#elif EFCORE
        public DbCommand CreateCommand(IQueryable query, IEntityType entity, List<Tuple<string, object>> values)
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
            string primaryKeys = "";

            if (isSqlServer)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] {entity});

                // GET mapping
                tableName = string.IsNullOrEmpty(sqlServer.Schema) ? string.Concat("[", sqlServer.TableName, "]") : string.Concat("[", sqlServer.Schema, "].[", sqlServer.TableName, "]");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] {propertyKey});

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string) columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.[", x, "] = B.[", x, "]")));
            }
            else if (isPostgreSQL)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] {entity});

                // GET mapping
                tableName = string.IsNullOrEmpty(sqlServer.Schema) ? string.Concat("\"", sqlServer.TableName, "\"") : string.Concat("\"", sqlServer.Schema, "\".\"", sqlServer.TableName, "\"");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] {propertyKey});

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string) columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".\"", x, "\" = B.\"", x, "\"")));
            }
            else if (isMySqlPomelo)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("`", sqlServer.TableName, "`");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.`", x, "` = B.`", x, "`")));
            }
            else if (isMySql)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("`", sqlServer.TableName, "`");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.`", x, "` = B.`", x, "`")));
            }
            else if (isSQLite)
            {
                var sqlServer = (IRelationalEntityTypeAnnotations)dynamicProviderEntityType.Invoke(null, new[] { entity });

                // GET mapping
                tableName = string.Concat("\"", sqlServer.TableName, "\"");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { propertyKey });

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string)columnNameProperty.GetValue(mappingProperty));
                }

                // GET primary key join
                primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat(tableName + ".\"", x, "\" = B.\"", x, "\"")));
            }

            // GET command text template
            var commandTextTemplate =
#if TODO
            BatchSize > 0 ?
            BatchDelayInterval > 0 ?
                CommandTextWhileDelayTemplate :
                CommandTextWhileTemplate :
#endif
                isPostgreSQL? CommandTextTemplate_PostgreSQL : 
                isMySql || isMySqlPomelo ?
                CommandTextTemplate_MySQL :
                isSQLite ?
                CommandTextTemplate_SQLite :
                CommandTextTemplate;

            // GET inner query
#if EFCORE
            RelationalQueryContext queryContext;
            var relationalCommand = query.CreateCommand(out queryContext);
#else
            var relationalCommand = query.CreateCommand();
#endif
            var querySelect = relationalCommand.CommandText;



            // GET updateSetValues
            var setValues = "";

            if (isSqlServer)
            {
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ? string.Concat("A.[", x.Item1, "] = ", ((ConstantExpression) x.Item2).Value) : string.Concat("A.[", x.Item1, "] = @zzz_BatchUpdate_", i)));
            }
            else if (isPostgreSQL)
            {
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ? string.Concat("\"", x.Item1, "\" = ", ((ConstantExpression)x.Item2).Value) : string.Concat("\"", x.Item1, "\" = @zzz_BatchUpdate_", i)));
            }
            else if (isMySql || isMySqlPomelo)
            {
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ? string.Concat("A.`", x.Item1, "` = ", ((ConstantExpression)x.Item2).Value) : string.Concat("A.`", x.Item1, "` = @zzz_BatchUpdate_", i)));
            }
            else if (isSQLite)
            {
                setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ? string.Concat("\"", x.Item1, "\" = ", ((ConstantExpression)x.Item2).Value) : string.Concat("\"", x.Item1, "\" = @zzz_BatchUpdate_", i)));
            }

            // REPLACE template
            commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                .Replace("{Select}", querySelect)
                .Replace("{PrimaryKeys}", primaryKeys)
                .Replace("{SetValue}", setValues)
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

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];

                if (value.Item2 is ConstantExpression)
                {
                    continue;
                }

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@zzz_BatchUpdate_" + i;
                parameter.Value = values[i].Item2 ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            return command;
        }
#endif

#if EF5 || EF6
        internal List<Tuple<string, object>> GetInnerValues<T>(IQueryable<T> query, Expression<Func<T, T>> updateFactory, SchemaEntityType<T> entity) where T : class
#elif EFCORE
        public List<Tuple<string, object>> GetInnerValues<T>(IQueryable<T> query, Expression<Func<T, T>> updateFactory, IEntityType entity) where T : class
#endif
        {
#if EF5 || EF6
            // GET mapping
            var mapping = entity.Info.EntityTypeMapping.MappingFragment;
#elif EFCORE
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

#endif
            // GET updateFactory command
            var values = ResolveUpdateFromQueryDictValues(updateFactory);
            var destinationValues = new List<Tuple<string, object>>();

            int valueI = -1;
            foreach (var value in values)
            {
                valueI++;

#if EF5 || EF6
                // FIND the mapped column
                var column = mapping.ScalarProperties.Find(x => x.Name == value.Key);
                if (column == null)
                {
                    throw new Exception("The destination column could not be found:" + value.Key);
                }
                var columnName = column.ColumnName;
#elif EFCORE

                var property = entity.FindProperty(value.Key);
                var mappingProperty = dynamicProviderProperty.Invoke(null, new[] { property });

                var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                var columnName = (string)columnNameProperty.GetValue(mappingProperty);
#endif

                if (value.Value is Expression)
                {
                    // Oops! the value is not resolved yet!
                    ParameterExpression parameterExpression = null;
                    ((Expression)value.Value).Visit((ParameterExpression p) =>
                    {
                        if (p.Type == typeof(T))
                            parameterExpression = p;

                        return p;
                    });

                    // GET the update value by creating a new select command
                    Type[] typeArguments = { typeof(T), ((Expression)value.Value).Type };
                    var lambdaExpression = Expression.Lambda(((Expression)value.Value), parameterExpression);
                    var selectExpression = Expression.Call(
                        typeof(Queryable),
                        "Select",
                        typeArguments,
                        Expression.Constant(query),
                        lambdaExpression);
                    var result = Expression.Lambda(selectExpression).Compile().DynamicInvoke();
#if EF5 || EF6
                    // GET the select command text
                    var commandText = ((IQueryable)result).ToString();
                    var parameters = ((IQueryable) result).GetObjectQuery().Parameters;

                    // GET the 'value' part
                    var pos = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                        commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                        commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                            commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                            commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                                commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                                commandText.IndexOf("FROM", StringComparison.InvariantCultureIgnoreCase) - 6;

                    var valueSql = commandText.Substring(6, pos);

                    // Add the destination name
                    valueSql = valueSql.Replace("AS [C1]", "");
                    valueSql = valueSql.Replace("AS `C1`", "");

                    var listReplace = new List<string>()
                    {
                        "[Extent1]",
                        "[Extent2]",
                        "[Extent3]",
                        "[Extent4]",
                        "[Extent5]",
                        "[Extent6]",
                        "[Extent7]",
                        "[Extent8]",
                        "[Extent9]",
                        "[Filter1]",
                        "[Filter2]",
                        "[Filter3]",
                        "[Filter4]",
                        "[Filter5]",
                        "[Filter6]",
                        "`Extent1`",
                        "`Extent2`",
                        "`Extent3`",
                        "`Extent4`",
                        "`Extent5`",
                        "`Extent6`",
                        "`Extent7`",
                        "`Extent8`",
                        "`Extent9`",
                        "`Filter1`",
                        "`Filter2`",
                        "`Filter3`",
                        "`Filter4`",
                        "`Filter5`",
                        "`Filter6`",
                    };

                    // Replace the first value found only!
                    foreach (var itemReplace in listReplace)
                    {
                        if (valueSql.Contains(itemReplace))
                        {
                            valueSql = valueSql.Replace(itemReplace, "B");
                            break;
                        }
                    }

                    // CHECK if valueSql end with ' AS [XYZ]'
                    if (valueSql.LastIndexOf('[') != -1 && valueSql.Substring(0, valueSql.LastIndexOf('[')).EndsWith(" AS ", StringComparison.InvariantCulture))
                    {
                        valueSql = valueSql.Substring(0, valueSql.LastIndexOf('[') - 4);
                    }

                    if (valueSql.LastIndexOf('`') != -1 && valueSql.Substring(0, valueSql.LastIndexOf('`')).EndsWith(" AS ", StringComparison.InvariantCulture))
                    {
                        valueSql = valueSql.Substring(0, valueSql.LastIndexOf('`') - 4);
                    }

#elif EFCORE
                    RelationalQueryContext queryContext;
                    var command = ((IQueryable)result).CreateCommand(out queryContext);
                    var commandText = command.CommandText;

#if NETSTANDARD1_3
                    // GET the 'value' part
                    var pos = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) != -1 ?
                        commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) - 6 :
                        commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.CurrentCultureIgnoreCase) != -1 ?
                            commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.CurrentCultureIgnoreCase) - 6 :
                            commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) != -1 ?
                                commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) - 6 :
                                commandText.IndexOf("FROM", StringComparison.CurrentCultureIgnoreCase) - 6;

                    var valueSql = commandText.Substring(6, pos);
#else
                    // GET the 'value' part
                    var pos = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                        commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                        commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                            commandText.IndexOf(Environment.NewLine + "    FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                            commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                                commandText.IndexOf(Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6 :
                                commandText.IndexOf("FROM", StringComparison.InvariantCultureIgnoreCase) - 6;

                    var valueSql = commandText.Substring(6, pos);
#endif

                    valueSql = valueSql.Trim();

                    // Add the destination name
                    valueSql = valueSql.Replace("[x]", "B");
                    valueSql = valueSql.Replace("[c]", "B");

                    if (valueSql.Length > 0 
                        && valueSql[0] == '['
                        && valueSql.IndexOf("]", StringComparison.CurrentCultureIgnoreCase) != -1
                        && valueSql.IndexOf("]", StringComparison.CurrentCultureIgnoreCase) == valueSql.IndexOf("].[", StringComparison.CurrentCultureIgnoreCase)
                        )
                    {
                        // Could contains [something].[column]
                        
                        // GET the tag
                        var tagToReplace = valueSql.Substring(0, valueSql.IndexOf("]", StringComparison.CurrentCultureIgnoreCase) + 1);
                        valueSql = valueSql.Replace(tagToReplace, "B");
                    }
#endif
#if EF5 || EF6
                    foreach (var additionalParameter in parameters)
                    {
                        var newName = additionalParameter.Name + "_" + valueI;
                        var newParameter = new ObjectParameter(newName, additionalParameter.ParameterType)
                        {
                            Value = additionalParameter.Value
                        };
                        destinationValues.Add(new Tuple<string, object>(columnName, newParameter));

                        valueSql = valueSql.Replace(additionalParameter.Name, newName);
                    }

                    // TODO: For EF Core?
#endif

                    destinationValues.Add(new Tuple<string, object>(columnName, Expression.Constant(valueSql)));
                }
                else
                {
                    destinationValues.Add(new Tuple<string, object>(columnName, value.Value ?? DBNull.Value));
                }
            }

            return destinationValues;
        }

        public string EscapeName(string name, bool isMySql, bool isOracle, bool isPostgreSQL)
        {
            return isMySql ? string.Concat("`", name, "`") :
                isOracle || isPostgreSQL ? string.Concat("\"", name, "\"") :
                    string.Concat("[", name, "]");
        }

        public Dictionary<string, object> ResolveUpdateFromQueryDictValues<T>(Expression<Func<T, T>> updateFactory)
        {
            var dictValues = new Dictionary<string, object>();
            var updateExpressionBody = updateFactory.Body;
            var entityType = typeof(T);

            // ENSURE: new T() { MemberInitExpression }
            var memberInitExpression = updateExpressionBody as MemberInitExpression;
            if (memberInitExpression == null)
            {
                throw new Exception("Invalid Cast. The update expression must be of type MemberInitExpression.");
            }

            foreach (var binding in memberInitExpression.Bindings)
            {
                var propertyName = binding.Member.Name;

                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                {
                    throw new Exception("Invalid Cast. The update expression MemberBinding must be of type MemberAssignment.");
                }

                var memberExpression = memberAssignment.Expression;

                // CHECK if the assignement has a property from the entity.
                var hasEntityProperty = false;
                memberExpression.Visit((ParameterExpression p) =>
                {
                    if (p.Type == entityType)
                    {
                        hasEntityProperty = true;
                    }

                    return p;
                });

                if (!hasEntityProperty)
                {
                    object value;

                    var constantExpression = memberExpression as ConstantExpression;

                    if (constantExpression != null)
                    {
                        value = constantExpression.Value;
                    }
                    else
                    {
                        // Compile the expression and get the value.
                        var lambda = Expression.Lambda(memberExpression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }

                    dictValues.Add(propertyName, value);
                }
                else
                {
                    // FIX all member access to remove variable
                    memberExpression = memberExpression.Visit((MemberExpression m) =>
                    {
                        if (m.Expression.NodeType == ExpressionType.Constant)
                        {
                            var lambda = Expression.Lambda(m, null);
                            var value = lambda.Compile().DynamicInvoke();
                            var c = Expression.Constant(value, m.Type);
                            return c;
                        }

                        return m;
                    });

                    // ADD expression, the expression will be resolved later
                    dictValues.Add(propertyName, memberExpression);
                }
            }

            if (dictValues.Count == 0)
            {
                throw new Exception("Invalid update expression. Atleast one column must be updated");
            }

            return dictValues;
        }
    }
}