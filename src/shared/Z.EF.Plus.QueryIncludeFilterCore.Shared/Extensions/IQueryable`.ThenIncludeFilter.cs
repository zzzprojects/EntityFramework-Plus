//// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
//// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
//// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
//// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
//// More projects: http://www.zzzprojects.com/
//// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Microsoft.EntityFrameworkCore.Query;

//namespace Z.EntityFramework.Plus
//{
//    public static partial class QueryIncludeFilterExtensions
//    {
//        /// <summary>
//        ///     An IQueryable&lt;T&gt; extension method that include and filter related entities.
//        /// </summary>
//        /// <typeparam name="T">Generic type parameter.</typeparam>
//        /// <typeparam name="TChild">Type of the child.</typeparam>
//        /// <param name="query">The query to filter included related entities.</param>
//        /// <param name="queryIncludeFilter">The query filter to apply on included related entities.</param>
//        /// <returns>An IQueryable&lt;T&gt; that include and filter related entities.</returns>
//        public static IQueryable<T> ThenIncludeFilter<TEntity, TPreviousProperty, TChild>(this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source, Expression<Func<TPreviousProperty, IEnumerable<TChild>>> queryIncludeFilter) where TPreviousProperty : class where TChild : class
//        {
//            return query.IncludeOptimizedSingle(queryIncludeFilter);
//        }

//        /// <summary>
//        ///     An IQueryable&lt;T&gt; extension method that include and filter related entities.
//        /// </summary>
//        /// <typeparam name="T">Generic type parameter.</typeparam>
//        /// <typeparam name="TChild">Type of the child.</typeparam>
//        /// <param name="query">The query to filter included related entities.</param>
//        /// <param name="queryIncludeFilter">The query filter to apply on included related entities.</param>
//        /// <returns>An IQueryable&lt;T&gt; that include and filter related entities.</returns>
//        public static IQueryable<T> ThenIncludeFilter<TEntity, TPreviousProperty, TChild>(this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source, Expression<Func<TPreviousProperty, TChild>> queryIncludeFilter) where TPreviousProperty : class where TChild : class
//        {
//            return query.IncludeOptimizedSingle(queryIncludeFilter);
//        }
//    }
//}