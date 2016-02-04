// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    internal static partial class ExpressionExtensions
    {
        /// <summary>
        ///     An Expression extension method that gets a property or field from an expression.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="this">The @this to act on.</param>
        /// <param name="parameterExpression">The parameter @this.</param>
        /// <returns>The property or field from the expression.</returns>
        internal static MemberInfo GetPropertyOrField(this Expression @this, ParameterExpression parameterExpression)
        {
            var memberExpression = RemoveConvert(@this) as MemberExpression;

            if (memberExpression == null)
            {
                throw new Exception("Invalid expression.");
            }

            if (memberExpression.Expression != parameterExpression)
            {
                throw new Exception("Invalid expression.");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            var fieldInfo = memberExpression.Member as FieldInfo;

            if (propertyInfo == null && fieldInfo == null)
            {
                throw new Exception("Invalid expression.");
            }

            return (MemberInfo) propertyInfo ?? fieldInfo;
        }

        /// <summary>
        ///     An Expression extension method that gets a property or field access from an expression.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="this">The @this to act on.</param>
        /// <param name="parameterExpression">The parameter @this.</param>
        /// <returns>The property or field access from the expression.</returns>
        internal static PropertyOrFieldAccessor GetPropertyOrFieldAccess(this Expression @this, ParameterExpression parameterExpression)
        {
            var paths = new List<MemberInfo>();

            MemberExpression memberExpression;

            do
            {
                memberExpression = RemoveConvert(@this) as MemberExpression;

                if (memberExpression == null)
                {
                    throw new Exception("Invalid expression.");
                }

                var propertyInfo = memberExpression.Member as PropertyInfo;
                var fieldInfo = memberExpression.Member as FieldInfo;

                if (propertyInfo != null)
                {
                    paths.Add(propertyInfo);
                }
                if (fieldInfo != null)
                {
                    paths.Add(fieldInfo);
                }

                @this = memberExpression.Expression;
            } while (memberExpression.Expression != parameterExpression);

            paths.Reverse();

            return new PropertyOrFieldAccessor(paths.AsReadOnly());
        }
    }
}