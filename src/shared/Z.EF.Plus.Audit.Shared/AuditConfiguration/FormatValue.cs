﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

#if EF5
using System.Data;
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        /// <summary>Format a value for the specified entry and property name.</summary>
        /// <param name="entry">The entry.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The formatted value.</returns>
#if EF5 || EF6
        public string FormatValue(object entity, string propertyName, object currentValue)
#elif EFCORE
        public string FormatValue(object entity, string propertyName, object currentValue)
#endif
        {
            if (entity != null && EntityValueFormatters.Count > 0)
            {
                var type = entity.GetType();
                // Null value doesn't work with scenario 2. So we add it to the key for this scenario.
                // Only scenario 2 use the currentValue
                // 1. EntityValueFormatters.Add((x, s, v) => x is T && s == accessor ? formatter : null);
                // 2. EntityValueFormatters.Add((x, s, v) => v != null && v.GetType() == typeof(T) ? func : null);
                var key = string.Concat(type.FullName, ";", propertyName, ";", currentValue == null ? "NULL" : currentValue.GetType().FullName);
                Func<object, object> formatter;

                if (!ValueFormatterDictionary.TryGetValue(key, out formatter))
                {
                    if (EntityValueFormatters.Count > 0)
                    { 
                        foreach (var formatProperty in EntityValueFormatters)
                        {
                            formatter = formatProperty(entity, propertyName, currentValue);

                            if (formatter != null)
                            {
                                break;
                            }
                        }
                    }

                    ValueFormatterDictionary.TryAdd(key, formatter);
                }

                if (formatter != null)
                {
                    currentValue = formatter(currentValue);
                }
            }

            return currentValue != null && currentValue != DBNull.Value ? currentValue.ToString() : null;
        }
    }
}