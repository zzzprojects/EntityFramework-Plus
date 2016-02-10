// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EFCORE

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.ExpressionVisitors.Internal;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

namespace Z.EntityFramework.Plus
{
    public static partial class IQueryableExtensions
    {
        public static IRelationalCommand GetDbCommand<T>(this IQueryable<T> query)
        {
            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(query.Provider);

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

            // CREATE query context
            var queryContext = new RelationalQueryContext(createQueryBufferDelegate, connection);

            // CREATE a query visitor
            var queryVisitor = new QueryAnnotatingExpressionVisitor().Visit(query.Expression);

            // CREATE new query from query visitor
            var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(queryVisitor, queryContext, evaluatableExpressionFilter);

            // PARSE new query
            var queryModel = createQueryParser.GetParsedQuery(newQuery);

            // CREATE query model visitor
            var queryModelVisitor = (RelationalQueryModelVisitor) queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Create(false).CreateQueryModelVisitor().CreateQueryExecutor()
            var createQueryExecutorMethod = queryModelVisitor.GetType().GetMethod("CreateQueryExecutor");
            var createQueryExecutorMethodGeneric = createQueryExecutorMethod.MakeGenericMethod(query.ElementType);
            createQueryExecutorMethodGeneric.Invoke(queryModelVisitor, new[] {queryModel});

            // RETURN the IRealationCommand
            var sqlQuery = queryModelVisitor.Queries.First();
            var relationalCommand = sqlQuery.CreateGenerator().GenerateSql(queryContext.ParameterValues);
            return relationalCommand;
        }
    }
}

#endif