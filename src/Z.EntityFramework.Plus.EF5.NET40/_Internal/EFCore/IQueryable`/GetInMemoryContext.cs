// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || QUERY_FUTURE
#if EFCORE
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static DbContext GetInMemoryContext<T>(this IQueryable<T> source)
        {
            var compilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = (QueryCompiler) compilerField.GetValue(source.Provider);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = queryContextFactoryField.GetValue(compiler);

#if EFCORE
            var stateManagerField = typeof(QueryContextFactory).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManagerDynamic = stateManagerField.GetValue(queryContextFactory);

            IStateManager stateManager = stateManagerDynamic as IStateManager;
            if (stateManager == null)
            {
                stateManager = ((dynamic) stateManagerDynamic).Value;
            }
#else
            var stateManagerField = typeof (QueryContextFactory).GetField("_stateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (IStateManager) stateManagerField.GetValue(queryContextFactory);
#endif

            return stateManager.Context;
        }
    }
}

#endif
#endif