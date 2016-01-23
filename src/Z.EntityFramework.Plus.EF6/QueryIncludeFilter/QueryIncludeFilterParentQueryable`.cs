// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query include query parent queryable.</summary>
    /// <typeparam name="T">The type of elements of the query.</typeparam>
    public class QueryIncludeFilterParentQueryable<T> : IOrderedQueryable<T>
    {
        /// <summary>Constructor.</summary>
        /// <param name="query">The query parent.</param>
        public QueryIncludeFilterParentQueryable(IQueryable<T> query)
        {
            OriginalQueryable = query;
            Childs = new List<BaseQueryIncludeFilterChild>();
        }

        /// <summary>Constructor.</summary>
        /// <param name="query">The query.</param>
        /// <param name="childs">The childs.</param>
        public QueryIncludeFilterParentQueryable(IQueryable<T> query, List<BaseQueryIncludeFilterChild> childs)
        {
            OriginalQueryable = query;
            Childs = childs;
        }

        /// <summary>Gets or sets the query childs.</summary>
        /// <value>The query childs.</value>
        public List<BaseQueryIncludeFilterChild> Childs { get; set; }

        /// <summary>Gets or sets the internal provider.</summary>
        /// <value>The internal provider.</value>
        public QueryIncludeFilterProvider<T> InternalProvider { get; set; }

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
            get { return InternalProvider ?? (InternalProvider = new QueryIncludeFilterProvider<T>(OriginalQueryable.Provider) {CurrentQueryable = this}); }
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
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Enumerates create enumerable in this collection.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process create enumerable in this collection.
        /// </returns>
        public IEnumerable<T> CreateEnumerable()
        {
            if (Childs.Count == 0)
            {
                return OriginalQueryable;
            }

            var method = GetType().GetMethod("SelectIncludeQuery");
            IQueryable newQuery = null;

            foreach (var child in Childs)
            {
                var q = child.CreateIncludeQuery(OriginalQueryable);

                if (newQuery == null)
                {
                    newQuery = OriginalQueryable.Select(x => new {x, q});
                }
                else
                {
                    var methodGeneric = method.MakeGenericMethod(newQuery.ElementType);
                    var newQuery2 = methodGeneric.Invoke(this, new object[] {newQuery, q});
                    newQuery = (IQueryable) newQuery2;
                }
            }

            var toListMethod = typeof (Enumerable).GetMethod("ToList").MakeGenericMethod(newQuery.ElementType);
            var toList = toListMethod.Invoke(null, new object[] {newQuery});

            var toList2 = (IEnumerable<object>) toList;

            try
            {
                while (true)
                {
                    toList2 = toList2.Select(x => ((dynamic)x).x).ToList();
                }
            }
            catch (Exception)
            {
        
            }
            
            return (IEnumerable<T>)toList2.Cast<T>();
        }

        public IQueryable SelectIncludeQuery<T>(IQueryable<T> parent, IQueryable child)
        {
            return parent.Select(x => new { x, child });
        }

        /// <summary>Creates the queryable.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>The new queryable.</returns>
        public IQueryable CreateQueryable()
        {
            if (Childs.Count == 0)
            {
                return OriginalQueryable;
            }

            var method = GetType().GetMethod("SelectIncludeQuery");
            IQueryable newQuery = null;

            foreach (var child in Childs)
            {
                var q = child.CreateIncludeQuery(OriginalQueryable);

                if (newQuery == null)
                {
                    newQuery = OriginalQueryable.Select(x => new {x, q});
                }
                else
                {
                    var methodGeneric = method.MakeGenericMethod(newQuery.ElementType);
                    var newQuery2 = methodGeneric.Invoke(this, new object[] {newQuery, q});
                    newQuery = (IQueryable) newQuery2;
                }
            }

            return newQuery;
        }
    }
}