// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5
using System.Data;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditStateEntry
    {
        /// <summary>Audit relationship deleted.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditRelationDeleted(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(objectStateEntry)
            {
                State = AuditEntryState.RelationshipDeleted
            };

            var values = objectStateEntry.OriginalValues;
            for (var i = 0; i < values.FieldCount; i++)
            {
                var relationName = values.GetName(i);
                var value = (EntityKey) values.GetValue(i);
                foreach (var keyValue in value.EntityKeyValues)
                {
                    // todo: better add a new property association?
                    var keyName = string.Concat(relationName, ";", keyValue.Key);
                    entry.Properties.Add(new AuditEntryProperty(keyName, keyValue.Value, null));
                }
            }

            audit.Entries.Add(entry);
        }
    }
}