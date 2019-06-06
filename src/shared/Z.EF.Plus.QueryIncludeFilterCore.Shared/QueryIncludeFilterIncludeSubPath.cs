using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    public static class QueryIncludeFilterIncludeSubPath
    {
        public static IQueryable<T> IncludeSubPath<T>(IQueryable<T> query, Expression expression)
        {
            if (!QueryIncludeFilterManager.AllowIncludeSubPath) return query;

            var pathVisitor = new QueryIncludeFilterExpressionToReduceVisitor { RootExpression = expression};
            pathVisitor.Visit(expression);

            // GET all expression to reduce
            if (pathVisitor.Expressions.Count > 0)
            {
                foreach (var node in pathVisitor.Expressions)
                {
                    var pathReduceVisitor = new QueryIncludeFilterExpressionReduceVisitor
                    {
                        RootExpression = expression,
                        NodeToReduce = node
                    };

                    // REDUCE the node
                    var nodeReduced = pathReduceVisitor.Visit(expression);

                    // ENSURE the node is reduced
                    if (nodeReduced == node)
                    {
                        throw new Exception(ExceptionMessage.QueryIncludeFilter_NodeReduce);
                    }

                    var typeSource = typeof (T);
                    var elementType = nodeReduced.Type.Name == "Func`2" ? nodeReduced.Type.GetGenericArguments()[1] : nodeReduced.Type;

                    var method = typeof (QueryIncludeFilterManager).GetMethod("IncludeFilterSingleLazy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(typeSource, elementType);

                    // CALL IncludeFilter with the new expression
                    query = (IQueryable<T>) method.Invoke(null, new object[] {query, nodeReduced});
                }
            }

            return query;
        }

        public static void RemoveLazyChild<T>(QueryIncludeFilterParentQueryable<T> parent)
        {
            if (!QueryIncludeFilterManager.AllowIncludeSubPath) return;

            var childs = parent.Childs;

            var includedChilds = childs.Where(x => x.IsLazy = false).ToList();
            var lazyChilds = childs.Where(x => x.IsLazy = true).ToList();

            if (lazyChilds.Count == 0) return;

            // KEEP only distinct expression from lazy child
            foreach (var lazyChild in lazyChilds)
            {
                if (includedChilds.All(x => x.GetFilter().ToString() != lazyChild.GetFilter().ToString()))
                {
                    includedChilds.Add(lazyChild);
                }
            }

            parent.Childs = includedChilds;
        }
    }
}