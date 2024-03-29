﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Common;
using System.Linq;
#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
#elif EFCORE
        public static void AuditEntityModified(Audit audit, EntityEntry objectStateEntry, AuditEntryState state, DbContext context)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, state)) :
new AuditEntry();

#if EF5 || EF6
            entry.Build(audit, objectStateEntry);
#elif EFCORE
            entry.Build(audit, objectStateEntry, context);
#endif
            entry.State = state;

#if EF5 || EF6
            AuditEntityModified(audit, entry, objectStateEntry, objectStateEntry.OriginalValues, objectStateEntry.CurrentValues);
#elif EFCORE
            AuditEntityModified(audit, entry, objectStateEntry);
#endif
            if (audit.Configuration.IgnoreEntityUnchanged)
            {
                if (entry.Properties != null && entry.Properties.Count > 0)
                { 
                    audit.Entries.Add(entry);
                }
            }
            else
            {
                audit.Entries.Add(entry);
            } 
        }

#if EF5 || EF6
        /// <summary>Audit entity modified.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        /// <param name="orginalRecord">The original record.</param>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityModified(Audit audit, AuditEntry entry, ObjectStateEntry objectStateEntry, DbDataRecord orginalRecord, DbUpdatableDataRecord currentRecord, string prefix = "")
        {
            bool hasModified = false;

            for (var i = 0; i < orginalRecord.FieldCount; i++)
            {
                var name = orginalRecord.GetName(i);
                var originalValue = orginalRecord.GetValue(i);
                var currentValue = currentRecord.GetValue(i);

                if (audit.Configuration.UseNullForDBNullValue && originalValue == DBNull.Value)
                {
                    originalValue = null;
                }

                if (audit.Configuration.UseNullForDBNullValue && currentValue == DBNull.Value)
                {
                    currentValue = null;
                }

                var valueRecord = originalValue as DbDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityModified(audit, entry, objectStateEntry, valueRecord, currentValue as DbUpdatableDataRecord, string.Concat(prefix, name, "."));
                }

				else if (objectStateEntry.EntityKey.EntityKeyValues.Any(x => x.Key == name)
                         || entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, string.Concat(prefix, name)))
                {
                    var isKey = objectStateEntry.EntityKey.EntityKeyValues.Any(x => x.Key == name);

                    if (!audit.Configuration.IgnorePropertyUnchanged
                        || isKey
                        || !Equals(currentValue, originalValue))
                    {
                        var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, string.Concat(prefix, name), originalValue, currentValue)) :
new AuditEntryProperty();
                        auditEntryProperty.IsKey = isKey;

                        auditEntryProperty.Build(entry, string.Concat(prefix, name), originalValue, currentRecord, i);
                        entry.Properties.Add(auditEntryProperty);

                        if (!isKey)
                        {
                            hasModified = true;
                        }
                    }
                }
            }

            if (audit.Configuration.IgnoreEntityUnchanged)
            {
                if (!hasModified)
                {
                    entry.Properties.Clear();
                }
            }
        }
#elif EFCORE
    /// <summary>Audit entity modified.</summary>
    /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityModified(Audit audit, AuditEntry entry, EntityEntry objectStateEntry)
        {
            bool hasModified = false;

            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (property.Metadata.IsKey() || entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name))
                {
                    if (!audit.Configuration.IgnorePropertyUnchanged || property.Metadata.IsKey() || !Equals(property.CurrentValue, property.OriginalValue))
                    {
                        var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                            entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, propertyEntry.Name, property.OriginalValue, property.CurrentValue)) :
                            new AuditEntryProperty();

                        auditEntryProperty.Build(entry, propertyEntry.Name, property.OriginalValue, property);
                        auditEntryProperty.IsKey = property.Metadata.IsKey();
                        entry.Properties.Add(auditEntryProperty);

                        if (!property.Metadata.IsKey())
                        {
                            hasModified = true;
                        }
                    }
                }
            } 

            if (audit.Configuration.IgnoreEntityUnchanged)
            { 
                if (!hasModified)
                {
                    entry.Properties.Clear(); 
                }
            }
        }
#endif
    }
}