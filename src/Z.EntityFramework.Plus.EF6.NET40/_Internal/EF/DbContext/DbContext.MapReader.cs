// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL
using System;
using System.Collections.Generic;
using System.Data.Common;
using Z.EntityFramework.Plus;
#if EF5
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

#elif EFCORE
using System.Linq;
using System.Reflection;
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
    internal static partial class InternalExtensions
    {
        internal static IEnumerable<T> MapReader<T>(this DbContext context, DbDataReader reader) where T : class
        {
#if EF5 || EF6
        return ((IObjectContextAdapter) context).ObjectContext.Translate<T>(reader);

#elif EFCORE
            var list = new List<T>();

            var query = (IQueryable<T>) context.Set<T>();

            // REFLECTION: Query._context.StateManager
            var stateManagerProperty = typeof (DbContext).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (StateManager) stateManagerProperty.GetValue(context);

            // REFLECTION: Query._context.StateManager._concurrencyDetector
            var concurrencyDetectorField = typeof (StateManager).GetField("_concurrencyDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var concurrencyDetector = (IConcurrencyDetector) concurrencyDetectorField.GetValue(stateManager);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(query.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderField = queryCompiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeTypeProvider = nodeTypeProviderField.GetValue(queryCompiler);

            // REFLECTION: Query.Provider._queryCompiler.CreateQueryParser();
            var createQueryParserMethod = queryCompiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var createQueryParser = (QueryParser) createQueryParserMethod.Invoke(null, new[] {nodeTypeProvider});

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

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
            var queryCompilationContextFactoryField = typeof (Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompilationContextFactory = (IQueryCompilationContextFactory) queryCompilationContextFactoryField.GetValue(database);

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
            var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
            var logger = (ISensitiveDataLogger) loggerField.GetValue(queryCompilationContextFactory);

            // CREATE connection
            var queryConnection = new CreateEntityRelationConnection(connection);

            // CREATE query context
            RelationalQueryContext queryContext;
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

                    queryContext = (RelationalQueryContext)relationalQueryContextConstructor.Invoke(new object[] { createQueryBufferDelegate, connection, lazyRefStateManager, concurrencyDetector, executionStrategyFactory });
                }
                else
                {
                    queryContext = (RelationalQueryContext) relationalQueryContextConstructor.Invoke(new object[] {createQueryBufferDelegate, connection, stateManager, concurrencyDetector});
                }
            }

            // CREATE new query from query visitor
            var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(query.Expression, queryContext, evaluatableExpressionFilter, logger);

            // PARSE new query
            var queryModel = createQueryParser.GetParsedQuery(newQuery);

            // CREATE query model visitor
            var queryModelVisitor = (RelationalQueryModelVisitor) queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Create(false).CreateQueryModelVisitor().CreateQueryExecutor()
            var createQueryExecutorMethod = queryModelVisitor.GetType().GetMethod("CreateQueryExecutor");
            var createQueryExecutorMethodGeneric = createQueryExecutorMethod.MakeGenericMethod(query.ElementType);
            var queryExecutor = (Func<QueryContext, IEnumerable<T>>) createQueryExecutorMethodGeneric.Invoke(queryModelVisitor, new[] {queryModel});

            // CREATE a fake reader since EntityFramework close it
            queryConnection.OriginalDataReader = new CreateEntityDataReader(reader);
            var queryEnumerable = queryExecutor(queryContext);
            var enumerator = queryEnumerable.GetEnumerator();

            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
#endif
        }
    }
}
#endif