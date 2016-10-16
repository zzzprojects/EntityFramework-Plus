// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter.</summary>
    /// <typeparam name="T">The type of the filter element.</typeparam>
    public class QueryFilterInterceptor<T> : BaseQueryFilterInterceptor where T : class
    {
        /// <summary>Constructor.</summary>
        /// <param name="filter">The filter.</param>
        public QueryFilterInterceptor(Func<IQueryable<T>, IQueryable<T>> filter)
        {
            ElementType = typeof(T);
            Filter = filter;
        }

        /// <summary>Gets or sets the filter.</summary>
        /// <value>The filter.</value>
        public Func<IQueryable<T>, IQueryable<T>> Filter { get; set; }

        /// <summary>Gets database expression.</summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        /// <returns>The database expression.</returns>
        public override DbExpression GetDbExpression(DbContext context, Type type)
        {
            var contextFullName = context.GetType().AssemblyQualifiedName ?? context.GetType().FullName;
            var typeFullName = type.AssemblyQualifiedName ?? type.FullName;
            var hookId = QueryFilterManager.PrefixHook + contextFullName + ";" + typeFullName + ";" + UniqueKey;

            if (!QueryFilterManager.DbExpressionByHook.ContainsKey(hookId))
            {
                // CREATE set
                var setMethod = typeof(DbContext).GetMethod("Set", new Type[0]).MakeGenericMethod(type);
                var dbSet = (IQueryable<T>)setMethod.Invoke(context, null);

                // APPLY filter
                dbSet = Filter(dbSet);

                // APPLY hook
                dbSet = QueryFilterManager.HookFilter(dbSet, hookId);

                // HOOK the filter
                var objectQuery = dbSet.GetObjectQuery();
                var commandTextAndParameters = objectQuery.GetCommandTextAndParameters();

                // ADD parameter
                QueryFilterManager.DbExpressionParameterByHook.AddOrUpdate(QueryFilterManager.DbExpressionByHook[hookId], commandTextAndParameters.Item2, (s, list) => list);
            }

            // TODO: WeakTable ?
            return QueryFilterManager.DbExpressionByHook[hookId];
        }
    }
}