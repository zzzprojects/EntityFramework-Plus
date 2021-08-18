// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter child.</summary>
    /// <typeparam name="T">The type of elements of the parent query.</typeparam>
    /// <typeparam name="TChild">The type of elements of the child.</typeparam>
    public class QueryIncludeFilterChild<T, TChild> : BaseQueryIncludeFilterChild
    {
        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        public QueryIncludeFilterChild(Expression<Func<T, TChild>> filter)
        {
            Filter = filter;
        }

        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        /// <param name="isLazy">true if this object is lazy, false if not.</param>
        public QueryIncludeFilterChild(Expression<Func<T, TChild>> filter, bool isLazy)
        {
            Filter = filter;
            IsLazy = isLazy;
        }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T, TChild>> Filter { get; set; }

	    internal Tuple<object,object> GetFinalCollectionQuery(Type listType, object subQuery)
		{
			Tuple<object, object> finalQuery = new Tuple<object, object>(listType, subQuery);
			var elementType = listType.GenericTypeArguments.Count() != 0 ? listType.GenericTypeArguments[0] : listType.BaseType.GenericTypeArguments[0]; 
		    var isCollection = typeof(IEnumerable).IsAssignableFrom(elementType);
			if (isCollection)
			{
#if EFCORE_2X
				var selectManyMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectMany", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				    ?.MakeGenericMethod(elementType, listType, typeof(IEnumerable<>).MakeGenericType(elementType));
			    var subQueryList2 = selectManyMethod?.Invoke(this, new object[] {subQuery});
#else

				var selectManyMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectManyWithReturn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
					?.MakeGenericMethod(elementType);
				var subQueryList2 = selectManyMethod?.Invoke(this, new object[] { subQuery });
#endif
				listType = elementType;

				finalQuery = GetFinalCollectionQuery(listType, subQueryList2);
			}

			return finalQuery;
	    }

	    /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        public override void CreateIncludeQuery(IQueryable rootQuery)
        {
            var queryable = rootQuery as IQueryable<T>;

            if (queryable == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            if (QueryIncludeFilterManager.AllowQueryBatch)
            {
                var isCollection = typeof(IEnumerable).IsAssignableFrom(typeof(TChild));

                if (isCollection)
                {
                    // Order.Select(x => x.OrderItems).SelectMany(x => x).Future();
                    var subQuery = queryable.Select(Filter);

                    var listType = subQuery.GetType().GenericTypeArguments[0];
	                var tuple = GetFinalCollectionQuery(listType, subQuery);
                    var elementType = ((Type)tuple.Item1).GenericTypeArguments.Count() != 0 ? ((Type)tuple.Item1).GenericTypeArguments[0] : ((Type)tuple.Item1).BaseType.GenericTypeArguments[0]; 
#if EFCORE_2X
                    var selectManyFutureMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectManyFuture", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
		                ?.MakeGenericMethod(elementType, (Type)tuple.Item1, typeof(IEnumerable<>).MakeGenericType(elementType));
	                selectManyFutureMethod?.Invoke(this, new object[] { tuple.Item2 });
#else
                    var selectManyFutureMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectManyFutureNoCast", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(elementType);
                    selectManyFutureMethod?.Invoke(this, new object[] { tuple.Item2 });
#endif
                }
                else
                {
                    var subQuery = queryable.Select(Filter);

                    if (subQuery is QueryIncludeFilterParentQueryable<TChild>)
                    {
                        subQuery = ((QueryIncludeFilterParentQueryable<TChild>) subQuery).OriginalQueryable;
                    }

                    subQuery.Future();
                }
            }
            else
            {
                var isCollection = typeof(IEnumerable).IsAssignableFrom(typeof(TChild));

                if (isCollection)
                {
                    // Order.Select(x => x.OrderItems).SelectMany(x => x).ToList();
                    var subQuery = queryable.Select(Filter);

                    var listType = subQuery.GetType().GenericTypeArguments[0];
                    var elementType = listType.GenericTypeArguments.Count() != 0 ? listType.GenericTypeArguments[0] : listType.BaseType.GenericTypeArguments[0];
#if EFCORE_2X
                    var selectManyToListMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectManyToList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(elementType, listType, typeof(IEnumerable<>).MakeGenericType(elementType));
#else
                    var selectManyToListMethod = typeof(QueryIncludeFilterManager).GetMethod("SelectManyNoCastToList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(elementType); 
#endif
                    var subQueryList = selectManyToListMethod?.Invoke(this, new object[] {subQuery});
                }
                else
                {
                    var list = queryable.Select(Filter).ToList();
                }
            }
        }

        public override Expression GetFilter()
        {
            return Filter;
        }

        public override IQueryable GetFilteredQuery(IQueryable rootQuery)
        {
            var queryable = rootQuery as IQueryable<T>;

            if (queryable == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            return queryable.Select(Filter);
        }
    }
}