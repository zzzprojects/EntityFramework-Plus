using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A query interceptor filter database command tree.</summary>
    public class QueryFilterInterceptorDbCommandTree : IDbCommandTreeInterceptor
    {
        public static ConstructorInfo DbQueryCommandTreeConstructor = typeof(DbQueryCommandTree).GetConstructor(new Type[] { typeof(MetadataWorkspace), typeof(DataSpace), typeof(DbExpression), typeof(bool), typeof(bool) });
        /// <summary>
        ///     This method is called after a new
        ///     <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCommandTree" /> has been created.
        ///     The tree that is used after interception can be changed by setting
        ///     <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandTreeInterceptionContext.Result" />
        ///     while intercepting.
        /// </summary>
        /// <param name="interceptionContext">Contextual information associated with the call.</param>
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
      
            var dbQueryCommandTree = interceptionContext.Result as DbQueryCommandTree;
            if (dbQueryCommandTree != null && interceptionContext.DbContexts.Count() == 1)
            {
                var context = interceptionContext.DbContexts.First();

                // Visit first to find filter ID && hook
                var visitorFilter = new QueryFilterInterceptorDbFilterExpression();
                var queryFiltered = dbQueryCommandTree.Query.Accept(visitorFilter);

                if (!string.IsNullOrEmpty(visitorFilter.HookID))
                {
                    if (!QueryFilterManager.DbExpressionByHook.ContainsKey(visitorFilter.HookID))
                    {
                        QueryFilterManager.DbExpressionByHook.TryAdd(visitorFilter.HookID, queryFiltered);
                    }
                }
                else
                {
                    var filterByContext = QueryFilterManager.AddOrGetFilterContext(context);
                    filterByContext.ClearCacheRequired = true;

                    var filterQuery = new QueryFilterInterceptorApply
                    {
                        InstanceFilters = filterByContext
                    };

                    if (visitorFilter.FilterID != null && visitorFilter.FilterID.Count > 0)
                    {
                        foreach (var filter in visitorFilter.FilterID)
                        {
                            if (filter == QueryFilterManager.DisableAllFilter)
                            {
                                // Disable all filter in the context!
                                filterQuery.ApplyFilterList.Add(interceptorFilter => false);
                            }
                            else if (filter.StartsWith(QueryFilterManager.EnableFilterById, StringComparison.InvariantCulture))
                            {
                                // Enable all specific filter
                                var filters = filter.Substring(QueryFilterManager.EnableFilterById.Length).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                                if (filters.Length == 0)
                                {
                                    filterQuery.ApplyFilterList.Add(interceptorFilter => false);
                                }
                                foreach (var applyFilter in filters)
                                {
                                    filterQuery.ApplyFilterList.Add(interceptorFilter => interceptorFilter.UniqueKey.ToString() == applyFilter ? true : (bool?)null);
                                }
                            }
                        }
                    }

                    // VISIT filter
                    var visitor = new QueryFilterInterceptorDbScanExpression
                    {
                        Context = context,
                        InstanceFilterContext = filterByContext,
                        FilterQuery = filterQuery
                    };

                    var newQuery = queryFiltered.Accept(visitor);

                    // CREATE a new Query
                    DbQueryCommandTree commandTree;
                    if (DbQueryCommandTreeConstructor != null)
                    {
                        commandTree = (DbQueryCommandTree)DbQueryCommandTreeConstructor.Invoke(new object[] { dbQueryCommandTree.MetadataWorkspace, dbQueryCommandTree.DataSpace, newQuery, true, interceptionContext.OriginalResult.DataSpace != DataSpace.CSpace });
                    }
                    else
                    {
                        commandTree = new DbQueryCommandTree(dbQueryCommandTree.MetadataWorkspace, dbQueryCommandTree.DataSpace, newQuery, true);

                    }

                    interceptionContext.Result = commandTree;
                }
            }
        }
    }
}