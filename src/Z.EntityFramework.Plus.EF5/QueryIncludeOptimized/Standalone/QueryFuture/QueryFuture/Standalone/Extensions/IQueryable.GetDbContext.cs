// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.


#if STANDALONE && EF7
using System.Linq;
using System.Reflection;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.Internal;

namespace Z.EntityFramework.Plus
{
    internal static partial class QueryFutureExtensions
    {
        internal static DbContext GetDbContext(this IQueryable source)
        {
            var compilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = (QueryCompiler)compilerField.GetValue(source.Provider);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (RelationalQueryContextFactory)queryContextFactoryField.GetValue(compiler);

            var stateManagerField = typeof(QueryContextFactory).GetField("_stateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (IStateManager)stateManagerField.GetValue(queryContextFactory);

            return stateManager.Context;
        }
    }
}
#endif