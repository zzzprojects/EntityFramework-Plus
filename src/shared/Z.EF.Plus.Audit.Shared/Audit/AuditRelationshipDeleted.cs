// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
#if EF5
using System;
using System.Data;
using System.Data.Objects;

#elif EF6
using System;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.Data.Entity.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Audit relationship deleted.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditRelationDeleted(Audit audit, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditRelationDeleted(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
                audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, AuditEntryState.RelationshipDeleted)) :
                new AuditEntry();

            entry.Build(audit, objectStateEntry);
            entry.State = AuditEntryState.RelationshipDeleted;

            audit.Entries.Add(entry);

            AuditRelationDeleted(audit, entry, objectStateEntry);
        }

#if EF5 || EF6
        public static void AuditRelationDeleted(Audit audit, AuditEntry entry, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditRelationDeleted(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var values = objectStateEntry.OriginalValues;
            for (var i = 0; i < values.FieldCount; i++)
            {
                var relationName = values.GetName(i);
                var entityKey = (EntityKey)values.GetValue(i);
                foreach (var keyValue in entityKey.EntityKeyValues)
                {
                    var value = keyValue.Value;

                    if (audit.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                    {
                        value = null;
                    }

                    var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, relationName, keyValue.Key, value, null)) :
                        new AuditEntryProperty();

                    auditEntryProperty.Build(entry, relationName, keyValue.Key, value, null);
                    entry.Properties.Add(auditEntryProperty);
                }
            }
        }
    }
}

#endif