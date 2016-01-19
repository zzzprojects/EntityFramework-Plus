// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.


#if STANDALONE
using System.Collections.Generic;
using System.Reflection;
#if EF5
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class QueryFilterExtensions
    {
        /// <summary>A DbContext extension method that gets DbSet properties.</summary>
        /// <param name="context">The context to get DbSet properties from.</param>
        /// <returns>DbSet properties from the context.</returns>
        internal static List<PropertyInfo> GetDbSetProperties(this DbContext context)
        {
            var dbSetProperties = new List<PropertyInfo>();
            var properties = context.GetType().GetProperties();

            foreach (var property in properties)
            {
                var setType = property.PropertyType;

#if EF5 || EF6
                var isDbSet = setType.IsGenericType && (typeof (IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof (IDbSet<>).FullName) != null);
#elif EF7
                var isDbSet = setType.IsGenericType && (typeof (DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));
#endif

                if (isDbSet)
                {
                    dbSetProperties.Add(property);
                }
            }

            return dbSetProperties;
        }
    }
}
#endif