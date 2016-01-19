// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EF7
using System.Linq;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.ExpressionVisitors.Internal;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>Interace for QueryFuture class.</summary>
    public abstract class BaseQueryFuture
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
#elif EF7
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
        internal CreateEntityRelationConnection QueryConnection { get; set; }

        /// <summary>Creates executor and get command.</summary>
        /// <returns>The new executor and get command.</returns>
        public virtual IRelationalCommand CreateExecutorAndGetCommand()
        {
            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(Query.Provider);

            // REFLECTION: Query.Provider._queryCompiler.CreateQueryParser();
            var createQueryParserMethod = queryCompiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var createQueryParser = (QueryParser) createQueryParserMethod.Invoke(null, new object[0]);

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

            // CREATE connection
            QueryConnection = new CreateEntityRelationConnection(connection);

            // CREATE query context
            var queryContext = new RelationalQueryContext(createQueryBufferDelegate, QueryConnection);

            // CREATE a query visitor
            var queryVisitor = new QueryAnnotatingExpressionVisitor().Visit(Query.Expression);

            // CREATE new query from query visitor
            var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(queryVisitor, queryContext, evaluatableExpressionFilter);

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
            var relationalCommand = sqlQuery.CreateGenerator().GenerateSql(queryContext.ParameterValues);
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
            var getExecutionPlan = getExecutionPlanMethod.Invoke(queryState, new object[] {null});

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
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false});
#elif EF6
            var create = createMethod.Invoke(resultShaperFactory, new object[] {reader, Query.Context, Query.Context.MetadataWorkspace, MergeOption.AppendOnly, false, true});
#endif

            // REFLECTION: Query.QueryState.GetExecutionPlan(null).ResultShaperFactory.Create(parameters).GetEnumerator()
            var getEnumeratorMethod = create.GetType().GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var getEnumerator = getEnumeratorMethod.Invoke(create, new object[0]);

            var enumerator = (IEnumerator<T>) getEnumerator;
            return enumerator;
#elif EF7
            QueryConnection.OriginalDataReader = reader;
            var queryExecutor = (Func<QueryContext, IEnumerable<T>>) QueryExecutor;
            var queryEnumerable = queryExecutor(QueryContext);
            return queryEnumerable.GetEnumerator();
#endif
        }
    }
}