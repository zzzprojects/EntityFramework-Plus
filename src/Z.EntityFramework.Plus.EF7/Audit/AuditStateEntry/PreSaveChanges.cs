// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5
using System.Data;
using System.Data.Entity;

#elif EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditStateEntry
    {
        /// <summary>Pre save changes.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        public static void PreSaveChanges(Audit audit, DbContext context)
        {
#if EF5 || EF6
            var objectContext = context.GetObjectContext();
            objectContext.DetectChanges();

            var changes = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted);

            foreach (var objectStateEntry in changes)
            {
                if (objectStateEntry.IsRelationship)
                {
                    if (objectStateEntry.State == EntityState.Added && audit.Configuration.IncludeRelationAdded)
                    {
                        AuditRelationAdded(audit, objectStateEntry);
                    }
                    else if (objectStateEntry.State == EntityState.Deleted && audit.Configuration.IncludeRelationDeleted)
                    {
                        AuditRelationDeleted(audit, objectStateEntry);
                    }
                }
                else
                {
                    if (objectStateEntry.State == EntityState.Added && audit.Configuration.IncludeEntityAdded)
                    {
                        AuditEntityAdded(audit, objectStateEntry);
                    }
                    else if (objectStateEntry.State == EntityState.Deleted && audit.Configuration.IncludeEntityDeleted)
                    {
                        AuditEntityDeleted(audit, objectStateEntry);
                    }
                    else if (objectStateEntry.State == EntityState.Modified && audit.Configuration.IncludeEntityModified)
                    {
                        AuditEntityModified(audit, objectStateEntry);
                    }
                }
            }
#elif EF7
            context.ChangeTracker.DetectChanges();
            var manager = context.ChangeTracker.GetStateManager();
            var entries = manager.Entries;

            foreach (var entry in entries)
            {
                if (entry.EntityState == EntityState.Added)
                {
                }
                else if (entry.EntityState == EntityState.Deleted)
                {
                }
                else if (entry.EntityState == EntityState.Modified)
                {
                }
            }
#endif
        }
    }
}