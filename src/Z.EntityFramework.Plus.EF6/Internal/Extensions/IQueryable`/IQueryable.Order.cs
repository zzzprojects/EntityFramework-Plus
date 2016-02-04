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
    public static partial class IQueryableExtensions
    {
        internal static IOrderedQueryable<TSource> Order<TSource>(this IQueryable<TSource> source, string propertyName, bool useOrderBy, bool ascending, object comparer = null)
        {
            IOrderedQueryable<TSource> query;

            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof (TSource), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            if (comparer == null)
            {
                // REFLECTION: source.[OrderMethod](x => x.[PropertyName])
                var orderMethod = useOrderBy ?
                    ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2) :
                    ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 2) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);

                var orderMethodGeneric = orderMethod.MakeGenericMethod(typeof (TSource), property.Type);

                query = (IOrderedQueryable<TSource>) orderMethodGeneric.Invoke(null, new object[] {source, lambda});
            }
            else
            {
                // REFLECTION: source.[OrderMethod](x => x.[PropertyName], comparer)
                var orderMethod = useOrderBy ?
                    ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 3) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 3) :
                    ascending ?
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenBy" && x.GetParameters().Length == 3) :
                        typeof (Queryable).GetMethods().First(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 3);

                var orderMethodGeneric = orderMethod.MakeGenericMethod(typeof (TSource), property.Type);

                query = (IOrderedQueryable<TSource>) orderMethodGeneric.Invoke(null, new[] {source, lambda, comparer});
            }


            return query;
        }
    }
}