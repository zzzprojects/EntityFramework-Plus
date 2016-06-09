// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditExtensions
    {
        /// <summary>Audits and saves all changes made in this context to the underlying database.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public static int SaveChanges(this DbContext context, Audit audit)
        {
            audit.PreSaveChanges(context);
            var rowAffecteds = context.SaveChanges();
            audit.PostSaveChanges();

            if (audit.CurrentOrDefaultConfiguration.AutoSavePreAction != null)
            {
                audit.CurrentOrDefaultConfiguration.AutoSavePreAction(context, audit);
                context.SaveChanges();
            }

            return rowAffecteds;
        }
    }
}