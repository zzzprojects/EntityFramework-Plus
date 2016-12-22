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
        /// <summary>Audit relationship added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditRelationAdded(Audit audit, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditRelationAdded(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
                audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, AuditEntryState.RelationshipAdded)) :
                new AuditEntry();

            entry.Build(audit, objectStateEntry);
            entry.State = AuditEntryState.RelationshipAdded;

            audit.Entries.Add(entry);
        }

#if EF5 || EF6
        public static void AuditRelationAdded(Audit audit, AuditEntry entry, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditRelationAdded(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var values = objectStateEntry.CurrentValues;

            var leftKeys = (EntityKey) values.GetValue(0);
            var rightKeys = (EntityKey) values.GetValue(1);

            var leftRelationName = values.GetName(0);
            var rightRelationName = values.GetName(1);

            foreach (var keyValue in leftKeys.EntityKeyValues)
            {
                var value = keyValue.Value;

                if (audit.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                {
                    value = null;
                }

                var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                    entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, leftRelationName, keyValue.Key, null, value)) :
                    new AuditEntryProperty();

                auditEntryProperty.Build(entry, leftRelationName, keyValue.Key, null, value);
                entry.Properties.Add(auditEntryProperty);
            }

            foreach (var keyValue in rightKeys.EntityKeyValues)
            {
                var value = keyValue.Value;

                if (audit.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                {
                    value = null;
                }

                var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                    entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, rightRelationName, keyValue.Key, null, value)) :
                    new AuditEntryProperty();

                auditEntryProperty.Build(entry, rightRelationName, keyValue.Key, null, value);
                entry.Properties.Add(auditEntryProperty);
            }
        }
    }
}

#endif