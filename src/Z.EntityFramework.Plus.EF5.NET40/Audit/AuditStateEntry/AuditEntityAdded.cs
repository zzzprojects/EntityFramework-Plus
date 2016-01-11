// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditStateEntry
    {
        /// <summary>Audit entity added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityAdded(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(objectStateEntry)
            {
                State = AuditEntryState.EntityAdded
            };

            // CHECK if the key should be resolved in POST Action
            if (objectStateEntry.EntityKey.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }

            AuditEntityAdded(entry, objectStateEntry.CurrentValues);
            audit.Entries.Add(entry);
        }

        public static void AuditEntityAdded(AuditEntry entry, DbUpdatableDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                var valueRecord = value as DbUpdatableDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityAdded(entry, valueRecord, string.Concat(prefix, name, "."));
                }
                else
                {
                    entry.Properties.Add(new AuditEntryProperty(string.Concat(prefix, name), null, value));
                }
            }
        }
    }
}