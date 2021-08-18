// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        public AuditConfiguration MetaProperty<T>(string propertyName, Func<T, object> oldValueFactory, Func<T, object> newValueFactory)
        {
            MetaProperties.Add(auditEntry =>
            {
                if (auditEntry.Entity != null && auditEntry.Entity is T)
                {
                    auditEntry.Properties.Add(new AuditEntryProperty
                    {
                        Parent = auditEntry,
                        PropertyName = propertyName,
                        OldValue = oldValueFactory?.Invoke((T) auditEntry.Entity),
                        NewValue = newValueFactory?.Invoke((T) auditEntry.Entity)
                    });
                }
            });

            return this;
        }
    }
}