// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EF7
using Microsoft.Data.Entity.ChangeTracking;

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
#elif EF7
        public static void AuditEntityAdded(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.EntityAdded
            };


            // CHECK if the key should be resolved in POST Action
#if EF5 || EF6
            if (objectStateEntry.EntityKey.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }
            AuditEntityAdded(entry, objectStateEntry.CurrentValues);
#elif EF7
    // TODO: We must check if the key IsTemporary! We can maybe use flag...
    //if (!objectStateEntry.IsKeySet)
    //{
                entry.DelayedKey = objectStateEntry;
            //}
            AuditEntityAdded(entry, objectStateEntry);
#endif

            audit.Entries.Add(entry);
        }

#if EF5 || EF6
        /// <summary>Audit entity added.</summary>
        /// <param name="auditEntry">The audit entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityAdded(AuditEntry auditEntry, DbUpdatableDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                var valueRecord = value as DbUpdatableDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityAdded(auditEntry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (auditEntry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(auditEntry.Entry, name))
                {
                    auditEntry.Properties.Add(new AuditEntryProperty(auditEntry, string.Concat(prefix, name), null, value));
                }
            }
        }
#elif EF7
    /// <summary>Audit entity added.</summary>
    /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityAdded(AuditEntry entry, EntityEntry objectStateEntry)
        {
            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, propertyEntry.Name, null, property.CurrentValue));
                }
            }
        }
#endif
    }
}