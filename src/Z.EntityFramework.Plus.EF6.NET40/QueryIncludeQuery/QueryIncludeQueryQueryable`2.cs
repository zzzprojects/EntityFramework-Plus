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
    /// <summary>A query include query queryable.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    public class QueryIncludeQueryQueryable<T1, T2> : IQueryIncludeQueryQueryable
    {
        /// <summary>Constructor.</summary>
        /// <param name="selector">The selector query to included filtered included related entities.</param>
        public QueryIncludeQueryQueryable(Expression<Func<T1, IEnumerable<T2>>> selector)
        {
            Selector = selector;
        }

        /// <summary>Gets or sets the selector query to included filtered included related entities.</summary>
        /// <value>The selector query to included filtered included related entities.</value>
        public Expression<Func<T1, IEnumerable<T2>>> Selector { get; set; }

        /// <summary>Query and filter included related entities.</summary>
        /// <param name="query">The query.</param>
        /// <returns>An IQueryable which included related entities are filtered.</returns>
        public IQueryable Select(IQueryable query)
        {
            var newQuery = query as IQueryable<T1>;

            if (newQuery == null)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            return newQuery.Select(Selector);
        }
    }
}