// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter provider.</summary>
#if NET45
    public class QueryInterceptorProvider : IDbAsyncQueryProvider
#else
    public class QueryInterceptorProvider : IQueryProvider
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
#if NET45
        public QueryInterceptorProvider(IDbAsyncQueryProvider originalProvider)
#else
        public QueryInterceptorProvider(IQueryProvider originalProvider)
#endif
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryInterceptorQueryable CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
#if NET45
        public IDbAsyncQueryProvider OriginalProvider { get; set; }
#else
        public IQueryProvider OriginalProvider { get; set; }
#endif

        /// <summary>Creates a query from the expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            var query = OriginalProvider.CreateQuery(expression);
            return new QueryInterceptorQueryable(query, CurrentQueryable.Visitors);
        }

        /// <summary>Creates a query from the expression.</summary>
        /// <typeparam name="TElement">The type of elements of the query.</typeparam>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var query = OriginalProvider.CreateQuery<TElement>(expression);
            return new QueryInterceptorQueryable<TElement>(query, CurrentQueryable.Visitors);
        }

        /// <summary>Executes the given expression.</summary>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public object Execute(Expression expression)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.Execute(expression);
        }

        /// <summary>Executes the given expression.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.Execute<TResult>(expression);
        }

#if NET45
        /// <summary>Executes the asynchronous operation.</summary>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task&lt;object&gt;</returns>
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.ExecuteAsync(expression, cancellationToken);
        }

        /// <summary>Executes the asynchronous operation.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task&lt;TResult&gt;</returns>
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.ExecuteAsync<TResult>(expression, cancellationToken);
        }
#endif
    }
}