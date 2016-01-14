// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        public static QueryDeferred<bool> DeferredSequenceEqual<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            if (source1 == null)
                throw Error.ArgumentNull("source1");
            if (source2 == null)
                throw Error.ArgumentNull("source2");

            return new QueryDeferred<bool>(source1.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.SequenceEqual, source1, source2),
                    new[] {source1.Expression, GetSourceExpression(source2)}
                    ));
        }

        public static QueryDeferred<bool> DeferredSequenceEqual<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
        {
            if (source1 == null)
                throw Error.ArgumentNull("source1");
            if (source2 == null)
                throw Error.ArgumentNull("source2");

            return new QueryDeferred<bool>(source1.GetObjectQuery(),
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.SequenceEqual, source1, source2, comparer),
                    new[]
                    {
                        source1.Expression,
                        GetSourceExpression(source2),
                        Expression.Constant(comparer, typeof (IEqualityComparer<TSource>))
                    }
                    ));
        }
    }
}