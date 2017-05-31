// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE
#if EFCORE
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static IRelationalCommand CreateCommand<T>(this IQueryable<T> source, out RelationalQueryContext queryContext)
        {
            var compilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = compilerField.GetValue(source.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderField = compiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeTypeProvider = nodeTypeProviderField.GetValue(compiler);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(compiler);

            queryContext = (RelationalQueryContext)queryContextFactory.Create();

            var evalutableExpressionFilterField = compiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);

            var evalutableExpressionFilter = (IEvaluatableExpressionFilter)evalutableExpressionFilterField.GetValue(null);
            var databaseField = compiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            var database = (IDatabase)databaseField.GetValue(compiler);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(source.Provider);

            // REFLECTION: Query.Provider._queryCompiler._evaluatableExpressionFilter
            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(null);

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
            var queryCompilationContextFactoryField = typeof(Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(database);

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
            var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
            var logger = (ISensitiveDataLogger)loggerField.GetValue(queryCompilationContextFactory);

            // CREATE new query from query visitor
            var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(source.Expression, queryContext, evaluatableExpressionFilter, logger);
            //var query = new QueryAnnotatingExpressionVisitor().Visit(source.Expression);
            //var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(query, queryContext, evalutableExpressionFilter);

            var queryParserMethod = compiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var queryparser = (QueryParser)queryParserMethod.Invoke(null, new [] { nodeTypeProvider });
            var queryModel = queryparser.GetParsedQuery(newQuery);



            var queryModelVisitor = (RelationalQueryModelVisitor)queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();
            var executor = queryModelVisitor.CreateQueryExecutor<T>(queryModel);

            var queries = queryModelVisitor.Queries;
            var sqlQuery = queries.ToList()[0];

            var command = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(queryContext.ParameterValues);

            return command;
        }

        public static IRelationalCommand CreateCommand(this IQueryable source, out RelationalQueryContext queryContext)
        {
            var compilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = compilerField.GetValue(source.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderField = compiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeTypeProvider = nodeTypeProviderField.GetValue(compiler);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(compiler);

            queryContext = (RelationalQueryContext)queryContextFactory.Create();

            var evalutableExpressionFilterField = compiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);

            var evalutableExpressionFilter = (IEvaluatableExpressionFilter)evalutableExpressionFilterField.GetValue(null);
            var databaseField = compiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            var database = (IDatabase)databaseField.GetValue(compiler);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(source.Provider);

            // REFLECTION: Query.Provider._queryCompiler._evaluatableExpressionFilter
            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(null);

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
            var queryCompilationContextFactoryField = typeof(Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(database);

            // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
            var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
            var logger = (ISensitiveDataLogger)loggerField.GetValue(queryCompilationContextFactory);

            // CREATE new query from query visitor
            var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(source.Expression, queryContext, evaluatableExpressionFilter, logger);
            //var query = new QueryAnnotatingExpressionVisitor().Visit(source.Expression);
            //var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(query, queryContext, evalutableExpressionFilter);

            var queryParserMethod = compiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var queryparser = (QueryParser)queryParserMethod.Invoke(null, new[] { nodeTypeProvider });
            var queryModel = queryparser.GetParsedQuery(newQuery);

            var queryModelVisitor = (RelationalQueryModelVisitor)queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();
            var createQueryExecutorMethod = queryModelVisitor.GetType().GetMethod("CreateQueryExecutor");
            var createQueryExecutorMethodGeneric = createQueryExecutorMethod.MakeGenericMethod(source.ElementType);
            createQueryExecutorMethodGeneric.Invoke(queryModelVisitor, new[] { queryModel });

            var queries = queryModelVisitor.Queries;
            var sqlQuery = queries.ToList()[0];


            var command = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(queryContext.ParameterValues);

            return command;
        }
    }
}

#endif
#endif