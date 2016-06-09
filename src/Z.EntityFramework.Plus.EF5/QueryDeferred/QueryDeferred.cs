// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

#elif EFCORE
using Microsoft.EntityFrameworkCore.Query.Internal;
using Remotion.Linq;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class to store immediate LINQ IQueryable query and expression deferred.</summary>
    /// <typeparam name="TResult">Type of the result of the query deferred.</typeparam>
    public class QueryDeferred<TResult>
    {
        /// <summary>Constructor.</summary>
        /// <param name="query">The deferred query.</param>
        /// <param name="expression">The deferred expression.</param>
#if EF5 || EF6
        public QueryDeferred(ObjectQuery query, Expression expression)
#elif EFCORE
        public QueryDeferred(IQueryable query, Expression expression)
#endif
        {
            Expression = expression;

#if EF5 || EF6
            // CREATE query from the deferred expression
            var provider = ((IQueryable)query).Provider;
            var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Expression), typeof(Type) }, null);
            Query = (IQueryable<TResult>)createQueryMethod.Invoke(provider, new object[] { expression, typeof(TResult) });
#elif EFCORE
            Query = new EntityQueryable<TResult>((IAsyncQueryProvider)query.Provider);
            var expressionProperty = typeof(QueryableBase<>).MakeGenericType(typeof(TResult)).GetProperty("Expression", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            expressionProperty.SetValue(Query, expression);
#endif
        }

        /// <summary>Gets or sets the deferred expression.</summary>
        /// <value>The deferred expression.</value>
        public Expression Expression { get; protected internal set; }

        /// <summary>Gets or sets the deferred query.</summary>
        /// <value>The deferred query.</value>
        public IQueryable<TResult> Query { get; protected internal set; }

        /// <summary>Execute the deferred expression and return the result.</summary>
        /// <returns>The result of the deferred expression executed.</returns>
        public TResult Execute()
        {
            return Query.Provider.Execute<TResult>(Expression);
        }

#if NET45
        /// <summary>Execute asynchrounously the deferred expression and return the result.</summary>
        /// <returns>The result of the deferred expression executed asynchrounously.</returns>
        public Task<TResult> ExecuteAsync()
        {
            return ExecuteAsync(default(CancellationToken));
        }

        /// <summary>Execute asynchrounously the deferred expression and return the result.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the deferred expression executed asynchrounously.</returns>
        public Task<TResult> ExecuteAsync(CancellationToken cancellationToken)
        {
#if EF5
            return Task.Run(() => Execute(), cancellationToken);
#elif EF6
            var asyncQueryProvider = Query.Provider as IDbAsyncQueryProvider;

            return asyncQueryProvider != null ?
                asyncQueryProvider.ExecuteAsync<TResult>(Expression, cancellationToken) :
                Task.Run(() => Execute(), cancellationToken);
#elif EFCORE
            var asyncQueryProvider = Query.Provider as IAsyncQueryProvider;

            return asyncQueryProvider != null ?
                asyncQueryProvider.ExecuteAsync<TResult>(Expression, cancellationToken) :
                Task.Run(() => Execute(), cancellationToken);
#endif
        }
#endif
    }
}