using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    /// <summary>A batch delete visitor.</summary>
    public class BatchDeleteVisitor : ExpressionVisitor
    {
        /// <summary>Gets or sets a value indicating whether the expression contains an OrderBy method.</summary>
        /// <value>true if the expression contains an OrderBy metho has order by, false if not.</value>
        public bool HasOrderBy { get; set; }

        /// <summary>Gets or sets a value indicating whether the expression contains an Skip method.</summary>
        /// <value>true if the expression contains an Skip metho has order by, false if not.</value>
        public bool HasSkip { get; set; }

        /// <summary>Gets or sets a value indicating whether the expression contains an Take method.</summary>
        /// <value>true if the expression contains an Take metho has order by, false if not.</value>
        public bool HasTake { get; set; }

        /// <summary>
        /// True if a query is simple and references only one Entity, doesn't include unions etc.
        /// </summary>
        public bool IsSimpleQuery { get; set; } = true;

        /// <summary>
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified; otherwise, returns the
        ///     original expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "OrderBy":
                    HasOrderBy = true;
                    break;
                case "Skip":
                    HasSkip = true;
                    break;
                case "Take":
                    HasTake = true;
                    break;
                case "Join":
                case "Select":
                case "SelectMany":
                case "Concat":
                case "Union":
                case "GroupBy":
                    IsSimpleQuery = false;
                    break;
            }
            return base.VisitMethodCall(node);
        }
    }
}