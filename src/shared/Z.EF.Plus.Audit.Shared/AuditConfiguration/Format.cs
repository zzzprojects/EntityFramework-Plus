// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        /// <summary>
        ///     Formats value for selected properties from entities of 'T' type or entities which the type
        ///     derive from 'T'.
        /// </summary>
        /// <typeparam name="T">Generic type to format selected properties.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="formatter">The formatter to use to format value.</param>
        /// <returns>An AuditConfiguration.</returns>
        public AuditConfiguration Format<T>(Expression<Func<T, object>> propertySelector, Func<object, object> formatter) where T : class
        {
            var propertyNames = propertySelector.GetPropertyOrFieldAccessors().Select(x => x.ToString()).ToList();

            foreach (var accessor in propertyNames)
            {
                EntityValueFormatters.Add((x, s, v) => x is T && s == accessor ? formatter : null);
            }

            return this;
        }
    }
}