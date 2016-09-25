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
        ///     Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified; otherwise, returns the
        ///     original expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "OrderBy")
            {
                HasOrderBy = true;
            }
            else if (node.Method.Name == "Skip")
            {
                HasSkip = true;
            }
            else if (node.Method.Name == "Take")
            {
                HasTake = true;
            }

            return base.VisitMethodCall(node);
        }
    }
}