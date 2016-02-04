// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2016 ZZZ Projects. All rights reserved.

#if EF5 || EF6

#if EF5
using System.Data;
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

#elif EF7
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
#elif EF7
        public static void AuditRelationAdded(Audit audit, EntityEntry objectStateEntry)
#endif
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.RelationshipAdded
            };

            var values = objectStateEntry.CurrentValues;


            var leftKeys = (EntityKey) values.GetValue(0);
            var rightKeys = (EntityKey) values.GetValue(1);

            if (leftKeys.IsTemporary || rightKeys.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }
            else
            {
                var leftRelationName = values.GetName(0);
                var rightRelationName = values.GetName(1);

                foreach (var keyValue in leftKeys.EntityKeyValues)
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, leftRelationName, keyValue.Key, null, keyValue.Value));
                }

                foreach (var keyValue in rightKeys.EntityKeyValues)
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, rightRelationName, keyValue.Key, null, keyValue.Value));
                }
            }

            audit.Entries.Add(entry);
        }
    }
}

#endif