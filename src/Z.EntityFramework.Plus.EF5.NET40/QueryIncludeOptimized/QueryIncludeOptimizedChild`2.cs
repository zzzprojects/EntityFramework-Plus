// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
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
        public QueryIncludeOptimizedChild(Expression<Func<T, IEnumerable<TChild>>> filter)
        {
            Filter = filter;
        }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T, IEnumerable<TChild>>> Filter { get; set; }

        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        public override void CreateIncludeQuery(IQueryable rootQuery)
        {
            var queryable = rootQuery as IQueryable<T>;

            if (queryable == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            queryable.Select(Filter).Future();
        }
    }
}