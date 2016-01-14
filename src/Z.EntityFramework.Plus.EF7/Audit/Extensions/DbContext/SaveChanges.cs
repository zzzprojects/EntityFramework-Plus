// Description: EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database.
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: http://www.zzzprojects.com/license-agreement/
// More projects: http://www.zzzprojects.com/
// Copyright (c) 2015 ZZZ Projects. All rights reserved.

#if EF5 || EF6
using System.Data.Entity;

#elif EF7
using Microsoft.Data.Entity;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class ChangeAuditExtensions
    {
        /// <summary>Audits and saves all changes made in this context to the underlying database.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public static int SaveChanges(this DbContext context, Audit audit)
        {
            AuditStateEntry.PreSaveChanges(audit, context);
            var result = context.SaveChanges();
            AuditStateEntry.PostSaveChanges(audit);

            if (audit.Configuration.AutoSaveAction != null)
            {
                audit.Configuration.AutoSaveAction(context, audit);
                context.SaveChanges();
            }

            return result;
        }
    }
}