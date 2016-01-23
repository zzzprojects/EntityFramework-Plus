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
    /// <summary>A class for query include query child.</summary>
    /// <typeparam name="T1">The type of elements of the parent query.</typeparam>
    /// <typeparam name="T2">The type of elements of the child.</typeparam>
    public class QueryIncludeFilterChild<T1, T2> : BaseQueryIncludeFilterChild
    {
        /// <summary>Constructor.</summary>
        /// <param name="filter">The query filter to apply on included related entities.</param>
        public QueryIncludeFilterChild(Expression<Func<T1, IEnumerable<T2>>> filter)
        {
            Filter = filter;
        }

        /// <summary>Gets or sets the query filter to include related entities.</summary>
        /// <value>The query filter to include related entities.</value>
        public Expression<Func<T1, IEnumerable<T2>>> Filter { get; set; }

        /// <summary>Creates the query to use to load related entities.</summary>
        /// <param name="rootQuery">The root query.</param>
        /// <returns>The query to use to load related entities.</returns>
        public override IQueryable<object> CreateIncludeQuery(IQueryable rootQuery)
        {
            var newQuery = rootQuery as IQueryable<T1>;

            if (newQuery == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            return newQuery.Select(Filter);
        }
    }
}