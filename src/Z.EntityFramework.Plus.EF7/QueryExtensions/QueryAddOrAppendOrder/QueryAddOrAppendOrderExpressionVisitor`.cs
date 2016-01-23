using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public class QueryAddOrAppendOrderExpressionVisitor<TSource> : ExpressionVisitor
    {
        private readonly List<string> ExistingKeyNames = new List<string>();
        private bool Ascending;
        private object Comparer;
        private bool IsOrderByMethodFound;
        private string[] KeyNames;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression expression = node;

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
                // ADD "OrderBy" | "ThenBy" to the query
                for (var i = 0; i < keyNames.Length; i++)
                {
                    expression = AddOrAppendOrderBy(expression, keyNames[i], i == 0);
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
            var parameter = Expression.Parameter(typeof (TSource), "x");
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

                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(typeof (TSource), property.Type);

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
                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(typeof (TSource), property.Type);

                newExpression = Expression.Call(null, orderByMethodGeneric, new[] {expression, Expression.Quote(lambda), Expression.Constant(Comparer, comparerGeneric)});
            }
            return newExpression;
        }
    }
}