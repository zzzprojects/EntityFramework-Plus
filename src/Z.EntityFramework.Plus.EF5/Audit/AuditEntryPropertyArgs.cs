#if EF5 
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public class AuditEntryPropertyArgs
    {
#if EF5 || EF6
        public AuditEntryPropertyArgs(AuditEntry parent, ObjectStateEntry objectStateEntry, string propertyName, object oldValue, object newValue)
            : this(parent, objectStateEntry, null, propertyName, oldValue, newValue)
#elif EFCORE
        public AuditEntryPropertyArgs(AuditEntry parent, EntityEntry entityEntry, string propertyName, object oldValue, object newValue)
            : this(parent, entityEntry, null, propertyName, oldValue, newValue)
#endif
        {
        }

#if EF5 || EF6
        public AuditEntryPropertyArgs(AuditEntry parent, ObjectStateEntry objectStateEntry, string relationName, string propertyName, object oldValue, object newValue)
#elif EFCORE
        public AuditEntryPropertyArgs(AuditEntry parent, EntityEntry entityEntry, string relationName, string propertyName, object oldValue, object newValue)
#endif
        {
            AuditEntry = parent;
            NewValue = newValue;
            OldValue = oldValue;
            PropertyName = propertyName;
            RelationName = relationName;

#if EF5 || EF6
            ObjectStateEntry = objectStateEntry;
#elif EFCORE
            EntityEntry = entityEntry;
#endif
        }

#if EF5 || EF6
        public ObjectStateEntry ObjectStateEntry { get; set; }
#elif EFCORE
        public EntityEntry EntityEntry { get; set; }
#endif
        public AuditEntry AuditEntry { get; set; }
        public string RelationName { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}