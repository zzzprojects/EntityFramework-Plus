// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE
using System.Linq;

namespace Z.EntityFramework.Plus
{
    internal static partial class QueryAddOrAppendOrderExtensions
    {
        internal static IQueryable<T> AddToRootOrAppendOrderBy<T>(this IQueryable<T> query, params string[] columns)
        {
            var visitor = new QueryAddOrAppendOrderExpressionVisitor<T> {AddToRoot = true};
            return visitor.OrderBy(query, columns);
        }
    }
}
#endif