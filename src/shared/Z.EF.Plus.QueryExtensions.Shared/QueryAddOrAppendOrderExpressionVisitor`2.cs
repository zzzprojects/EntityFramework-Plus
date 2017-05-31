// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if !QUERY_INCLUDEOPTIMIZED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if NETCORE50
using System.Reflection;
#endif

namespace Z.EntityFramework.Plus
{
    public class QueryAddOrAppendOrderExpressionVisitor<TSource, TKey> : ExpressionVisitor
    {
        private bool Ascending;
        private IComparer<TKey> Comparer;
        private Expression<Func<TSource, TKey>> KeySelector;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "OrderBy"
                || node.Method.Name == "OrderByDescending"
                || node.Method.Name == "ThenBy"
                || node.Method.Name == "ThenByDescending")
            {
                return AppendThenByExpression(node);
            }

            return base.VisitMethodCall(node);
        }

        public IQueryable<TSource> OrderBy(IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector)
        {
            return AddOrAppendOrderBy(query, keySelector, true);
        }

        public IQueryable<TSource> OrderBy(IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            return AddOrAppendOrderBy(query, keySelector, true, comparer);
        }

        public IQueryable<TSource> OrderByDescending(IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector)
        {
            return AddOrAppendOrderBy(query, keySelector, false);
        }

        public IQueryable<TSource> OrderByDescending(IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            return AddOrAppendOrderBy(query, keySelector, false, comparer);
        }

        private IQueryable<TSource> AddOrAppendOrderBy(IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, bool ascending, IComparer<TKey> comparer = null)
        {
            Ascending = ascending;
            Comparer = comparer;
            KeySelector = keySelector;

            // VISIT expression to append "ThenBy" to the last "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" method
            var expression = Visit(query.Expression);

            if (expression == query.Expression
                || expression.ToString() == query.Expression.ToString())
            {
                // ADD "OrderBy" to the query
                query = Comparer == null ?
                    Ascending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector) :
                    Ascending ? query.OrderBy(keySelector, Comparer) : query.OrderByDescending(keySelector, Comparer);
            }
            else
            {
                // CREATE query from the modified appended "ThenBy" expression
                query = query.Provider.CreateQuery<TSource>(expression);
            }

            return query;
        }

        public Expression AppendThenByExpression(Expression expression)
        {
            if (Comparer == null)
            {
                var orderByMethod = Ascending ?
                    typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 2) :
                    typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);
                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(typeof (TSource), typeof (TKey));
                expression = Expression.Call(null, orderByMethodGeneric, new[] {expression, Expression.Quote(KeySelector)});
            }
            else
            {
                var orderByMethod = Ascending ?
                    typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 3) :
                    typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 3);
                var orderByMethodGeneric = orderByMethod.MakeGenericMethod(typeof (TSource), typeof (TKey));
                expression = Expression.Call(null, orderByMethodGeneric, new[] {expression, Expression.Quote(KeySelector), Expression.Constant(Comparer, typeof (IComparer<TKey>))});
            }

            return expression;
        }
    }
}
#endif