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
        /// <summary>Posts a save changes.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        public static void PostSaveChanges(Audit audit)
        {
            //foreach (var entry in audit.Entries)
            //{
            //    if (entry.DelayedKey != null)
            //    {
            //        var objectStateEntry = entry.DelayedKey as ObjectStateEntry;
            //        if (objectStateEntry != null)
            //        {
            //            if (objectStateEntry.IsRelationship)
            //            {
            //                var values = objectStateEntry.CurrentValues;
            //                var value_0 = (EntityKey) values.GetValue(0);
            //                var value_1 = (EntityKey) values.GetValue(1);
            //                var relationName_0 = values.GetName(0);
            //                var relationName_1 = values.GetName(1);

            //                foreach (var keyValue in value_0.EntityKeyValues)
            //                {
            //                    var keyName = string.Concat(relationName_0, ";", keyValue.Key);
            //                    entry.Properties.Add(new AuditEntryProperty(keyName, null, keyValue.Value));
            //                }

            //                foreach (var keyValue in value_1.EntityKeyValues)
            //                {
            //                    var keyName = string.Concat(relationName_1, ";", keyValue.Key);
            //                    entry.Properties.Add(new AuditEntryProperty(keyName, null, keyValue.Value));
            //                }
            //            }
            //            else
            //            {
            //                foreach (var keyValue in objectStateEntry.EntityKey.EntityKeyValues)
            //                {
            //                    var property = entry.Properties.First(x => x.PropertyName == keyValue.Key);
            //                    property.NewValue = keyValue.Value;
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}