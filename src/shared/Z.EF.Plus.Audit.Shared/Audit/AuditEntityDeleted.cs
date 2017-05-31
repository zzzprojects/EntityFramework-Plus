// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Common;
using System.Linq;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Audit entity deleted.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditEntityDeleted(Audit audit, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditEntityDeleted(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
                audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, AuditEntryState.EntityDeleted)) :
                new AuditEntry();

            entry.Build(audit, objectStateEntry);
            entry.State = AuditEntryState.EntityDeleted;

            audit.Entries.Add(entry);

#if EF5 || EF6
            AuditEntityDeleted(entry, objectStateEntry, objectStateEntry.OriginalValues);
#elif EFCORE
            AuditEntityDeleted(entry, objectStateEntry);
#endif
        }

#if EF5 || EF6
        /// <summary>Audit entity deleted.</summary>
        /// <param name="entry">The entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityDeleted(AuditEntry entry, ObjectStateEntry objectStateEntry, DbDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                if (entry.Parent.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                {
                    value = null;
                }

                var valueRecord = value as DbDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityDeleted(entry, objectStateEntry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (objectStateEntry.EntityKey.EntityKeyValues.Any(x => x.Key == name) || entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, name))
                {
                    var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, string.Concat(prefix, name), value, null)) :
                        new AuditEntryProperty();

                    auditEntryProperty.Build(entry, string.Concat(prefix, name), value, null);
                    entry.Properties.Add(auditEntryProperty);
                }
            }
        }
#elif EFCORE
    /// <summary>Audit entity deleted.</summary>
    /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityDeleted(AuditEntry entry, EntityEntry objectStateEntry)
        {
            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (property.Metadata.IsKey() || entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
    entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, propertyEntry.Name, property.OriginalValue, null)) :
    new AuditEntryProperty();

                    auditEntryProperty.Build(entry, propertyEntry.Name, property.OriginalValue, null);
                    entry.Properties.Add(auditEntryProperty);
                }
            }
        }
#endif
    }
}