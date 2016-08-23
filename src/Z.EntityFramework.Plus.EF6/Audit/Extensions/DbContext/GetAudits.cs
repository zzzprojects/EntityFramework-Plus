// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System;
using System.Data.Entity;
using System.Linq;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditExtensions
    {
        public static IQueryable<AuditEntry> GetAudits<T>(this DbContext context, T entry) where T : class
        {
            var auditSet = AuditManager.GetAuditSet(context);
            var keyNames = context.GetKeyNames<T>();

            if (entry == null)
            {
                return auditSet.Where(x => false);
            }

            var query = auditSet.Where(x => x.EntityTypeName == typeof (T).Name);

            foreach (var keyName in keyNames)
            {
                var property = entry.GetType().GetProperty(keyName);
                var value = property.GetValue(entry);

                query = query.Where(x => x.Properties.Any(y => y.PropertyName == property.Name && y.NewValueFormatted == value.ToString()));
            }

            query = query.Include(x => x.Properties).OrderBy(x => x.CreatedDate);

            return query;
        }

        public static IQueryable<AuditEntry> GetAudits<T>(this DbContext context, params object[] keyValues) where T : class
        {
            var auditSet = AuditManager.GetAuditSet(context);
            var keyNames = context.GetKeyNames<T>();

            var query = auditSet.Where(x => x.EntityTypeName == typeof (T).Name);

            if (keyValues == null)
            {
                throw new Exception(ExceptionMessage.Audit_Key_Null);
            }
            if (keyValues.Length != keyNames.Length)
            {
                throw new Exception(ExceptionMessage.Audit_Key_OutOfBound);
            }

            for (var i = 0; i < keyNames.Length; i++)
            {
                var propertyName = keyNames[i];
                var value = keyValues[i] != null ? keyValues[i].ToString() : "";

                query = query.Where(x => x.Properties.Any(y => y.PropertyName == propertyName && y.NewValueFormatted == value));
            }

            query = query.Include(x => x.Properties).OrderBy(x => x.CreatedDate);

            return query;
        }
    }
}