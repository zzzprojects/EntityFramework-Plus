// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5
using System;
using System.Data.Objects;
using System.Linq;

#elif EF6
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Audit entity added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditEntityAdded(Audit audit, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditEntityAdded(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
                audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, AuditEntryState.EntityAdded)) :
                new AuditEntry();

            entry.Build(audit, objectStateEntry);
            entry.State = AuditEntryState.EntityAdded;

            audit.Entries.Add(entry);
        }

#if EF5 || EF6
        /// <summary>Audit entity added.</summary>
        /// <param name="auditEntry">The audit entry.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityAdded(AuditEntry auditEntry, ObjectStateEntry objectStateEntry, DbUpdatableDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                if (auditEntry.Parent.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                {
                    value = null;
                }

                var valueRecord = value as DbUpdatableDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityAdded(auditEntry, objectStateEntry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (objectStateEntry.EntitySet.ElementType.KeyMembers.Any(x => x.Name == name) || auditEntry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(auditEntry.Entry, name))
                {
                    var auditEntryProperty = auditEntry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        auditEntry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(auditEntry, objectStateEntry, string.Concat(prefix, name), null, value)) :
                        new AuditEntryProperty();

                    auditEntryProperty.Build(auditEntry, string.Concat(prefix, name), null, value);
                    auditEntry.Properties.Add(auditEntryProperty);
                }
            }
        }
#elif EFCORE
    /// <summary>Audit entity added.</summary>
    /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityAdded(AuditEntry entry, EntityEntry objectStateEntry)
        {
            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (property.Metadata.IsKey() || entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, propertyEntry.Name, null, property.CurrentValue)) :
                        new AuditEntryProperty();

                    auditEntryProperty.Build(entry, propertyEntry.Name, null, property.CurrentValue);
                    entry.Properties.Add(auditEntryProperty);
                }
            }
        }
#endif
    }
}