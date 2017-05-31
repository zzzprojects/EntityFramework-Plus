// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>Select from the query all names.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to select name from.</param>
        /// <param name="names">The name to select.</param>
        /// <returns>An IQueryable with the name selected.</returns>
        internal static IQueryable SelectByName<T>(this IQueryable<T> query, List<string> names)
        {
            var type = typeof(T);

            if (names.Count == 1)
            {
                var expressionQuery = Expression.Parameter(typeof(IQueryable<T>));

                var expressionParameter = Expression.Parameter(type);
                var expressionProperty = Expression.Property(expressionParameter, type.GetProperty(names[0]));
                var lambdaSelector = Expression.Lambda(expressionProperty, expressionParameter);
                var selectMethod = typeof(Queryable).GetMethods().First(x => x.Name == "Select"
                                                                             && x.GetParameters().Length == 2
                                                                             && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);

                var selectMethodGeneric = selectMethod.MakeGenericMethod(type, expressionProperty.Type);
                var expressionSelect = Expression.Call(selectMethodGeneric, new Expression[] { expressionQuery, lambdaSelector });
                var lambdaSelect = Expression.Lambda<Func<IQueryable<T>, object>>(expressionSelect, expressionQuery).Compile();

                // query.Select(x => x.Property1);
                return (IQueryable)lambdaSelect(query);
            }
            else
            {
                var expressionParameter = Expression.Parameter(type);
                var list = names.Select((x, i) => new Tuple<string, Expression>(x, Expression.Property(expressionParameter, type.GetProperty(names[i])))).ToList();
                var anonymousType = DynamicAnonymousType.CreateExpression(list);


                var lambdaSelector = Expression.Lambda(anonymousType, expressionParameter);

                var selectMethod = typeof(Queryable).GetMethods().First(x => x.Name == "Select"
                                                                             && x.GetParameters().Length == 2
                                                                             && x.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);

                var expressionQuery = Expression.Parameter(typeof(IQueryable<T>));
                var selectMethodGeneric = selectMethod.MakeGenericMethod(type, anonymousType.Type);
                var expressionSelect = Expression.Call(selectMethodGeneric, new Expression[] { expressionQuery, lambdaSelector });
                var lambdaSelect = Expression.Lambda<Func<IQueryable<T>, object>>(expressionSelect, expressionQuery).Compile();

                // query.Select(x => new { x.Property1, x.Property2, ..., x.PropertyN });
                return (IQueryable)lambdaSelect(query);
            }
        }
    }
}
#endif