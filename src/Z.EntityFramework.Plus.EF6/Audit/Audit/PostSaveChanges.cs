// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


using System;

#if EF5
using System.Data;
using System.Data.Objects;

#elif EF6

#elif EFCORE
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Updates audit entries after the save changes has been executed.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        public static void PostSaveChanges(Audit audit)
        {
            foreach (var entry in audit.Entries)
            {
                switch (entry.State)
                {
                    case AuditEntryState.EntityAdded:
#if EF5 || EF6
                        AuditEntityAdded(entry, entry.Entry, entry.Entry.CurrentValues);
#elif EFCORE
                        AuditEntityAdded(entry, entry.Entry);
#endif
                        break;
                    case AuditEntryState.EntityDeleted:
                    case AuditEntryState.RelationshipDeleted:
                        break;
                    case AuditEntryState.EntityModified:
                    case AuditEntryState.EntitySoftAdded:
                    case AuditEntryState.EntitySoftDeleted:
#if EF5 || EF6
                        foreach (var property in entry.Properties)
                        {
                            if (!property.IsValueSet)
                            {
                                var currentValue = property.DbUpdatableDataRecord.GetValue(property.DbUpdatableDataRecordPosition);

                                if (audit.Configuration.UseNullForDBNullValue && currentValue == DBNull.Value)
                                {
                                    currentValue = null;
                                }
                                property.NewValue = currentValue;
                            }
                        }
#elif EFCORE
                        foreach (var property in entry.Properties)
                        {
                            if (!property.IsValueSet)
                            {
                                var currentValue = property.PropertyEntry.CurrentValue;
                                property.NewValue = currentValue;
                            }
                        }
#endif
                        break;
                    case AuditEntryState.RelationshipAdded:
#if EF5 || EF6
                        AuditRelationAdded(audit, entry, entry.Entry);
#endif
                        break;
                }
            }
            //            foreach (var entry in audit.Entries)
            //            {
            //                if (entry.DelayedKey != null)
            //                {
            //#if EF5 || EF6
            //                    var objectStateEntry = entry.DelayedKey as ObjectStateEntry;
            //#elif EFCORE
            //                    var objectStateEntry = entry.DelayedKey as EntityEntry;
            //#endif
            //                    if (objectStateEntry != null)
            //                    {
            //#if EF5 || EF6
            //                        if (objectStateEntry.IsRelationship)
            //                        {
            //                            var values = objectStateEntry.CurrentValues;
            //                            var leftKeys = (EntityKey) values.GetValue(0);
            //                            var rightKeys = (EntityKey) values.GetValue(1);
            //                            var leftRelationName = values.GetName(0);
            //                            var rightRelationName = values.GetName(1);

            //                            foreach (var keyValue in leftKeys.EntityKeyValues)
            //                            {
            //                                var auditEntryProperty = audit.Configuration.AuditEntryPropertyFactory != null ?
            //                                    audit.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, leftRelationName, keyValue.Key, null, keyValue.Value)) :
            //                                    new AuditEntryProperty();

            //                                auditEntryProperty.Build(entry, leftRelationName, keyValue.Key, null, keyValue.Value);
            //                                entry.Properties.Add(auditEntryProperty);
            //                            }

            //                            foreach (var keyValue in rightKeys.EntityKeyValues)
            //                            {
            //                                var auditEntryProperty = audit.Configuration.AuditEntryPropertyFactory != null ?
            //                                    audit.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, rightRelationName, keyValue.Key, null, keyValue.Value)) :
            //                                    new AuditEntryProperty();

            //                                auditEntryProperty.Build(entry, rightRelationName, keyValue.Key, null, keyValue.Value);
            //                                entry.Properties.Add(auditEntryProperty);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            foreach (var keyValue in objectStateEntry.EntityKey.EntityKeyValues)
            //                            {
            //                                var property = entry.Properties.FirstOrDefault(x => x.InternalPropertyName == keyValue.Key);

            //                                // ENSURE the property is audited
            //                                if (property != null)
            //                                {
            //                                    property.NewValue = keyValue.Value;
            //                                }
            //                            }
            //                        }
            //                    }
            //#elif EFCORE
            //                        foreach (var keyValue in objectStateEntry.Metadata.GetKeys())
            //                        {
            //                            var key = objectStateEntry.Property(keyValue.Properties[0].Name);
            //                            var property = entry.Properties.FirstOrDefault(x => x.InternalPropertyName == keyValue.Properties[0].Name);

            //                            // ENSURE the property is audited
            //                            if (property != null)
            //                            {
            //                                property.NewValue = key.CurrentValue;
            //                            }
            //                        }
            //                    }
            //#endif
            //                }
            //}
        }
    }
}