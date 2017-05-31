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
#if NET45
using System.Data.Entity.Infrastructure;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter provider.</summary>
#if EF6 && NET45
    public class QueryIncludeFilterProvider<T> : IDbAsyncQueryProvider
#else
    public class  QueryIncludeFilterProvider<T> : IQueryProvider
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
        public QueryIncludeFilterProvider(IQueryProvider originalProvider)
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryIncludeFilterParentQueryable<T> CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
        public IQueryProvider OriginalProvider { get; set; }

        /// <summary>Creates a query from the expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            throw new Exception(ExceptionMessage.QueryIncludeFilter_CreateQueryElement);
        }

        /// <summary>Creates a query from the expression.</summary>
        /// <typeparam name="TElement">The type of elements of the query.</typeparam>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof (TElement) != typeof (T))
            {
                throw new Exception(ExceptionMessage.QueryIncludeFilter_CreateQueryElement);
            }

            // CREATE the query
            var query = OriginalProvider.CreateQuery<TElement>(expression);

            // CREATE a new IncludeFilter and append previous query childs
            return new QueryIncludeFilterParentQueryable<TElement>(query, CurrentQueryable.Childs);
        }

        /// <summary>Executes the given expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public object Execute(Expression expression)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Executes the given expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression to execute.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            var methodCall = expression as MethodCallExpression;

            if (methodCall == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            if (methodCall.Method.Name == "All"
                || methodCall.Method.Name == "Any"
                || methodCall.Method.Name == "Average"
                || methodCall.Method.Name == "Contains"
                || methodCall.Method.Name == "Count"
                || methodCall.Method.Name == "LongCount"
                || methodCall.Method.Name == "Max"
                || methodCall.Method.Name == "Min"
                || methodCall.Method.Name == "SequenceEqual"
                || methodCall.Method.Name == "Sum")
            {
                return OriginalProvider.Execute<TResult>(expression);
            }

            var currentQuery = CurrentQueryable;
            var currentMethod = methodCall.Method.GetGenericMethodDefinition();

            // CHECK if the internal expression can be supported
            var isExpressionSupported = false;

            var firstExpression = methodCall.Arguments.FirstOrDefault(x => x.Type.IsSubclassOf(typeof (Expression)));
            var unaryExpression = firstExpression as UnaryExpression;
            if (firstExpression != null && unaryExpression != null && methodCall.Arguments.Count == 2)
            {
                var lambdaExpression = unaryExpression.Operand as LambdaExpression;

                if (lambdaExpression != null)
                {
                    if (lambdaExpression.Type == typeof (Func<,>).MakeGenericType(currentQuery.ElementType, typeof (bool)))
                    {
                        var method = typeof (Queryable).GetMethods().First(x => x.Name == "Where" && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);
                        var methodGeneric = method.MakeGenericMethod(currentQuery.ElementType);
                        currentQuery = (QueryIncludeFilterParentQueryable<T>) methodGeneric.Invoke(null, new object[] {currentQuery, lambdaExpression});
                        currentMethod = typeof (Queryable).GetMethods().FirstOrDefault(x => x.Name == currentMethod.Name && x.GetParameters().Length == 1);
                        isExpressionSupported = currentMethod != null;
                    }
                }
            }

            if (firstExpression != null && !isExpressionSupported)
            {
                throw new Exception(ExceptionMessage.QueryIncludeFilter_ArgumentExpression);
            }

            // CREATE the new query by selecting included entities in an anonymous type
            var newQuery = currentQuery.CreateQueryable();

            // CREATE a new immediate method from the immediate method used by the user
            var newImmediateMethod = currentMethod.MakeGenericMethod(newQuery.ElementType);

            // REPLACE the first argument with the new query expression
            var arguments = methodCall.Arguments.ToList();
            arguments[0] = newQuery.Expression;

            // REMOVE the last argument if a "Predicate" method was previously used
            if (firstExpression != null)
            {
                arguments.RemoveAt(1);
            }

            // CREATE the new expression
            var newExpression = Expression.Call(null, newImmediateMethod, arguments);

            // EXECUTE the new expression
            var value = OriginalProvider.Execute(newExpression);

            // CHECK if a value has been returned
            if (value == null)
            {
                return (TResult) (object) null;
            }

            // GET the query result from the anonymous type
            var result = value;
            PropertyInfo property;

            while ((property = result.GetType().GetProperty("x")) != null)
            {
                result = property.GetValue(result, null);
            }

#if EF6
            // FIX lazy loading
            QueryIncludeFilterLazyLoading.SetLazyLoaded(result, currentQuery.Childs);
#endif

            // FIX null collection
            QueryIncludeFilterNullCollection.NullCollectionToEmpty(result, currentQuery.Childs);

            return (TResult) result;
        }

#if EF6 && NET45
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.Run(() => Execute<TResult>(expression), cancellationToken);
        }
#endif
    }
}