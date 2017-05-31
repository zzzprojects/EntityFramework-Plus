// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include optimized child.</summary>
    /// <typeparam name="T">The type of elements of the parent query.</typeparam>
    /// <typeparam name="TChild">The type of elements of the child.</typeparam>
    public class QueryIncludeOptimizedChild<T, TChild> : BaseQueryIncludeOptimizedChild
    {
        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        public QueryIncludeOptimizedChild(Expression<Func<T, TChild>> filter)
        {
            Filter = filter;
        }
        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        /// <param name="isLazy">true if this object is lazy, false if not.</param>
        public QueryIncludeOptimizedChild(Expression<Func<T, TChild>> filter, bool isLazy)
        {
            Filter = filter;
            IsLazy = isLazy;
        }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T, TChild>> Filter { get; set; }

        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        public override void CreateIncludeQuery(IQueryable rootQuery)
        {
            var queryable = rootQuery as IQueryable<T>;

            if (queryable == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            if (QueryIncludeOptimizedManager.AllowQueryBatch)
            {
                var subQuery = queryable.Select(Filter);

                if (subQuery is QueryIncludeOptimizedParentQueryable<TChild>)
                {
                    subQuery = ((QueryIncludeOptimizedParentQueryable<TChild>) subQuery).OriginalQueryable;
                }

                subQuery.Future();
            }
            else
            {
                var list = queryable.Select(Filter).ToList();
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