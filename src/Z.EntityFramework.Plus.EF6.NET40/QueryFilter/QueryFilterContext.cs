// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A query filter context.</summary>
    public class QueryFilterContext
    {
        /// <summary>Create a new QueryFilterContext.</summary>
        /// <param name="context">The context associated to the filter context.</param>
        public QueryFilterContext(DbContext context) : this(context, false)
        {
        }

        /// <summary>Create a new QueryFilterContext.</summary>
        /// <param name="context">The context associated to the filter context.</param>
        /// <param name="isGenericContext">true if this filter context is the generic context used by other filter context.</param>
        public QueryFilterContext(DbContext context, bool isGenericContext)
        {
            if (isGenericContext)
            {
                LoadGenericContextInfo(context);
            }
            else
            {
                Context = context;
                Filters = new Dictionary<object, IQueryFilter>();

                var genericContext = QueryFilterManager.AddOrGetGenericFilterContext(context);
                FilterSetByType = genericContext.FilterSetByType;
                FilterSets = genericContext.FilterSets;
            }
        }

        /// <summary>Gets or sets the context associated with the filter context.</summary>
        /// <value>The context associated with the filter context.</value>
        public DbContext Context { get; set; }

        /// <summary>Gets or sets the filters.</summary>
        /// <value>The filters.</value>
        public Dictionary<object, IQueryFilter> Filters { get; set; }

        /// <summary>Gets or sets filter set by type.</summary>
        /// <value>The filter set by type.</value>
        public Dictionary<Type, List<QueryFilterSet>> FilterSetByType { get; set; }

        /// <summary>Gets or sets filter sets.</summary>
        /// <value>The filter sets.</value>
        public List<QueryFilterSet> FilterSets { get; set; }

        /// <summary>Adds a filter to the filter context associated with the specified key.</summary>
        /// <typeparam name="T">Generic type to filter.</typeparam>
        /// <param name="key">The filter key.</param>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The filter context associated with the specified key.</returns>
        public IQueryFilter AddFilter<T>(object key, Func<IQueryable<T>, IQueryable<T>> predicate)
        {
            var filter = new QueryFilter<T>(this, predicate);
            Filters.Add(key, filter);

            return filter;
        }

        /// <summary> Returns a new query where the entities are filtered by using filter from specified keys.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="keys">A variable-length parameters list containing keys associated to filter to use.</param>
        /// <returns>The new query where the entities are filtered by using filter from specified keys.</returns>
        public IQueryable<TEntity> ApplyFilter<TEntity>(IQueryable<TEntity> query, object[] keys)
        {
            object newQuery = query;
            foreach (var key in keys)
            {
                var filter = GetFilter(key);

                if (filter != null)
                {
                    newQuery = ((IQueryable) filter.ApplyFilter<TEntity>(newQuery));
                }
            }

            return (IQueryable<TEntity>) newQuery;
        }

        /// <summary>Disable this filter on the specified types.</summary>
        /// <param name="filter">The filter to disable.</param>
        /// <param name="types">A variable-length parameters list containing types to disable the filter on.</param>
        public void DisableFilter(IQueryFilter filter, params Type[] types)
        {
            List<QueryFilterSet> filterSets;

            if (FilterSetByType.TryGetValue(filter.ElementType, out filterSets))
            {
                if (types != null)
                {
                    var applySets = new List<QueryFilterSet>();

                    foreach (var type in types)
                    {
                        List<QueryFilterSet> setToAdd;
                        if (FilterSetByType.TryGetValue(type, out setToAdd))
                        {
                            applySets.AddRange(setToAdd);
                        }
                    }

                    filterSets = filterSets.Intersect(applySets.Distinct()).ToList();
                }

                foreach (var set in filterSets)
                {
                    set.AddOrGetFilterQueryable(Context).DisableFilter(filter);
                }
            }
        }

        /// <summary>Enable this filter on the specified types.</summary>
        /// <param name="filter">The filter to enable.</param>
        /// <param name="types">A variable-length parameters list containing types to enable the filter on.</param>
        public void EnableFilter(IQueryFilter filter, params Type[] types)
        {
            List<QueryFilterSet> filterSets;

            if (FilterSetByType.TryGetValue(filter.ElementType, out filterSets))
            {
                if (types != null)
                {
                    var applySets = new List<QueryFilterSet>();

                    foreach (var type in types)
                    {
                        List<QueryFilterSet> setToAdd;
                        if (FilterSetByType.TryGetValue(type, out setToAdd))
                        {
                            applySets.AddRange(setToAdd);
                        }
                    }

                    filterSets = filterSets.Intersect(applySets.Distinct()).ToList();
                }

                foreach (var set in filterSets)
                {
                    set.AddOrGetFilterQueryable(Context).EnableFilter(filter);
                }
            }
        }

        /// <summary>Gets the filter associated to the specified key.</summary>
        /// <param name="key">The filter key.</param>
        /// <returns>The filter associated to the specified key.</returns>
        public IQueryFilter GetFilter(object key)
        {
            IQueryFilter filter;
            Filters.TryGetValue(key, out filter);
            return filter;
        }

        /// <summary>Load generic filter context information.</summary>
        /// <param name="context">The context to use to load the generic filter context information.</param>
        public void LoadGenericContextInfo(DbContext context)
        {
            FilterSetByType = new Dictionary<Type, List<QueryFilterSet>>();
            FilterSets = new List<QueryFilterSet>();

            var dbSetProperties = context.GetDbSetProperties();

            // ADD DbSet
            foreach (var dbSetProperty in dbSetProperties)
            {
                FilterSets.Add(new QueryFilterSet(dbSetProperty));
            }

            // LINK DbSet to Type
            foreach (var filterDbSet in FilterSets)
            {
                var baseType = filterDbSet.ElementType;

                while (baseType != null && baseType != typeof (object))
                {
                    // LINK type
                    FilterSetByType.AddOrAppend(baseType, filterDbSet);

                    // LINK interface
                    var interfaces = baseType.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        FilterSetByType.AddOrAppend(@interface, filterDbSet);
                    }

                    baseType = baseType.BaseType;
                }
            }
        }
    }
}