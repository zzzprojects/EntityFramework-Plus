// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

#elif EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

#elif EFCORE
using System.Linq;
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    public partial class Audit
    {
        /// <summary>Adds audit entries before the save changes has been executed.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        public static void PreSaveChanges(Audit audit, DbContext context)
        {
#if EF5 || EF6
            var objectContext = ((IObjectContextAdapter) context).ObjectContext;
            objectContext.DetectChanges();
            var changes = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted);
#elif EFCORE
            context.ChangeTracker.DetectChanges();
            var changes = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added ||
                                                                     x.State == EntityState.Modified ||
                                                                     x.State == EntityState.Deleted);
#endif

            foreach (var objectStateEntry in changes)
            {
#if EF5 || EF6
                // Relationship
                if (objectStateEntry.IsRelationship)
                {
                    // Relationship Added
                    if (objectStateEntry.State == EntityState.Added
                        && !audit.CurrentOrDefaultConfiguration.IgnoreRelationshipAdded)
                    {
                        AuditRelationAdded(audit, objectStateEntry);
                    }

                    // Relationship Deleted
                    else if (objectStateEntry.State == EntityState.Deleted
                             && !audit.CurrentOrDefaultConfiguration.IgnoreRelationshipDeleted)
                    {
                        AuditRelationDeleted(audit, objectStateEntry);
                    }
                }

                // Entity
                else
                {
#endif
                    // Entity Added
                    if (objectStateEntry.State == EntityState.Added
                        && !audit.CurrentOrDefaultConfiguration.IgnoreEntityAdded
                        && audit.CurrentOrDefaultConfiguration.IsAuditedEntity(objectStateEntry))
                    {
                        AuditEntityAdded(audit, objectStateEntry);
                    }

                    // Entity Deleted
                    else if (objectStateEntry.State == EntityState.Deleted
                             && !audit.CurrentOrDefaultConfiguration.IgnoreEntityDeleted
                             && audit.CurrentOrDefaultConfiguration.IsAuditedEntity(objectStateEntry))
                    {
                        AuditEntityDeleted(audit, objectStateEntry);
                    }

                    // Entity Modified
                    else if (objectStateEntry.State == EntityState.Modified
                             && audit.CurrentOrDefaultConfiguration.IsAuditedEntity(objectStateEntry))
                    {
                        var auditState = audit.CurrentOrDefaultConfiguration.GetEntityModifiedState(objectStateEntry);

                        // Entity Modified
                        if (auditState == AuditEntryState.EntityModified
                            && !audit.CurrentOrDefaultConfiguration.IgnoreEntityModified)
                        {
                            AuditEntityModified(audit, objectStateEntry, auditState);
                        }

                        // Entity Soft Added
                        else if (auditState == AuditEntryState.EntitySoftAdded
                                 && !audit.CurrentOrDefaultConfiguration.IgnoreEntitySoftAdded)
                        {
                            AuditEntityModified(audit, objectStateEntry, auditState);
                        }

                        // Entity Soft Deleted
                        else if (auditState == AuditEntryState.EntitySoftDeleted
                                 && !audit.CurrentOrDefaultConfiguration.IgnoreEntitySoftDeleted)
                        {
                            AuditEntityModified(audit, objectStateEntry, auditState);
                        }
                    }
#if EF5 || EF6
                }
#endif
            }
        }
    }
}