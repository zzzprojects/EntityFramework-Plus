// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


#if STANDALONE
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    internal static partial class ExpressionExtensions
    {
        /// <summary>
        ///     An Expression extension method that removes all convert expression from the expression.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>An Expression without convert expression.</returns>
        internal static Expression RemoveConvert(this Expression @this)
        {
            while (@this.NodeType == ExpressionType.Convert || @this.NodeType == ExpressionType.ConvertChecked)
            {
                @this = ((UnaryExpression) @this).Operand;
            }

            return @this;
        }
    }
}
#endif