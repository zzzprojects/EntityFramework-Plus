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
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Audit entity modified.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditEntityModified(Audit audit, ObjectStateEntry objectStateEntry, AuditEntryState state)
#elif EF7
        public static void AuditEntityModified(Audit audit, EntityEntry objectStateEntry, AuditEntryState state)
#endif
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = state
            };

#if EF5 || EF6
            AuditEntityModified(audit, entry, objectStateEntry, objectStateEntry.OriginalValues, objectStateEntry.CurrentValues);
#elif EF7
            AuditEntityModified(audit, entry, objectStateEntry);
#endif
            audit.Entries.Add(entry);
        }

#if EF5 || EF6
    /// <summary>Audit entity modified.</summary>
    /// <param name="audit">The audit to use to add changes made to the context.</param>
    /// <param name="entry">The entry.</param>
    /// <param name="objectStateEntry">The object state entry.</param>
    /// <param name="orginalRecord">The orginal record.</param>
    /// <param name="currentRecord">The current record.</param>
    /// <param name="prefix">The prefix.</param>
        public static void AuditEntityModified(Audit audit, AuditEntry entry, ObjectStateEntry objectStateEntry, DbDataRecord orginalRecord, DbUpdatableDataRecord currentRecord, string prefix = "")
        {
            for (var i = 0; i < orginalRecord.FieldCount; i++)
            {
                var name = orginalRecord.GetName(i);
                var originalValue = orginalRecord.GetValue(i);
                var currentValue = currentRecord.GetValue(i);

                var valueRecord = originalValue as DbDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityModified(audit, entry, objectStateEntry, valueRecord, currentValue as DbUpdatableDataRecord, string.Concat(prefix, name, "."));
                }

                else if (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, name))
                {
                    if (!audit.Configuration.IgnorePropertyUnchanged
                        || objectStateEntry.EntityKey.EntityKeyValues.Any(x => x.Key == name)
                        || !Equals(currentValue, originalValue))
                    {
                        entry.Properties.Add(new AuditEntryProperty(entry, string.Concat(prefix, name), originalValue, currentValue));
                    }
                }
            }
        }
#elif EF7
        /// <summary>Audit entity modified.</summary>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityModified(Audit audit, AuditEntry entry, EntityEntry objectStateEntry)
        {
            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    if (audit.Configuration.IgnorePropertyUnchanged || property.Metadata.IsKey() || property.IsModified)
                    {
                        entry.Properties.Add(new AuditEntryProperty(entry, propertyEntry.Name, property.OriginalValue, property.CurrentValue));
                    }
                }
            }
        }
#endif
    }
}