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
using System.Threading.Tasks;
#if EF5
using System.Data.Metadata.Edm;

#elif EF6
using System.Data.Entity.Core.Metadata.Edm;
#endif
#if NET45
using System.Data.Entity.Infrastructure;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include optimized parent queryable.</summary>
    /// <typeparam name="T">The type of elements of the query.</typeparam>
#if EF6 && NET45
    public class QueryIncludeOptimizedParentQueryable<T> : IOrderedQueryable<T>, IDbAsyncEnumerable<T>
#else
    public class QueryIncludeOptimizedParentQueryable<T> : IOrderedQueryable<T>
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="query">The query parent.</param>
        public QueryIncludeOptimizedParentQueryable(IQueryable<T> query)
        {
            OriginalQueryable = query;
            Childs = new List<BaseQueryIncludeOptimizedChild>();
        }

        /// <summary>Constructor.</summary>
        /// <param name="query">The query.</param>
        /// <param name="childs">The childs.</param>
        public QueryIncludeOptimizedParentQueryable(IQueryable<T> query, List<BaseQueryIncludeOptimizedChild> childs)
        {
            OriginalQueryable = query;
            Childs = childs;
        }

        /// <summary>Gets or sets the query childs.</summary>
        /// <value>The query childs.</value>
        public List<BaseQueryIncludeOptimizedChild> Childs { get; set; }

        /// <summary>Gets or sets the internal provider.</summary>
        /// <value>The internal provider.</value>
        public QueryIncludeOptimizedProvider<T> InternalProvider { get; set; }

        /// <summary>Gets or sets the original queryable.</summary>
        /// <value>The original queryable.</value>
        public IQueryable<T> OriginalQueryable { get; set; }

        /// <summary>Gets the type of the element.</summary>
        /// <value>The type of the element.</value>
        public Type ElementType
        {
            get { return OriginalQueryable.ElementType; }
        }

        /// <summary>Gets the expression.</summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get { return OriginalQueryable.Expression; }
        }

        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public IQueryProvider Provider
        {
            get { return InternalProvider ?? (InternalProvider = new QueryIncludeOptimizedProvider<T>(OriginalQueryable.Provider) {CurrentQueryable = this}); }
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerable().GetEnumerator();
        }

        /// <summary>Gets the enumerator.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Enumerates create enumerable in this collection.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process create enumerable in this collection.
        /// </returns>
        public IEnumerable<T> CreateEnumerable()
        {
            // MODIFY query if necessary
#if EF5 || EF6
            var objectContext = OriginalQueryable.GetObjectQuery().Context;
            var keyMembers = ((dynamic) objectContext).CreateObjectSet<T>().EntitySet.ElementType.KeyMembers;
            var keyNames = ((IEnumerable<EdmMember>) keyMembers).Select(x => x.Name).ToArray();
#elif EF7

                var context = currentQuery.OriginalQueryable.GetDbContext();

                var keyNames = context.Model.FindEntityType(typeof (TResult).DisplayName(true))
                    .GetKeys().ToList()[0]
                    .Properties.Select(x => x.Name).ToArray();
#endif
            var newQuery = OriginalQueryable.AddToRootOrAppendOrderBy(keyNames).Select(x => x);

            foreach (var child in Childs)
            {
                child.CreateIncludeQuery(newQuery);
            }

            // RESOLVE current and all future child queries
            return newQuery.Future().ToList();
        }

        /// <summary>Creates the queryable.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>The new queryable.</returns>
        public void CreateQueryable(IQueryable<T> query)
        {
            foreach (var child in Childs)
            {
                child.CreateIncludeQuery(query);
            }
        }

        /// <summary>Includes the related entities path in the query.</summary>
        /// <param name="path">The related entities path in the query to include.</param>
        /// <returns>The new queryable.</returns>
        public IQueryable Include(string path)
        {
            throw new Exception(ExceptionMessage.QueryIncludeOptimized_Include);
        }

#if EF6 && NET45
        /// <summary>Gets the asynchrounously enumerator.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>The asynchrounously enumerator.</returns>
        IDbAsyncEnumerator<T> IDbAsyncEnumerable<T>.GetAsyncEnumerator()
        {
            return new LazyAsyncEnumerator<T>(token => Task.Run(() => CreateEnumerable(), token));
        }

        /// <summary>Gets the asynchrounously enumerator.</summary>
        /// <returns>The asynchrounously enumerator.</returns>
        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            return new LazyAsyncEnumerator<T>(token => Task.Run(() => CreateEnumerable(), token));
        }
#endif
    }
}