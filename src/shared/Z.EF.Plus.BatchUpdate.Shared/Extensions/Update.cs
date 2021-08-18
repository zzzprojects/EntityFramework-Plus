// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Extensions;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchUpdateExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update expression.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory) where T : class
        {
			return query.Update(updateFactory, null);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update expression.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
#if EFCORE
            return query.UpdateFromQuery(updateFactory, batchUpdateBuilder);
#else
            return query.UpdateFromQuery(updateFactory, options =>
            {
                options.InternalIsEntityFrameworkPlus = true;
                options.BatchUpdateBuilder = batchUpdateBuilder;
            });
#endif
        }


        #region Expando & Dictionary & Anonymous

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, ExpandoObject expandoObject) where T : class
        {
            return Update(query, (IDictionary<string, object>)expandoObject, null);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, ExpandoObject expandoObject, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
            return Update(query, (IDictionary<string, object>)expandoObject, batchUpdateBuilder);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, IDictionary<string, object> dictionary) where T : class
        {
            return Update(query, dictionary, null);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, IDictionary<string, object> dictionary, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
#if EFCORE
            return query.UpdateFromQuery(dictionary, batchUpdateBuilder);
#else
            return query.UpdateFromQuery(dictionary, options =>
            {
                options.InternalIsEntityFrameworkPlus = true;
                options.BatchUpdateBuilder = batchUpdateBuilder;
            });
#endif
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression) where T : class
        {
            return Update(query, updateExpression, null);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="batchUpdateBuilder">The batch builder action to change default configuration.</param>
        /// <returns>The number of row affected.</returns>
        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
#if EFCORE
            return query.UpdateFromQuery(updateExpression, batchUpdateBuilder);
#else
            return query.UpdateFromQuery(updateExpression, options =>
            {
                options.InternalIsEntityFrameworkPlus = true;
                options.BatchUpdateBuilder = batchUpdateBuilder;
            });
#endif
        }

		#endregion
	}
}