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
                    toList2 = toList2.Select(x => ((dynamic) x).x).ToList();
                }
            }
            catch (Exception)
            {
            }

            //var result = toList;
            //var itemProperty = result.GetType().GetProperty("Item");
            //result = itemProperty.GetValue(result, new object[] {0});

            //PropertyInfo property;
            //var properties = result.GetType().GetProperties();
            //while ((property = result.GetType().GetProperty("x")) != null)
            //{
            //    result = property.GetValue(result, null);
            //}
//            return null;
            return toList2.Cast<T>();


            //// TODO: Modify to unlimited

            //IEnumerable<T> newEnumerable;
            //switch (Childs.Count)
            //{
            //    case 1:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 2:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 3:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 4:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 5:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 6:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //        var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 7:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //        var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //        var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 8:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //        var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //        var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //        var p7 = Childs[7].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    case 9:
            //    {
            //        var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //        var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //        var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //        var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //        var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //        var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //        var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //        var p7 = Childs[7].CreateIncludeQuery(OriginalQueryable);
            //        var p8 = Childs[8].CreateIncludeQuery(OriginalQueryable);
            //        newEnumerable = OriginalQueryable.Select(x => new {x, p0, p1, p2, p3, p4, p5, p6, p7, p8}).ToList().Select(x => x.x);
            //        break;
            //    }
            //    default:
            //    {
            //        throw new Exception(ExceptionMessage.QueryIncludeQuery_ToManyInclude);
            //    }
            //}
            return null;

            // return newEnumerable;
        }

        public IQueryable SelectIncludeQuery<T>(IQueryable<T> parent, IQueryable child)
        {
            return parent.Select(x => new {x, child});
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
                    newQuery = OriginalQueryable.Select(x => new {x, z = 1, q});
                }
                else
                {
                    var methodGeneric = method.MakeGenericMethod(newQuery.ElementType);
                    var newQuery2 = methodGeneric.Invoke(this, new object[] {newQuery, q});
                    newQuery = (IQueryable) newQuery2;
                }
            }


            //IQueryable<object> newQuery = null;

            //foreach (var child in Childs)
            //{
            //    var q = child.CreateIncludeQuery(OriginalQueryable);

            //    newQuery = newQuery == null ?
            //        (IQueryable<object>) OriginalQueryable.Select(x => new {x, q}) :
            //        newQuery.Select(x => new {x, q});
            //}

            //switch (Childs.Count)
            //{
            //    case 1:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, y = new List<object> { p0 } });
            //            break;
            //        }
            //    case 2:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, y = p0 }).Select(x => new { x, y = p1 });
            //            break;
            //        }
            //    case 3:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, y = new { p0, z = new { p1, p2 } } });
            //            break;
            //        }
            //    case 4:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3 });
            //            break;
            //        }
            //    case 5:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3, p4 });
            //            break;
            //        }
            //    case 6:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //            var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3, p4, p5 });
            //            break;
            //        }
            //    case 7:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //            var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //            var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3, p4, p5, p6 });
            //            break;
            //        }
            //    case 8:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //            var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //            var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //            var p7 = Childs[7].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3, p4, p5, p6, p7 });
            //            break;
            //        }
            //    case 9:
            //        {
            //            var p0 = Childs[0].CreateIncludeQuery(OriginalQueryable);
            //            var p1 = Childs[1].CreateIncludeQuery(OriginalQueryable);
            //            var p2 = Childs[2].CreateIncludeQuery(OriginalQueryable);
            //            var p3 = Childs[3].CreateIncludeQuery(OriginalQueryable);
            //            var p4 = Childs[4].CreateIncludeQuery(OriginalQueryable);
            //            var p5 = Childs[5].CreateIncludeQuery(OriginalQueryable);
            //            var p6 = Childs[6].CreateIncludeQuery(OriginalQueryable);
            //            var p7 = Childs[7].CreateIncludeQuery(OriginalQueryable);
            //            var p8 = Childs[8].CreateIncludeQuery(OriginalQueryable);
            //            newQuery = OriginalQueryable.Select(x => new { x, p0, p1, p2, p3, p4, p5, p6, p7, p8 });
            //            break;
            //        }
            //    default:
            //        {
            //            throw new Exception(ExceptionMessage.QueryIncludeQuery_ToManyInclude);
            //        }
            //}

            return newQuery;
        }
    }
}