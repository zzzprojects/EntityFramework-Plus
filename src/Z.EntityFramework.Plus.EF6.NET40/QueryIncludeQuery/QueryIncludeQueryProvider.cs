// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    /// <summary>A batch query provider.</summary>
    public class QueryIncludeQueryProvider<T> : IQueryProvider
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
        public QueryIncludeQueryProvider(IQueryProvider originalProvider)
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryIncludeQueryQueryable<T> CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
        public IQueryProvider OriginalProvider { get; set; }

        /// <summary>Creates a query.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression.</param>
        /// <returns>The new query.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Creates a query.</summary>
        /// <typeparam name="TElement">Type of the element.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The new query.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // CREATE the query
            var query = OriginalProvider.CreateQuery<TElement>(expression);

            // CREATE a new IncludeQuery and append previous query childs
            return new QueryIncludeQueryQueryable<TElement>(query, CurrentQueryable.Queries);
        }

        /// <summary>Executes the given expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression.</param>
        /// <returns>An object.</returns>
        public object Execute(Expression expression)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Executes the given expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>A TResult.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            var methodCall = expression as MethodCallExpression;

            if (methodCall == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            if (methodCall.Arguments.Count > 1 && methodCall.Arguments.Any(x => x.Type.IsSubclassOf(typeof (Expression))))
            {
                throw new Exception(ExceptionMessage.QueryIncludeQuery_ArgumentExpression);
            }

            // CREATE the new query by selecting included entities in an anonymous type
            var newQuery = CurrentQueryable.CreateQueryable();

            // CREATE a new immediate method from the immediate method used by the user
            var newImmediateMethod = methodCall.Method.GetGenericMethodDefinition().MakeGenericMethod(newQuery.ElementType);

            // REPLACE the first argument with the new query expression
            var arguments = methodCall.Arguments.ToList();
            arguments[0] = newQuery.Expression;

            // CREATE the new expression
            var newExpression = Expression.Call(null, newImmediateMethod, arguments);

            // EXECUTE the new expression
            var value = OriginalProvider.Execute(newExpression);

            // CHECK if a value has been returned
            if (value == null)
            {
                return (TResult) (object) null;
            }

            // GET the value from the anonymous type
            return (TResult) value.GetType().GetProperty("x").GetValue(value, null);
        }
    }
}