using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using Z.EntityFramework.Plus.QueryInterceptorFilter;

namespace Z.EntityFramework.Plus
{
    /// <summary>A database scan expression visitor.</summary>
    public class QueryFilterInterceptorDbScanExpression : DefaultExpressionVisitor
    {
        /// <summary>The context.</summary>
        public DbContext Context;

        /// <summary>The filtered expression.</summary>
        public DbExpression FilteredExpression;

        /// <summary>The filter query.</summary>
        public QueryFilterInterceptorApply FilterQuery;

        /// <summary>Context for the instance filter.</summary>
        public QueryFilterContextInterceptor InstanceFilterContext;

        /// <summary>
        ///     Implements the visitor pattern for a scan over an entity set or relationship set, as
        ///     indicated by the Target property.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbScanExpression expression)
        {
            var baseExpression = base.Visit(expression);
            var baseType = expression.Target.ElementType;
            var fullName = baseType.FullName;

            baseExpression = ApplyFilter(baseExpression, fullName);

            return baseExpression;
        }

        /// <summary>Implements the visitor pattern for retrieving an instance property.</summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The implemented visitor.</returns>
        public override DbExpression Visit(DbPropertyExpression expression)
        {
            var baseExpression = base.Visit(expression);
            var baseExpressionProperty = baseExpression as DbPropertyExpression;

            if (baseExpressionProperty == null)
            {
                return baseExpression;
            }

            var navProp = baseExpressionProperty.Property as NavigationProperty;
            if (navProp != null && baseExpression.ResultType.ToString().Contains("Transient.collection["))
            {
                var targetEntityType = navProp.ToEndMember.GetEntityType();
                var fullName = targetEntityType.FullName;

                baseExpression = ApplyFilter(baseExpression, fullName);
            }

            return baseExpression;
        }

        /// <summary>Applies the filter.</summary>
        /// <param name="baseExpression">The base expression.</param>
        /// <param name="fullName">Name of the full.</param>
        /// <returns>A DbExpression.</returns>
        public DbExpression ApplyFilter(DbExpression baseExpression, string fullName)
        {
            if (InstanceFilterContext.TypeByEntitySetBase.ContainsKey(fullName))
            {
                var filters = InstanceFilterContext.GetGlobalApplicableFilter(fullName);
                var type = InstanceFilterContext.TypeByEntitySetBase[fullName];

                if (filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        var filterQueryEnabled = FilterQuery.IsEnabled(filter);
                        if ((filterQueryEnabled.HasValue && !filterQueryEnabled.Value)
                            || (!filterQueryEnabled.HasValue && !filter.IsTypeEnabled(type)))
                        {
                            continue;
                        }

                        var expression2 = filter.GetDbExpression(Context, type);

                        if (expression2 != null)
                        {
                            var visitor = new QueryFilterInterceptorDbProjectExpression();
                            visitor.DbScanExpression = baseExpression;
                            visitor.ParameterCollection = QueryFilterManager.DbExpressionParameterByHook[expression2];

                            var filetered = expression2.Accept(visitor);
                            baseExpression = filetered;
                        }
                    }
                }
            }
            if (InstanceFilterContext.TypeByEntitySetBase.ContainsKey(fullName))
            {
                var filters = InstanceFilterContext.GetApplicableFilter(fullName);
                var type = InstanceFilterContext.TypeByEntitySetBase[fullName];

                if (filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        var filterQueryEnabled = FilterQuery.IsEnabled(filter);
                        if ((filterQueryEnabled.HasValue && !filterQueryEnabled.Value)
                            || (!filterQueryEnabled.HasValue && !filter.IsTypeEnabled(type)))
                        {
                            continue;
                        }

                        var expression2 = filter.GetDbExpression(Context, type);

                        if (expression2 != null)
                        {
                            var visitor = new QueryFilterInterceptorDbProjectExpression();
                            visitor.DbScanExpression = baseExpression;
                            visitor.ParameterCollection = QueryFilterManager.DbExpressionParameterByHook[expression2];

                            var filetered = expression2.Accept(visitor);

                            baseExpression = filetered;
                        }
                    }
                }
            }

            return baseExpression;
        }
    }
}