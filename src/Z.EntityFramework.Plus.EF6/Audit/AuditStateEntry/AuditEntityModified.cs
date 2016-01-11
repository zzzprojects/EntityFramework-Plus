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
        /// <summary>Audit entity modified.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityModified(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(objectStateEntry)
            {
                State = AuditEntryState.EntityModified
            };

            if (audit.Configuration.IsSoftAdded != null && audit.Configuration.IsSoftAdded(objectStateEntry.Entity))
            {
                entry.State = AuditEntryState.EntitySoftAdded;
            }
            else if (audit.Configuration.IsSoftDeleted != null && audit.Configuration.IsSoftDeleted(objectStateEntry.Entity))
            {
                entry.State = AuditEntryState.EntitySoftDeleted;
            }

            AuditEntityModified(audit, entry, objectStateEntry.OriginalValues, objectStateEntry.CurrentValues);
            audit.Entries.Add(entry);
        }

        public static void AuditEntityModified(Audit audit, AuditEntry entry, DbDataRecord orginalRecord, DbUpdatableDataRecord currentRecord, string prefix = "")
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
                    AuditEntityModified(audit, entry, valueRecord, currentValue as DbUpdatableDataRecord, string.Concat(prefix, name, "."));
                }
                else
                {
                    if (audit.Configuration.IncludePropertyUnchanged || !Equals(currentValue, originalValue))
                    {
                        entry.Properties.Add(new AuditEntryProperty(string.Concat(prefix, name), originalValue, currentValue));
                    }
                }
            }
        }
    }
}