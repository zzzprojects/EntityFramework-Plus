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

namespace Z.EntityFramework.Plus
{
    /// <summary>A query include query queryable.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class QueryIncludeQueryQueryable<T> : IOrderedQueryable<T>
    {
        /// <summary>Constructor.</summary>
        /// <param name="query">The query.</param>
        public QueryIncludeQueryQueryable(IQueryable<T> query)
        {
            OriginalQueryable = query;
            Queries = new List<IQueryIncludeQueryQueryable>();
        }

        /// <summary>Constructor.</summary>
        /// <param name="query">The query.</param>
        /// <param name="queries">The childs.</param>
        public QueryIncludeQueryQueryable(IQueryable<T> query, List<IQueryIncludeQueryQueryable> queries)
        {
            OriginalQueryable = query;
            Queries = queries;
        }

        /// <summary>Gets or sets the original queryable.</summary>
        /// <value>The original queryable.</value>
        public IQueryable<T> OriginalQueryable { get; set; }

        /// <summary>Gets or sets the queries.</summary>
        /// <value>The queries.</value>
        public List<IQueryIncludeQueryQueryable> Queries { get; set; }

        /// <summary>Gets or sets the internal provider.</summary>
        /// <value>The internal provider.</value>
        public QueryIncludeQueryProvider<T> InternalProvider { get; set; }

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
            get { return InternalProvider ?? (InternalProvider = new QueryIncludeQueryProvider<T>(OriginalQueryable.Provider) {CurrentQueryable = this}); }
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
            IEnumerable<T> newEnumerable;
            switch (Queries.Count)
            {
                case 1:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0}).ToList().Select(x => x.x);
                    break;
                }
                case 2:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1}).ToList().Select(x => x.x);
                    break;
                }
                case 3:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2}).ToList().Select(x => x.x);
                    break;
                }
                case 4:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3}).ToList().Select(x => x.x);
                    break;
                }
                case 5:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4}).ToList().Select(x => x.x);
                    break;
                }
                case 6:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5}).ToList().Select(x => x.x);
                    break;
                }
                case 7:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6}).ToList().Select(x => x.x);
                    break;
                }
                case 8:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    var p7 = Queries[7].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7}).ToList().Select(x => x.x);
                    break;
                }
                case 9:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    var p7 = Queries[7].Select(OriginalQueryable);
                    var p8 = Queries[8].Select(OriginalQueryable);
                    newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7, p8}).ToList().Select(x => x.x);
                    break;
                }
                default:
                {
                    throw new Exception(ExceptionMessage.QueryIncludeQuery_ToManyInclude);
                }
            }

            return newEnumerable;
        }

        /// <summary>Creates the queryable.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <returns>The new queryable.</returns>
        public IQueryable CreateQueryable()
        {
            IQueryable newQuery;
            switch (Queries.Count)
            {
                case 1:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0});
                    break;
                }
                case 2:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1});
                    break;
                }
                case 3:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2});
                    break;
                }
                case 4:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3});
                    break;
                }
                case 5:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4});
                    break;
                }
                case 6:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5});
                    break;
                }
                case 7:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6});
                    break;
                }
                case 8:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    var p7 = Queries[7].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7});
                    break;
                }
                case 9:
                {
                    var p0 = Queries[0].Select(OriginalQueryable);
                    var p1 = Queries[1].Select(OriginalQueryable);
                    var p2 = Queries[2].Select(OriginalQueryable);
                    var p3 = Queries[3].Select(OriginalQueryable);
                    var p4 = Queries[4].Select(OriginalQueryable);
                    var p5 = Queries[5].Select(OriginalQueryable);
                    var p6 = Queries[6].Select(OriginalQueryable);
                    var p7 = Queries[7].Select(OriginalQueryable);
                    var p8 = Queries[8].Select(OriginalQueryable);
                    newQuery = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7, p8});
                    break;
                }
                default:
                {
                    throw new Exception(ExceptionMessage.QueryIncludeQuery_ToManyInclude);
                }
            }

            return newQuery;
        }
    }
}