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
    public static class QueryIncludeQueryExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that allow to filter and query included related
        ///     entities.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter for included entities.</typeparam>
        /// <param name="source">The source query to filter and query included related entities.</param>
        /// <param name="selector">The selector query to included filtered included related entities.</param>
        /// <returns>
        ///     An IQueryable&lt;T&gt; that allow to filter and query included related entities.
        /// </returns>
        public static IQueryable<T1> IncludeQuery<T1, T2>(this IQueryable<T1> source, Expression<Func<T1, IEnumerable<T2>>> selector) where T1 : class where T2 : class
        {
            // GET query root
            var includeOrderedQueryable = source as QueryIncludeQueryQueryable<T1> ?? new QueryIncludeQueryQueryable<T1>(source);

            // ADD sub query
            includeOrderedQueryable.Queries.Add(new QueryIncludeQueryQueryable<T1, T2>(selector));

            // RETURN root
            return includeOrderedQueryable;
        }
    }
}