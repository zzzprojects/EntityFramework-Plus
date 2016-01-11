// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

using System.Data.Common;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditStateEntry
    {
        /// <summary>Audit entity deleted.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityDeleted(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(objectStateEntry)
            {
                State = AuditEntryState.EntityDeleted
            };

            AuditEntityDeleted(entry, objectStateEntry.OriginalValues);
            audit.Entries.Add(entry);
        }

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
                else
                {
                    entry.Properties.Add(new AuditEntryProperty(string.Concat(prefix, name), value, null));
                }
            }
        }
    }
}