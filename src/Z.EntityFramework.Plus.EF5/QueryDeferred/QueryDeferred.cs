// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

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

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class to store immediate LINQ IQueryable query and expression deferred.</summary>
    /// <typeparam name="TResult">Type of the result of the query deferred.</typeparam>
    public class QueryDeferred<TResult>
    {
        /// <summary>Constructor.</summary>
        /// <param name="objectQuery">The deferred objectQuery.</param>
        /// <param name="expression">The deferred expression.</param>
        public QueryDeferred(ObjectQuery objectQuery, Expression expression)
        {
            Expression = expression;

            // CREATE query from the deferred expression
            var provider = ((IQueryable) objectQuery).Provider;
            var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (Expression), typeof (Type)}, null);
            Query = (IQueryable) createQueryMethod.Invoke(provider, new object[] {expression, typeof (TResult)});
        }

        /// <summary>Gets or sets the deferred expression.</summary>
        /// <value>The deferred expression.</value>
        public Expression Expression { get; protected internal set; }

        /// <summary>Gets or sets the deferred query.</summary>
        /// <value>The deferred query.</value>
        public IQueryable Query { get; protected internal set; }

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
#endif
        }
#endif
    }
}