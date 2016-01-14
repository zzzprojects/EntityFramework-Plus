// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        public static QueryDeferred<TSource> DeferredElementAtOrDefault<TSource>(this IQueryable<TSource> source, int index)
        {
            if (source == null)
                throw Error.ArgumentNull("source");

            return new QueryDeferred<TSource>(source.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.ElementAtOrDefault, source, index),
                    new[] {source.Expression, Expression.Constant(index)}
                    ));
        }
    }
}