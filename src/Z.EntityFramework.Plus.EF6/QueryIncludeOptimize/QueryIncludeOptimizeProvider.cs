// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter provider.</summary>
    public class QueryIncludeOptimizeProvider<T> : IQueryProvider
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
        public QueryIncludeOptimizeProvider(IQueryProvider originalProvider)
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryIncludeOptimizeParentQueryable<T> CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
        public IQueryProvider OriginalProvider { get; set; }

        /// <summary>Creates a query from the expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Creates a query from the expression.</summary>
        /// <typeparam name="TElement">The type of elements of the query.</typeparam>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof (TElement) != typeof (T))
            {
                // NO LONGER support "Include"
                return OriginalProvider.CreateQuery<TElement>(expression);
            }
            // CREATE the query
            var query = OriginalProvider.CreateQuery<T>(expression);

            // CREATE a new IncludeQuery and append previous query childs
            return (dynamic) new QueryIncludeOptimizeParentQueryable<T>(query, CurrentQueryable.Childs);
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

            var currentQuery = CurrentQueryable;
            var currentMethod = methodCall.Method.GetGenericMethodDefinition();

            // CHECK if the internal expression can be supported]
            var isExpressionSupported = false;

            var firstExpression = methodCall.Arguments.FirstOrDefault(x => x.Type.IsSubclassOf(typeof (Expression)));
            if (firstExpression != null && methodCall.Arguments.Count == 2)
            {
                var quoteExpression = ((UnaryExpression) firstExpression).Operand;
                var lambdaExpression = quoteExpression as LambdaExpression;
                if (lambdaExpression != null)
                {
                    if (lambdaExpression.Type == typeof (Func<,>).MakeGenericType(currentQuery.ElementType, typeof (bool)))
                    {
                        var method = typeof (Queryable).GetMethods().First(x => x.Name == "Where" && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GenericTypeArguments.Length == 2);
                        var methodGeneric = method.MakeGenericMethod(currentQuery.ElementType);
                        currentQuery = (QueryIncludeOptimizeParentQueryable<T>) methodGeneric.Invoke(null, new object[] {currentQuery, lambdaExpression});
                        currentMethod = typeof (Queryable).GetMethods().FirstOrDefault(x => x.Name == currentMethod.Name && x.GetParameters().Length == 1);
                        isExpressionSupported = currentMethod != null;
                    }
                }
            }

            if (firstExpression != null && !isExpressionSupported)
            {
                throw new Exception(ExceptionMessage.QueryIncludeQuery_ArgumentExpression);
            }

            // CREATE the new query by selecting included entities in an anonymous type
            var newQuery = currentQuery;

            // REPLACE the first argument with the new query expression
            var arguments = methodCall.Arguments.ToList();
            arguments[0] = newQuery.Expression;

            // REMOVE the last argument if the "Where" method was previously used
            if (firstExpression != null)
            {
                arguments.RemoveAt(1);
            }

            var objectQuery = CurrentQueryable.OriginalQueryable.GetObjectQuery();

            // GET provider
            var objectQueryProviderProperty = objectQuery.GetType().GetProperty("ObjectQueryProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            var provider = (IQueryProvider)objectQueryProviderProperty.GetValue(objectQuery);

            // CREATE query from the expression
            var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Expression) }, null);
            createQueryMethod = createQueryMethod.MakeGenericMethod(typeof(TResult));
            var query = (ObjectQuery<T>)createQueryMethod.Invoke(provider, new object[] { expression });

            var objectContext = CurrentQueryable.OriginalQueryable.GetObjectQuery().Context;
            var keyMembers = ((dynamic) objectContext).CreateObjectSet<T>().EntitySet.ElementType.KeyMembers;
            var keyNames = ((IEnumerable<EdmMember>) keyMembers).Select(x => x.Name).ToArray();

            var currentNewQuery = currentQuery.AddOrAppendOrderBy(keyNames);

            // First, FirstOrDefault... todo: Last, LsatOrDefault
            currentNewQuery = currentNewQuery.Take(1);
            currentQuery.CreateQueryable(currentNewQuery);

            // EXECUTE the new expression
            var futureValue = query.FutureValue();
            var value = futureValue.Value;

            // CHECK if a value has been returned
            if (value == null)
            {
                return (TResult)(object)null;
            }

            return (TResult)(object) value;
        }
    }
}