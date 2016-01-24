// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter provider.</summary>
    #if NET45
    public class QueryInterceptorProvider<T> : IDbAsyncQueryProvider
#else
    public class QueryInterceptorProvider<T> : IQueryProvider
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
        public QueryInterceptorQueryable<T> CurrentQueryable { get; set; }

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
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public object Execute(Expression expression)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.Execute(expression);
        }

        /// <summary>Executes the given expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.ExecuteAsync(expression, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            expression = CurrentQueryable.Visit(expression);
            return OriginalProvider.ExecuteAsync<TResult>(expression, cancellationToken);
        }
    }
}