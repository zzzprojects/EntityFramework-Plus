// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_UPDATE || QUERY_INCLUDEFILTER || QUERY_INCLUDEOPTIMIZED

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>
        ///     A LambdaExpression extension method that gets property or field accessor.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The property or field accessor.</returns>
        internal static PropertyOrFieldAccessor GetPropertyOrFieldAccessor(this LambdaExpression @this)
        {
            PropertyOrFieldAccessor memberAccessor;
            var parameterExpression = @this.Parameters.Single();
            var newExpression = @this.Body.RemoveConvert() as NewExpression;

            if (newExpression != null)
            {
                if (newExpression.Arguments.Count > 1)
                {
                    throw new Exception("More than one property or field found in the @this.");
                }

                var memberAccessExpression = newExpression.Arguments[0];
                memberAccessor = memberAccessExpression.GetPropertyOrFieldAccess(parameterExpression);
            }
            else
            {
                memberAccessor = @this.Body.GetPropertyOrFieldAccess(parameterExpression);
            }

            return memberAccessor;
        }
    }
}
#endif