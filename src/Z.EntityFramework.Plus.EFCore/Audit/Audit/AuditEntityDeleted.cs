// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Common;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
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
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.EntityDeleted
            };


#if EF5 || EF6
            AuditEntityDeleted(entry, objectStateEntry.OriginalValues);
#elif EFCORE
            AuditEntityDeleted(entry, objectStateEntry);
#endif
            audit.Entries.Add(entry);
        }

#if EF5 || EF6
        /// <summary>Audit entity deleted.</summary>
        /// <param name="entry">The entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityDeleted(AuditEntry entry, DbDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                var valueRecord = value as DbDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityDeleted(entry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, name))
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, string.Concat(prefix, name), value, null));
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

                if (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, propertyEntry.Name, property.OriginalValue, null));
                }
            }
        }
#endif
    }
}