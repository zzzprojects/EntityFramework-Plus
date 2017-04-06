// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#if NETCORE50
using System.Reflection;
#endif

namespace Z.EntityFramework.Plus
{
#if !QUERY_INCLUDEOPTIMIZED
    internal class QueryAddOrAppendOrderExpressionVisitor<TSource> : ExpressionVisitor
#else
    public class QueryAddOrAppendOrderExpressionVisitor<TSource> : ExpressionVisitor
#endif
    {
        private readonly List<string> ExistingKeyNames = new List<string>();
        private bool Ascending;
        private object Comparer;
        private bool IsOrderByMethodFound;
        private string[] KeyNames;
        private bool VisitAddToRoot;

        public bool AddToRoot { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression expression = node;

            if (VisitAddToRoot)
            {
                var isOrderBy = node.Method.Name == "OrderBy" || node.Method.Name == "OrderByDescending";
                var isThenBy = node.Method.Name == "ThenBy" || node.Method.Name == "ThenByDescending";

                if (isOrderBy || isThenBy)
                {
                    // TODO: Use expression visitor instead?
                    var column = node.ToString()
                        .Split(new[] {"=>"}, StringSplitOptions.None).Last()
                        .Replace(")", "")
                        .Split('.').Last()
                        .Trim();

                    ExistingKeyNames.Add(column);

                    if (isOrderBy)
                    {
                        if (!IsOrderByMethodFound)
                        {
                            // EXCLUDE already ordered columns
                            var includeKeyNames = KeyNames.Except(ExistingKeyNames).ToList();

                            foreach (var orderColumn in includeKeyNames)
                            {
                                expression = AddOrAppendOrderBy(node, orderColumn, false);
                            }

                            IsOrderByMethodFound = true;
                        }
                    }

                    if (isThenBy)
                    {
                        if (!IsOrderByMethodFound)
                        {
                            IsOrderByMethodFound = true;

                            // VISIT expression to get existing columns
                            base.VisitMethodCall(node);

                            // EXCLUDE already ordered columns
                            var includeKeyNames = KeyNames.Except(ExistingKeyNames).ToList();

                            foreach (var orderColumn in includeKeyNames)
                            {
                                expression = AddOrAppendOrderBy(node, orderColumn, false);
                            }
                        }
                        else
                        {
                            expression = base.VisitMethodCall(node);
                        }
                    }
                }
                else
                {
                    expression = base.VisitMethodCall(node);
                }
            }
            else
            {
                if ((node.Method.Name == "MergeAs" || node.Method.Name == "Select" || node.Method.Name == "SelectMany")
                    && KeyNames.All(x => expression.Type.GetGenericArguments()[0].GetProperty(x) != null))
                {
                    // ADD "OrderBy" | "ThenBy" to the query
                    for (var i = 0; i < KeyNames.Length; i++)
                    {
                        expression = AddOrAppendOrderBy(expression, KeyNames[i], i == 0);
                    }
                }
                else
                {
                    expression = base.VisitMethodCall(node);
                }
            }

            return expression;
        }

        public IQueryable<TSource> OrderBy(IQueryable<TSource> query, string[] keyNames)
        {
            return AddOrAppendOrderBy(query, keyNames, true);
        }

        public IQueryable<TSource> OrderBy(IQueryable<TSource> query, string[] keyNames, object comparer)
        {
            return AddOrAppendOrderBy(query, keyNames, true, comparer);
        }

        public IQueryable<TSource> OrderByDescending(IQueryable<TSource> query, string[] keyNames)
        {
            return AddOrAppendOrderBy(query, keyNames, false);
        }

        public IQueryable<TSource> OrderByDescending(IQueryable<TSource> query, string[] keyNames, object comparer)
        {
            return AddOrAppendOrderBy(query, keyNames, false, comparer);
        }

        public IQueryable<TSource> AddOrAppendOrderBy(IQueryable<TSource> query, string[] keyNames, bool ascending, object comparer = null)
        {
            Ascending = ascending;
            Comparer = comparer;
            KeyNames = keyNames;

            // VISIT expression to append "ThenBy" to the last "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" method
            var expression = Visit(query.Expression);

            if (!IsOrderByMethodFound)
            {
                if (AddToRoot)
                {
                    VisitAddToRoot = true;
                    expression = Visit(expression);
                }
                else
                {
                    // ADD "OrderBy" | "ThenBy" to the query
                    for (var i = 0; i < keyNames.Length; i++)
                    {
                        expression = AddOrAppendOrderBy(expression, keyNames[i], i == 0);
                    }
                }
            }

            // CREATE query from the modified expression
            query = query.Provider.CreateQuery<TSource>(expression);

            return query;
        }

        public MethodCallExpression AddOrAppendOrderBy(Expression expression, string propertyName, bool useOrderBy)
        {
            MethodCallExpression newExpression;

            // LAMBDA: x => x.[PropertyName]
            var elementType = expression.Type.GetGenericArguments()[0];
            var parameter = Expression.Parameter(elementType, "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            if (Comparer == null)
            {
                // EXPRESSION: expression.[OrderMethod](x => x.[PropertyName])
                var orderByMethod = useOrderBy ?
                    Ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2) :
                    Ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 2) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);

                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(elementType, property.Type);

                newExpression = Expression.Call(null, orderByMethodGeneric, new[] {expression, Expression.Quote(lambda)});
            }
            else
            {
                // EXPRESSION: expression.[OrderMethod](x => x.[PropertyName], comparer)
                var orderByMethod = useOrderBy ?
                    Ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 3) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 3) :
                    Ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 3) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 3);

                var comparerGeneric = typeof (IComparer<>).MakeGenericType(Comparer.GetType().GetElementType());
                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(elementType, property.Type);

                newExpression = Expression.Call(null, orderByMethodGeneric, new[] {expression, Expression.Quote(lambda), Expression.Constant(Comparer, comparerGeneric)});
            }
            return newExpression;
        }
    }
}