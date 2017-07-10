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
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Interace for QueryFuture class.</summary>
#if QUERY_INCLUDEOPTIMIZED
    internal abstract class BaseQueryFuture
#else
    public abstract class BaseQueryFuture
#endif
    {
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

        public virtual void ExecuteInMemory()
        {
            
        }

        /// <summary>Creates executor and get command.</summary>
        /// <returns>The new executor and get command.</returns>
        public virtual IRelationalCommand CreateExecutorAndGetCommand(out RelationalQueryContext queryContext)
        {
            bool isEFCore2x = false;

            var context = Query.GetDbContext();

            // REFLECTION: Query._context.StateManager
            var stateManagerProperty = typeof(DbContext).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (StateManager)stateManagerProperty.GetValue(context);

            // REFLECTION: Query._context.StateManager._concurrencyDetector
            var concurrencyDetectorField = typeof(StateManager).GetField("_concurrencyDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var concurrencyDetector = (IConcurrencyDetector)concurrencyDetectorField.GetValue(stateManager);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(Query.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderField = queryCompiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeTypeProvider = nodeTypeProviderField.GetValue(queryCompiler);

            // REFLECTION: Query.Provider._queryCompiler.CreateQueryParser();
            var createQueryParserMethod = queryCompiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var createQueryParser = (QueryParser)createQueryParserMethod.Invoke(null, new[] { nodeTypeProvider });

            // REFLECTION: Query.Provider._queryCompiler._database
            var databaseField = queryCompiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            var database = (IDatabase) databaseField.GetValue(queryCompiler);

            // REFLECTION: Query.Provider._queryCompiler._evaluatableExpressionFilter
            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter) evaluatableExpressionFilterField.GetValue(null);

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
                innerConnectionField.SetValue(QueryConnection, new Microsoft.EntityFrameworkCore.Internal.LazyRef<DbConnection>(() => innerConnection));
            }


            // CREATE query context
            {
                var relationalQueryContextType = typeof(RelationalQueryContext);
                var relationalQueryContextConstructor = relationalQueryContextType.GetConstructors()[0];

                // EF Core 1.1 preview
                if (relationalQueryContextConstructor.GetParameters().Length == 5)
                {
                    // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.ExecutionStrategyFactory
                    var executionStrategyFactoryField = queryContextFactory.GetType().GetProperty("ExecutionStrategyFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    var executionStrategyFactory = executionStrategyFactoryField.GetValue(queryContextFactory);

                    var lazyRefStateManager = new LazyRef<IStateManager>(() => stateManager);

                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { createQueryBufferDelegate, QueryConnection, lazyRefStateManager, concurrencyDetector, executionStrategyFactory });
                }
                else if(isEFCore2x)
                {
                    // REFLECTION: Query.Provider._queryCompiler._queryContextFactory.ExecutionStrategyFactory
                    var executionStrategyFactoryField = queryContextFactory.GetType().GetProperty("ExecutionStrategyFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    var executionStrategyFactory = executionStrategyFactoryField.GetValue(queryContextFactory);

                    var lazyRefStateManager = new LazyRef<IStateManager>(() => stateManager);

                    var dependenciesProperty3 = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                    var dependencies3 = dependenciesProperty3.GetValue(queryContextFactory);

                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { dependencies3, createQueryBufferDelegate, QueryConnection, executionStrategyFactory });
                }
                else
                {
                    queryContext = (RelationalQueryContext) relationalQueryContextConstructor.Invoke(new object[] {createQueryBufferDelegate, QueryConnection, stateManager, concurrencyDetector});
                }
            }



            Expression newQuery;

            if(isEFCore2x)
            {
                var parameterExtractingExpressionVisitorConstructor = typeof(ParameterExtractingExpressionVisitor).GetConstructors().First(x => x.GetParameters().Length == 5);

                var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] {evaluatableExpressionFilter, queryContext, logger, false, false} );
            
                // CREATE new query from query visitor
                newQuery = parameterExtractingExpressionVisitor.ExtractParameters(Query.Expression);
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

            // RETURN the IRealationCommand
            var sqlQuery = queryModelVisitor.Queries.First();
            var relationalCommand = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(queryContext.ParameterValues);
            return relationalCommand;
        }
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
#elif EFCORE
            ((CreateEntityConnection)QueryConnection.DbConnection).OriginalDataReader = reader;
            var queryExecutor = (Func<QueryContext, IEnumerable<T>>) QueryExecutor;
            var queryEnumerable = queryExecutor(QueryContext);
            return queryEnumerable.GetEnumerator();
#endif
        }

        public virtual void GetResultDirectly()
        {

        }
    }
}