// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

#if EF6
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryDbSetFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryDbSetFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryDbSetFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryDbSetFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryDbSetFilterSet;
#else
using AliasBaseQueryFilter = Z.EntityFramework.Plus.BaseQueryFilter;
using AliasBaseQueryFilterQueryable = Z.EntityFramework.Plus.BaseQueryFilterQueryable;
using AliasQueryFilterContext = Z.EntityFramework.Plus.QueryFilterContext;
using AliasQueryFilterManager = Z.EntityFramework.Plus.QueryFilterManager;
using AliasQueryFilterSet = Z.EntityFramework.Plus.QueryFilterSet;
#endif

namespace Z.EntityFramework.Plus
{
    /// <summary>A class for query filter context.</summary>
#if EF6
    public class QueryDbSetFilterContext
#else
    public class QueryFilterContext
#endif
    {
        /// <summary>Constructor.</summary>
        /// <param name="context">The context associated to the filter context.</param>
#if EF6
        public QueryDbSetFilterContext(DbContext context) : this(context, false)
#else
        public QueryFilterContext(DbContext context) : this(context, false)
#endif
        {
        }

        /// <summary>Create a new QueryFilterContext.</summary>
        /// <param name="context">The context associated to the filter context.</param>
        /// <param name="isGenericContext">true if this filter context is the generic context used by other filter context.</param>
#if EF6
        public QueryDbSetFilterContext(DbContext context, bool isGenericContext)
#else
        public QueryFilterContext(DbContext context, bool isGenericContext)
#endif
        {
            if (isGenericContext)
            {
                LoadGenericContextInfo(context);
            }
            else
            {
                Context = context;
                Filters = new Dictionary<object, AliasBaseQueryFilter>();

                var genericContext = AliasQueryFilterManager.AddOrGetGenericFilterContext(context);

                FilterSetByType = genericContext.FilterSetByType;
                FilterSets = genericContext.FilterSets;
            }
        }

        /// <summary>Gets or sets the context associated with the filter context.</summary>
        /// <value>The context associated with the filter context.</value>
        public DbContext Context { get; set; }

        /// <summary>Gets or sets the filters.</summary>
        /// <value>The filters.</value>
        public Dictionary<object, AliasBaseQueryFilter> Filters { get; set; }

        /// <summary>Gets or sets filter set by type.</summary>
        /// <value>The filter set by type.</value>
        public Dictionary<Type, List<AliasQueryFilterSet>> FilterSetByType { get; set; }

        /// <summary>Gets or sets filter sets.</summary>
        /// <value>The filter sets.</value>
        public List<AliasQueryFilterSet> FilterSets { get; set; }

        /// <summary>Adds a query filter to the filter context associated with the specified key.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="key">The filter key.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The query filter added to the filter context associated with the specified ke .</returns>
        public AliasBaseQueryFilter AddFilter<T>(object key, Func<IQueryable<T>, IQueryable<T>> filter)
        {
#if EF6
            var queryFilter = new QueryDbSetFilter<T>(this, filter);
#else
            var queryFilter = new QueryFilter<T>(this, filter);
#endif
            Filters.Add(key, queryFilter);

            return queryFilter;
        }

        /// <summary>Filter the query using context filters associated with specified keys.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="query">The query to filter using context filters associated with specified keys.</param>
        /// <returns>The query filtered using context filters associated with specified keys.</returns>
        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
        {
            object newQuery = query;

            foreach (var filter in Filters)
            {
                if (filter.Value.IsDefaultEnabled)
                {
                    newQuery = (IQueryable) filter.Value.ApplyFilter<T>(newQuery);
                }
            }

            return (IQueryable<T>) newQuery;
        }

        /// <summary>Filter the query using context filters associated with specified keys.</summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="query">The query to filter using context filters associated with specified keys.</param>
        /// <param name="keys">
        ///     A variable-length parameters list containing keys associated to context
        ///     filters to use to filter the query.
        /// </param>
        /// <returns>The query filtered using context filters associated with specified keys.</returns>
        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, object[] keys)
        {
            object newQuery = query;
            foreach (var key in keys)
            {
                var filter = GetFilter(key);

                if (filter != null)
                {
                    newQuery = ((IQueryable)filter.ApplyFilter<T>(newQuery));
                }
            }

            return (IQueryable<T>)newQuery;
        }

        /// <summary>Disable this filter on the specified types.</summary>
        /// <param name="filter">The filter to disable.</param>
        /// <param name="types">A variable-length parameters list containing types to disable the filter on.</param>
        /// m>
        public void DisableFilter(AliasBaseQueryFilter filter, params Type[] types)
        {
            List<AliasQueryFilterSet> filterSets;

            // CHECK if the element type can be used in the context
            if (FilterSetByType.TryGetValue(filter.ElementType, out filterSets))
            {
                if (types != null)
                {
                    var applySets = new List<AliasQueryFilterSet>();

                    foreach (var type in types)
                    {
                        List<AliasQueryFilterSet> setToAdd;
                        if (FilterSetByType.TryGetValue(type, out setToAdd))
                        {
                            applySets.AddRange(setToAdd);
                        }
                    }

                    // KEEP only applicable filter set
                    filterSets = filterSets.Intersect(applySets.Distinct()).ToList();
                }

                foreach (var set in filterSets)
                {
                    set.AddOrGetFilterQueryable(Context).DisableFilter(filter);
                }
            }
        }

        /// <summary>Enables this filter on the specified types.</summary>
        /// <param name="filter">The filter to enable.</param>
        /// <param name="types">A variable-length parameters list containing types to enable the filter on.</param>
        public void EnableFilter(AliasBaseQueryFilter filter, params Type[] types)
        {
            List<AliasQueryFilterSet> filterSets;

            // CHECK if the element type can be used in the context
            if (FilterSetByType.TryGetValue(filter.ElementType, out filterSets))
            {
                if (types != null)
                {
                    var applySets = new List<AliasQueryFilterSet>();

                    foreach (var type in types)
                    {
                        List<AliasQueryFilterSet> setToAdd;
                        if (FilterSetByType.TryGetValue(type, out setToAdd))
                        {
                            applySets.AddRange(setToAdd);
                        }
                    }

                    // KEEP only applicable filter set
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
        public AliasBaseQueryFilter GetFilter(object key)
        {
            AliasBaseQueryFilter filter;
            Filters.TryGetValue(key, out filter);
            return filter;
        }

        /// <summary>Load context information to the generic context.</summary>
        /// <param name="context">The context to use to load information to the generic context.</param>
        public void LoadGenericContextInfo(DbContext context)
        {
            FilterSetByType = new Dictionary<Type, List<AliasQueryFilterSet>>();
            FilterSets = new List<AliasQueryFilterSet>();

            var dbSetProperties = context.GetDbSetProperties();

            // ADD DbSet
            foreach (var dbSetProperty in dbSetProperties)
            {
                FilterSets.Add(new AliasQueryFilterSet(dbSetProperty));
            }

            // LINK DbSet to Type
            foreach (var filterDbSet in FilterSets)
            {
                var baseType = filterDbSet.ElementType;

                while (baseType != null && baseType != typeof(object))
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