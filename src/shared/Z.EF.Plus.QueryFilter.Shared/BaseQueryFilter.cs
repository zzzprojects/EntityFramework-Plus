// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

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
    /// <summary>A base class for query filter.</summary>
#if EF6
    public abstract class BaseQueryDbSetFilter
#else
    public abstract class BaseQueryFilter
#endif
    {
        /// <summary>Gets or sets the type of the filter element.</summary>
        /// <value>The type of the filter element.</value>
        public Type ElementType { get; set; }

        /// <summary>Gets or sets a value indicating whether the filter is enabled by default.</summary>
        /// <value>true if the filter is enabled by default, false if not.</value>
        public bool IsDefaultEnabled { get; set; }

        /// <summary>Gets or sets the filter context that owns this filter.</summary>
        /// <value>The filter context that owns this filter.</value>
        public AliasQueryFilterContext OwnerFilterContext { get; set; }

        /// <summary>Apply the filter on the query and return the new filtered query.</summary>
        /// <param name="query">The query to filter.</param>
        /// <returns>The new query filered query.</returns>
        public virtual object ApplyFilter<TEntity>(object query)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Disables this filter.</summary>
        public void Disable()
        {
            Disable(null);
        }

        /// <summary>Disables this filter on the speficied type.</summary>
        /// <typeparam name="TType">Type of the element to disable the filter on.</typeparam>
        public void Disable<TType>()
        {
            Disable(typeof(TType));
        }

        /// <summary>Disable this filter on the specified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to disable the filter on.</param>
        public void Disable(params Type[] types)
        {
            if (OwnerFilterContext == null)
            {
                AliasQueryFilterManager.GlobalInitializeFilterActions.Add(new Tuple<AliasBaseQueryFilter, Action<AliasBaseQueryFilter>>(this, filter => filter.Disable(types)));
            }
            else
            {
                OwnerFilterContext.DisableFilter(this, types);
            }
        }

        /// <summary>Enables this filter.</summary>
        public void Enable()
        {
            Enable(null);
        }

        /// <summary>Enables this filter on the speficied type.</summary>
        /// <typeparam name="TType">Type of the element to enable the filter on.</typeparam>
        public void Enable<TType>()
        {
            Enable(typeof(TType));
        }

        /// <summary>Enables this filter on the specified types.</summary>
        /// <param name="types">A variable-length parameters list containing types to enable the filter on.</param>
        public void Enable(params Type[] types)
        {
            if (OwnerFilterContext == null)
            {
                AliasQueryFilterManager.GlobalInitializeFilterActions.Add(new Tuple<AliasBaseQueryFilter, Action<AliasBaseQueryFilter>>(this, filter => filter.Enable(types)));
            }
            else
            {
                OwnerFilterContext.EnableFilter(this, types);
            }
        }

        /// <summary>Gets the filter.</summary>
        /// <returns>The filter.</returns>
        public virtual object GetFilter()
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }

        /// <summary>Makes a deep copy of this filter.</summary>
        /// <param name="filterContext">The filter context that owns the filter copy.</param>
        /// <returns>A copy of this filter.</returns>
        public virtual AliasBaseQueryFilter Clone(AliasQueryFilterContext filterContext)
        {
            throw new Exception(ExceptionMessage.GeneralException);
        }
    }
}