// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Extensions;

namespace Z.EntityFramework.Plus
{
    public static class QueryIncludeFilterManager
    {
        static QueryIncludeFilterManager()
        {
            EntityFrameworkManager.IsEntityFrameworkPlus = true;
        }
        public static bool AllowQueryBatch { get; set; } = true;
        public static bool AllowIncludeSubPath { get; set; } = true;

        internal static IQueryable<T> IncludeFilterSingleLazy<T, TChild>(this IQueryable<T> query, Expression<Func<T, TChild>> queryIncludeFilter) where T : class where TChild : class
        {
            // Used by: QueryIncludeFilterIncludeSubPath.IncludeSubPath

            // GET query root
            var includeOrderedQueryable = query as QueryIncludeFilterParentQueryable<T> ?? new QueryIncludeFilterParentQueryable<T>(query);

            // ADD sub query
            includeOrderedQueryable.Childs.Add(new QueryIncludeFilterChild<T, TChild>(queryIncludeFilter, true));

            // RETURN root
            return includeOrderedQueryable;
        }

	    internal static IQueryable<TSource> SelectMany<TSource, TList, TEnumerable>(object test) where TList : TEnumerable
	    {
		    var method = typeof(Queryable).GetMethods().First(x => x.Name == "SelectMany");
		    var methodGeneric = method.MakeGenericMethod(typeof(TList), typeof(TSource));

		    Expression<Func<TList, TEnumerable>> selectorConvert = source => source;
		    Expression<Func<TList, TList>> selectorNoConvert = source => source;
		    object selector = typeof(TList) == typeof(TEnumerable) ? (object)selectorNoConvert : selectorConvert;
		    var selectManyQuery = (IQueryable<TSource>)methodGeneric.Invoke(null, new[] { test, selector });

		    return selectManyQuery;
        }

        internal static void SelectManyFutureNoCast<IEntity>(IQueryable<IEnumerable<IEntity>> query)
        {
            query.SelectMany(x => x).Future();
        }


        internal static IEnumerable<IEntity> SelectManyWithReturn<IEntity>(IQueryable<IEnumerable<IEntity>> query)
        { 
            return query.SelectMany(x => x);
        }
       

        internal static IEnumerable<IEntity> SelectManyNoCastToList<IEntity>(IQueryable<IEnumerable<IEntity>> query)
        {
            return query.SelectMany(x => x).ToList();
        }


        internal static IQueryable<TSource> SelectManyFuture<TSource, TList, TEnumerable>(object test) where TList : TEnumerable
        {
            var method = typeof(Queryable).GetMethods().First(x => x.Name == "SelectMany");
            var methodGeneric = method.MakeGenericMethod(typeof(TList), typeof(TSource));
           
            Expression<Func<TList, TEnumerable>> selectorConvert = source => source;
            Expression<Func<TList, TList>> selectorNoConvert = source => source;
            object selector = typeof(TList) == typeof(TEnumerable) ? (object) selectorNoConvert : selectorConvert;
            var selectManyQuery = (IQueryable<TSource>) methodGeneric.Invoke(null, new[] {test, selector });
            selectManyQuery.Future();

            return selectManyQuery;
        }

        internal static List<TSource> SelectManyToList<TSource, TList, TEnumerable>(object test) where TList : TEnumerable
        {
            var method = typeof(Queryable).GetMethods().First(x => x.Name == "SelectMany");
            var methodGeneric = method.MakeGenericMethod(typeof(TList), typeof(TSource));

            Expression<Func<TList, TEnumerable>> selector = source => source;
            var selectManyQuery = (IQueryable<TSource>) methodGeneric.Invoke(null, new[] {test, selector});

            return selectManyQuery.ToList();
        }
    }
}