// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public static partial class QueryDeferredExtensions
    {
        public static QueryDeferred<TSource> DeferredElementAt<TSource>(this IQueryable<TSource> source, int index)
        {
            if (source == null)
                throw Error.ArgumentNull("source");
            if (index < 0)
                throw Error.ArgumentOutOfRange("index");

            return new QueryDeferred<TSource>(
#if EF5 || EF6
                source.GetObjectQuery(),
#elif EF7 
                source,
#endif
                Expression.Call(
                    null,
                    GetMethodInfo(Queryable.ElementAt, source, index),
                    new[] {source.Expression, Expression.Constant(index)}
                    ));
        }
    }
}