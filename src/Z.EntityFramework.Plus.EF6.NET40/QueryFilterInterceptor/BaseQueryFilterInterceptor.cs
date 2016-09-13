// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;

namespace Z.EntityFramework.Plus
{
    /// <summary>A base class for query filter.</summary>
    public abstract class BaseQueryFilterInterceptor
    {
        /// <summary>The unique key.</summary>
        public readonly string UniqueKey = Guid.NewGuid().ToString();

        /// <summary>List of is filter enabled.</summary>
        public List<Func<Type, bool?>> IsFilterEnabledList = new List<Func<Type, bool?>>();

        /// <summary>Gets or sets the context that owns this item.</summary>
        /// <value>The owner context.</value>
        public QueryFilterContextInterceptor OwnerContext { get; set; }

        /// <summary>Gets or sets a value indicating whether the filter is enabled by default.</summary>
        /// <value>true if the filter is enabled by default, false if not.</value>
        public bool IsDefaultEnabled { get; set; }

        /// <summary>Gets or sets the type of the element.</summary>
        /// <value>The type of the element.</value>
        public Type ElementType { get; set; }

        /// <summary>Gets database expression.</summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        /// <returns>The database expression.</returns>
        public abstract DbExpression GetDbExpression(DbContext context, Type type);

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
            if (types == null || types.Length == 0)
            {
                if (IsFilterEnabledList.Count == 0)
                {
                    IsDefaultEnabled = false;
                }
                else
                {
                    IsFilterEnabledList.Add(type1 => false);
                }
            }
            else
            {
                foreach (var type in types)
                {
                    IsFilterEnabledList.Add(type1 => type.IsAssignableFrom(type1) ? false : (bool?)null);
                }
            }

            if (OwnerContext != null)
            {
                OwnerContext.ClearCache();
            }
            else
            {
                QueryFilterManager.ClearAllCache();
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
            if (types == null || types.Length == 0)
            {
                if (IsFilterEnabledList.Count == 0)
                {
                    IsDefaultEnabled = true;
                }
                else
                {
                    IsFilterEnabledList.Add(type1 => true);
                }
            }
            else
            {
                foreach (var type in types)
                {
                    IsFilterEnabledList.Add(type1 => type.IsAssignableFrom(type1) ? true : (bool?)null);
                }
            }

            if (OwnerContext != null)
            {
                OwnerContext.ClearCache();
            }
            else
            {
                QueryFilterManager.ClearAllCache();
            }
        }

        /// <summary>Queries if a type is enabled.</summary>
        /// <param name="type">The type.</param>
        /// <returns>true if a type is enabled, false if not.</returns>
        public bool IsTypeEnabled(Type type)
        {
            var isEnabled = IsDefaultEnabled;

            foreach (var isFiltedEnabled in IsFilterEnabledList)
            {
                var isEnabledFactory = isFiltedEnabled(type);

                if (isEnabledFactory.HasValue)
                {
                    isEnabled = isEnabledFactory.Value;
                }
            }

            return isEnabled;
        }
    }
}