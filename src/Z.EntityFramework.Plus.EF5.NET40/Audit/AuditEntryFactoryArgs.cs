#if EF5
using System.Data.Objects;

#elif EF6
using System.Data.Entity.Core.Objects;

#elif EFCORE
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endif

namespace Z.EntityFramework.Plus
{
    public class AuditEntryFactoryArgs
    {
#if EF5 || EF6
        public AuditEntryFactoryArgs(Audit audit, ObjectStateEntry objectStateEntry, AuditEntryState auditEntryState)
#elif EFCORE
        public AuditEntryFactoryArgs(Audit audit, EntityEntry entityEntry, AuditEntryState auditEntryState)
#endif
        {
            Audit = audit;
            AuditEntryState = auditEntryState;

#if EF5 || EF6
            ObjectStateEntry = objectStateEntry;
#elif EFCORE
            EntityEntry = entityEntry;
#endif
        }

        public Audit Audit { get; set; }
        public AuditEntryState AuditEntryState { get; set; }

#if EF5 || EF6
        public ObjectStateEntry ObjectStateEntry { get; set; }
#elif EFCORE
        public EntityEntry EntityEntry { get; set; }
#endif
    }
}