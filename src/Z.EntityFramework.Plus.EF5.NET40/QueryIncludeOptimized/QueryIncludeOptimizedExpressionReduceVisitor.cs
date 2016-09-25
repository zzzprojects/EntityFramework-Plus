using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    /// <summary>A query include optimized expression reduce visitor.</summary>
    public class QueryIncludeOptimizedExpressionReduceVisitor : ExpressionVisitor
    {
        public List<Expression> Expressions = new List<Expression>();
        public List<Expression> LambdaToChecks = new List<Expression>();
        public Expression NodeToReduce;
        public Expression RootExpression;

        /// <summary>Adds a member expression.</summary>
        /// <param name="node">The node.</param>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="outExpression">[out] The out expression.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool AddMemberExpression(LambdaExpression node, MemberExpression memberExpression, out Expression outExpression)
        {
            outExpression = null;

            // ADD
            // x => x.Single.Single.Many to x => x.Single.Single
            // x => x.Single.Single to x => x.Single
            // 
            // NOT
            // x => x.Single

            Expression currentExpression = memberExpression;

            while ((memberExpression = currentExpression as MemberExpression) != null && memberExpression.Expression is MemberExpression)
            {
                if (memberExpression == NodeToReduce)
                {
                    var reduceExpression = memberExpression.Expression;

                    var sourceType = node.Parameters[0].Type;
                    var elementType = reduceExpression.Type;

                    var genericFunc = typeof (Func<,>).MakeGenericType(sourceType, elementType);
                    var lambdaMethod = typeof (Expression).GetMethods()
                        .Single(x => x.Name == "Lambda"
                                     && x.IsGenericMethod
                                     && x.GetParameters().Length == 2
                                     && !x.GetParameters()[1].ParameterType.IsArray).MakeGenericMethod(genericFunc);

                    var newExpression = (Expression) lambdaMethod.Invoke(null, new object[] {reduceExpression, node.Parameters});

                    outExpression = newExpression;
                    return true;
                }

                currentExpression = memberExpression.Expression;
            }

            return false;
        }

        /// <summary>
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified; otherwise, returns the
        ///     original expression.
        /// </returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node == RootExpression || LambdaToChecks.Contains(node))
            {
                var currentNode = node.Body;
                var memberExpression = node.Body as MemberExpression;

                if (memberExpression != null)
                {
                    Expression outExpression;
                    if (AddMemberExpression(node, memberExpression, out outExpression))
                    {
                        return outExpression;
                    }
                }
                else
                {
                    MethodCallExpression callExpression;
                    while ((callExpression = currentNode as MethodCallExpression) != null)
                    {
                        memberExpression = callExpression.Arguments[0] as MemberExpression;
                        if (memberExpression != null)
                        {
                            Expression outExpression;
                            if (AddMemberExpression(node, memberExpression, out outExpression))
                            {
                                return outExpression;
                            }
                        }

                        var isSelectMethod = callExpression.Method.ReflectedType != null
                                             && callExpression.Method.ReflectedType.FullName == "System.Linq.Enumerable"
                                             && (callExpression.Method.Name == "Select"
                                                 || callExpression.Method.Name == "SelectMany");

                        if (isSelectMethod)
                        {
                            // ALL Select && SelectMany method are modified but only the last is reduced
                            if (callExpression == NodeToReduce)
                            {
                                // ADD
                                // x => x.Many.Select(y => Many.Select(z => z.Many) to x.Many.Select(y => y.Many)
                                // x => x.Many.Select(y => y.Many) to x => x.Many

                                var reduceExpression = callExpression.Arguments[0];

                                var isEnumerable = reduceExpression.Type.GetGenericArguments().Length == 1;

                                if (isEnumerable)
                                {
                                    var typeSource = node.Parameters[0].Type;
                                    var elementType = reduceExpression.Type.GetGenericArguments()[0];

                                    var genericFunc = typeof (Func<,>).MakeGenericType(typeSource, typeof (IEnumerable<>).MakeGenericType(elementType));
                                    var lambdaMethod = typeof (Expression).GetMethods()
                                        .Single(x => x.Name == "Lambda"
                                                     && x.IsGenericMethod
                                                     && x.GetParameters().Length == 2
                                                     && !x.GetParameters()[1].ParameterType.IsArray).MakeGenericMethod(genericFunc);
                                    var newExpression = (Expression) lambdaMethod.Invoke(null, new object[] {reduceExpression, node.Parameters});

                                    return newExpression;
                                }
                                else
                                {
                                    var sourceType = node.Parameters[0].Type;
                                    var elementType = reduceExpression.Type;

                                    var genericFunc = typeof (Func<,>).MakeGenericType(sourceType, elementType);
                                    var lambdaMethod = typeof (Expression).GetMethods()
                                        .Single(x => x.Name == "Lambda"
                                                     && x.IsGenericMethod
                                                     && x.GetParameters().Length == 2
                                                     && !x.GetParameters()[1].ParameterType.IsArray).MakeGenericMethod(genericFunc);
                                    var newExpression = (LambdaExpression) lambdaMethod.Invoke(null, new object[] {reduceExpression, node.Parameters});

                                    return newExpression;
                                }
                            }

                            // TRY reduce lambda...
                            {
                                var visitor = new QueryIncludeOptimizedExpressionReduceVisitor();
                                visitor.RootExpression = callExpression.Arguments[1];
                                visitor.NodeToReduce = NodeToReduce;
                                var reducedExpression = visitor.Visit(callExpression.Arguments[1]);

                                if (reducedExpression != callExpression.Arguments[1])
                                {
                                    MethodCallExpression reducedMethod;

                                    // FIX method
                                    {
                                        var baseMethod = callExpression.Method.GetGenericMethodDefinition();
                                        var sourceType = callExpression.Arguments[0].Type.GetGenericArguments()[0];
                                        var elementType = callExpression.Method.Name == "SelectMany" ?
                                            reducedExpression.Type.GetGenericArguments()[1].GetGenericArguments()[0] :
                                            reducedExpression.Type.GetGenericArguments()[1];


                                        var genericMethod = baseMethod.MakeGenericMethod(sourceType, elementType);
                                        var arguments = callExpression.Arguments.ToList();
                                        arguments[1] = reducedExpression;

                                        reducedMethod = Expression.Call(null, genericMethod, arguments);
                                    }

                                    // FIX lambda expression
                                    {
                                        var typeSource = node.Parameters[0].Type;
                                        var elementType = reducedMethod.Type.GetGenericArguments()[0];

                                        var genericFunc = typeof (Func<,>).MakeGenericType(typeSource, typeof (IEnumerable<>).MakeGenericType(elementType));
                                        var lambdaMethod = typeof (Expression).GetMethods()
                                            .Single(x => x.Name == "Lambda"
                                                         && x.IsGenericMethod
                                                         && x.GetParameters().Length == 2
                                                         && !x.GetParameters()[1].ParameterType.IsArray).MakeGenericMethod(genericFunc);
                                        var newExpression = (Expression) lambdaMethod.Invoke(null, new object[] {reducedMethod, node.Parameters});

                                        return newExpression;
                                    }
                                }
                            }
                        }

                        currentNode = callExpression.Arguments[0];
                    }
                }
            }

            var baseExpression = base.VisitLambda(node);
            return baseExpression;
        }
    }
}