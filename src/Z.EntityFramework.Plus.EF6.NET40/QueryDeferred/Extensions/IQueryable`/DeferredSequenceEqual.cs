// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

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

            return new QueryDeferred<bool>(
#if EF5 || EF6
                source1.GetObjectQuery(),
#elif EFCORE 
                source1,
#endif
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

            return new QueryDeferred<bool>(
#if EF5 || EF6
                source1.GetObjectQuery(),
#elif EFCORE 
                source1,
#endif
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