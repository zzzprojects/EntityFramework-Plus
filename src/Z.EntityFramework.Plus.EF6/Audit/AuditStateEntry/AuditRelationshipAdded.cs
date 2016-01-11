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
        /// <summary>Audit relationship added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditRelationAdded(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(objectStateEntry)
            {
                State = AuditEntryState.RelationshipAdded
            };

            var values = objectStateEntry.CurrentValues;


            var value_0 = (EntityKey) values.GetValue(0);
            var value_1 = (EntityKey) values.GetValue(1);

            if (value_0.IsTemporary || value_1.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }
            else
            {
                var relationName_0 = values.GetName(0);
                var relationName_1 = values.GetName(1);

                foreach (var keyValue in value_0.EntityKeyValues)
                {
                    var keyName = string.Concat(relationName_0, ";", keyValue.Key);
                    entry.Properties.Add(new AuditEntryProperty(keyName, null, keyValue.Value));
                }

                foreach (var keyValue in value_1.EntityKeyValues)
                {
                    var keyName = string.Concat(relationName_1, ";", keyValue.Key);
                    entry.Properties.Add(new AuditEntryProperty(keyName, null, keyValue.Value));
                }
            }

            audit.Entries.Add(entry);
        }
    }
}