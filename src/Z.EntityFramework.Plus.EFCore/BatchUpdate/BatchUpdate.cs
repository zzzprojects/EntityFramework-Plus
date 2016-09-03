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
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

#elif EF6
using System.Data.Entity.Core.Objects;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

#elif EFCORE
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Class to batch delete.</summary>
    public class BatchUpdate
    {
        /// <summary>The command text template.</summary>
        internal const string CommandTextTemplate = @"
UPDATE A 
SET {SetValue}
FROM {TableName} AS A
INNER JOIN ( {Select}
           ) AS B ON {PrimaryKeys}
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

        /// <summary>Executes the batch delete operation.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query used to execute the batch operation.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute<T>(IQueryable<T> query, Expression<Func<T, T>> updateFactory) where T : class
        {
            string expression = query.Expression.ToString();

            if (Regex.IsMatch(expression, @"\.Where\(\w+ => False\)"))
            {
                return 0;
            }

#if EF5 || EF6
            var objectQuery = query.GetObjectQuery();

            // GET model and info
            var model = query.GetDbContext().GetModel();
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

                var rowAffecteds = command.ExecuteNonQuery();
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
            var dbContext = query.GetDbContext();
            var entity = dbContext.Model.FindEntityType(typeof (T));
           
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
            var command = query.Context.CreateStoreCommand();
            bool isMySql = command.GetType().FullName.Contains("MySql");

            // GET mapping
            var mapping = entity.Info.EntityTypeMapping.MappingFragment;
            var store = mapping.StoreEntitySet;

            string tableName;

            if (isMySql)
            {
                tableName = string.Concat("`", store.Table, "`");
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
                isMySql ? CommandTextTemplate_MySQL : CommandTextTemplate;

            // GET inner query
            var querySelect = query.ToTraceString();

            // GET primary key join
            var primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.", EscapeName(x, isMySql), " = B.", EscapeName(x, isMySql), "")));

            // GET updateSetValues
            var setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                string.Concat("A.", EscapeName(x.Item1, isMySql), " = ", ((ConstantExpression) x.Item2).Value) :
                string.Concat("A.", EscapeName(x.Item1, isMySql), " = @zzz_BatchUpdate_", i)));

            // REPLACE template
            commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                .Replace("{Select}", querySelect)
                .Replace("{PrimaryKeys}", primaryKeys)
                .Replace("{SetValue}", setValues);

            // CREATE command
            command.CommandText = commandTextTemplate;

            // ADD Parameter
            var parameterCollection = query.Parameters;
            foreach (var parameter in parameterCollection)
            {
                var param = command.CreateParameter();
                param.ParameterName = parameter.Name;
                param.Value = parameter.Value;

                command.Parameters.Add(param);
            }

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
#elif EFCORE
        public DbCommand CreateCommand(IQueryable query, IEntityType entity, List<Tuple<string, object>> values)
        {
#if NETSTANDARD1_3
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(new AssemblyName("Microsoft.EntityFrameworkCore.SqlServer"));
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionMessage.BatchOperations_AssemblyNotFound);
            }

            if (assembly != null)
            {
                var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerMetadataExtensions");
                var sqlServerEntityTypeMethod = type.GetMethod("SqlServer", new[] {typeof (IEntityType)});
                var sqlServerPropertyMethod = type.GetMethod("SqlServer", new[] {typeof (IProperty)});
                var sqlServer = (IRelationalEntityTypeAnnotations) sqlServerEntityTypeMethod.Invoke(null, new[] {entity});

                // GET mapping
                var tableName = string.IsNullOrEmpty(sqlServer.Schema) ?
                    string.Concat("[", sqlServer.TableName, "]") :
                    string.Concat("[", sqlServer.Schema, "].[", sqlServer.TableName, "]");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = sqlServerPropertyMethod.Invoke(null, new[] {propertyKey});

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string) columnNameProperty.GetValue(mappingProperty));
                }
#else
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.InvariantCulture));

            if (assembly != null)
            {
                var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerMetadataExtensions");
                var sqlServerEntityTypeMethod = type.GetMethod("SqlServer", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (IEntityType)}, null);
                var sqlServerPropertyMethod = type.GetMethod("SqlServer", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (IProperty)}, null);
                var sqlServer = (IRelationalEntityTypeAnnotations) sqlServerEntityTypeMethod.Invoke(null, new[] {entity});

                // GET mapping
                var tableName = string.IsNullOrEmpty(sqlServer.Schema) ?
                    string.Concat("[", sqlServer.TableName, "]") :
                    string.Concat("[", sqlServer.Schema, "].[", sqlServer.TableName, "]");

                // GET keys mappings
                var columnKeys = new List<string>();
                foreach (var propertyKey in entity.GetKeys().ToList()[0].Properties)
                {
                    var mappingProperty = sqlServerPropertyMethod.Invoke(null, new[] {propertyKey});

                    var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                    columnKeys.Add((string) columnNameProperty.GetValue(mappingProperty));
                }
#endif
                // GET command text template
                var commandTextTemplate =
#if TODO
                BatchSize > 0 ?
                BatchDelayInterval > 0 ?
                    CommandTextWhileDelayTemplate :
                    CommandTextWhileTemplate :
#endif
                    CommandTextTemplate;

                // GET inner query
#if EFCORE
                RelationalQueryContext queryContext;
                var relationalCommand = query.CreateCommand(out queryContext);
#else
                var relationalCommand = query.CreateCommand();
#endif
                var querySelect = relationalCommand.CommandText;

                // GET primary key join
                var primaryKeys = string.Join(Environment.NewLine + "AND ", columnKeys.Select(x => string.Concat("A.[", x, "] = B.[", x, "]")));

                // GET updateSetValues
                var setValues = string.Join("," + Environment.NewLine, values.Select((x, i) => x.Item2 is ConstantExpression ?
                    string.Concat("A.[", x.Item1, "] = ", ((ConstantExpression) x.Item2).Value) :
                    string.Concat("A.[", x.Item1, "] = @zzz_BatchUpdate_", i)));

                // REPLACE template
                commandTextTemplate = commandTextTemplate.Replace("{TableName}", tableName)
                    .Replace("{Select}", querySelect)
                    .Replace("{PrimaryKeys}", primaryKeys)
                    .Replace("{SetValue}", setValues);

                // CREATE command
                var command = query.GetDbContext().CreateStoreCommand();
                command.CommandText = commandTextTemplate;

#if EFCORE
                // ADD Parameter
                foreach (var parameter in queryContext.ParameterValues)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = parameter.Key;
                    param.Value = parameter.Value;

                    command.Parameters.Add(param);
                }
#else
                                // ADD Parameter
                var parameterCollection = relationalCommand.Parameters;
                foreach (var parameter in parameterCollection)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = parameter.Name;
                    param.Value = parameter.Value;

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
            return null;
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

#if NETSTANDARD1_3
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(new AssemblyName("Microsoft.EntityFrameworkCore.SqlServer"));
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionMessage.BatchOperations_AssemblyNotFound);
            }

            if (assembly == null)
            {
                throw new Exception(ExceptionMessage.BatchOperations_AssemblyNotFound);
            }

            var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerMetadataExtensions");
            var sqlServerPropertyMethod = type.GetMethod("SqlServer", new[] {typeof (IProperty)});
#else
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith("Microsoft.EntityFrameworkCore.SqlServer"));

            if (assembly == null)
            {
                throw new Exception(ExceptionMessage.BatchOperations_AssemblyNotFound);
            }

            var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerMetadataExtensions");
            var sqlServerPropertyMethod = type.GetMethod("SqlServer", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (IProperty)}, null);

#endif
#endif


            // GET updateFactory command
            var values = ResolveUpdateFromQueryDictValues(updateFactory);
            var destinationValues = new List<Tuple<string, object>>();

            foreach (var value in values)
            {
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
                var mappingProperty = sqlServerPropertyMethod.Invoke(null, new[] {property});

                var columnNameProperty = mappingProperty.GetType().GetProperty("ColumnName", BindingFlags.Public | BindingFlags.Instance);
                var columnName = (string) columnNameProperty.GetValue(mappingProperty);
#endif

                if (value.Value is Expression)
                {
                    // Oops! the value is not resolved yet!
                    ParameterExpression parameterExpression = null;
                    ((Expression) value.Value).Visit((ParameterExpression p) =>
                    {
                        if (p.Type == typeof (T))
                            parameterExpression = p;

                        return p;
                    });

                    // GET the update value by creating a new select command
                    Type[] typeArguments = {typeof (T), ((Expression) value.Value).Type};
                    var lambdaExpression = Expression.Lambda(((Expression) value.Value), parameterExpression);
                    var selectExpression = Expression.Call(
                        typeof (Queryable),
                        "Select",
                        typeArguments,
                        Expression.Constant(query),
                        lambdaExpression);
                    var result = Expression.Lambda(selectExpression).Compile().DynamicInvoke();
#if EF5 || EF6
                    // GET the select command text
                    var commandText = ((IQueryable) result).ToString();

                    // GET the 'value' part
                    var valueSql = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                        commandText.Substring(6, commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6) :
                        commandText.Substring(6, commandText.IndexOf("FROM", StringComparison.InvariantCultureIgnoreCase) - 6);

                    // Add the destination name
                    valueSql = valueSql.Replace("AS [C1]", "");
                    valueSql = valueSql.Replace("[Extent1]", "B");
#elif EFCORE
                    RelationalQueryContext queryContext;
                    var command = ((IQueryable) result).CreateCommand(out queryContext);
                    var commandText = command.CommandText;

#if NETSTANDARD1_3
                    // GET the 'value' part
                    var valueSql = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) != -1 ?
                        commandText.Substring(6, commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.CurrentCultureIgnoreCase) - 6) :
                        commandText.Substring(6, commandText.IndexOf("FROM", StringComparison.CurrentCultureIgnoreCase) - 6);
#else
                                        // GET the 'value' part
                    var valueSql = commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) != -1 ?
                        commandText.Substring(6, commandText.IndexOf("AS [value]" + Environment.NewLine + "FROM", StringComparison.InvariantCultureIgnoreCase) - 6) :
                        commandText.Substring(6, commandText.IndexOf("FROM", StringComparison.InvariantCultureIgnoreCase) - 6);
#endif                

                    // Add the destination name
                    valueSql = valueSql.Replace("[x]", "B");
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

        public string EscapeName(string name, bool isMySql)
        {
            return isMySql ? string.Concat("`", name, "`") : string.Concat("[", name, "]");
        }

        public Dictionary<string, object> ResolveUpdateFromQueryDictValues<T>(Expression<Func<T, T>> updateFactory)
        {
            var dictValues = new Dictionary<string, object>();
            var updateExpressionBody = updateFactory.Body;
            var entityType = typeof (T);

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