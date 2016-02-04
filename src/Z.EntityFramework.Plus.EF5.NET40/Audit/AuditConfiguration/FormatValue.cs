// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

using System;
#if EF5
using System.Data;
using System.Data.Entity;
using System.Data.Objects;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

#elif EF7
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

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
        public string FormatValue(ObjectStateEntry entry, string propertyName, object currentValue)
#elif EF7
        public string FormatValue(EntityEntry entry, string propertyName, object currentValue)
#endif
        {
            if (EntityValueFormatters.Count > 0)
            {
                if (entry.State != EntityState.Detached && entry.Entity != null)
                {
                    var type = entry.Entity.GetType();
                    var key = string.Concat(type.FullName, ";", propertyName);
                    Func<object, object> formatter;

                    if (!ValueFormatterDictionary.TryGetValue(key, out formatter))
                    {
                        if (EntityValueFormatters.Count > 0)
                        {
                            foreach (var formatProperty in EntityValueFormatters)
                            {
                                formatter = formatProperty(entry.Entity, propertyName);

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
            }

            return currentValue != null ? currentValue.ToString() : null;
        }
    }
}