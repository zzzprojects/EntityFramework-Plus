// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5
using System;
using System.Data.Objects;
using System.Linq;

#elif EF6
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

#elif EFCORE
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Audit entity added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
#if EF5 || EF6
        public static void AuditEntityAdded(Audit audit, ObjectStateEntry objectStateEntry)
#elif EFCORE
        public static void AuditEntityAdded(Audit audit, EntityEntry objectStateEntry, DbContext context)
#endif
        {
            var entry = audit.Configuration.AuditEntryFactory != null ?
                audit.Configuration.AuditEntryFactory(new AuditEntryFactoryArgs(audit, objectStateEntry, AuditEntryState.EntityAdded)) :
                new AuditEntry();
#if EF5 || EF6
            entry.Build(audit, objectStateEntry);
#elif EFCORE
            entry.Build(audit, objectStateEntry, context);
#endif
            entry.State = AuditEntryState.EntityAdded;

            audit.Entries.Add(entry);
        }

#if EF5 || EF6
        /// <summary>Audit entity added.</summary>
        /// <param name="auditEntry">The audit entry.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityAdded(AuditEntry auditEntry, ObjectStateEntry objectStateEntry, DbUpdatableDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);
	            var type = record.GetFieldType(i);

				if (auditEntry.Parent.Configuration.UseNullForDBNullValue && value == DBNull.Value)
                {
                    value = null;
                }

                var valueRecord = value as DbUpdatableDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityAdded(auditEntry, objectStateEntry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (objectStateEntry.EntitySet.ElementType.KeyMembers.Any(x => x.Name == name) ||
                         (auditEntry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(auditEntry.Entry, string.Concat(prefix, name))
                         && !auditEntry.Parent.CurrentOrDefaultConfiguration.IgnorePropertyAdded))
                { 
					var auditEntryProperty = auditEntry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        auditEntry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(auditEntry, objectStateEntry, string.Concat(prefix, name), null, value)) :
                        new AuditEntryProperty();
					auditEntryProperty.IsKey = objectStateEntry.EntitySet.ElementType.KeyMembers.Any(x => x.Name == name);

                    if (auditEntry.Parent.CurrentOrDefaultConfiguration.IgnoreEntityAddedDefaultValue && type != typeof(object) && value != null)
	                {
		                var checkDefaultValue = DefaultValue(type);
						if (checkDefaultValue != null && checkDefaultValue.Equals(value))
		                {
			                value = null;
		                } 
					}

					auditEntryProperty.Build(auditEntry, string.Concat(prefix, name), null, value);
                    auditEntry.Properties.Add(auditEntryProperty);
                }
            }
        }



#elif EFCORE
        /// <summary>Audit entity added.</summary>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityAdded(AuditEntry entry, EntityEntry objectStateEntry)
        {
            foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
            {
                var property = objectStateEntry.Property(propertyEntry.Name);

                if (property.Metadata.IsKey() || (entry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(entry.Entry, propertyEntry.Name)
					&& !entry.Parent.CurrentOrDefaultConfiguration.IgnorePropertyAdded))
                {
                    var auditEntryProperty = entry.Parent.Configuration.AuditEntryPropertyFactory != null ?
                        entry.Parent.Configuration.AuditEntryPropertyFactory(new AuditEntryPropertyArgs(entry, objectStateEntry, propertyEntry.Name, null, property.CurrentValue)) :
                        new AuditEntryProperty();
                    auditEntryProperty.IsKey = property.Metadata.IsKey();
                    var value = property.CurrentValue;

					if (entry.Parent.CurrentOrDefaultConfiguration.IgnoreEntityAddedDefaultValue && property.Metadata.FieldInfo != null &&  property.Metadata.FieldInfo.FieldType != typeof(object) && property.CurrentValue != null)
	                {
		                var checkDefaultValue = DefaultValue(property.Metadata.FieldInfo.FieldType);
		                if (checkDefaultValue != null && checkDefaultValue.Equals(property.CurrentValue))
		                {
			                value = null;
		                }
	                } 

					auditEntryProperty.Build(entry, propertyEntry.Name, null, value);
                    entry.Properties.Add(auditEntryProperty);
                }
            }
        }
#endif

		// https://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
		internal static object DefaultValue(Type type)
	    {
		    if (type.IsValueType)
			    return Activator.CreateInstance(type);

		    return null;
	    }
	}
}