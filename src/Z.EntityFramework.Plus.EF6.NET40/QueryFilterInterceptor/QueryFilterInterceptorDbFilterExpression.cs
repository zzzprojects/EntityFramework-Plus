using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;

namespace Z.EntityFramework.Plus
{
    /// <summary>A query interceptor filter database filter expression.</summary>
    public class QueryFilterInterceptorDbFilterExpression : DefaultExpressionVisitor
    {
        /// <summary>Identifier for the filter.</summary>
        public List<string> FilterID;

        /// <summary>Identifier for the hook.</summary>
        public string HookID;

        /// <summary>
        ///     Implements the visitor pattern for a predicate applied to filter an input set.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbFilterExpression expression)
        {
            var predicate = expression.Predicate;
            var comparison = predicate as DbComparisonExpression;

            if (comparison != null)
            {
                var leftConstant = comparison.Left as DbConstantExpression;

                if (leftConstant != null)
                {
                    {
                        var valueString = leftConstant.Value as string;

                        if (valueString != null)
                        {
                            if (valueString.StartsWith(QueryFilterManager.PrefixFilter, StringComparison.InvariantCulture))
                            {
                                if (FilterID == null)
                                {
                                    FilterID = new List<string>();
                                }

                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);

                                // Add after visiting
                                FilterID.Add(valueString);

                                // It's a fake filter! Do nothing
                                return baseInnerExpression.Input.Expression;
                            }
                            if (valueString.StartsWith(QueryFilterManager.PrefixHook, StringComparison.InvariantCulture))
                            {
                                HookID = valueString;

                                // It's a fake filter! Do nothing
                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);
                                return baseInnerExpression.Input.Expression;
                            }
                            if (valueString.StartsWith(QueryFilterManager.PrefixFilterID, StringComparison.InvariantCulture))
                            {
                                // It's a fake filter! Do nothing
                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);
                                return baseInnerExpression.Input.Expression;
                            }
                        }
                    }
                }

                var rightConstant = comparison.Right as DbConstantExpression;

                if (rightConstant != null)
                {
                    {
                        var valueString = rightConstant.Value as string;

                        if (valueString != null)
                        {
                            if (valueString.StartsWith(QueryFilterManager.PrefixFilter, StringComparison.InvariantCulture))
                            {
                                if (FilterID == null)
                                {
                                    FilterID = new List<string>();
                                }

                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);

                                // Add after visiting
                                FilterID.Add(valueString);

                                // It's a fake filter! Do nothing
                                return baseInnerExpression.Input.Expression;
                            }
                            if (valueString.StartsWith(QueryFilterManager.PrefixHook, StringComparison.InvariantCulture))
                            {
                                HookID = valueString;

                                // It's a fake filter! Do nothing
                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);
                                return baseInnerExpression.Input.Expression;
                            }
                            if (valueString.StartsWith(QueryFilterManager.PrefixFilterID, StringComparison.InvariantCulture))
                            {
                                // It's a fake filter! Do nothing
                                var baseInnerExpression = (DbFilterExpression)base.Visit(expression);
                                return baseInnerExpression.Input.Expression;
                            }
                        }
                    }
                }
            }

            var baseExpression = base.Visit(expression);
            return baseExpression;
        }
    }
}