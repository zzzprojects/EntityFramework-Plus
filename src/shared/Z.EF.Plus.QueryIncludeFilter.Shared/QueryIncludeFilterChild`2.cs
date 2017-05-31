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

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include filter child.</summary>
    /// <typeparam name="T">The type of elements of the parent query.</typeparam>
    /// <typeparam name="TChild">The type of elements of the child.</typeparam>
    public class QueryIncludeFilterChild<T, TChild> : BaseQueryIncludeFilterChild
    {
        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        public QueryIncludeFilterChild(Expression<Func<T, IEnumerable<TChild>>> filter)
        {
            Filter = filter;
        }

        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        public QueryIncludeFilterChild(Expression<Func<T, TChild>> filter)
        {
            FilterSingle = filter;
        }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T, IEnumerable<TChild>>> Filter { get; set; }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T, TChild>> FilterSingle { get; set; }

        public override Expression GetFilter()
        {
            return (Expression)Filter ?? FilterSingle;
        }

        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        /// <returns>The query to use to load related entities.</returns>
        public override IQueryable CreateIncludeQuery(IQueryable rootQuery)
        {
            var queryable = rootQuery as IQueryable<T>;

            if (queryable == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            if (Filter != null)
            {
                return queryable.Select(Filter);
            }
            return queryable.Select(FilterSingle);
        }
    }
}