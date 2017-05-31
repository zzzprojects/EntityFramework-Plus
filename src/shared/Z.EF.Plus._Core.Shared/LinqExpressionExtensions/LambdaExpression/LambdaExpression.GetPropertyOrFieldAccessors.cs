// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_UPDATE || QUERY_INCLUDEFILTER || QUERY_INCLUDEOPTIMIZED
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>
        ///     Gets the property or field accessors in this collection.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>An array of property or field accessor.</returns>
        internal static PropertyOrFieldAccessor[] GetPropertyOrFieldAccessors(this LambdaExpression @this)
        {
            PropertyOrFieldAccessor[] memberAccessors;
            var parameterExpression = @this.Parameters.Single();
            var newExpression = @this.Body.RemoveConvert() as NewExpression;

            if (newExpression != null)
            {
                var arguments = newExpression.Arguments;
                memberAccessors = arguments.Select(x => x.GetPropertyOrFieldAccess(parameterExpression)).ToArray();
            }
            else
            {
                var memberAccesor = @this.Body.GetPropertyOrFieldAccess(parameterExpression);
                memberAccessors = new[] {memberAccesor};
            }

            return memberAccessors;
        }
    }
}
#endif