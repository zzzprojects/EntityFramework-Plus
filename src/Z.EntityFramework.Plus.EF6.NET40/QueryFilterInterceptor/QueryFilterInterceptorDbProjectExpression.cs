using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Z.EntityFramework.Plus.QueryInterceptorFilter
{
    /// <summary>A database project to input expression visitor.</summary>
    public class QueryFilterInterceptorDbProjectExpression : DefaultExpressionVisitor
    {
        /// <summary>The database scan expression.</summary>
        public DbExpression DbScanExpression;

        /// <summary>Collection of parameters.</summary>
        public DbParameterCollection ParameterCollection;

        /// <summary>
        ///     Implements the visitor pattern for the projection of a given input set over the specified
        ///     expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbProjectExpression expression)
        {
            var baseExpression = base.Visit(expression);
            var baseDbProject = baseExpression as DbProjectExpression;

            if (baseDbProject != null)
            {
                return baseDbProject.Input.Expression;
            }


            // This situation may happen when another user-defined interceptor is used
            // The library may not be compatible in this situation
            return baseExpression;
        }

        /// <summary>
        ///     Implements the visitor pattern for a scan over an entity set or relationship set, as
        ///     indicated by the Target property.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbScanExpression expression)
        {
            return DbScanExpression;
        }

        /// <summary>
        ///     Implements the visitor pattern for a reference to a parameter declared on the command tree
        ///     that contains this expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbParameterReferenceExpression expression)
        {
            if (ParameterCollection.Contains(expression.ParameterName))
            {
                return DbExpressionBuilder.Constant(ParameterCollection[expression.ParameterName].Value);
            }

            return base.Visit(expression);
        }
    }
}