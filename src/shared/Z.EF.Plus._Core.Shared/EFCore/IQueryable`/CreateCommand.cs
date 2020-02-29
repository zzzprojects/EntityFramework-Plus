// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_CACHE
#if EFCORE
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#if EFCORE_2X
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
#endif

#if EFCORE_3X
using System;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
#endif

#if EFCORE_2X
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
#if EFCORE_2X
        //        public static IRelationalCommand CreateCommand4<T>(this IQueryable<T> source, out RelationalQueryContext queryContext)
        //        {
        //            var compilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var compiler = compilerField.GetValue(source.Provider);

        //            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
        //            var nodeTypeProviderField = compiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var nodeTypeProvider = nodeTypeProviderField.GetValue(compiler);

        //            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(compiler);

        //            queryContext = (RelationalQueryContext)queryContextFactory.Create();

        //            var evalutableExpressionFilterField = compiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);

        //            var evalutableExpressionFilter = (IEvaluatableExpressionFilter)evalutableExpressionFilterField.GetValue(null);
        //            var databaseField = compiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var database = (IDatabase)databaseField.GetValue(compiler);

        //            // REFLECTION: Query.Provider._queryCompiler
        //            var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var queryCompiler = queryCompilerField.GetValue(source.Provider);

        //            // REFLECTION: Query.Provider._queryCompiler._evaluatableExpressionFilter
        //            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
        //            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(null);

        //            Expression newQuery;
        //            IQueryCompilationContextFactory queryCompilationContextFactory;

        //            var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        //            if(dependenciesProperty != null)
        //            {
        //                var dependencies = dependenciesProperty.GetValue(database);

        //                var queryCompilationContextFactoryField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Storage.DatabaseDependencies")
        //                                                                           .GetProperty("QueryCompilationContextFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        //                queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(dependencies);

        //                var dependenciesProperty2 = typeof(QueryCompilationContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        //                var dependencies2 = dependenciesProperty2.GetValue(queryCompilationContextFactory);

        //                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
        //                var loggerField =  typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.Internal.QueryCompilationContextDependencies")
        //                                                    .GetProperty("Logger", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //                var logger = loggerField.GetValue(dependencies2);

        //                var parameterExtractingExpressionVisitorConstructor = typeof(ParameterExtractingExpressionVisitor).GetConstructors().First(x => x.GetParameters().Length == 5);

        //                var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] {evaluatableExpressionFilter, queryContext, logger, true, false} );

        //                // CREATE new query from query visitor
        //                newQuery = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);
        //            }
        //            else
        //            {
        //                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
        //                var queryCompilationContextFactoryField = typeof(Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //                queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(database);

        //                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
        //                var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
        //                var logger = loggerField.GetValue(queryCompilationContextFactory);

        //                // CREATE new query from query visitor
        //                var extractParametersMethods = typeof(ParameterExtractingExpressionVisitor).GetMethod("ExtractParameters", BindingFlags.Public | BindingFlags.Static);
        //                newQuery = (Expression) extractParametersMethods.Invoke(null, new object[] {source.Expression, queryContext, evaluatableExpressionFilter, logger});
        //            }

        //            //var query = new QueryAnnotatingExpressionVisitor().Visit(source.Expression);
        //            //var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(query, queryContext, evalutableExpressionFilter);

        //            var queryParserMethod = compiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
        //            var queryparser = (QueryParser)queryParserMethod.Invoke(null, new [] { nodeTypeProvider });
        //            var queryModel = queryparser.GetParsedQuery(newQuery);



        //            var queryModelVisitor = (RelationalQueryModelVisitor)queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();
        //            var executor = queryModelVisitor.CreateQueryExecutor<T>(queryModel);

        //            var queries = queryModelVisitor.Queries;
        //            var sqlQuery = queries.ToList()[0];

        //#if EFCORE_3X
        //            var commandBuilderFactory = queryContext.Context.Database.GetService<IRelationalCommandBuilderFactory>();
        //            var command = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(commandBuilderFactory, queryContext.ParameterValues, null);
        //#else
        //            var command = sqlQuery.CreateDefaultQuerySqlGenerator().GenerateSql(queryContext.ParameterValues);
        //#endif

        //            return command;
        //        }

        public static IRelationalCommand CreateCommand(this IQueryable source, out RelationalQueryContext queryContext)
        {
            bool EFCore_2_1 = false;
#if EFCORE
            bool isEFCore3x = EFCoreHelper.IsVersion3x;
#endif

            var compilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = compilerField.GetValue(source.Provider);

            // REFLECTION: Query.Provider.NodeTypeProvider (Use property for nullable logic)
            var nodeTypeProviderProperty = compiler.GetType().GetProperty("NodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);

            object nodeTypeProvider;
            object QueryModelGenerator = null;

            if (nodeTypeProviderProperty == null)
            {
                EFCore_2_1 = true;

                var QueryModelGeneratorField = compiler.GetType().GetField("_queryModelGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
                QueryModelGenerator = QueryModelGeneratorField.GetValue(compiler);

                var nodeTypeProviderField = QueryModelGenerator.GetType().GetField("_nodeTypeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
                nodeTypeProvider = nodeTypeProviderField.GetValue(QueryModelGenerator);
            }
            else
            {
                nodeTypeProvider = nodeTypeProviderProperty.GetValue(compiler);
            } 

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(compiler);

            queryContext = (RelationalQueryContext)queryContextFactory.Create();

            var databaseField = compiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
            var database = (IDatabase)databaseField.GetValue(compiler);

            // REFLECTION: Query.Provider._queryCompiler
            var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryCompiler = queryCompilerField.GetValue(source.Provider);

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
               evaluatableExpressionFilter = (IEvaluatableExpressionFilter) compiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompiler);
           }
#else
            var evalutableExpressionFilterField = compiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evalutableExpressionFilter = (IEvaluatableExpressionFilter)evalutableExpressionFilterField.GetValue(null);

            var evaluatableExpressionFilterField = queryCompiler.GetType().GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Static);
            var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(null);
#endif

            Expression newQuery;
            IQueryCompilationContextFactory queryCompilationContextFactory;

            var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            if(dependenciesProperty != null)
            {
                var dependencies = dependenciesProperty.GetValue(database);

                var queryCompilationContextFactoryField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Storage.DatabaseDependencies")
                                                                           .GetProperty("QueryCompilationContextFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(dependencies);

                var dependenciesProperty2 = typeof(QueryCompilationContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                var dependencies2 = dependenciesProperty2.GetValue(queryCompilationContextFactory);

                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
                var loggerField =  typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.Internal.QueryCompilationContextDependencies")
                                                    .GetProperty("Logger", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var logger = loggerField.GetValue(dependencies2);

                var parameterExtractingExpressionVisitorConstructors = typeof(ParameterExtractingExpressionVisitor).GetConstructors();

                if (isEFCore3x)
                {
                    var parameterExtractingExpressionVisitorConstructor = typeof(ParameterExtractingExpressionVisitor).GetConstructors().First(x => x.GetParameters().Length == 6);

                    var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, queryContext.GetType(), logger, true, false });

                    // CREATE new query from query visitor
                    newQuery = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);
                }
                else if (parameterExtractingExpressionVisitorConstructors.Any(x => x.GetParameters().Length == 5))
                {
                    // EF Core 2.1
                    var parameterExtractingExpressionVisitorConstructor = parameterExtractingExpressionVisitorConstructors.First(x => x.GetParameters().Length == 5);
                    var parameterExtractingExpressionVisitor = (ParameterExtractingExpressionVisitor)parameterExtractingExpressionVisitorConstructor.Invoke(new object[] { evaluatableExpressionFilter, queryContext, logger, true, false });

                    // CREATE new query from query visitor
                    newQuery = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);
                }
                else
                {
                    // EF Core 2.1 Preview 2. 
                    var parameterExtractingExpressionVisitorConstructor = parameterExtractingExpressionVisitorConstructors.First(x => x.GetParameters().Length == 6);
                    var _context = queryContext.GetType().GetProperty("Context", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    var context = _context.GetValue(queryContext);

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
                    newQuery = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);
                }  
            }
            else
            {
                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory
                var queryCompilationContextFactoryField = typeof(Database).GetField("_queryCompilationContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
                queryCompilationContextFactory = (IQueryCompilationContextFactory)queryCompilationContextFactoryField.GetValue(database);

                // REFLECTION: Query.Provider._queryCompiler._database._queryCompilationContextFactory.Logger
                var loggerField = queryCompilationContextFactory.GetType().GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
                var logger = loggerField.GetValue(queryCompilationContextFactory);

                // CREATE new query from query visitor
                var extractParametersMethods = typeof(ParameterExtractingExpressionVisitor).GetMethod("ExtractParameters", BindingFlags.Public | BindingFlags.Static);
                newQuery = (Expression) extractParametersMethods.Invoke(null, new object[] {source.Expression, queryContext, evaluatableExpressionFilter, logger});
                //ParameterExtractingExpressionVisitor.ExtractParameters(source.Expression, queryContext, evaluatableExpressionFilter, logger);
            }

            //var query = new QueryAnnotatingExpressionVisitor().Visit(source.Expression);
            //var newQuery = ParameterExtractingExpressionVisitor.ExtractParameters(query, queryContext, evalutableExpressionFilter);

#if NETSTANDARD2_0
            QueryParser queryparser = null;
            if (EFCore_2_1)
            {
                var queryParserMethod = QueryModelGenerator.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Instance);
                queryparser = (QueryParser)queryParserMethod.Invoke(QueryModelGenerator, new[] { nodeTypeProvider });
            }
            else
            {
                var queryParserMethod = compiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Instance);
                queryparser = (QueryParser)queryParserMethod.Invoke(compiler, new[] { nodeTypeProvider });
            }
#else
            var queryParserMethod = compiler.GetType().GetMethod("CreateQueryParser", BindingFlags.NonPublic | BindingFlags.Static);
            var queryparser = (QueryParser)queryParserMethod.Invoke(null, new[] { nodeTypeProvider });
#endif
            var queryModel = queryparser.GetParsedQuery(newQuery);

            var queryModelVisitor = (RelationalQueryModelVisitor)queryCompilationContextFactory.Create(false).CreateQueryModelVisitor();
            var createQueryExecutorMethod = queryModelVisitor.GetType().GetMethod("CreateQueryExecutor");
            var createQueryExecutorMethodGeneric = createQueryExecutorMethod.MakeGenericMethod(source.ElementType);
            createQueryExecutorMethodGeneric.Invoke(queryModelVisitor, new[] { queryModel });

			var isRequiresClientEval = (bool)queryModelVisitor.GetType().GetProperty("RequiresClientEval", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientSelectMany = (bool)queryModelVisitor.GetType().GetField("_requiresClientSelectMany", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientJoin = (bool)queryModelVisitor.GetType().GetField("_requiresClientJoin", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientFilter = (bool)queryModelVisitor.GetType().GetField("_requiresClientFilter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientProjection = (bool)queryModelVisitor.GetType().GetField("_requiresClientProjection", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientOrderBy = (bool)queryModelVisitor.GetType().GetField("_requiresClientOrderBy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);
			var isRequiresClientResultOperator = (bool)queryModelVisitor.GetType().GetField("_requiresClientResultOperator", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryModelVisitor);

			if (isRequiresClientEval || isRequiresClientSelectMany || isRequiresClientJoin || isRequiresClientFilter || isRequiresClientProjection || isRequiresClientOrderBy || isRequiresClientResultOperator)
			{
				throw new Exception(ExceptionMessage.Unexpected_ClientSideEvaluation);
			} 

			var queries = queryModelVisitor.Queries;
            var sqlQuery = queries.ToList()[0];

            IRelationalCommand relationalCommand = null;
            var dynamicSqlGenerator = (dynamic)sqlQuery.CreateDefaultQuerySqlGenerator();

            if (isEFCore3x)
            {
                var commandBuilderFactory = queryContext.Context.Database.GetService<IRelationalCommandBuilderFactory>();
                // TODO: Fix null for DbLogger
                relationalCommand = (IRelationalCommand)dynamicSqlGenerator.GenerateSql(commandBuilderFactory, queryContext.ParameterValues, null);
            }
            else
            {
                relationalCommand = (IRelationalCommand)dynamicSqlGenerator.GenerateSql(queryContext.ParameterValues);
            }

            return relationalCommand;
        }

#elif EFCORE_3X
        //public static IRelationalCommand CreateCommand(this IQueryable source, out RelationalQueryContext queryContext)
        //{
        //    // REFLECTION: source.Provider._queryCompiler
        //    var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var queryCompiler = queryCompilerField.GetValue(source.Provider);

        //    // REFLECTION: queryCompiler._database
        //    var databaseField = queryCompiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var database = (RelationalDatabase)databaseField.GetValue(queryCompiler);

        //    // REFLECTION: queryCompiler._queryContextFactory().Create()
        //    var queryContextFactoryField = queryCompiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(queryCompiler);
        //    queryContext = (RelationalQueryContext)queryContextFactory.Create();

        //    // REFLECTION: queryCompiler._evaluatableExpressionFilter
        //    var evaluatableExpressionFilterField = typeof(QueryCompiler).GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(queryCompiler);

        //    // REFLECTION: database.Dependencies
        //    var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var dependencies = (DatabaseDependencies)dependenciesProperty.GetValue(database);

        //    // queryCompilationContext
        //    var queryCompilationContextFactory = dependencies.QueryCompilationContextFactory;
        //    var queryCompilationContext = queryCompilationContextFactory.Create(false);

        //    // parameterExtractingExpressionVisitor
        //    var parameterExtractingExpressionVisitor = new ParameterExtractingExpressionVisitor(evaluatableExpressionFilter, queryContext, queryContext.GetType(), queryCompilationContext.Logger, true, false);

        //    // CREATE new query from query visitor
        //    var queryExpression = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);

        //    // REFLECTION: database.CompileQuery<TResult>(queryExpression, false)
        //    object compileQuery = null;

        //    {
        //        // the code below somewhat replace the "CompileQuery" code like this:
        //        // var queryingEnumerableType = database.GetType().Assembly.GetType("Microsoft.EntityFrameworkCore.Query.RelationalShapedQueryCompilingExpressionVisitor+QueryingEnumerable`1").MakeGenericType(source.ElementType);
        //        // var compileQueryMethod = database.GetType().GetMethod("CompileQuery", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(queryingEnumerableType);
        //        // compileQuery = compileQueryMethod.Invoke(database, new object[] { queryExpression, false });
        //        var query = queryExpression;

        //        var queryCompilationFactory = (QueryCompilationContextFactory)dependencies.QueryCompilationContextFactory;
        //        var queryCompilation = queryCompilationFactory.Create(false);

        //        // get private stuff
        //        var _queryOptimizerFactory = (IQueryOptimizerFactory)queryCompilation.GetType().GetField("_queryOptimizerFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
        //        var _queryableMethodTranslatingExpressionVisitorFactory =
        //            (IQueryableMethodTranslatingExpressionVisitorFactory)queryCompilation.GetType().GetField("_queryableMethodTranslatingExpressionVisitorFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
        //        var _shapedQueryOptimizerFactory = (IShapedQueryOptimizerFactory)queryCompilation.GetType().GetField("_shapedQueryOptimizerFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
        //        var _shapedQueryCompilingExpressionVisitorFactory = (IShapedQueryCompilingExpressionVisitorFactory)queryCompilation.GetType().GetField("_shapedQueryCompilingExpressionVisitorFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryCompilation);
        //        var InsertRuntimeParametersMethod = queryCompilation.GetType().GetMethod("InsertRuntimeParameters", BindingFlags.NonPublic | BindingFlags.Instance);

        //        query = _queryOptimizerFactory.Create(queryCompilation).Visit(query);
        //        query = _queryableMethodTranslatingExpressionVisitorFactory.Create(queryCompilation.Model).Visit(query);

        //        // required, otherwise the compileQuery return directly the result and not the command
        //        {
        //            var shapedQuery = (Microsoft.EntityFrameworkCore.Query.ShapedQueryExpression)query;
        //            if (shapedQuery.ResultCardinality != ResultCardinality.Enumerable)
        //            {

        //                shapedQuery.ResultCardinality = ResultCardinality.Enumerable;
        //            }
        //        }

        //        query = _shapedQueryOptimizerFactory.Create(queryCompilation).Visit(query);
        //        query = _shapedQueryCompilingExpressionVisitorFactory.Create(queryCompilation).Visit(query);
        //        query = (Expression)InsertRuntimeParametersMethod.Invoke(queryCompilation, new object[] { query });

        //        var method = typeof(BaseQueryFuture).GetMethod("SelfCompile", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(query.Type);
        //        compileQuery = method.Invoke(null, new object[] { query, queryCompilation });
        //    }

        //    var compiledQuery = ((dynamic)compileQuery)(queryContext);

        //    // REFLECTION: compiledQuery._selectExpression
        //    var selectExpressionField = ((Type)compiledQuery.GetType()).GetField("_selectExpression", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var selectExpression = (SelectExpression)selectExpressionField.GetValue(compiledQuery);

        //    // REFLECTION: compiledQuery._querySqlGeneratorFactory
        //    var querySqlGeneratorFactoryField = ((Type)compiledQuery.GetType()).GetField("_querySqlGeneratorFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var querySqlGeneratorFactory = (IQuerySqlGeneratorFactory)querySqlGeneratorFactoryField.GetValue(compiledQuery);
        //    var querySqlGenerator = querySqlGeneratorFactory.Create();

        //    var command = querySqlGenerator.GetCommand(selectExpression);

        //    return command;

        //    //queryContext = null;

        //    //// REFLECTION: source.Provider._queryCompiler
        //    //var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var queryCompiler = queryCompilerField.GetValue(source.Provider);

        //    //// REFLECTION: queryCompiler._database
        //    //var databaseField = queryCompiler.GetType().GetField("_database", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var database = (RelationalDatabase)databaseField.GetValue(queryCompiler);

        //    //// REFLECTION: queryCompiler._queryContextFactory().Create()
        //    //var queryContextFactoryField = queryCompiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var queryContextFactory = (IQueryContextFactory)queryContextFactoryField.GetValue(queryCompiler);
        //    //queryContext = (RelationalQueryContext)queryContextFactory.Create();

        //    //// REFLECTION: queryCompiler._evaluatableExpressionFilter
        //    //var evaluatableExpressionFilterField = typeof(QueryCompiler).GetField("_evaluatableExpressionFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var evaluatableExpressionFilter = (IEvaluatableExpressionFilter)evaluatableExpressionFilterField.GetValue(queryCompiler);

        //    //// REFLECTION: database.Dependencies
        //    //var dependenciesProperty = typeof(Database).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var dependencies = (DatabaseDependencies)dependenciesProperty.GetValue(database);

        //    //// queryCompilationContext
        //    //var queryCompilationContextFactory = dependencies.QueryCompilationContextFactory;
        //    //var queryCompilationContext = queryCompilationContextFactory.Create(false);


        //    ////var test1 = queryCompilationContext.GetType().GetField("_queryableMethodTranslatingExpressionVisitorFactory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    ////var test2 = (IQueryableMethodTranslatingExpressionVisitorFactory)test1.GetValue(queryCompilationContext);

        //    ////var t3 = queryCompilationContext.CreateQueryExecutor<int>(source.Expression);
        //    ////var q2 = test2.Create(queryCompilationContext.Model).Visit(source.Expression);
        //    ////var test3 = test2.Create(queryCompilationContext.Model).TranslateSubquery(q2);

        //    //// parameterExtractingExpressionVisitor
        //    //var parameterExtractingExpressionVisitor = new ParameterExtractingExpressionVisitor(evaluatableExpressionFilter, queryContext, queryContext.GetType(), queryCompilationContext.Logger, true, true);

        //    //// CREATE new query from query visitor
        //    //var queryExpression = parameterExtractingExpressionVisitor.ExtractParameters(source.Expression);

        //    //// REFLECTION: database.CompileQuery<TResult>(queryExpression, false)

        //    //object compileQuery = null;

        //    //try
        //    //{
        //    //    var queryingEnumerableType = database.GetType().Assembly.GetType("Microsoft.EntityFrameworkCore.Query.RelationalShapedQueryCompilingExpressionVisitor+QueryingEnumerable`1").MakeGenericType(source.ElementType);
        //    //    var compileQueryMethod = database.GetType().GetMethod("CompileQuery", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(queryingEnumerableType);
        //    //    compileQuery = compileQueryMethod.Invoke(database, new object[] { queryExpression, false });
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    var compileQueryMethod = database.GetType().GetMethod("CompileQuery", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(source.ElementType);
        //    //    compileQuery = compileQueryMethod.Invoke(database, new object[] { queryExpression, false });
        //    //}


        //    //// DYNAMIC: compileQuery(queryContext);
        //    //var compiledQuery = ((dynamic)compileQuery)(queryContext);

        //    //// REFLECTION: compiledQuery._selectExpression
        //    //var selectExpressionField = ((Type)compiledQuery.GetType()).GetField("_selectExpression", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var selectExpression = (SelectExpression)selectExpressionField.GetValue(compiledQuery);

        //    //// REFLECTION: compiledQuery._querySqlGeneratorFactory
        //    //var querySqlGeneratorFactoryField = ((Type)compiledQuery.GetType()).GetField("_querySqlGeneratorFactory", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //var querySqlGeneratorFactory = (IQuerySqlGeneratorFactory)querySqlGeneratorFactoryField.GetValue(compiledQuery);
        //    //var querySqlGenerator = querySqlGeneratorFactory.Create();

        //    //var command = querySqlGenerator.GetCommand(selectExpression);

        //    //return command;
        //}
#endif
    }
}

#endif
#endif