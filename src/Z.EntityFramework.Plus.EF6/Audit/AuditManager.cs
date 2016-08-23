// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for audits.</summary>
    public static class AuditManager
    {
        /// <summary>Static constructor.</summary>
        static AuditManager()
        {
            AuditSet = new ConcurrentDictionary<Type, PropertyInfo>();
            DefaultConfiguration = new AuditConfiguration();
        }

        /// <summary>Gets or sets the default audit configuration.</summary>
        /// <value>The default audit configuration.</value>
        public static AuditConfiguration DefaultConfiguration { get; set; }

        public static ConcurrentDictionary<Type, PropertyInfo> AuditSet { get; set; }

        public static DbSet<AuditEntry> GetAuditSet(DbContext context)
        {
            var auditSetProperty = AuditSet.GetOrAdd(context.GetType(), type => GetAuditSetProperty(context));

            return (DbSet<AuditEntry>) auditSetProperty.GetValue(context);
        }

        public static PropertyInfo GetAuditSetProperty(this DbContext context)
        {
            var sets = context.GetDbSetProperties();

            var candidates = new List<PropertyInfo>();

            foreach (var set in sets)
            {
                var element = set.PropertyType.GetDbSetElementType();

                if (element == typeof (AuditEntry))
                {
                    candidates.Add(set);
                }
            }

            PropertyInfo property;

            if (candidates.Count == 0)
            {
                throw new Exception(ExceptionMessage.Audit_DbSet_NotFound);
            }

            if (candidates.Count == 1)
            {
                property = candidates[0];
            }
            else
            {
                property = candidates.FirstOrDefault(x => x.Name == "AuditEntries");

                if (property == null)
                {
                    throw new Exception(ExceptionMessage.Audit_DbSet_NotFound);
                }
            }

            return property;
        }
    }
}