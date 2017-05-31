// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
#if EF5
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;

#elif EF6
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
#elif EFCORE
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Query.Internal;
using Remotion.Linq;

#endif
#if NET45
using System.Data.Entity.Infrastructure;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include optimized provider.</summary>
#if EF6 && NET45
    public class QueryIncludeOptimizedProvider<T> : IDbAsyncQueryProvider
#else
    public class QueryIncludeOptimizedProvider<T> : IQueryProvider
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="originalProvider">The original provider.</param>
        public QueryIncludeOptimizedProvider(IQueryProvider originalProvider)
        {
            OriginalProvider = originalProvider;
        }

        /// <summary>Gets or sets the current queryable.</summary>
        /// <value>The current queryable.</value>
        public QueryIncludeOptimizedParentQueryable<T> CurrentQueryable { get; set; }

        /// <summary>Gets or sets the original provider.</summary>
        /// <value>The original provider.</value>
        public IQueryProvider OriginalProvider { get; set; }

        /// <summary>Creates a query from the expression.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            throw new Exception(ExceptionMessage.QueryIncludeOptimized_CreateQueryElement);
        }

        /// <summary>Creates a query from the expression.</summary>
        /// <typeparam name="TElement">The type of elements of the query.</typeparam>
        /// <param name="expression">The expression to create the query from.</param>
        /// <returns>The new query created from the expression.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // CREATE the query
            var query = OriginalProvider.CreateQuery<TElement>(expression);

            if (typeof (TElement) != typeof (T))
            {
                // CANNOT throw error since used in QueryIncludeOptimizeChild
                return query;
            }

            // CREATE a new IncludeOptimize and append previous query childs
            return new QueryIncludeOptimizedParentQueryable<TElement>(query, CurrentQueryable.Childs);
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

            QueryIncludeOptimizedIncludeSubPath.RemoveLazyChild(CurrentQueryable);

            var currentQuery = CurrentQueryable;
            var currentMethod = methodCall.Method.GetGenericMethodDefinition();

            // CHECK if the internal expression can be supported
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
                        var method = typeof (Queryable).GetMethods().First(x => x.Name == "Where" && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);
                        var methodGeneric = method.MakeGenericMethod(currentQuery.ElementType);
                        currentQuery = (QueryIncludeOptimizedParentQueryable<T>) methodGeneric.Invoke(null, new object[] {currentQuery, lambdaExpression});
                        currentMethod = typeof (Queryable).GetMethods().FirstOrDefault(x => x.Name == currentMethod.Name && x.GetParameters().Length == 1);
                        isExpressionSupported = currentMethod != null;
                    }
                }
            }

            if (firstExpression != null && !isExpressionSupported)
            {
                throw new Exception(ExceptionMessage.QueryIncludeOptimized_ArgumentExpression);
            }

            // CREATE the new query by selecting included entities in an anonymous type
            var newQuery = currentQuery;

            // REPLACE the first argument with the new query expression
            var arguments = methodCall.Arguments.ToList();
            arguments[0] = newQuery.Expression;

            // REMOVE the last argument if a "Predicate" method was previously used
            if (firstExpression != null)
            {
                arguments.RemoveAt(1);
            }

            // RESOLE parent queries using .FutureValue();
#if EF5 || EF6
            var objectQuery = CurrentQueryable.OriginalQueryable.GetObjectQuery();
         
            // GET provider
            var objectQueryProviderField = typeof (ObjectQuery).GetProperty("ObjectQueryProvider", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var provider = (IQueryProvider) objectQueryProviderField.GetValue(objectQuery, null);

            // CREATE query from the expression
            var createQueryMethod = provider.GetType().GetMethod("CreateQuery", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (Expression)}, null);
            createQueryMethod = createQueryMethod.MakeGenericMethod(typeof (TResult));
            var immediateQuery = (ObjectQuery<T>) createQueryMethod.Invoke(provider, new object[] {expression});
#elif EFCORE
            var immediateQuery = new EntityQueryable<TResult>((IAsyncQueryProvider)OriginalProvider);
            var expressionProperty = typeof(QueryableBase<>).MakeGenericType(typeof(TResult)).GetProperty("Expression", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            expressionProperty.SetValue(immediateQuery, expression);
#endif

            object value = null;

            if (QueryIncludeOptimizedManager.AllowQueryBatch)
            {
                var futureValue = immediateQuery.FutureValue();

                // RESOLVE child queries using .Future()
                {
                    // MODIFY query if necessary
#if EF5 || EF6
                    var objectContext = CurrentQueryable.OriginalQueryable.GetObjectQuery().Context;
                    var keyMembers = ((dynamic)objectContext).CreateObjectSet<T>().EntitySet.ElementType.KeyMembers;
                    var keyNames = ((IEnumerable<EdmMember>)keyMembers).Select(x => x.Name).ToArray();
#elif EFCORE

                var context = currentQuery.OriginalQueryable.GetDbContext();

                var keyNames = context.Model.FindEntityType(typeof (TResult).DisplayName(true))
                    .GetKeys().ToList()[0]
                    .Properties.Select(x => x.Name).ToArray();
#endif

                    var currentNewQuery = methodCall.Method.Name == "First" || methodCall.Method.Name == "FirstOrDefault" ?
                        currentQuery.AddToRootOrAppendOrderBy(keyNames).Take(1) :
                        methodCall.Method.Name == "Last" || methodCall.Method.Name == "LastOrDefault" ?
                            currentQuery.AddToRootOrAppendOrderBy(keyNames).Reverse().Take(1) :
                            currentQuery;

                    currentQuery.CreateQueryable(currentNewQuery);
                }

                value = futureValue.Value;
            }
            else
            {
                value = immediateQuery.Execute(objectQuery.MergeOption).FirstOrDefault();

                // RESOLVE child queries using .Future()
                {
                    // MODIFY query if necessary
#if EF5 || EF6
                    var objectContext = CurrentQueryable.OriginalQueryable.GetObjectQuery().Context;
                    var keyMembers = ((dynamic)objectContext).CreateObjectSet<T>().EntitySet.ElementType.KeyMembers;
                    var keyNames = ((IEnumerable<EdmMember>)keyMembers).Select(x => x.Name).ToArray();
#elif EFCORE

                var context = currentQuery.OriginalQueryable.GetDbContext();

                var keyNames = context.Model.FindEntityType(typeof (TResult).DisplayName(true))
                    .GetKeys().ToList()[0]
                    .Properties.Select(x => x.Name).ToArray();
#endif

                    var currentNewQuery = methodCall.Method.Name == "First" || methodCall.Method.Name == "FirstOrDefault" ?
                        currentQuery.AddToRootOrAppendOrderBy(keyNames).Take(1) :
                        methodCall.Method.Name == "Last" || methodCall.Method.Name == "LastOrDefault" ?
                            currentQuery.AddToRootOrAppendOrderBy(keyNames).Reverse().Take(1) :
                            currentQuery;

                    currentQuery.CreateQueryable(currentNewQuery);
                }

            }


            // EXECUTE the new expression
            //var value = QueryIncludeOptimizedManager.AllowQueryBatch ? immediateQuery.FutureValue().Value : immediateQuery.FirstOrDefault();

            // CHECK if a value has been returned
            if (value == null)
            {
                return (TResult) (object) null;
            }

#if EF6
            // FIX lazy loading
            QueryIncludeOptimizedLazyLoading.SetLazyLoaded(value, CurrentQueryable.Childs);
#endif

            // FIX null collection
            QueryIncludeOptimizedNullCollection.NullCollectionToEmpty(value, CurrentQueryable.Childs);

#if EF5 || EF6
            return (TResult) (object) value;
#elif EFCORE
            return (TResult)value;
#endif
        }

#if EF6 && NET45
        /// <summary>Executes the given expression asynchronously.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="expression">The expression to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Executes the given expression asynchronously.</summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="expression">The expression to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The object returned by the execution of the expression.</returns>
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.Run(() => Execute<TResult>(expression), cancellationToken);
        }
#endif
    }
}