// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Data.Common;
#if EF5
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

#elif EF7
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.ExpressionVisitors.Internal;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;

#endif

public static partial class DbContextExtensions
{
    internal static IEnumerable<T> MapReader<T>(this DbContext context, DbDataReader reader) where T : class
    {
#if EF5 || EF6
        return ((IObjectContextAdapter) context).ObjectContext.Translate<T>(reader);

#elif EF7
        var list = new List<T>();

        var query = (IQueryable<T>) context.Set<T>();

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

        // CREATE connection
        var queryConnection = new CreateEntityRelationConnection(connection);

        // CREATE query context
        var queryContext = new RelationalQueryContext(createQueryBufferDelegate, queryConnection);

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