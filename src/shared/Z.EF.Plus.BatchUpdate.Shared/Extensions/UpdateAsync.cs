// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Extensions;

namespace Z.EntityFramework.Plus
{
    public static partial class BatchUpdateExtensions
    {
        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Update(updateFactory, null), cancellationToken);
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows from the query using an
        ///     expression  without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="updateFactory">The update factory.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task with the number of rows affected.</returns>
        public static Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> updateFactory, Action<BatchUpdate> batchUpdateBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.Run(() => query.Update(updateFactory, batchUpdateBuilder), cancellationToken);
        }


        #region Expando

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, ExpandoObject expandoObject) where T : class
        {
            return await Task.Run(() => query.Update(expandoObject)).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, ExpandoObject expandoObject, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(expandoObject), cancellationToken).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, ExpandoObject expandoObject, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
            return await Task.Run(() => query.Update(expandoObject, batchUpdateBuilder)).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="expandoObject">The expandoObject.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, ExpandoObject expandoObject, Action<BatchUpdate> batchUpdateBuilder, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(expandoObject, batchUpdateBuilder), cancellationToken).ConfigureAwait(false); ;
        }

        #endregion

        #region Dictionary

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, IDictionary<string, object> dictionary) where T : class
        {
            return await Task.Run(() => query.Update(dictionary)).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, IDictionary<string, object> dictionary, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(dictionary), cancellationToken).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, IDictionary<string, object> dictionary, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
            return await Task.Run(() => query.Update(dictionary, batchUpdateBuilder)).ConfigureAwait(false); ;
        }

        /// <summary>
        ///     An IQueryable&lt;T&gt; extension method that updates all rows asynchronously from the query
        ///     using an expression without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to update rows from without retrieving entities.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, IDictionary<string, object> dictionary, Action<BatchUpdate> batchUpdateBuilder, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(dictionary, batchUpdateBuilder), cancellationToken).ConfigureAwait(false); ;
        }

        #endregion

        #region Anonymous

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <returns>The number of row affected.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression) where T : class
        {
            return await Task.Run(() => query.Update(updateExpression)).ConfigureAwait(false);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of row affected.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(updateExpression), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <returns>The number of row affected.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression, Action<BatchUpdate> batchUpdateBuilder) where T : class
        {
            return await Task.Run(() => query.Update(updateExpression, batchUpdateBuilder)).ConfigureAwait(false);
        }

        /// <summary>An IQueryable&lt;T&gt; extension method that updates from query.</summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="batchUpdateBuilder">The batch update builder to change default configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of row affected.</returns>
        public static async Task<int> UpdateAsync<T>(this IQueryable<T> query, Expression<Func<T, object>> updateExpression, Action<BatchUpdate> batchUpdateBuilder, CancellationToken cancellationToken) where T : class
        {
            return await Task.Run(() => query.Update(updateExpression, batchUpdateBuilder), cancellationToken).ConfigureAwait(false);
        }
        #endregion
    }
}

#endif