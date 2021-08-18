// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection; 
#if NET45
using System.Threading;
using System.Threading.Tasks;
#endif


#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

#endif

#if EFCORE_2X
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Clauses;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

#elif EFCORE_3X
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Extensions;
#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Interface for QueryFuture class.</summary>
#if QUERY_INCLUDEOPTIMIZED
    internal abstract class BaseQueryFuture
#else
    public abstract class BaseQueryFuture
#endif
    { 
#if EFCORE_3X
        internal bool IsIncludeOptimizedNullCollectionNeeded { get; set; }
        internal List<BaseQueryIncludeOptimizedChild> Childs { get; set; }
#endif
        /// <summary>Gets the value indicating whether the query future has a value.</summary>
        /// <value>true if this query future has a value, false if not.</value>
        public bool HasValue { get; protected set; }

        /// <summary>Gets or sets the batch that owns the query future.</summary>
        /// <value>The batch that owns the query future.</value>
        public QueryFutureBatch OwnerBatch { get; set; }

#if EF5 || EF6
        /// <summary>Gets or sets the query deferred.</summary>
        /// <value>The query deferred.</value>
        public ObjectQuery Query { get; set; }
#elif EFCORE
    /// <summary>Gets or sets the query deferred.</summary>
    /// <value>The query deferred.</value>
        public IQueryable Query { get; set; }

        /// <summary>Gets or sets the query deferred executor.</summary>
        /// <value>The query deferred executor.</value>
        public object QueryExecutor { get; set; }

        /// <summary>Gets or sets a context for the query deferred.</summary>
        /// <value>The query deferred context.</value>
        public QueryContext QueryContext { get; set; }

        /// <summary>Gets or sets the query connection.</summary>
        /// <value>The query connection.</value>
        internal IRelationalConnection QueryConnection { get; set; }

#if EFCORE_3X
        internal object CompiledQuery { get; set; }
#endif

        internal Action RestoreConnection { get;set;}

        public virtual void ExecuteInMemory()
        {
            
        }

#if EFCORE_2X
        /// <summary>Creates executor and get command.</summary>
        /// <returns>The new executor and get command.</returns>
        public virtual IRelationalCommand CreateExecutorAndGetCommand(out RelationalQueryContext queryContext)
        {
            queryContext = null;

            bool isEFCore2x = false;
            bool EFCore_2_1 = false;
#if EFCORE
            bool isEFCore3x = EFCoreHelper.IsVersion3x;
#endif

            var context = Query.GetDbContext();

            // REFLECTION: Query._context.StateManager
#if NETSTANDARD2_0
            var stateManager = context.ChangeTracker.GetStateManager();
#else
            var stateManagerProperty = typeof(DbContext).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (StateManager)stateManagerProperty.GetValue(context);
#endif

            // REFLECTION: Query._context.StateManager._concurrencyDetector
            var concurrencyDetectorField = typeof(StateManager).GetField("_concurrencyDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var concurrencyDetector = (IConcurrencyDetector)concurrencyDetectorField.GetValue(stateManager);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(Query.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderProperty = queryCompiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            object nodeTypeProvider;
            object QueryModelGenerator = null;

            if (nodeTypeProviderProperty == null)
            {
                EFCore_2_1 = true;

                var QueryModelGeneratorField = queryCompiler.GetType().GetField("_queryModelGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
                QueryModelGenerator = QueryModelGeneratorField.GetValue(queryCompiler);

                var nodeTypeProviderField = QueryModelGenerator.GetType().GetField("_nodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
                nodeTypeProvider = nodeTypeProviderField.GetValue(QueryModelGenerator);
            }
            else
            {
                nodeTypeProvider = nodeTypeProviderProperty.GetValue(queryCompiler);
            }

            // REFLECTION: Query.Provider._queryCompiler.CreateQueryParser();
#if NETSTANDARD2_0
            QueryParser createQueryParser = null;
            if (EFCore_2_1)
            {
                var queryParserMethod = QueryModelGenerator.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Instance);
                createQueryParser = (QueryParser)queryParserMethod.Invoke(QueryModelGenerator, new[] { nodeTypeProvider });
            }
            else
            {
                var queryParserMethod = queryCompiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Instance);
                createQueryParser = (QueryParser)queryParserMethod.Invoke(queryCompiler, new[] { nodeTypeProvider });
            }
#else
            var createQueryParserMethod = queryCompiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var createQueryParser = (QueryParser)createQueryParserMethod.Invoke(null, new[] { nodeTypeProvider });
#endif

            // REFLECTION: Query.Provider._queryCompiler._database
            var databaseField = queryCompiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            var database = (IDatabase) databaseField.GetValue(queryCompiler);

            // REFLECTION: Query.Provider._queryCompiler._evaluatableExpressionFilter
#if NETSTANDARD2_0
            IEvaluatableExpressionFilter evaluatableExpressionFilter = null;

            if (isEFCore3x)
            {
                evaluatableExpressionFilter = (RelationalEvaluatableExpressionFilter)QueryModelGenerator.GetType().GetProperty("EvaluatableExpressionFilter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(QueryModelGenerator);
            }
            else if (EFCore_2_1)
            {
                evaluatableExpressionFilter = (IEvaluatableExpressionFilter)QueryModelGenerator.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(QueryModelGenerator);
            }
            else
            {
                evaluatableExpressionFilter = (IEvaluatableExpressionFilter)queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompiler);
            }
#else
            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter) evaluatableExpressionFilterField.GetValue(null);
#endif

            // REFLECTION: Query.Provider._queryCompiler._queryContextFactory
            var queryContextFactoryField = queryCompiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (IQueryContextFactory) queryContextFactoryField.GetValue(queryCompiler);

            // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.CreateQueryBuffer
            var createQueryBufferDelegateMethod = (typeof (QueryContextFactory)).GetMethod("CreateQueryBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var createQueryBufferDelegate = (Func<IQueryBuffer>) createQueryBufferDelegateMethod.CreateDelegate(typeof (Func<IQueryBuffer>), queryContextFactory);

            // REFLECTION: Query.Provider._queryCompiler._queryContextFactory._connection
            var connectionField = queryContextFactory.GetType().GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
            var connection = (IRelationalConnection) connectionField.GetValue(queryContextFactory);

            IQueryCompilationContextFactory queryCompilationContextFactory;
            object logger;

            var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            if(dependenciesProperty != null)
            {
                // EFCore 2.x
                
                isEFCore2x = true;
                var dependencies = dependenciesProperty.GetValue(database);

                var queryCompilationContextFactoryField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Storage.DatabaseDependencies")
                                                                           .GetProperty("QueryCompilationContextFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(dependencies);

                var dependenciesProperty2 = typeof(QueryCompilationContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                var dependencies2 = dependenciesProperty2.GetValue(queryCompilationContextFactory);

                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
                var loggerField =  typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.Internal.QueryCompilationContextDependencies")
                                                    .GetProperty("Logger", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                // (IInterceptingLogger<LoggerCategory.Query>)
                logger = loggerField.GetValue(dependencies2);
            }
            else
            {
                // EFCore 1.x
                
                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
                var queryCompilationContextFactoryField = typeof (Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
                queryCompilationContextFactory = (IQueryCompilationContextFactory) queryCompilationContextFactoryField.GetValue(database);

                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
                var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
                logger = loggerField.GetValue(queryCompilationContextFactory);
            }


            // CREATE connection
            {
                QueryConnection = context.Database.GetService<IRelationalConnection>();
               
                var innerConnection = new CreateEntityConnection(QueryConnection.DbConnection, null);
                var innerConnectionField = typeof(RelationalConnection).GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
                var initalConnection = innerConnectionField.GetValue(QueryConnection);

#if EFCORE_3X
                innerConnectionField.SetValue(QueryConnection, innerConnection);
#else
                innerConnectionField.SetValue(QueryConnection, LazyHelper.NewLazy<DbConnection>(() => innerConnection));
#endif

                RestoreConnection = () => innerConnectionField.SetValue(QueryConnection, initalConnection);
            }


            // CREATE query context
            {
                var relationalQueryContextType = typeof(RelationalQueryContext);
                var relationalQueryContextConstructor = relationalQueryContextType.GetConstructors()[0];

                // EF Core 1.1 preview
                if (isEFCore3x)
                {
                    // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.ExecutionStrategyFactory
                    var executionStrategyFactoryField = queryContextFactory.GetType().GetProperty("ExecutionStrategyFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    var executionStrategyFactory = executionStrategyFactoryField.GetValue(queryContextFactory);

                    var dependenciesProperty3 = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                    var dependencies3 = dependenciesProperty3.GetValue(queryContextFactory);

                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { dependencies3, createQueryBufferDelegate, QueryConnection, executionStrategyFactory });
                }
                else if (relationalQueryContextConstructor.GetParameters().Length == 5)
                {
                    // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.ExecutionStrategyFactory
                    var executionStrategyFactoryField = queryContextFactory.GetType().GetProperty("ExecutionStrategyFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    var executionStrategyFactory = executionStrategyFactoryField.GetValue(queryContextFactory);

#if !EFCORE_3X
                    var lazyRefStateManager = LazyHelper.NewLazy(() => stateManager);
                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { createQueryBufferDelegate, QueryConnection, lazyRefStateManager, concurrencyDetector, executionStrategyFactory });
#endif



                }
                else if(isEFCore2x)
                {
                    // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.ExecutionStrategyFactory
                    var executionStrategyFactoryField = queryContextFactory.GetType().GetProperty("ExecutionStrategyFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    var executionStrategyFactory = executionStrategyFactoryField.GetValue(queryContextFactory);

#if !EFCORE_3X
                    var lazyRefStateManager = LazyHelper.NewLazy(() => stateManager);

                    var dependenciesProperty3 = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                    var dependencies3 = dependenciesProperty3.GetValue(queryContextFactory);

                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { dependencies3, createQueryBufferDelegate, QueryConnection, executionStrategyFactory });
#endif
                }
                else
                {
                    queryContext = (RelationalQueryContext) relationalQueryContextConstructor.Invoke(new object[] {createQueryBufferDelegate, QueryConnection, stateManager, concurrencyDetector});
                }
            }


            Expression newQuery = null;

            if (isEFCore3x)
            {
#if EFCORE_3X
                var visitor = new ParameterExtractingExpressionVisitor(evaluatableExpressionFilter, queryContext, queryContext.GetType(), (IDiagnosticsLogger<DbLoggerCategory.Query>)logger, true, false);
                newQuery = visitor.ExtractParameters(Query.Expression);
#endif
                //var parameterExtractingExpressionVisitorConstructor = typeof(ParameterExtractingExpressionVisitor).GetConstructors().First(x => x.GetParameters().Length == 6);
                //var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, queryContext.GetType(), logger, true, false });
                //// CREATE new query from query visitor
                //newQuery = parameterExtractingExpressionVisitor.ExtractParameters(Query.Expression);
            }
            else if(isEFCore2x)
            {
                var parameterExtractingExpressionVisitorConstructors = typeof(ParameterExtractingExpressionVisitor).GetConstructors();

                if (parameterExtractingExpressionVisitorConstructors.Any(x => x.GetParameters().Length == 5))
                {
                    // EF Core 2.1
                    var parameterExtractingExpressionVisitorConstructor = parameterExtractingExpressionVisitorConstructors.First(x => x.GetParameters().Length == 5);
                    var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, logger, true, false });

                    // CREATE new query from query visitor
                    newQuery = parameterExtractingExpressionVisitor.ExtractParameters(Query.Expression);
                }
                else
                {

	                var parameterExtractingExpressionVisitorConstructor = parameterExtractingExpressionVisitorConstructors.First(x => x.GetParameters().Length == 6);

					ParameterExtractingExpressionVisitor parameterExtractingExpressionVisitor = null;

                    if (parameterExtractingExpressionVisitorConstructor.GetParameters().Where(x => x.ParameterType == typeof(DbContext)).Any())
					{
						parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, logger, context, true, false });
					}
					else
					{
						parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, logger, null, true, false });
					} 

                    // CREATE new query from query visitor
                    newQuery = parameterExtractingExpressionVisitor.ExtractParameters(Query.Expression);
                }
            }
            else
            {
                // CREATE new query from query visitor
                var extractParametersMethods = typeof(ParameterExtractingExpressionVisitor).GetMethod("ExtractParameters", BindingFlags.Public | BindingFlags.Static);
                newQuery = (Expression) extractParametersMethods.Invoke(null, new object[] {Query.Expression, queryContext, evaluatableExpressionFilter, logger});
            }

            // PARSE new query
            var queryModel = createQueryParser.GetParsedQuery(newQuery);

            // CREATE query model visitor
            var queryModelVisitor = (RelationalQueryModelVisitor) queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Create(false).CreateQueryModelVisitor().CreateQueryExecutor()
            var createQueryExecutorMethod = queryModelVisitor.GetType().GetMethod("CreateQueryExecutor");
            var createQueryExecutorMethodGeneric = createQueryExecutorMethod.MakeGenericMethod(Query.ElementType);
            var queryExecutor = createQueryExecutorMethodGeneric.Invoke(queryModelVisitor, new[] {queryModel});

            // SET value
            QueryExecutor = queryExecutor;
            QueryContext = queryContext;
            SelectExpression sqlQuery = null;

            if (queryModelVisitor.Queries.Count == 0)
            {
                var _subQueryModelVisitorsBySource = queryModelVisitor.GetType().GetField("_subQueryModelVisitorsBySource", BindingFlags.NonPublic | BindingFlags.Instance);
                var subQueryModelVisitorsBySources = (Dictionary<IQuerySource, RelationalQueryModelVisitor>)_subQueryModelVisitorsBySource.GetValue(queryModelVisitor);
                if (subQueryModelVisitorsBySources.Count == 1)
                {
                    sqlQuery = subQueryModelVisitorsBySources.First().Value.Queries.First();
                }
                else
                {
                    throw new Exception("More than one query has been found inside the same query.");
                }
            }
            else
            {
                sqlQuery = queryModelVisitor.Queries.First();
            }

            // RETURN the IRelationalCommand
#if EFCORE
            IRelationalCommand relationalCommand = null;
            var dynamicSqlGenerator = (dynamic)sqlQuery.CreateDefaultQuerySqlGenerator();

            if (isEFCore3x)
            {
                var commandBuilderFactory = context.Database.GetService<IRelationalCommandBuilderFactory>();
                
                // TODO: Fix null for DbLogger
                relationalCommand = (IRelationalCommand)dynamicSqlGenerator.GenerateSql(commandBuilderFactory, queryContext.ParameterValues, null);
            }
            else
            {
                relationalCommand = (IRelationalCommand)dynamicSqlGenerator.GenerateSql(queryContext.ParameterValues);
            }
            
#else
            var relationalCommand = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(queryContext.ParameterValues);
#endif

            return relationalCommand;
        }
#elif EFCORE_3X

        //private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        //private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        //private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        //private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        //private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        /// <summary>Creates executor and get command.</summary>
        /// <returns>The new executor and get command.</returns>
        public virtual IRelationalCommand CreateExecutorAndGetCommand(out RelationalQueryContext queryContext)
        {
            object compiledQueryOut;

            var relationalCommand = Query.EFPlusCreateCommand(queryable =>
            {
                var context = queryable.GetDbContext();

                QueryConnection = context.Database.GetService<IRelationalConnection>();

                var innerConnection = new CreateEntityConnection(QueryConnection.DbConnection, null);
                var innerConnectionField = typeof(RelationalConnection).GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
                var initalConnection = innerConnectionField.GetValue(QueryConnection);

                innerConnectionField.SetValue(QueryConnection, innerConnection);

                RestoreConnection = () => innerConnectionField.SetValue(QueryConnection, initalConnection);
            }, out queryContext, out compiledQueryOut);

            QueryContext = queryContext;
            CompiledQuery = compiledQueryOut;

            return relationalCommand;

            //var source = Query;

            //// CREATE connection
            //{
            //    var context = Query.GetDbContext();

            //    QueryConnection = context.Database.GetService<IRelationalConnection>();

            //    var innerConnection = new CreateEntityConnection(QueryConnection.DbConnection, null);
            //    var innerConnectionField = typeof(RelationalConnection).GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
            //    var initalConnection = innerConnectionField.GetValue(QueryConnection);

            //    innerConnectionField.SetValue(QueryConnection, innerConnection);

            //    RestoreConnection = () => innerConnectionField.SetValue(QueryConnection, initalConnection);
            //}

            //// REFLECTION: source.Provider._queryCompiler
            //var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            //var queryCompiler = queryCompilerField.GetValue(source.Provider);

            //// REFLECTION: queryCompiler._database
            //var databaseField = queryCompiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            //var database = (RelationalDatabase) databaseField.GetValue(queryCompiler);

            //// REFLECTION: queryCompiler._queryContextFactory().Create()
            //var queryContextFactoryField = queryCompiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            //var queryContextFactory = (IQueryContextFactory) queryContextFactoryField.GetValue(queryCompiler);
            //queryContext = (RelationalQueryContext) queryContextFactory.Create();

            //// REFLECTION: queryCompiler._evaluatableExpressionFilter
            //var evaluatableExpressionFilterField = typeof(QueryCompiler).GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            //var evaluatableExpressionFilter = (IEvaluatableExpressionFilter) evaluatableExpressionFilterField.GetValue(queryCompiler);

            //// REFLECTION: database.Dependencies
            //var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            //var dependencies = (DatabaseDependencies) dependenciesProperty.GetValue(database);

            //// queryCompilationContext
            //var queryCompilationContextFactory = dependencies.QueryCompilationContextFactory;
            //var queryCompilationContext = queryCompilationContextFactory.Create(false);

            //// parameterExtractingExpressionVisitor
            //var parameterExtractingExpressionVisitor = new ParameterExtractingExpressionVisitor(evaluatableExpressionFilter, queryContext, queryContext.GetType(), queryCompilationContext.Logger, true, false);

            //// CREATE new query from query visitor
            //var queryExpression = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);

            //// REFLECTION: database.CompileQuery<TResult>(queryExpression, false)
            //object compileQuery = null;

            //{
            //    // the code below somewhat replace the "CompileQuery" code like this:
            //    // var queryingEnumerableType = database.GetType().Assembly.GetType("Microsoft.EntityFrameworkCore.Query.RelationalShapedQueryCompilingExpressionVisitor+QueryingEnumerable`1").MakeGenericType(source.ElementType);
            //    // var compileQueryMethod = database.GetType().GetMethod("CompileQuery", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(queryingEnumerableType);
            //    // compileQuery = compileQueryMethod.Invoke(database, new object[] { queryExpression, false });
            //    var query = queryExpression;

            //    var queryCompilationFactory = (QueryCompilationContextFactory) dependencies.QueryCompilationContextFactory;
            //    var queryCompilation = queryCompilationFactory.Create(false);

            //    // get private stuff
            //    var _queryOptimizerFactory = (IQueryOptimizerFactory)queryCompilation.GetType().GetField("_queryOptimizerFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
            //    var _queryableMethodTranslatingExpressionVisitorFactory =
            //        (IQueryableMethodTranslatingExpressionVisitorFactory)queryCompilation.GetType().GetField("_queryableMethodTranslatingExpressionVisitorFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
            //    var _shapedQueryOptimizerFactory = (IShapedQueryOptimizerFactory)queryCompilation.GetType().GetField("_shapedQueryOptimizerFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
            //    var _shapedQueryCompilingExpressionVisitorFactory = (IShapedQueryCompilingExpressionVisitorFactory)queryCompilation.GetType().GetField("_shapedQueryCompilingExpressionVisitorFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
            //    var InsertRuntimeParametersMethod = queryCompilation.GetType().GetMethod("InsertRuntimeParameters", BindingFlags.NonPublic | BindingFlags.Instance);

            //    query = _queryOptimizerFactory.Create(queryCompilation).Visit(query);
            //    query = _queryableMethodTranslatingExpressionVisitorFactory.Create(queryCompilation.Model).Visit(query);
                
            //    // required, otherwise the compileQuery return directly the result and not the command
            //    {
            //        var shapedQuery = (Microsoft.EntityFrameworkCore.Query.ShapedQueryExpression)query;
            //        if (shapedQuery.ResultCardinality != ResultCardinality.Enumerable)
            //        {

            //            shapedQuery.ResultCardinality = ResultCardinality.Enumerable;
            //        }
            //    }

            //    query = _shapedQueryOptimizerFactory.Create(queryCompilation).Visit(query);
            //    query = _shapedQueryCompilingExpressionVisitorFactory.Create(queryCompilation).Visit(query);
            //    query = (Expression) InsertRuntimeParametersMethod.Invoke(queryCompilation, new object[] {query});

            //    var method = typeof(BaseQueryFuture).GetMethod("SelfCompile", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(query.Type);
            //    compileQuery = method.Invoke(null, new object[] {query, queryCompilation});
            //}
            
            //var compiledQuery = ((dynamic) compileQuery)(queryContext);

            //// REFLECTION: compiledQuery._selectExpression
            //var selectExpressionField = ((Type) compiledQuery.GetType()).GetField("_selectExpression", BindingFlags.NonPublic | BindingFlags.Instance);
            //var selectExpression = (SelectExpression) selectExpressionField.GetValue(compiledQuery);

            //// REFLECTION: compiledQuery._querySqlGeneratorFactory
            //var querySqlGeneratorFactoryField = ((Type) compiledQuery.GetType()).GetField("_querySqlGeneratorFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            //var querySqlGeneratorFactory = (IQuerySqlGeneratorFactory) querySqlGeneratorFactoryField.GetValue(compiledQuery);
            //var querySqlGenerator = querySqlGeneratorFactory.Create();

            //var command = querySqlGenerator.GetCommand(selectExpression);

            //CompiledQuery = compiledQuery;
            //QueryContext = queryContext;

            //return command;
        }

        //public virtual IRelationalCommand CreateExecutorAndGetCommand(out RelationalQueryContext queryContext)
        //{
        //    return Query.CreateCommand(out queryContext);
        //}
#endif
#endif

        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public virtual void SetResult(DbDataReader reader)
        {
        }

        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public IEnumerator<T> GetQueryEnumerator<T>(DbDataReader reader)
        {
#if EF5 || EF6
            // REFLECTION: Query.QueryState
            var queryStateProperty = Query.GetType().GetProperty("QueryState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var queryState = queryStateProperty.GetValue(Query, null);

            // REFLECTION: Query.QueryState.GetExecutionPlan(null)
            var getExecutionPlanMethod = queryState.GetType().GetMethod("GetExecutionPlan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getExecutionPlan = getExecutionPlanMethod.Invoke(queryState, new object[] { null });

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory
            var resultShaperFactoryField = getExecutionPlan.GetType().GetField("ResultShaperFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (resultShaperFactoryField == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }
            var resultShaperFactory = resultShaperFactoryField.GetValue(getExecutionPlan);

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters)
            var createMethod = resultShaperFactory.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if EF5
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, Query.MergeOption, false});
#elif EF6
            var create = createMethod.Invoke(resultShaperFactory, new object[] { reader, Query.Context, Query.Context.MetadataWorkspace, Query.MergeOption, false, true });
#endif

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters).GetEnumerator()
            var getEnumeratorMethod = create.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getEnumerator = getEnumeratorMethod.Invoke(create, new object[0]);

            var enumerator = (IEnumerator<T>)getEnumerator;
            return enumerator;
#elif EFCORE_2X
           
            ((CreateEntityConnection)QueryConnection.DbConnection).OriginalDataReader = reader;
            var queryExecutor = (Func<QueryContext, IEnumerable<T>>) QueryExecutor;
            var queryEnumerable = queryExecutor(QueryContext);
            return queryEnumerable.GetEnumerator();
#elif EFCORE_3X
            ((CreateEntityConnection)QueryConnection.DbConnection).OriginalDataReader = reader;
            var compiledQuery = CompiledQuery;
            var getEnumeratorMethod = compiledQuery.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getEnumerator = getEnumeratorMethod.Invoke(compiledQuery, new object[0]);
            var enumerator = (IEnumerator<T>) getEnumerator;

            {

#if EFCORE_5X

                {
                    //https://github.com/dotnet/efcore/blob/b970bf29a46521f40862a01db9e276e6448d3cb0/src/EFCore.Relational/Storage/RelationalCommand.cs#L380
                    //public virtual RelationalDataReader ExecuteReader(RelationalCommandParameterObject parameterObject)
          
                    //-columns
                    var fieldrelationalCommandCache = enumerator.GetType().GetField("_relationalCommandCache", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (fieldrelationalCommandCache != null)
				    {
                        var relationalCommandCache = fieldrelationalCommandCache.GetValue(enumerator);

                        if (relationalCommandCache != null && relationalCommandCache is RelationalCommandCache relationalCommand && relationalCommand.ReaderColumns != null)
                        { 
                            var fielCacheReaderColumns = relationalCommandCache.GetType().GetField("<ReaderColumns>k__BackingField", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            if (fielCacheReaderColumns != null)
                            {
                                fielCacheReaderColumns.SetValue(relationalCommand, null);
                            }

                        }
                    }
                }
#else
                // EFCore.Relational\Storage\RelationalCommandParameterObject.cs
                /// <param name="readerColumns"> The expected columns if the reader needs to be buffered, or null otherwise. </param>
                /// public RelationalCommandParameterObject(
                ///   // pour bloquer cette logique : 
                //
                // if (readerColumns != null)
                // {
                //     reader = new BufferedDataReader(reader).Initialize(readerColumns);
                // } 
                var fielReaderColumns = enumerator.GetType().GetField("_readerColumns", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (fielReaderColumns != null)
                {
	                fielReaderColumns.SetValue(enumerator, null);
                }
#endif
            }

            return enumerator;
            //object queryExecutor;
            //var queryEnumerable = queryExecutor(QueryContext);
            //return null;
            //return ((IQueryable)compiledQuery).GetEnumerator();
#endif
        }

        public virtual void GetResultDirectly()
        {

        }


#if NET45
		public virtual Task GetResultDirectlyAsync(CancellationToken cancellationToken)
	    {
		    throw new Exception("Not implemented");
	    }
#endif
	}
}