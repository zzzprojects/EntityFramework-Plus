// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if NET45

using System.Threading;
using System.Threading.Tasks;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;
#endif

namespace Z.EntityFramework.Plus
{
    public static partial class AuditExtensions
    {
        /// <summary>Asynchronously audits and saves all changes made in this context to the underlying database.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the number of objects written
        ///     to the underlying database
        /// </returns>
        public static Task<int> SaveChangesAsync(this DbContext context, Audit audit)
        {
            return context.SaveChangesAsync(audit, CancellationToken.None);
        }

        /// <summary>A DbContext extension method that saves the changes asynchronous.</summary>
        /// <param name="context">The context used to audits and saves all changes made.</param>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the number of objects written
        ///     to the underlying database
        /// </returns>
        public static async Task<int> SaveChangesAsync(this DbContext context, Audit audit, CancellationToken cancellationToken)
        {
            audit.PreSaveChanges(context);
            var rowAffecteds = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            audit.PostSaveChanges();

            if (audit.CurrentOrDefaultConfiguration.AutoSavePreAction != null)
            {
                audit.CurrentOrDefaultConfiguration.AutoSavePreAction(context, audit);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return rowAffecteds;
        }
    }
}

#endif